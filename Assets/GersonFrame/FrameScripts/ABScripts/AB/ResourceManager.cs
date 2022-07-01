using GersonFrame.Tool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GersonFrame.ABFrame
{

    /// <summary>
    /// 资源加载优先级
    /// </summary>
    public enum LoadResPriority
    {
        Res_Heigh = 0,//最高优先级
        Res_Middle,//一般优先级
        Res_Slow,//低优先级
        Res_Num,//优先级数量
    }


    /// <summary>
    /// 实例化资源对象
    /// </summary>
    public class ResourceObj
    {
        public uint m_Crc = 0;
        ///存ResourceItem
        public ResourceItem m_ResItem = null;
        /// <summary>
        /// 实例化出来的gameObject
        /// </summary>
        public GameObject m_CloneObj = null;
        /// <summary>
        /// 是否跳场景清除
        /// </summary>
        public bool m_bClear = true;
        /// <summary>
        /// 父物体是否为场景父物体节点
        /// </summary>
        public bool m_setSceneParent = false;
        /// <summary>
        /// 储存Guid
        /// </summary>
        public int m_Guid = 0;
        /// <summary>
        /// 是否已经放回对象池
        /// </summary>
        public bool m_Already = false;
        /// <summary>
        /// 资源实例化完成后的回调
        /// </summary>
        public OnAsyncObjFinishDele m_FinishCallback = null;
        //============异步参数===========================
        public object Param1, Param2, Param3 = null;
        ///离线数据
        public OffLineData m_offLineData = null;
        public void Reset()
        {
            m_Crc = 0;
            m_CloneObj = null;
            m_bClear = true;
            m_Guid = 0;
            m_ResItem = null;
            m_Already = false;
            m_FinishCallback = null;
            this.m_setSceneParent = false;
            Param1 = null;
            Param2 = null;
            Param3 = null;
            m_offLineData = null;
        }
    }

    /// <summary>
    /// 异步加载资源信息
    /// </summary>
    public class AsyncLoadResInfo
    {
        /// <summary>
        /// 资源加载完成后的所有回调
        /// </summary>
        public List<AsnycCallBackInfo> m_DeleFinishCallBacks = new List<AsnycCallBackInfo>();

        public uint m_Crc;
        ///资源路径
        public string m_Path;
        /// <summary>
        /// 是否是一张图片
        /// </summary>
        public bool m_IsSprite=false;
        /// <summary>
        /// 资源加载优先级
        /// </summary>
        public LoadResPriority m_Priority = LoadResPriority.Res_Slow;

        public void Reset()
        {
            m_DeleFinishCallBacks.Clear();
            this.m_Crc = 0;
            this.m_Path = "";
            m_IsSprite = false;
            this.m_Priority = LoadResPriority.Res_Slow;
        }
    }

    /// <summary>
    /// 异步资源加载完成回调参数
    /// </summary>
    public class AsnycCallBackInfo
    {

        /// <summary>
        /// 实例化资源加载完成回调(针对objectManager)
        /// </summary>
        public OnAsyncInstanceFinishDele m_DealResFinish = null;
        /// <summary>
        /// ResourceObj 中间类
        /// </summary>
        public ResourceObj m_ResObj = null;
        //=====================================================
        /// <summary>
        /// 非实例化资源加载完成回调
        /// </summary>
        public OnAsyncObjFinishDele m_DealObjFinish = null;


        /// <summary>
        /// 回调参数
        /// </summary>
        public object param1, param2, param3 = null;


        public void Reset()
        {
            m_DealObjFinish = null;
            m_DealResFinish = null;
            this.param1 = param2 = param3 = null;
            m_ResObj = null;
        }
    }


    /// <summary>
    /// 资源加载模式
    /// </summary>
    public enum AssetLoadModel
    {
        LoadFromEditor,
        LoadFromAssetBundle,
    }


    /// <summary>
    /// 非实例化资源异步加载完回调委托 
    /// </summary>
    public delegate void OnAsyncObjFinishDele(string path, Object obj, object param1 = null, object param2 = null, object param3 = null);
    /// <summary>
    /// 实例化资源异步加载完回调委托 
    /// </summary>
    public delegate void OnAsyncInstanceFinishDele(string path, ResourceObj obj, object param1 = null, object param2 = null, object param3 = null);

    public class ResourceManager : Singleton<ResourceManager>
    {

        ResourceManager() { }
        protected int m_Guid = 0;

        private AssetLoadModel m_assetBundleLoadModel = AssetLoadModel.LoadFromAssetBundle;

        /// <summary>
        /// 缓存正在使用的资源池
        /// </summary>
        public Dictionary<uint, ResourceItem> m_UsingAssetDic { get; set; } = new Dictionary<uint, ResourceItem>();

        /// <summary>
        /// 缓存引用计数为0的资源列表 为了防止资源还会再被利用而从磁盘加载 减少加载时间
        /// </summary>
        protected CMapList<ResourceItem> m_noRefResourceMapList = new CMapList<ResourceItem>();

        //==================================================异步=======================
        /// <summary>
        /// 使用Mono 开启协程进行异步加载
        /// </summary>
        protected MonoBehaviour m_asyncMono;
        ///正在异步加载的资源列表包含优先级
        protected List<AsyncLoadResInfo>[] m_AsyncLoadingAssetList = new List<AsyncLoadResInfo>[(int)LoadResPriority.Res_Num];
        ///正在异步加载的资源Dic 用来做重复判断
        protected Dictionary<uint, AsyncLoadResInfo> m_asnycloadingAssetDic = new Dictionary<uint, AsyncLoadResInfo>();

        /// <summary>
        /// 资源加载完成回调信息对象池
        /// </summary>
        protected ClassObjectPool<AsnycCallBackInfo> m_asyncCallBackInfoPool = new ClassObjectPool<AsnycCallBackInfo>(50);

        /// <summary>
        /// 异步加载资源信息对象池
        /// </summary>
        protected ClassObjectPool<AsyncLoadResInfo> m_asyncResInfoPool = new ClassObjectPool<AsyncLoadResInfo>(50);

        ///最长连续卡着的加载时间 微秒
        private const float MAXLOADRESTIME = 200000;
        /// <summary>
        /// 资源最大缓存个数 可以根据手机高低配置进行设置
        /// </summary>
        private const int MAXCACHECOUNT = 500;



        /// <summary>
        /// 异步加载初始化
        /// </summary>
        public void StartLoadAsync(MonoBehaviour asyncMono)
        {
            this.m_asyncMono = asyncMono;
            this.m_asyncMono.StartCoroutine(AsyncLoadCor());
        }


        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
#if UNITY_EDITOR
            ABConfig assetloadconfig = UnityEditor.AssetDatabase.LoadAssetAtPath<ABConfig>(ABFrameConfigGeter.Config.AbconfigPath);
            this.m_assetBundleLoadModel = assetloadconfig.m_AssetsLoadeModel;
#else
            this.m_assetBundleLoadModel= AssetLoadModel.LoadFromAssetBundle;
#endif

            for (int i = 0; i < (int)LoadResPriority.Res_Num; i++)
            {
                if (m_AsyncLoadingAssetList[i]==null)
                    m_AsyncLoadingAssetList[i] = new List<AsyncLoadResInfo>();
            }
        }

        /// <summary>
        /// 创建唯一的Guid
        /// </summary>
        /// <returns></returns>
        public int CreateGuid()
        {
            return this.m_Guid++;
        }

        /// <summary>
        /// 异步加载（while循环监听请求）
        /// </summary>
        /// <returns></returns>
        IEnumerator AsyncLoadCor()
        {
            List<AsnycCallBackInfo> callbackList = null;
            ///上一次yield 的时间
            long lastYieldTime = System.DateTime.Now.Ticks;
#if UNITY_EDITOR
            WaitForSeconds editorwait = new WaitForSeconds(0.5f);
#endif
            while (true)
            {
                bool hasyiled = false;
                for (int i = 0; i < (int)LoadResPriority.Res_Num; i++)
                {
                    if (m_AsyncLoadingAssetList[(int)LoadResPriority.Res_Heigh].Count > 0)
                        i = (int)LoadResPriority.Res_Heigh;
                    else if (m_AsyncLoadingAssetList[(int)LoadResPriority.Res_Middle].Count > 0)
                        i = (int)LoadResPriority.Res_Middle;
                    List<AsyncLoadResInfo> asyncresList = m_AsyncLoadingAssetList[i];
                    if (asyncresList.Count < 1) continue;

                    AsyncLoadResInfo loadResInfo = asyncresList[0];
                    asyncresList.RemoveAt(0);
                    callbackList = loadResInfo.m_DeleFinishCallBacks;

                    Object obj = null;
                    ResourceItem item = null;
#if UNITY_EDITOR
                    if (m_assetBundleLoadModel== AssetLoadModel.LoadFromEditor)
                    {
                        if (loadResInfo.m_IsSprite)
                            obj = LoadAssetByEditor<Sprite>(loadResInfo.m_Path);
                       else 
                            obj = LoadAssetByEditor<Object>(loadResInfo.m_Path);
                        //模拟异步加载
                        yield return editorwait;
                        item = ABManager.Instance.FinResourceItemByCrc(loadResInfo.m_Crc);
                        if (item == null)
                        {
                            item = new ResourceItem();
                            item.m_Crc = loadResInfo.m_Crc;
                        }
                    }
#endif
                    ///从assetbundle中加载
                    if (obj == null)
                    {
                        item = ABManager.Instance.LoadResourceAssetBundle(loadResInfo.m_Crc, loadResInfo.m_Path);
                        if (item != null && item.m_AssetBundle != null)
                        {
                            AssetBundleRequest abrequest = null;
                            //是图片资源
                            if (loadResInfo.m_IsSprite)
                                abrequest = item.m_AssetBundle.LoadAssetAsync<Sprite>(item.m_AssetName);
                            else
                                abrequest = item.m_AssetBundle.LoadAssetAsync(item.m_AssetName);
                            yield return abrequest;
                            if (abrequest.isDone)
                                obj = abrequest.asset;

                            lastYieldTime = System.DateTime.Now.Ticks;
                        }
                    }
                    ///缓存资源
                    CacheResource(loadResInfo.m_Path, ref item, loadResInfo.m_Crc, obj, callbackList.Count);
                    for (int j = 0; j < callbackList.Count; j++)
                    {
                        AsnycCallBackInfo callback = callbackList[j];
                        //是否进行实例化资源回调
                        if (callback != null && callback.m_DealResFinish != null && callback.m_ResObj != null)
                        {
                            callback.m_DealResFinish(loadResInfo.m_Path, callback.m_ResObj, callback.param1, callback.param2, callback.param3);
                            callback.m_DealResFinish = null;
                        }
                        if (callback != null && callback.m_DealObjFinish != null)
                        {
                            callback.m_DealObjFinish(loadResInfo.m_Path, obj, callback.param1, callback.param2, callback.param3);
                            callback.m_DealObjFinish = null;
                        }
                        //回收
                        callback.Reset();
                        m_asyncCallBackInfoPool.Recycle(callback);
                    }
                    obj = null;
                    callbackList.Clear();
                    m_asnycloadingAssetDic.Remove(loadResInfo.m_Crc);
                    loadResInfo.Reset();
                    m_asyncResInfoPool.Recycle(loadResInfo);

                    if (System.DateTime.Now.Ticks - lastYieldTime > MAXLOADRESTIME)
                    {
                        yield return null;
                        lastYieldTime = System.DateTime.Now.Ticks;
                        hasyiled = true;
                    }
                }
                //减少等待时间
                if (!hasyiled || System.DateTime.Now.Ticks - lastYieldTime > MAXLOADRESTIME)
                {
                    lastYieldTime = System.DateTime.Now.Ticks;
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 资源异步加载（仅仅是不需要实例化的资源 例如音频 图片等等）
        /// </summary>
        public void AsnycLoadResource(string path, OnAsyncObjFinishDele onAsyncObjFinish, LoadResPriority loadResPriority,bool isSprite=false, object param1 = null, object param2 = null, object param3 = null)
        {
            if (m_asyncMono == null)
                Debug.LogError("请先调用 StartLoadAsync 开启异步加载");

               uint crc = Crc32.GetCrc32(path);
            ResourceItem item = GetCatchResourceItem(crc);
            //资源已经存在
            if (item != null)
            {
                onAsyncObjFinish?.Invoke(path, item.m_Obj, param1, param2, param3);
                return;
            }
            //资源不存在
                AsyncLoadResInfo resInfo = null;
                //正在预备加载的资源
                if (!this.m_asnycloadingAssetDic.TryGetValue(crc, out resInfo) || resInfo == null)
                {
                    resInfo = m_asyncResInfoPool.Spwan(true);
                    resInfo.m_Crc = crc;
                    resInfo.m_Path = path;
                    resInfo.m_IsSprite = isSprite;
                    resInfo.m_Priority = loadResPriority;
                    m_asnycloadingAssetDic.Add(crc, resInfo);
                    m_AsyncLoadingAssetList[(int)resInfo.m_Priority].Add(resInfo);
                }
                //添加回到到回调列表
                AsnycCallBackInfo callBackInfo = m_asyncCallBackInfoPool.Spwan(true);
                callBackInfo.m_DealObjFinish = onAsyncObjFinish;
                callBackInfo.param1 = param1;
                callBackInfo.param2 = param2;
                callBackInfo.param3 = param3;
                resInfo.m_DeleFinishCallBacks.Add(callBackInfo);
            
        }


        /// <summary>
        /// 实例化资源异步加载 针对objectmanager 的异步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onAsyncObjFinish"></param>
        /// <param name="loadResPriority"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <param name="param3"></param>
        /// <param name="crc"></param>
        public void AsnycLoadObjResource(string path, ResourceObj resourceObj, OnAsyncInstanceFinishDele onAsyncObjFinish, LoadResPriority loadResPriority)
        {
            if (m_asyncMono==null)
                Debug.LogError("请先调用 StartLoadAsync 开启异步加载");

            ResourceItem item = GetCatchResourceItem(resourceObj.m_Crc);
            //资源已经存在
            if (item != null)
            {
                resourceObj.m_ResItem = item;
                onAsyncObjFinish?.Invoke(path, resourceObj);
                return;
            }
            //资源不存在
            AsyncLoadResInfo resInfo = null;
            //不在正在预备加载的资源中
            if (!this.m_asnycloadingAssetDic.TryGetValue(resourceObj.m_Crc, out resInfo) || resInfo == null)
            {
                resInfo = m_asyncResInfoPool.Spwan(true);
                resInfo.m_Crc = resourceObj.m_Crc;
                resInfo.m_Path = path;
                resInfo.m_Priority = loadResPriority;
                m_asnycloadingAssetDic.Add(resourceObj.m_Crc, resInfo);
                m_AsyncLoadingAssetList[(int)resInfo.m_Priority].Add(resInfo);
            }

            //添加回到到回调列表
            AsnycCallBackInfo callBackInfo = m_asyncCallBackInfoPool.Spwan(true);
            callBackInfo.m_DealResFinish = onAsyncObjFinish;
            callBackInfo.m_ResObj = resourceObj;
            resInfo.m_DeleFinishCallBacks.Add(callBackInfo);
        }

        //==============================================同步====================================
        /// <summary>
        /// 加载ResourceObj 针对ObjManager的接口
        /// </summary>
        /// <returns></returns>
        public ResourceObj LoadResource(string path, ResourceObj resobj)
        {
            if (resobj == null) return null;
            uint crc = resobj.m_Crc == 0 ? Crc32.GetCrc32(path) : resobj.m_Crc;
            ResourceItem item = GetCatchResourceItem(crc);
            if (item != null)
            {
                resobj.m_ResItem = item;
                return resobj;
            }
            Object obj = null;
#if UNITY_EDITOR
            if (m_assetBundleLoadModel== AssetLoadModel.LoadFromEditor)
            {
                item = ABManager.Instance.FinResourceItemByCrc(crc);
                if (item != null && item.m_Obj != null)
                    obj = item.m_Obj;
                else
                {
                    if (item == null)
                    {
                        MyDebuger.LogWarning("not found assets " + path);
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = ABManager.Instance.LoadResourceAssetBundle(crc, path);
                if (item != null && item.m_AssetBundle != null)
                {
                    if (item.m_Obj != null) 
                        obj = item.m_Obj;
                    else
                        obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
                }
            }

            if (item!=null)
            {
                CacheResource(path, ref item, crc, obj);
                resobj.m_ResItem = item;
                item.m_Clear = resobj.m_bClear;
            }
            return resobj;
        }


        /// <summary>
        /// 同步加载资源 外部直接调用 仅加载不需要实例化的资源 例如texture,音频等等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadResource<T>(string path) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(path)) return null;

            uint crc = Crc32.GetCrc32(path);
            ResourceItem item = GetCatchResourceItem(crc);

            if (item != null)
            {
                return item.m_Obj as T;
            }
            T obj = null;
#if UNITY_EDITOR
            if (m_assetBundleLoadModel== AssetLoadModel.LoadFromEditor)
            {
                item = ABManager.Instance.FinResourceItemByCrc(crc);
                if (item != null && item.m_AssetBundle != null)
                {
                    if (item.m_Obj != null)
                        obj = (T)item.m_Obj;
                    else
                        obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
                }
                else
                {
                    if (item == null)
                    {
                        MyDebuger.LogWarning("not fount asset "+path);
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<T>(path);
                    if (obj==null) 
                        Debug.LogError("not fount asset " + path);
                }
            }
#endif
            if (obj == null)
            {
                item = ABManager.Instance.LoadResourceAssetBundle(crc, path);
                if (item != null && item.m_AssetBundle != null)
                {
                    if (item.m_Obj != null) 
                        obj = item.m_Obj as T;
                    else
                        obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
                }
            }
            CacheResource(path, ref item, crc, obj);
            return obj;
        }

        /// <summary>
        /// 同步加载资源 外部直接调用 仅加载不需要实例化的资源 例如texture,音频等等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public T LoadResource<T>(params string[] path) where T : UnityEngine.Object
        {
            MyStringBuilder builder = StringBuilderTool.GetStringBuilder(path);
            return LoadResource<T>(builder.ToString());
        }


        /// <summary>
        /// 根据ResourceObj 卸载资源
        /// </summary>
        /// <returns></returns>
        public bool ReleaseResource(ResourceObj obj, bool destoryobj = false)
        {
            if (obj == null) 
                return false;
            ResourceItem item = null;
          
            if (!m_UsingAssetDic.TryGetValue(obj.m_Crc, out item) || item == null)
            {
                MyDebuger.LogError("m_UsingAssetDic 里不存在该资源:" + obj.m_CloneObj.name + " 可能释放了多次");
            }
            if (obj.m_CloneObj != null)
            {
                GameObject.Destroy(obj.m_CloneObj);
                obj.m_CloneObj = null;
            }
               
            if (item!=null)
            {
                item.RefCount--;
                DestoryResourceItem(item, destoryobj);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 不需要实例化的资源的卸载 根据资源guid查找资源
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="destoryObj"></param>
        /// <returns></returns>
        public bool ReleaseResource(Object obj, bool destoryObj = false)
        {
            if (obj == null) return false;
            ResourceItem item = null;

            //todo 查找这部分可以进行优化
            foreach (ResourceItem res in m_UsingAssetDic.Values)
            {
                if (res.m_Guid == obj.GetInstanceID())
                {
                    item = res;
                    break;
                }
            }

            if (item == null)
            {
                MyDebuger.LogError("AssetDic 不存在该资源：" + obj.name + " 可能释放了多次");
                return false;
            }

            item.RefCount--;
            DestoryResourceItem(item, destoryObj);
            return true;
        }

        /// <summary>
        /// 释放资源 根据资源路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destoryObj"></param>
        /// <returns></returns>
        public bool ReleaseResource(string path, bool destoryObj = false)
        {
            if (string.IsNullOrEmpty(path)) return false;
            uint crc = Crc32.GetCrc32(path);
            ResourceItem item = null;
            if (!m_UsingAssetDic.TryGetValue(crc, out item) || item == null)
            {
                MyDebuger.LogError("AssetDic 里不存在该资源：" + path + " 可能释放了多次");
            }
            if (item!=null)
            {
                item.RefCount--;
                DestoryResourceItem(item, destoryObj);
                return true;
            }
            return false;
        }


        /// <summary>
        /// 清空缓存 一般在跳场景时候使用
        /// </summary>
        public void ClearCache()
        {
            List<ResourceItem> tempList = new List<ResourceItem>();
            foreach (ResourceItem item in m_UsingAssetDic.Values)
            {
                if (item.m_Clear)
                    tempList.Add(item);
            }
            foreach (var item in tempList)
                DestoryResourceItem(item, true);
            tempList.Clear();
        }

        /// <summary>
        /// 根据obj 增加引用计数
        /// </summary>
        /// <returns></returns>
        public int IncreaseResourceRef(ResourceObj resobj, int count = 1)
        {
            return resobj != null ?IncreaseResourceRef(resobj.m_Crc, count): 0;
        }

        /// <summary>
        /// 根据path 增加引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int IncreaseResourceRef(uint crc=0, int count = 1)
        {
            ResourceItem item = null;
            if (!m_UsingAssetDic.TryGetValue(crc, out item) || item == null)
                return 0;
            item.RefCount += count;
            //MyDebuger.Log("IncreaseResourceRef " + item.m_ABName + " assetName=" + item.m_AssetName + item.RefCount);
            item.m_LastUseTime = Time.realtimeSinceStartup;
            return item.RefCount;
        }

        /// <summary>
        /// 减少引用计数
        /// </summary>
        /// <returns></returns>
        public int DecreaseResourceRef(ResourceObj resobj, int count = 1)
        {
            return resobj != null ? DecreaseResourceRef(resobj.m_Crc, count) : 0;
        }

        /// <summary>
        /// 根据路径减少引用计数
        /// </summary>
        /// <param name="crc"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public int DecreaseResourceRef(uint crc, int count = 1)
        {
            ResourceItem item = null;
            if (!m_UsingAssetDic.TryGetValue(crc, out item) || item == null)
                return 0;
            item.RefCount -= count;
          //MyDebuger.Log("DecreaseResourceRef "+ item.m_ABName+" assetName="+item.m_AssetName+ item.RefCount);
            return item.RefCount;
        }

        /// <summary>
        ///取消异步加载
        /// </summary>
        public bool CancelAsyncLoad(ResourceObj res)
        {
            AsyncLoadResInfo asyinfo = null;
            if (m_asnycloadingAssetDic.TryGetValue(res.m_Crc, out asyinfo) && m_AsyncLoadingAssetList[(int)asyinfo.m_Priority].Contains(asyinfo))
            {
                //取消所有加载完成回调
                for (int i = asyinfo.m_DeleFinishCallBacks.Count; i >= 0; i--)
                {
                    AsnycCallBackInfo tempcallback = asyinfo.m_DeleFinishCallBacks[i];
                    if (tempcallback != null && res == tempcallback.m_ResObj)
                    {
                        tempcallback.Reset();
                        m_asyncCallBackInfoPool.Recycle(tempcallback);
                        asyinfo.m_DeleFinishCallBacks.Remove(tempcallback);
                    }
                }
                if (asyinfo.m_DeleFinishCallBacks.Count <= 0)
                {
                    asyinfo.Reset();
                    m_AsyncLoadingAssetList[(int)asyinfo.m_Priority].Remove(asyinfo);
                    m_asyncResInfoPool.Recycle(asyinfo);
                    m_asnycloadingAssetDic.Remove(res.m_Crc);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 预加载资源 贴图音效等资源
        /// </summary>
        /// <param name="path"></param>
        public void PreLoadRes(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            uint crc = Crc32.GetCrc32(path);
            //预加载不需要引用计数
            ResourceItem item = GetCatchResourceItem(crc, 0);
            if (item != null) return;

            Object obj = null;
#if UNITY_EDITOR
            if (m_assetBundleLoadModel== AssetLoadModel.LoadFromEditor)
            {
                item = ABManager.Instance.FinResourceItemByCrc(crc);
                if (item!=null&&item.m_Obj != null) obj = item.m_Obj;
                else
                {
                    if (item == null)
                    {
                        item = new ResourceItem();
                        item.m_Crc = crc;
                    }
                    obj = LoadAssetByEditor<Object>(path);
                }
            }
#endif
            if (obj == null)
            {
                item = ABManager.Instance.LoadResourceAssetBundle(crc, path);
                if (item != null && item.m_AssetBundle != null)
                {
                    if (item.m_Obj != null) obj = item.m_Obj;
                    else
                        obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
                }
            }
            CacheResource(path, ref item, crc, obj);
            //跳场景不清除缓存
            item.m_Clear = false;
            ReleaseResource(obj, false);
        }

        /// <summary>
        /// 缓存加载的资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="crc"></param>
        /// <param name="obj"></param>
        /// <param name="addrefcount"></param>
        void CacheResource(string path, ref ResourceItem item, uint crc, Object obj, int addrefcount = 1)
        {   
            //当缓存达到最大数量时 清空缓存
            WashOut();

            if (item == null)
            {
                MyDebuger.LogError("ResourceItem Load Fail  path:" + path);
                return;
            }

            if (obj == null) {
                MyDebuger.LogError("ResourceLoad Fail  path:" + path);
                return;
            }
            
            item.m_Obj = obj;
            item.m_Guid = obj.GetInstanceID();
            item.m_LastUseTime = Time.realtimeSinceStartup;
            item.RefCount += addrefcount;

            ResourceItem oldItem = null;
            if (m_UsingAssetDic.TryGetValue(item.m_Crc, out oldItem))
                m_UsingAssetDic[item.m_Crc] = item;
            else
                m_UsingAssetDic.Add(item.m_Crc, item);


        }


        /// <summary>
        /// 缓存太多 清除最早没有使用的资源 当内存不足的时候进行清除缓存操作 根据手机内存大小进行判断
        /// </summary>
        protected void WashOut()
        {
            //当大于缓存个数时 进行一般资源释放
            if (m_noRefResourceMapList.Size() > MAXCACHECOUNT)
            {
                Debug.Log("WashOut  Res Out MaxSize =" + MAXCACHECOUNT+" Recycle 1/5 Res");
                for (int i = 0; i < MAXCACHECOUNT / 5; i++)
                {
                    ResourceItem item = m_noRefResourceMapList.Back();
                    DestoryResourceItem(item, true);
                }
            }
        }

        /// <summary>
        /// 释放资源 当资源引用计数不为0时  无法释放资源
        /// </summary>
        /// <param name="item"></param>
        /// <param name="destory"></param>
        protected void DestoryResourceItem(ResourceItem item, bool destoryCache = false)
        {
            //判断引用计数 是否还在被其他资源使用
            if (item == null || item.RefCount > 0) return;

            //缓存
            if (!destoryCache)
            {
                //加入到双向链表表头
                m_noRefResourceMapList.InsertToHead(item);
                return;
            }

            //不在正在使用的资源中
            if (!m_UsingAssetDic.Remove(item.m_Crc))
                return;

            //  MyDebuger.Log("destory Asset "+item.m_ABName+" name "+item.m_AssetName);
            //从双向链表中移出
            m_noRefResourceMapList.Remove(item);
            //不缓存 清空assetbundle 引用
            ABManager.Instance.ReleaseAssetBundle(item);

            //清空资源对应的对象池
            ObjectManager.Instance.ClearPoolObject(item.m_Crc);

            if (item.m_Obj != null)
            {
                item.m_Obj = null;
#if UNITY_EDITOR
                Resources.UnloadUnusedAssets();
#endif
            }
        }

#if UNITY_EDITOR
        protected T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object
        {
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        }
#endif


        ResourceItem GetCatchResourceItem(uint crc, int addrefcount = 1)
        {
            ResourceItem item = null;
            if (m_UsingAssetDic.TryGetValue(crc, out item))
            {
                if (item != null)
                {
                    item.RefCount += addrefcount;
                    item.m_LastUseTime = Time.realtimeSinceStartup;
                    //if (item.RefCount<1)
                    //    m_noRefResourceMapList.Remove(item);
                }
            }
            return item;
        }



        //==================================Tool=================================
        /// <summary>
        /// 加载文本 获取文本内容
        /// </summary>
        /// <param name="floder"></param>
        /// <param name="jsonpath"></param>
        /// <returns></returns>
        public string LoadTextAssets(string floder, string jsonpath)
        {
            MyStringBuilder stringBuilder = StringBuilderTool.GetStringBuilder(floder, jsonpath);
            TextAsset text =LoadResource<TextAsset>(stringBuilder.ToString());
            if (text != null) return text.text;
            else
            {
                MyDebuger.Log("GetConfigJsonStr fail " + floder + jsonpath);
                return "";
            }
        }


    }





#region 链表

    /// <summary>
    /// 双向链表节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleLinkListNode<T> where T : class, new()
    {

        /// <summary>
        /// 前一个节点
        /// </summary>
        public DoubleLinkListNode<T> m_PreNode = null;

        /// <summary>
        /// 后一个节点
        /// </summary>
        public DoubleLinkListNode<T> m_NextNode = null;

        /// <summary>
        /// 当前节点所存储的值
        /// </summary>
        public T m_CurrentValue = null;

    }

    /// <summary>
    /// 双向链表结构 只需存储头结点和尾节点
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DoubleLinkList<T> where T : class, new()
    {
        /// <summary>
        /// 表头
        /// </summary>
        public DoubleLinkListNode<T> m_HeadNode = null;
        /// <summary>
        /// 表尾
        /// </summary>
        public DoubleLinkListNode<T> m_TailNode = null;
        /// <summary>
        /// 双向链表结构对象池
        /// </summary>
        protected ClassObjectPool<DoubleLinkListNode<T>> m_DoubleLineNodePool = ObjectManager.Instance.GetOrCreateClassPool<DoubleLinkListNode<T>>(500);

        /// <summary>
        /// 节点个数
        /// </summary>
        public int Count { get; protected set; } = 0;

        /// <summary>
        /// 添加到头部
        /// </summary>
        public DoubleLinkListNode<T> AddToHead(T t)
        {
            DoubleLinkListNode<T> node = m_DoubleLineNodePool.Spwan(true);
            node.m_NextNode = null;
            node.m_PreNode = null;
            node.m_CurrentValue = t;
            return AddToHead(node); ;
        }

        /// <summary>
        /// 添加一个节点到头部
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public DoubleLinkListNode<T> AddToHead(DoubleLinkListNode<T> node)
        {
            if (node == null) return null;
            node.m_PreNode = null;
            if (m_HeadNode == null)
            {
                m_HeadNode = node;
                m_TailNode = node;
            }
            else//引用地址交换
            {
                node.m_NextNode = this.m_HeadNode;
                m_HeadNode.m_PreNode = node;
                m_HeadNode = node;
            }
            Count++;
            return m_HeadNode;
        }



        /// <summary>
        /// 添加节点到尾部
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public DoubleLinkListNode<T> AddToTrail(T t)
        {
            DoubleLinkListNode<T> node = m_DoubleLineNodePool.Spwan(true);
            node.m_NextNode = null;
            node.m_PreNode = null;
            node.m_CurrentValue = t;
            return AddToTrail(node);
        }


        /// <summary>
        /// 添加节点到尾部
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public DoubleLinkListNode<T> AddToTrail(DoubleLinkListNode<T> node)
        {
            if (node == null) return null;
            node.m_NextNode = null;
            if (this.m_TailNode == null)
            {
                this.m_HeadNode = this.m_TailNode = node;
            }
            else//引用地址交换
            {
                node.m_PreNode = m_TailNode;
                m_TailNode.m_NextNode = node;
                m_TailNode = node;
            }
            Count++;
            return m_TailNode;
        }


        /// <summary>
        /// 删除节点
        /// </summary>
        public void RemoveNode(DoubleLinkListNode<T> node)
        {
            if (node == null) return;
            if (node == m_HeadNode)
                m_HeadNode = node.m_NextNode;

            if (node == m_TailNode)
                m_TailNode = node.m_PreNode;
            if (node.m_PreNode != null)
                node.m_PreNode.m_NextNode = node.m_NextNode;
            if (node.m_NextNode != null)
                node.m_NextNode.m_PreNode = node.m_PreNode;

            node.m_PreNode = node.m_NextNode = null;
            node.m_CurrentValue = null;
            m_DoubleLineNodePool.Recycle(node);
            Count--;
        }


        /// <summary>
        /// 把某个有效节点移动到头部
        /// </summary>
        public void MoveToHead(DoubleLinkListNode<T> node)
        {
            if (node == null || node == m_HeadNode) return;
            //节点已经被回收了
            if (node.m_NextNode == null && node.m_PreNode == null)
                return;
            if (node == m_TailNode)
                m_TailNode = node.m_PreNode;

            if (node.m_PreNode != null)
                node.m_PreNode.m_NextNode = node.m_NextNode;

            if (node.m_NextNode != null)
                node.m_NextNode.m_PreNode = node.m_PreNode;

            node.m_PreNode = null;
            node.m_NextNode = m_HeadNode;
            m_HeadNode.m_PreNode = node;
            m_HeadNode = node;
            if (m_TailNode == null)
                m_TailNode = m_HeadNode;

        }


    }


#endregion


    /// <summary>
    /// 双向链表封装
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CMapList<T> where T : class, new()
    {
        public DoubleLinkList<T> m_DLinkList = new DoubleLinkList<T>();

        /// <summary>
        /// 要用到的值 和它所在链表中的节点
        /// </summary>
        Dictionary<T, DoubleLinkListNode<T>> m_findNodeDic = new Dictionary<T, DoubleLinkListNode<T>>();

        /// <summary>
        ///析构函数 销毁时会调用
        /// </summary>
        ~CMapList()
        {
            Clear();
        }

        /// <summary>
        /// 插入节点到头部
        /// </summary>
        /// <param name="t"></param>
        public void InsertToHead(T t)
        {
            DoubleLinkListNode<T> node = null;
            if (m_findNodeDic.TryGetValue(t, out node) && node != null)
            {
                m_DLinkList.AddToHead(node);
                return;
            }
            m_DLinkList.AddToHead(t);
            m_findNodeDic.Add(t, m_DLinkList.m_HeadNode);
        }


        /// <summary>
        /// 从表尾取出一个节点
        /// </summary>
        public void Pop()
        {
            if (m_DLinkList.m_TailNode != null)
                Remove(m_DLinkList.m_TailNode.m_CurrentValue);
        }

        /// <summary>
        ///移除
        /// </summary>
        public void Remove(T t)
        {
            DoubleLinkListNode<T> node = null;
            if (!m_findNodeDic.TryGetValue(t, out node) || node == null)
                return;
            m_DLinkList.RemoveNode(node);
            m_findNodeDic.Remove(t);
        }


        /// <summary>
        /// 清空链表
        /// </summary>
        public void Clear()
        {
            while (m_DLinkList.m_TailNode != null)
            {
                Remove(m_DLinkList.m_TailNode.m_CurrentValue);
            }
        }


        /// <summary>
        /// 获取尾部节点
        /// </summary>
        /// <returns></returns>
        public T Back()
        {
            return m_DLinkList.m_TailNode == null ? null : m_DLinkList.m_TailNode.m_CurrentValue;
        }


        /// <summary>
        /// 节点个数
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return m_findNodeDic.Count;
        }


        /// <summary>
        /// 查找节点是否存在
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Find(T t)
        {
            DoubleLinkListNode<T> node = null;
            if (!m_findNodeDic.TryGetValue(t, out node) || node == null)
                return false;
            return true;
        }

        /// <summary>
        /// 刷新某个节点 把节点移动到前面去
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Reflesh(T t)
        {
            DoubleLinkListNode<T> node = null;
            if (!m_findNodeDic.TryGetValue(t, out node) || node == null)
                return false;
            m_DLinkList.MoveToHead(node);
            return true;
        }

    }
}
