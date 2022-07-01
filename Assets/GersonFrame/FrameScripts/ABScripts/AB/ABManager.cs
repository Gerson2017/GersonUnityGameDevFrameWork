using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GersonFrame.Tool;
using UnityEngine.Networking;

namespace GersonFrame.ABFrame
{

    public class ABManager : Singleton<ABManager>
    {
        ABManager()
        {
        }


        public const string ABConfigABName = "assetbundleconfig";

        /// <summary>
        /// AB文件路径
        /// </summary>
        public static string ABFilePath
        {
            get
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                return HotPatchManager.mUnPackPath+ "/";
#else
                return Application.streamingAssetsPath + "/AssetBundles/";
#endif

            }
        }

        /// <summary>
        /// 资源关系依赖表 可以根据crc 来找到对应的资源块
        /// </summary>
        protected Dictionary<uint, ResourceItem> m_resourceItemCrcDic = new Dictionary<uint, ResourceItem>();



        /// <summary>
        /// AssetBundle资源 管理加载和卸载 key为crc
        /// </summary>
        protected Dictionary<uint, AssetBundleItem> m_assetBundleItemDic = new Dictionary<uint, AssetBundleItem>();


        /// <summary>
        /// AssetbundleItem对象池
        /// </summary>
        protected ClassObjectPool<AssetBundleItem> m_assetBundleItemPool = ObjectManager.Instance.GetOrCreateClassPool<AssetBundleItem>(500);


        /// <summary>
        /// AB配置文件
        /// </summary>
        public AssetBundleConfig ABConfig
        {
            get; private set;
        }


        /// <summary>
        /// 加载AB 配置表
        /// </summary>
        /// <returns></returns>
        public bool LoadAssetBundleConfig()
        {
            if (m_resourceItemCrcDic.Count > 0) 
                return true;
            m_resourceItemCrcDic.Clear();

            AssetLoadModel assetLoadModel = AssetLoadModel.LoadFromAssetBundle;
            //资源加载模式切换
#if UNITY_EDITOR
            ABConfig assetloadconfig = UnityEditor.AssetDatabase.LoadAssetAtPath<ABConfig>(ABFrameConfigGeter.Config.AbconfigPath);
            assetLoadModel = assetloadconfig.m_AssetsLoadeModel;
#endif
            TextAsset textAsset = null;
            switch (assetLoadModel)
            {
                case AssetLoadModel.LoadFromEditor:
#if UNITY_EDITOR
                    this.ABConfig = BinarySerializeOpt.XmlDeserialize<AssetBundleConfig>(ABFrameConfigGeter.Config.AssetbundleXMLConfigPath);
#endif
                    break;
                case AssetLoadModel.LoadFromAssetBundle:
                    //======================HotUpdate======================
                    string hotABPath = HotPatchManager.Instance.ComputeABPath(ABConfigABName);
                    string configPath = string.IsNullOrEmpty(hotABPath) ? ABFilePath + ABConfigABName : hotABPath;
                    byte[] bytes = AES.AESFileByteDecrypt(configPath, "Gerson");
                    if (bytes == null)
                        return false;
                    AssetBundle configAB = AssetBundle.LoadFromMemory(bytes);
                    textAsset = configAB.LoadAsset<TextAsset>(ABConfigABName);
                    if (textAsset == null)
                    {
                        MyDebuger.LogError("AssetBundleConfig.bytes is  not exit");
                        return false;
                    }
                    ///反序列化二进制数据
                    MemoryStream stream = new MemoryStream(textAsset.bytes);
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    this.ABConfig = (AssetBundleConfig)binaryFormatter.Deserialize(stream);
                    stream.Close();
                    break;
                default:
                    MyDebuger.LogError("can not found asset from abManager");
                    break;
            }

            //从配表中获取ab资源信息
            for (int i = 0; i < this.ABConfig.ABList.Count; i++)
            {
                ABBase ab = this.ABConfig.ABList[i];
                ResourceItem item = new ResourceItem();
                item.m_Crc = ab.Crc;
                item.m_AssetName = ab.AssetName;
                item.m_ABName = ab.ABName.ToLower();
                item.m_DependenceBundle = ab.ABDependences;

                if (this.m_resourceItemCrcDic.ContainsKey(ab.Crc))
                    MyDebuger.LogError("资源crc 重复：包名 " + ab.ABName + " 资源名:" + ab.AssetName);
                else
                    this.m_resourceItemCrcDic.Add(item.m_Crc, item);
            }
            return true;
        }


        /// <summary>
        /// 根据路径的Crc 加载ResoureItem
        /// </summary>
        /// <param name="crc"></param>
        /// <returns></returns>
        public ResourceItem LoadResourceAssetBundle(uint crc, string assetname = null)
        {
            ResourceItem item = null;

            if (!this.m_resourceItemCrcDic.TryGetValue(crc, out item) || item == null)
            {
                MyDebuger.LogError(string.Format("LoadResourceAssetBundle error: can not found crc {0} in AssetBundleConfig {1} ", crc.ToString(), assetname));
                return item;
            }

            if (item.m_AssetBundle != null)
                return item;

            item.m_AssetBundle = LoadAssetBundle(item.m_ABName);

            if (item.m_DependenceBundle != null)
            {
                for (int i = 0; i < item.m_DependenceBundle.Count; i++)
                    LoadAssetBundle(item.m_DependenceBundle[i]);
                  
            }
            return item;
        }


