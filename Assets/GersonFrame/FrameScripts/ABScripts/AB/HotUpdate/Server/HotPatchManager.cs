using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace GersonFrame.ABFrame
{
    public class SendServerXmlOrApkInfo
    {
        public string UDID;
        public string AppVer;
        public string Channel;
        //android iOS
        public string Platform;
    }

    [System.Serializable]
    public class GetServerXmlOrApkInfo
    {
        public int NewVersion;
        public int Force;
        public string AppURL;
        //android iOS
        public string ResURL;
        public List<string> VersionTips;
        public List<string> VersionImages;

        public override string ToString()
        {
            string info = "NewVersion " + NewVersion + " force " + Force + " AppURL " + AppURL + " ResURL " + ResURL;

            if (this.VersionTips != null)
            {
                info += " VersionTips ";
                for (int i = 0; i < VersionTips.Count; i++)
                    info += VersionTips[i];
            }

            if (this.VersionImages != null)
            {
                info += " VersionImages ";
                for (int i = 0; i < VersionImages.Count; i++)
                    info += VersionImages[i];
            }
            return info;
        }
    }

    public enum ABAssetsState
    {
        None,
        /// <summary>
        /// 是否开始解压
        /// </summary>
        StartUnPack,
        /// <summary>
        /// 获取远端Xml文件
        /// </summary>
        GetServerXml,

        /// <summary>
        /// 没有新版
        /// </summary>
        NoNewVersion,
        /// <summary>
        /// 有新版本但不要更新
        /// </summary>
        FoundNewVersion,
        /// <summary>
        /// 新版本且要更新
        /// </summary>
        NewVersionAndUpdate,
        /// <summary>
        /// 是否开始下载资源
        /// </summary>
        StartDownLoad,
    }

    //==============================HotUpdate=====================
    public class HotPatchManager : Singleton<HotPatchManager>
    {
        HotPatchManager() { }

        public ABAssetsState MAssetState { get; private set; } = ABAssetsState.None;
        /// <summary>
        /// 获取资源下载地址（apk或xml）
        /// </summary>
        public const string GetServerXmlOrApkUrl = "https://subscribe.game520.com/moling_ver/?op=info";
        //UDID=123&AppVer=1.0.0&Channel=common&Platform=android
        public string ServerUrl = "https://imgcn.game520.com/unity/dragonrun/";
        /// <summary>
        /// 补丁下载地址头
        /// </summary>
        private string PathcDownLoadHead = "";

        /// <summary>
        /// 补丁包文件下载地址
        /// </summary>
        public static string mDownLoadPath { get; } = Application.persistentDataPath + "/DownLoad";

        /// <summary>
        /// 资源解压路径
        /// </summary>
        public static string mUnPackPath { get; } = Application.persistentDataPath + "/Origin";


        private string m_curversion;

        /// <summary>
        /// 当前版本
        /// </summary>
        public string CurrentVersion
        {
            get
            {
                if (string.IsNullOrEmpty(m_curversion))
                    ReadLocalVersion();
                return m_curversion;
            }
        }

        /// <summary>
        /// 当前热更资源版本
        /// </summary>
        public string CurResVersion = "";

        /// <summary>
        /// 当前渠道
        /// </summary>
        public string CurChannel
        {
            get; private set;
        }

        /// <summary>
        /// 当前包名
        /// </summary>
        public string CurrentPackageName
        {
            get; private set;
        }


        public bool mIsBusiness => CurrentVersion == "7.7.7";

        /// <summary>
        /// 当前游戏的资源补丁
        /// </summary>
        public GameVersion MGameVersion { get; private set; }
        /// <summary>
        /// 当前热更的补丁包
        /// </summary>
        public Pathces m_CurrentPatches { get; private set; }
        /// <summary>
        ///将服务器的热更配置文件下载到本地存储的地址
        /// </summary>
        private string m_serverXMLPath = Application.persistentDataPath + "/ServerInfo.xml";
        /// <summary>
        /// 本地下载的资源配置文件 用来做断点续传检测
        /// </summary>
        private string m_LocalXmlPath = Application.persistentDataPath + "/LocalInfo.xml";
        /// <summary>
        /// 本地资源配置信息
        /// </summary>
        private ServerInfo m_LocalInfo;

        /// <summary>
        /// 从服务器获取的所有需要热更的资源
        /// </summary>
        private Dictionary<string, Patch> m_HotFixDic = new Dictionary<string, Patch>();
        /// <summary>
        /// 所有需要下载的资源
        /// </summary>
        private List<Patch> m_DownLoadPatchList = new List<Patch>();

        /// <summary>
        /// 存储需要下载的资源 字典 方便查找
        /// </summary>
        private Dictionary<string, Patch> m_DownLoadPatchDIc = new Dictionary<string, Patch>();
        /// <summary>
        /// 服务器上需要下载的资源Md5码 用于下载后的MD5校验
        /// </summary>
        private Dictionary<string, string> m_NeedDownLoadMD5Dic = new Dictionary<string, string>();
        /// <summary>
        /// 计算解压的文件list
        /// </summary>
        private List<string> m_UnPackFileList = new List<string>();
        /// <summary>
        /// 原包记录的md5码 stramingassets目录下的
        /// </summary>
        private Dictionary<string, ABMD5Base> m_PackedMd5Dic = new Dictionary<string, ABMD5Base>();

        /// <summary>
        /// 服务器列表获取错误回调
        /// </summary>
        public Action ServerInfoErrorCallBack;
        /// <summary>
        /// 解压资源错误回调
        /// </summary>
        public Action UnPackFileErrorCallBack;

        /// <summary>
        /// 资源下载失败回调
        /// </summary>
        public Action<string> DownLoadItemErrorCallBack;
        /// <summary>
        /// 资源下载完成回调
        /// </summary>
        public Action DownLoadOverCallback;
        /// <summary>
        /// 存储已经下载的资源
        /// </summary>
        public List<Patch> m_AlreadyDownList = new List<Patch>();


        /// <summary>
        /// 当前正在下载的资源
        /// </summary>
        private DownLoadAssetBundle m_curDownLoadAB = null;


        /// <summary>
        /// 重新下载次数
        /// </summary>
        private int m_currentReTryDownLoadCount = 0;
        /// <summary>
        /// 最大重新下载次数
        /// </summary>
        private const int MaxReTryDownLoadCount = 4;

        /// <summary>
        /// 需要下载资源的个数
        /// </summary>
        public int LoadFileCount { get; private set; } = 0;
        /// <summary>
        /// 需要下载的资源总大小
        /// </summary>
        public float LoadSumSize { get; private set; } = 0;



        /// <summary>
        /// 服务端是否没有找到当前版本
        /// </summary>
        public string NewVersionUrl { get; private set; } = string.Empty;


        /// <summary>
        /// 解压文件总大小
        /// </summary>
        public float UnPackSumSize { get; private set; } = 0;
        /// <summary>
        /// 已解压文件大小
        /// </summary>
        public float AlreadyUnPackSumSize { get; private set; } = 0;

        /// <summary>
        /// 服务器配置文件解析数据
        /// </summary>
        public ServerInfo MServerHotConfigInfo
        {
            get; private set;
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        private MonoBehaviour m_mono;


        public void Init(MonoBehaviour mono)
        {
            this.m_mono = mono;
            ReadMd5();
        }

        /// <summary>
        /// 读取本地资源md5码
        /// </summary>
        void ReadMd5()
        {
            m_PackedMd5Dic.Clear();
            TextAsset md5 = Resources.Load<TextAsset>("ABMD5");
            if (md5 == null) Debug.LogError("未读取到本地MD5配置文件");

            using (MemoryStream stream = new MemoryStream(md5.bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                ABMD5 aBMD5 = bf.Deserialize(stream) as ABMD5;
                foreach (ABMD5Base abmdsbase in aBMD5.ABMD5List)
                {
                    m_PackedMd5Dic.Add(abmdsbase.Name, abmdsbase);
                }
            }
        }



        /// <summary>
        /// 计算解压文件
        /// </summary>
        /// <returns>是否需要解压</returns>
        public bool ComputeUnPackFile()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Directory.Exists(mUnPackPath))
                Directory.CreateDirectory(mUnPackPath);

            m_UnPackFileList.Clear();
            UnPackSumSize = 0;
            foreach (string filename in m_PackedMd5Dic.Keys)
            {
                string filepath = mUnPackPath + "/" + filename;
                if (File.Exists(filepath))//校验md5码
                {
                    string md5 = MD5Manager.Instance.BuildFileMd5(filepath);
                    if (m_PackedMd5Dic[filename].Md5 != md5 && !m_UnPackFileList.Contains(filename))
                        m_UnPackFileList.Add(filename);
                }
                else if(!m_UnPackFileList.Contains(filename))
                    m_UnPackFileList.Add(filename);
            }

            foreach (string filename in m_UnPackFileList)
            {
                if (m_PackedMd5Dic.ContainsKey(filename))
                    UnPackSumSize += m_PackedMd5Dic[filename].Size;
            }
            return m_UnPackFileList.Count > 0;
#else
            return false;
#endif
        }

        /// <summary>
        /// 获取解压进度
        /// </summary>
        /// <returns></returns>
        public float GetUnPackProgress()
        {
            return this.AlreadyUnPackSumSize / this.UnPackSumSize;
        }

        /// <summary>
        /// 开始解压所有AB资源文件
        /// </summary>
        public void StartUnPackFile(Action callback)
        {
            MAssetState = ABAssetsState.StartUnPack;
            m_mono.StartCoroutine(UnPackToPersistent(callback));
        }

        /// <summary>
        /// 解压文件到本地存储目录
        /// </summary>
        /// <returns></returns>
        IEnumerator UnPackToPersistent(Action callback)
        {
            foreach (string filename in m_UnPackFileList)
            {
                yield return  UnPackSingleABFile(filename,null,UnPackFileErrorCallBack);
                if (m_PackedMd5Dic.ContainsKey(filename))
                    AlreadyUnPackSumSize += m_PackedMd5Dic[filename].Size;
            }
            callback?.Invoke();
        }

        /// <summary>
        /// 解压指定AB资源
        /// </summary>
        public void StartUnPackSingleABFile(string abname, Action successcall,Action failcallback)
        {
             m_UnPackFileList.Add(abname);
            m_mono.StartCoroutine(UnPackSingleABFile(abname, successcall,failcallback));
        }

        /// <summary>
        /// 解压指定AB资源
        /// </summary>
        IEnumerator UnPackSingleABFile(string abname, Action callback, Action failcallback)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(Application.streamingAssetsPath + "/AssetBundles/" + abname);
            webRequest.timeout = 30;
            yield return webRequest.SendWebRequest();
            if (webRequest.result== UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("UnPackSingleABFile  has error " + webRequest.error + " file name=" + abname);
                failcallback?.Invoke();
            }
            else
            {
                byte[] bytes = webRequest.downloadHandler.data;
                string unoackfilename = mUnPackPath + "/" + abname;
                if (!CreateUnPackOrDownLoadFile(unoackfilename, bytes))
                {
                    Debug.LogError("UnPackSingleABFile  create file error file name=" + abname);
                    failcallback?.Invoke();
                }
            }
            webRequest.Dispose();
            callback?.Invoke();
        }


        /// <summary>
        /// 创建解压资源或者下载的更新资源
        /// </summary>
        bool CreateUnPackOrDownLoadFile(string filename, byte[] filebytes)
        {
            try
            {
                FileTool.CreateFile(filename, filebytes);
                return true;
            }
            catch (Exception e)
            {
                MyDebuger.LogError("CreateUnPackOrDownLoadFile fail " + filename + " error " + e.ToString());
            }
            return false;
        }


        /// <summary>
        /// 计算AB包路径 如果是热更文件 则返回存储路径 否则返回空
        /// </summary>
        /// <returns></returns>
        public string ComputeABPath(string patchName)
        {
            Patch patch = null;
            m_HotFixDic.TryGetValue(patchName, out patch);
            if (patch != null)
                return mDownLoadPath + "/" + patchName;
            return string.Empty;
        }


        /// <summary>
        /// 获取服务器热更版本文件信息
        /// </summary>
        public void StartGetAPKorHotResServerXML(Action<string> ongetover)
        {
            CurChannel = Application.identifier;
            this.m_mono.StartCoroutine(GetAPKOrHotResServerXML(ongetover));
        }


        /// <summary>
        /// 获取服务端XML文件
        /// </summary>
        /// <param name="chanel"></param>
        /// <returns></returns>
        IEnumerator GetAPKOrHotResServerXML(Action<string> ongetover)
        {
            this.MAssetState = ABAssetsState.GetServerXml;
            ReadLocalVersion();
            SendServerXmlOrApkInfo xmlOrApkInfo = new SendServerXmlOrApkInfo();
            xmlOrApkInfo.UDID = SystemInfo.deviceUniqueIdentifier;
            xmlOrApkInfo.AppVer = this.CurrentVersion;
            xmlOrApkInfo.Channel = CurChannel;
            xmlOrApkInfo.Platform = "android";
            switch (Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    xmlOrApkInfo.Platform = "iOS";
                    break;
                case RuntimePlatform.Android:
                    xmlOrApkInfo.Platform = "android";
                    break;
                default:
                    Debug.Log(" GetServerXML not found " + Application.platform);
                    break;
            }
            string url = GetServerXmlOrApkUrl + "&UDID=" + xmlOrApkInfo.UDID + "&AppVer=" + xmlOrApkInfo.AppVer + "&Channel=" + xmlOrApkInfo.Channel + "&Platform=" + xmlOrApkInfo.Platform;
            UnityWebRequest unityWeb = UnityWebRequest.Get(url);
            yield return unityWeb.SendWebRequest();
            if (unityWeb.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("DownLoad ServerConfig Error " + unityWeb.error + " url " + url);
                yield return new WaitForSeconds(1f);
                StartGetAPKorHotResServerXML(ongetover);
            }
            else
            {
                string text = unityWeb.downloadHandler.text;
                GetServerXmlOrApkInfo data = LitJson.JsonMapper.ToObject<GetServerXmlOrApkInfo>(text);
                MyDebuger.Log(" Get ServerUrl url=" + url + " data=" + data);
                if (data == null)
                {
                    yield return new WaitForSeconds(1f);
                    StartGetAPKorHotResServerXML(ongetover);
                }
                else
                {
                    this.ServerUrl = data.ResURL;
                    MyDebuger.Log($"Get ServerUrl from {url} serverurl {ServerUrl} " );
                    //有新版本
                    if (data.NewVersion == 1)
                    {
                        this.NewVersionUrl = data.AppURL;
                        MyDebuger.Log("Get NewVersionUrl " + NewVersionUrl);
                        if (data.Force == 1)
                            this.MAssetState = ABAssetsState.NewVersionAndUpdate;
                        else
                            this.MAssetState = ABAssetsState.FoundNewVersion;
                    }
                    else
                        this.MAssetState = ABAssetsState.NoNewVersion;

                    ongetover?.Invoke(CheckServerAndNewVersionUrl());
                }
            }
        }


        string CheckServerAndNewVersionUrl()
        {
            string error = string.Empty;
            if (!string.IsNullOrEmpty(ServerUrl))
            {
                int serverindex = ServerUrl.IndexOf("ServerInfo.xml");
                if (serverindex > -1)
                {
                    PathcDownLoadHead = ServerUrl.Remove(serverindex);
                    MyDebuger.Log("PathcDownLoadHead " + PathcDownLoadHead);
                }
                else
                    error += "Get ServerUrl " + ServerUrl + " notfound ServerInfo.xml";

                int channelindex = ServerUrl.IndexOf(CurChannel);
                int versionindex = ServerUrl.IndexOf(CurrentVersion);
                if (channelindex < 0)
                    error += "Get ServerUrl " + ServerUrl + " notfound cur channel  " + CurChannel + " 请检查服务端资源下载配置文件Url设置 与当前渠道设置不统一";
                if (versionindex < 0)
                    error += "Get ServerUrl " + ServerUrl + " notfound cur version  " + CurrentVersion + " 请检查服务端资源下载配置文件Url设置 与当前版本设置不统一";
            }

            if (!string.IsNullOrEmpty(NewVersionUrl))
            {
                int channelindex = NewVersionUrl.IndexOf(CurChannel);
                if (channelindex < 0)
                    error += "Get NewVersionUrl " + NewVersionUrl + " notfound cur channel  " + CurChannel + " 请检查服务端APK下载配置文件Url设置 与当前渠道设置不统一";
            }
            return error;
        }


        /// <summary>
        /// 检查版本
        /// </summary>
        /// <param name="hotCallBack"></param>
        public void CheckVersion(Action<bool> hotCallBack = null)
        {
            MyDebuger.Log("CheckVersion CurrentVersion " + CurrentVersion);
            m_HotFixDic.Clear();
            m_currentReTryDownLoadCount = 0;
            m_mono.StartCoroutine(ReadServerXML(() =>
            {
                if (MServerHotConfigInfo == null)
                {
                    ///下载服务器配置/解析服务器配置出错回调
                    ServerInfoErrorCallBack?.Invoke();
                    return;
                }

                if (CurrentVersion != MServerHotConfigInfo.GameVersion.Version)
                    MyDebuger.LogError("请检查配置的版本号 当前版本 " + MGameVersion + " 服务器上的版本 " + MServerHotConfigInfo.GameVersion);

                MGameVersion = MServerHotConfigInfo.GameVersion;

                //获取服务器上所有需要热更的ab包
                GetHotPatchAB();
                ///需要下载
                if (CheckLocalAndServerPatch())
                {   //计算下载资源
                    ComputeDownLoad();
                    if (File.Exists(m_serverXMLPath))
                    {
                        if (File.Exists(m_LocalXmlPath))
                            File.Delete(m_LocalXmlPath);
                        File.Move(m_serverXMLPath, m_LocalXmlPath);
                    }
                }//不需要下载
                else
                    ComputeDownLoad();

                this.LoadFileCount = m_DownLoadPatchList.Count;
                this.LoadSumSize = m_DownLoadPatchList.Sum(patch => patch.Size);
                hotCallBack?.Invoke(m_DownLoadPatchList.Count > 0);

            }));
        }


        /// <summary>
        /// 校验本地和服务器的补丁配置文件  本地是否下载过补丁
        /// </summary>
        /// <returns>是否需要热更</returns>
        bool CheckLocalAndServerPatch()
        {
            ///从来没有下载过热更
            if (!File.Exists(m_LocalXmlPath))
                return true;
            m_LocalInfo = BinarySerializeOpt.XmlDeserialize<ServerInfo>(m_LocalXmlPath);

            GameVersion localGameVersion = null;
            if (m_LocalInfo != null)
            {
                if (m_LocalInfo.GameVersion.Version == CurrentVersion)
                    //获取当前版本信息
                    localGameVersion = m_LocalInfo.GameVersion;
            }

            //和服务器拉取的热更信息 进行对比
            if (localGameVersion != null && localGameVersion.Pathces != null && localGameVersion.Pathces.Length > 0 && MGameVersion.Pathces != null && MGameVersion.Pathces.Length > 0 && MGameVersion.Pathces[MGameVersion.Pathces.Length - 1].Version != localGameVersion.Pathces[localGameVersion.Pathces.Length - 1].Version)
                return true;
            return false;
        }


        /// <summary>
        /// 读取当前所处的应用版本(打包是已经写入到resource 目录下)
        /// </summary>
        void ReadLocalVersion()
        {
            if (!string.IsNullOrEmpty(m_curversion))
            {
                return;
            }
            TextAsset vesiontex = Resources.Load<TextAsset>("Version");
            if (vesiontex == null)
            {
                Debug.LogError("未读取到本地版本");
                return;
            }
            HotUpdateVersionInfo versionInfo = null;
            using (MemoryStream stream = new MemoryStream(vesiontex.bytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                versionInfo = bf.Deserialize(stream) as HotUpdateVersionInfo;
            }
            if (versionInfo == null)
            {
                Debug.LogError("解析本地版本信息文件失败 ");
                return;
            }
            m_curversion = versionInfo.Version;
            CurResVersion = CurrentVersion + "_0";
            CurrentPackageName = versionInfo.PackageName;
        }


        /// <summary>
        ///下载并读取服务器XML配置文件
        /// </summary>
        /// <param name="callBack"></param>
        /// <returns></returns>
        IEnumerator ReadServerXML(Action callBack)
        {
            string xmlUrl = ServerUrl + "?" + DateTime.Now.Ticks;
            MyDebuger.Log("ReadServerXML " + xmlUrl);
            UnityWebRequest www = UnityWebRequest.Get(xmlUrl);
            www.timeout = 30;
            yield return www.SendWebRequest();

            //下载报错=================
            if (www.result == UnityWebRequest.Result.ConnectionError)
                Debug.LogError("DownLoad ServerConfig Error " + www.error);
            else
            {
                //存储服务器热更配置文件
                FileTool.CreateFile(m_serverXMLPath, www.downloadHandler.data);
                if (File.Exists(m_serverXMLPath))
                    //xml反序列化
                    MServerHotConfigInfo = BinarySerializeOpt.XmlDeserialize<ServerInfo>(m_serverXMLPath);
                else
                    Debug.LogError("热更配置错误 服务器热更文件下载失败 本地未找到服务器热更配置文件");
            }
            callBack?.Invoke();
        }

        /// <summary>
        /// 获取当前热更补丁的AB资源
        /// </summary>
        void GetHotPatchAB()
        {
            if (MGameVersion != null && MGameVersion.Pathces != null && MGameVersion.Pathces.Length > 0)
            {
                ///最后一个补丁包 也是最新的补丁包
                m_CurrentPatches = MGameVersion.Pathces[MGameVersion.Pathces.Length - 1];
                CurResVersion = CurrentVersion + "_" + m_CurrentPatches.Version;
                if (m_CurrentPatches != null && m_CurrentPatches.Files != null)
                {
                    foreach (Patch patch in m_CurrentPatches.Files)
                        m_HotFixDic.Add(patch.Name, patch);
                }
            }
        }

        /// <summary>
        /// 计算需要下载的资源
        /// </summary>
        void ComputeDownLoad()
        {
            m_DownLoadPatchDIc.Clear();
            m_DownLoadPatchList.Clear();
            m_NeedDownLoadMD5Dic.Clear();
            if (MGameVersion != null && MGameVersion.Pathces != null && MGameVersion.Pathces.Length > 0)
            {
                m_CurrentPatches = MGameVersion.Pathces[MGameVersion.Pathces.Length - 1];
                if (m_CurrentPatches.Files != null && m_CurrentPatches.Files.Count > 0)
                {
                    foreach (Patch patch in m_CurrentPatches.Files)
                    {
                        if ((Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("StandaloneWindows64"))
                            AddPatchToDownLoadList(patch);
                        else if ((Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("Android"))
                            AddPatchToDownLoadList(patch);
                        else if ((Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.WindowsEditor) && patch.Platform.Contains("IOS"))
                            AddPatchToDownLoadList(patch);
                    }
                }
            }
        }

        /// <summary>
        /// 加入到下载队列
        /// </summary>
        /// <param name="patch"></param>
        void AddPatchToDownLoadList(Patch patch)
        {
            string filePath = mDownLoadPath + "/" + patch.Name;
            ///存在文件进行对比 查看md5码是否一致 不一致加入到下载队列
            if (File.Exists(filePath))
            {
                string md5 = MD5Manager.Instance.BuildFileMd5(filePath);
                if (patch.MD5 != md5)
                {
                    m_DownLoadPatchList.Add(patch);
                    this.m_DownLoadPatchDIc.Add(patch.Name, patch);
                    this.m_NeedDownLoadMD5Dic.Add(patch.Name, patch.MD5);
                }
            }
            else
            {
                m_DownLoadPatchList.Add(patch);
                this.m_DownLoadPatchDIc.Add(patch.Name, patch);
                this.m_NeedDownLoadMD5Dic.Add(patch.Name, patch.MD5);
            }
        }

        /// <summary>
        /// 获取下载进度
        /// </summary>
        /// <returns></returns>
        public float GetProgress()
        {
            return GetLoadSize() / LoadSumSize;
        }

        /// <summary>
        /// 获取已经下载的大小
        /// </summary>
        /// <returns></returns>
        public float GetLoadSize()
        {
            float alreadySize = m_AlreadyDownList.Sum(x => x.Size);
            float currentSize = 0;

            Patch patch = FindPatchByGamePath(m_curDownLoadAB.FileName);
            if (patch != null && !m_AlreadyDownList.Contains(patch))
            {
                currentSize = this.m_curDownLoadAB.GetProgress() * patch.Size;
            }
            return alreadySize + currentSize;
        }


        /// <summary>
        /// 开始下载AB包
        /// </summary>
        /// <returns></returns>
        public IEnumerator StartDownLoadAB(Action callBack, List<Patch> allPatch = null)
        {
            m_AlreadyDownList.Clear();
            MAssetState = ABAssetsState.StartDownLoad;
            if (allPatch == null)//指定下载资源
                allPatch = m_DownLoadPatchList;

            if (!Directory.Exists(mDownLoadPath))
                Directory.CreateDirectory(mDownLoadPath);

            ///资源下载列表
            List<DownLoadAssetBundle> downLoadAssetBundles = new List<DownLoadAssetBundle>();
            foreach (Patch patch in allPatch)
            {
                DownLoadAssetBundle downLoadAsset = new DownLoadAssetBundle(this.PathcDownLoadHead + patch.Url, mDownLoadPath);
                downLoadAssetBundles.Add(downLoadAsset);
            }

            ///下载所有文件
            foreach (DownLoadAssetBundle download in downLoadAssetBundles)
            {
                m_curDownLoadAB = download;
                yield return m_mono.StartCoroutine(download.DownLoad());
                Patch patch = FindPatchByGamePath(download.FileName);
                if (patch != null)
                    m_AlreadyDownList.Add(patch);
                download.Destory();
            }
            ///资源下载完成后 MD5码校验 如果校验没有通过  自动下载没有通过的文件， 重复下载计数 达到一定次数后 反馈具体文件下载失败
            VertifyMD5(downLoadAssetBundles, callBack);
        }

        /// <summary>
        /// 校验MD5码 和服务器拉取到的配置文件中的Md5进行校验
        /// </summary>
        /// <param name="downLoadAssetBundles"></param>
        /// <param name="callBack"></param>
        void VertifyMD5(List<DownLoadAssetBundle> downLoadAssetBundles, Action callBack)
        {

            List<Patch> downLoadList = new List<Patch>();
            foreach (DownLoadAssetBundle downLoadAssetBundle in downLoadAssetBundles)
            {
                string md5 = "";
                if (m_NeedDownLoadMD5Dic.TryGetValue(downLoadAssetBundle.FileName, out md5))
                {
                    //使用存储路径作为md5进行判断
                    if (MD5Manager.Instance.BuildFileMd5(downLoadAssetBundle.SaveFilePath) != md5)
                    {
                        Debug.LogWarning(string.Format("此文件{0}校验失败 即将重新下载{1}{2}", downLoadAssetBundle.FileName, PathcDownLoadHead, downLoadAssetBundle.Url));
                        Patch patch = FindPatchByGamePath(downLoadAssetBundle.FileName);
                        if (patch != null)
                            downLoadList.Add(patch);
                    }
                }
            }
            ///没有需要下载的资源
            if (downLoadList.Count <= 0)
            {
                MAssetState = ABAssetsState.None;
                m_NeedDownLoadMD5Dic.Clear();
                callBack?.Invoke();
                DownLoadOverCallback?.Invoke();
            }
            //重新下载
            else
            {
                if (this.m_currentReTryDownLoadCount >= MaxReTryDownLoadCount)
                {
                    string allName = "";
                    MAssetState = ABAssetsState.None;
                    foreach (Patch patch in downLoadList)
                        allName += patch.Name + ";";
                    Debug.LogError(string.Format("资源重复下载{0}次都失败,请检查CDN资源配置和版本是否一致{1}  ", MaxReTryDownLoadCount, allName));
                    DownLoadItemErrorCallBack?.Invoke(allName);
                }
                else
                {
                    m_currentReTryDownLoadCount++;
                    m_NeedDownLoadMD5Dic.Clear();
                    foreach (Patch patch in downLoadList)
                        ///重新加入到需要重复服务器下载的补丁字典中
                        m_NeedDownLoadMD5Dic.Add(patch.Name, patch.MD5);
                    //自动重新下载校验失败的文件
                    this.m_mono.StartCoroutine(StartDownLoadAB(callBack, downLoadList));
                }
            }

        }


        /// <summary>
        /// 根据名字查找热更补丁
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Patch FindPatchByGamePath(string name)
        {
            Patch patch = null;
            m_DownLoadPatchDIc.TryGetValue(name, out patch);
            return patch;
        }

    }


    public class FileTool
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="bytes"></param>
        public static void CreateFile(string filepath, byte[] bytes)
        {
            if (File.Exists(filepath))
                File.Delete(filepath);

            FileInfo file = new FileInfo(filepath);
            Stream stream = file.Create();
            stream.Write(bytes, 0, bytes.Length);
            stream.Close();
            stream.Dispose();
        }
    }
}