        /// <summary>
        ///释放ab资源 根据resourItem
        /// </summary>
        public void ReleaseAssetBundle(ResourceItem item)
        {
            if (item == null) return;
            if (item.m_DependenceBundle != null && item.m_DependenceBundle.Count > 0)
            {
                for (int i = 0; i < item.m_DependenceBundle.Count; i++)
                    UnLoadAssetBundle(item.m_DependenceBundle[i]);
            }
            UnLoadAssetBundle(item.m_ABName);
        }


        /// <summary>
        /// 卸载ab包
        /// </summary>
        /// <param name="abName"></param>
        private void UnLoadAssetBundle(string abName)
        {
            uint crc = Crc32.GetCrc32(abName);
            AssetBundleItem abitem = null;
            if (m_assetBundleItemDic.TryGetValue(crc, out abitem) && abitem != null)
            {
                abitem.RefCount--;
                if (abitem.RefCount <= 0 && abitem.m_AssetBundle != null)
                {
                    abitem.m_AssetBundle.Unload(true);
                    abitem.Reset();
                    m_assetBundleItemPool.Recycle(abitem);
                    m_assetBundleItemDic.Remove(crc);
                }
            }
        }

        /// <summary>
        /// 加载Assetbundle 并对资源引用计数
        /// </summary>
        /// <returns></returns>
        private AssetBundle LoadAssetBundle(string name)
        {
            AssetBundleItem bundleItem = null;
            uint crc = Crc32.GetCrc32(name);
            //从未加载过该资源
            if (!m_assetBundleItemDic.TryGetValue(crc, out bundleItem))
            {
                AssetBundle assetBundle = null;
                //======================HotUpdate======================
                string hotABPath = HotPatchManager.Instance.ComputeABPath(name);
                string fullPath = string.IsNullOrEmpty(hotABPath) ? ABFilePath + name : hotABPath;
                byte[] bytes = AES.AESFileByteDecrypt(fullPath, "Gerson");
                assetBundle = AssetBundle.LoadFromMemory(bytes);

                if (assetBundle == null)
                    MyDebuger.LogError(string.Format("Load AssetBundle:{0} faile ", name));

                bundleItem = m_assetBundleItemPool.Spwan(true);
                bundleItem.Reset();
                bundleItem.m_AssetBundle = assetBundle;
                bundleItem.RefCount++;
                m_assetBundleItemDic.Add(crc, bundleItem);
            }
            else
                bundleItem.RefCount++;

            return bundleItem.m_AssetBundle;
        }



        /// <summary>
        /// 根据crc 找到ResourceItem 资源
        /// </summary>
        /// <returns></returns>
        public ResourceItem FinResourceItemByCrc(uint crc)
        {
            ResourceItem item = null;
            m_resourceItemCrcDic.TryGetValue(crc, out item);
            return item;
        }
    }


    public class AssetBundleItem
    {
        public AssetBundle m_AssetBundle;

        /// <summary>
        /// 被用到引用个数
        /// </summary>
        public int RefCount = 0;

        /// <summary>
        /// 重置 清除引用个数 清空资源
        /// </summary>
        public void Reset()
        {
            m_AssetBundle = null;
            RefCount = 0;
        }

    }

    /// <summary>
    /// 对应的ABConfig里的资源 其实就是Assetbundle资源里面的具体资源
    /// </summary>
    public class ResourceItem
    {
        public uint m_Crc = 0;
        /// <summary>
        /// 资源文件名
        /// </summary>
        public string m_AssetName = string.Empty;
        /// <summary>
        /// 资源所在的Assetbundle名字
        /// </summary>
        public string m_ABName = string.Empty;
        /// <summary>
        /// 该资源所依赖的Assetbundle名字
        /// </summary>
        public List<string> m_DependenceBundle = null;
        //=============================================================
        /// <summary>
        /// 该资源所加载完的AB包
        /// </summary>
        public AssetBundle m_AssetBundle = null;
        /// <summary>
        /// 资源对象
        /// </summary>
        public Object m_Obj = null;
        /// <summary>
        /// 资源唯一标识
        /// </summary>
        public int m_Guid = 0;
        /// <summary>
        /// 资源最后使用的时间
        /// </summary>
        public float m_LastUseTime = 0.0f;
        /// <summary>
        /// 对于引用计数为0的资源 决定在清理缓存时 是否要清理 
        /// </summary>
        public bool m_Clear = true;
        protected int m_refCount = 0;

        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount
        {
            get { return this.m_refCount; }
            set
            {
                m_refCount = value;
                if (m_refCount < 0)
                {
                    Debug.LogError("m_refCount<0 " + m_refCount + " " + (m_Obj != null ? m_Obj.name : "name is null"));
                    m_refCount = 0;
                }
            }
        }


    }
}