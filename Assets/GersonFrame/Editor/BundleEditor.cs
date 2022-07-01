using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using GersonFrame.ABFrame;

public class HotUpdateConfig
{
    public string hotcount;
    public string adm5path;
    public string VersionName;
    public string HotDesc;

}

public class BundleEditor
{
    //========================HotUpdate===========================


    /// <summary>
    /// 热更ABMD5版本配置文件信息 之后的热更信息都是以该文件进行对比
    /// </summary>
    private static string m_VersionMd5Path = Application.dataPath + "/../Hot/ABVersionConfig/" + EditorUserBuildSettings.activeBuildTarget.ToString();

    /// <summary>
    /// 热更AB文件及配置文本信息
    /// </summary>
    public static string m_HotPath = Application.dataPath + "/../Hot/ABFile/" + EditorUserBuildSettings.activeBuildTarget.ToString();

    /// <summary>
    /// 热更配置文件中的ABmd5信息数据
    /// </summary>
    private static Dictionary<string, ABMD5Base> m_PackageMd5Dic = new Dictionary<string, ABMD5Base>();

    /// <summary>
    /// 打包文件生成目录
    /// </summary>
    public static string AssetBundlePath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();
    private static string ABCONFIGPATH = ABFrameConfigGeter.Config.AbconfigPath;

    /// <summary>
    /// 所有文件夹ab包dic key是ab包名 value是路径 
    /// </summary>
    private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();

    /// <summary>
    /// 存储文件夹AB包资源路径 用来判断AB包资源重复文件 进行过滤 包含依赖文件
    /// </summary>
    private static List<string> m_AllFileAB = new List<string>();

    /// <summary>
    /// 单个prefab的独立依赖项  key值就是prefab 名字
    /// </summary>
    private static Dictionary<string, List<string>> m_PrefabAllDependsDic = new Dictionary<string, List<string>>();

    /// <summary>
    /// 所有依赖项中引用计数超过2的资源
    /// </summary>
    private static List<string> m_CommonDepends = new List<string>();


    /// <summary>
    /// 储存所有AB资源有效路径（不包含依赖文件） 过滤不需要动态加载的文件
    /// </summary>
    private static List<string> m_ValueableABFilePaths = new List<string>();

    /// <summary>
    /// 资源xml配置字典    key为全路径 value 为包名 用来生成XML配置
    /// </summary>
    static Dictionary<string, string> m_respathDic = new Dictionary<string, string>();

    static ABConfig m_aBConfig = null;



    #region MenueItem


    [MenuItem("MyTools/AB/刷新AB配置文件(编辑器环境下)", false, 20)]
    public static void ReflashABEditorConfigFiles()
    {
        MyDebuger.InitLogger(LogLevel.All);
        try
        {
            SetABsInfo();
            ReflashEditorABConfig();
        }
        catch (System.Exception e)
        {
            Debug.LogError("ReflashABEditorConfigFiles error " + e.ToString());
        }
        EditorUtility.ClearProgressBar();
    }




    [MenuItem("MyTools/AB/打开AB资源文件夹", false, 35)]
    public static void OpenABFileFloder()
    {
        EditorTool.OpenFloder(AssetBundlePath);
    }


    [MenuItem("MyTools/AB/拷贝AB文件到工程", false, 40)]
    public static void CopyABFilesToProject()
    {
        DeleteMainfest();
        EditorTool.DeleteDir(Application.streamingAssetsPath + "/AssetBundles");
        string abPath = AssetBundlePath + "/";
        string targetpath = Application.streamingAssetsPath + "/AssetBundles/";
        EditorTool.Copy(abPath, targetpath);
        AssetDatabase.Refresh();
    }


    [MenuItem("MyTools/AB/打开本地拷贝AB文件夹")]
    private static void OpenOrilABFloder()
    {
        EditorTool.OpenFloder(Application.persistentDataPath + "/Origin");
    }


    [MenuItem("MyTools/热更/打开热更AB资源文件夹", false, 45)]
    private static void OpenHotABFileFloder()
    {
        EditorTool.OpenFloder(m_HotPath);
    }


    [MenuItem("MyTools/热更/打开更ABMD5配置信息文件夹", false, 50)]
    private static void OpenHotABMd5InfoFileFloder()
    {
        EditorTool.OpenFloder(m_VersionMd5Path);
    }

    #endregion


    /// <summary>
    /// 创建AB包
    /// </summary>
    /// <param name="hotfix"></param>
    /// <param name="adm5path"></param>
    /// <param name="hotcount"></param>
    public static void BuildAB(BuildTarget buildTarget)
    {
        //ClearABNamesAndProgress();
        EditorTool.DeleteDir(AssetBundlePath);
        if (!CommonBuildAB()) return;
        //  构建AssetBundle
        BuildAssetBundle(buildTarget);
        WriteABMD5();
    }

    /// <summary>
    /// build的共性
    /// </summary>
    /// <returns></returns>
    static bool CommonBuildAB()
    {
        MyDebuger.InitLogger(LogLevel.All);
        m_aBConfig = null;
        SetABsInfo();
        SetABNames();
        ///刷新AB配置文件
        if (!ReflashABConfigFileInfo())
            return false;
        //生成 AB包配置表
        WriteABByteInfoConfig();
        SetAssetbundleBytesConfigAB();
        return true;
    }

    /// <summary>
    /// 创建热更AB包 1.对比原始文件记录需要重新打包的文件 2设置所有ab包 3.检查包名 文件名是否重复 4.写入配置文件 5.设置需要重新打包的文件 6打包 7.生成包变更信息配置文本
    /// </summary>
    /// <param name="hotfix"></param>
    /// <param name="adm5path"></param>
    /// <param name="hotcount"></param>
    public static bool BuildABHot(HotUpdateConfig hotUpdateConfig,BuildTarget buildTarget)
    {
        if (!CommonBuildAB())
            return false;
        // 构建AssetBundle
        BuildAssetBundle(buildTarget);
        ReadMD5Info(hotUpdateConfig.adm5path, hotUpdateConfig.hotcount, hotUpdateConfig.VersionName,hotUpdateConfig.HotDesc);
        Application.OpenURL("file://" + m_HotPath);
        return true;
    }



    /// <summary>
    /// 设置AB包信息
    /// </summary>
    static void SetABsInfo()
    {
        m_ValueableABFilePaths.Clear();
        m_PrefabAllDependsDic.Clear();
        m_CommonDepends.Clear();
        m_AllFileAB.Clear();
        m_AllFileDir.Clear();
        m_aBConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
        //文件夹打包
        foreach (ABConfig.FileDirABName filedir in m_aBConfig.m_AllFileDirAB)
        {
            string lowerabname = filedir.ABName.ToLower();
            if (m_AllFileDir.ContainsKey(lowerabname))
                Debug.LogError(string.Format("ab包 {0} 名字配置重复 请注意包名大小写 所有包名需要用小写 path{1}", lowerabname, filedir.Path));
            else
            {
                m_AllFileDir.Add(lowerabname, filedir.Path);
                m_AllFileAB.Add(filedir.Path);
                m_ValueableABFilePaths.Add(filedir.Path);
            }
        }

        if (m_aBConfig.m_PrefabsFilePath.Count < 1)
            return;

        Dictionary<string, uint> commondependenice = new Dictionary<string, uint>();
        string[] allstr = AssetDatabase.FindAssets("t:Prefab", m_aBConfig.m_PrefabsFilePath.ToArray());
        //所有 prefab 资源路径guid
        for (int i = 0; i < allstr.Length; i++)
        {
            //guid 转路径
            string path = AssetDatabase.GUIDToAssetPath(allstr[i]);
            EditorUtility.DisplayCancelableProgressBar("查找prefab ", path, i * 1.0f / allstr.Length);
            m_ValueableABFilePaths.Add(path);

            //找到prefab所有依赖项
            if (!IsContainAllFileAB(path))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] alldepend = AssetDatabase.GetDependencies(path);
    
                if (alldepend==null)
                    continue;

                //获取所有依赖项  会包含脚本 包含自身
               
                List<string> alldependPathList = new List<string>();
                for (int j = 0; j < alldepend.Length; j++)
                {
                    //判断依赖项是否在文件夹ab包资源站中且不是脚本
                    if (!alldepend[j].EndsWith(".cs"))
                    {
                        if (!IsContainAllFileAB(alldepend[j]))
                        {
                            //添加到过滤列表
                            m_AllFileAB.Add(alldepend[j]);
                            //加入当前预制体的依赖队列
                            alldependPathList.Add(alldepend[j]);
                        }
                        //是否已经被加入
                        bool added = false;
                        foreach (ABConfig.FileDirABName filedir in m_aBConfig.m_AllFileDirAB)
                        {
                            if (alldepend[j].Contains(filedir.Path))
                            {
                                added = true;
                                break;
                            }
                        }

                        if (!added)
                        {
                            if (!commondependenice.ContainsKey(alldepend[j]))
                                commondependenice[alldepend[j]] = 1;
                            else
                            {
                                uint dependicecount = commondependenice[alldepend[j]]++;
                                if (dependicecount > 1 && !m_CommonDepends.Contains(alldepend[j]))
                                {
                                    m_CommonDepends.Add(alldepend[j]);
                                    m_ValueableABFilePaths.Add(alldepend[j]);
                                }
                            }
                        }
                    }
                }

                string lowerAbName = obj.name.ToLower();
                if (m_AllFileDir.ContainsKey(lowerAbName))
                    Debug.LogError(string.Format("ab包 {0} 名字配置重复 请注意包名大小写 所有包名需要用小写 path{1}", lowerAbName, path));

                //预制体和资源文件的依赖关系
                if (m_PrefabAllDependsDic.ContainsKey(lowerAbName))
                {
                    //打印重名文件和路径
                    for (int l = m_aBConfig.m_PrefabsFilePath.Count - 1; l >= 0; l--)
                    {
                        if (path.IndexOf(m_aBConfig.m_PrefabsFilePath[l]) > -1)
                        {
                            Debug.LogError("存在相同名字的 prefab " + lowerAbName + " 文件 " + path + " 所在AB包路径 " + m_aBConfig.m_PrefabsFilePath[l]);
                            break;
                        }
                    }
                }
                else
                    m_PrefabAllDependsDic.Add(lowerAbName, alldependPathList);
            }
        }
        MyDebuger.Log("m_PrefabAllDependDir  " + m_PrefabAllDependsDic.Count + " m_AllFileDir " + m_AllFileDir.Count);
    }

    /// <summary>
    /// 构建assetbundle文件
    /// </summary>
    static void BuildAssetBundle(BuildTarget buildTarget)
    {
        if (!Directory.Exists(AssetBundlePath))
            Directory.CreateDirectory(AssetBundlePath);
        ///兼容小游戏 WEBGL
        
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(AssetBundlePath, BuildAssetBundleOptions.ChunkBasedCompression, buildTarget);
        if (manifest == null)
            MyDebuger.LogError("Asset Bundle 打包失败");
        else
            MyDebuger.Log("Asset Bundle 打包完毕");

        DeleteUnUseAsseBundle();
        EncryptAB();
    }

    [MenuItem("MyTools/AB/加密AB包")]
    public static void EncryptAB()
    {
        DirectoryInfo directory = new DirectoryInfo(AssetBundlePath);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        if (files.Length < 1)
        {
            Debug.LogWarning("没有可以加密的文件");
            return;
        }
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest"))
            {
                continue;
            }
            AES.AESFileEncrypt(files[i].FullName, "Gerson");
        }
        Debug.Log("AB包加密完毕");
    }

    [MenuItem("MyTools/AB/解密AB包")]
    public static void DecryptAB()
    {
        DirectoryInfo directory = new DirectoryInfo(AssetBundlePath);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest"))
                continue;
            AES.AESFileDecrypt(files[i].FullName, "Gerson");
        }
        Debug.Log("AB包解密完毕");
    }


    /// <summary>
    /// 删除manifest 文件
    /// </summary>
    static void DeleteMainfest()
    {
        DirectoryInfo directory = new DirectoryInfo(AssetBundlePath);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".manifest"))
                File.Delete(files[i].FullName);
        }
    }

    //============================刷新AB配置文件=========================
    /// <summary>
    /// 刷新编辑器下的AB配置信息
    /// </summary>
    static void ReflashEditorABConfig()
    {
        m_respathDic.Clear();

        foreach (var name in m_AllFileDir.Keys)
        {
            string path = m_AllFileDir[name];
            SetEditorABPath(path, name);
        }

        for (int i = 0; i < m_CommonDepends.Count; i++)
            SetEditorABPath(m_CommonDepends[i], "commonpendences");

        //设置prefab ab 包名
        foreach (var name in m_PrefabAllDependsDic.Keys)
        {
            List<string> dependences = m_PrefabAllDependsDic[name];
            for (int i = 0; i < dependences.Count; i++)
                SetEditorABPath(dependences[i], name);
        }

        AssetBundleConfig assetBundleConfig = GeneralAssetBundleConfigfiles();
        //写入xml
        string xmlpath = ABFrameConfigGeter.Config.AssetbundleXMLConfigPath;
        if (File.Exists(xmlpath)) File.Delete(xmlpath);
        EditorTool. WriteXMLFileInfo(xmlpath, assetBundleConfig);
    }

    static void SetEditorABPath(string path, string abName)
    {
        if (!Directory.Exists(path))
        {
            if (!m_respathDic.ContainsKey(path))
                m_respathDic.Add(path, abName);
        }
        else
        {
            string[] fileList = Directory.GetFileSystemEntries(path);
            foreach (string file in fileList)
            {
                if (Directory.Exists(file))
                    SetEditorABPath(file, abName);
                else
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(path);
                    string filename = Path.GetFileName(file);
                    if (filename.EndsWith(".cs") || filename.EndsWith(".meta"))
                        continue;
                    stringBuilder.Append("/");
                    stringBuilder.Append(filename);
                    stringBuilder.Replace("\\", "/");
                    string filepath = stringBuilder.ToString();
                    if (m_respathDic.ContainsKey(filepath))
                        MyDebuger.LogError(filepath + " 已经加入过打包文件 请检查是否包名重复 " + abName);
                    else
                        m_respathDic.Add(filepath, abName);
                }
            }
        }
    }

    /// <summary>
    /// 刷新AB配置信息
    /// </summary>
    static bool ReflashABConfigFileInfo()
    {
        m_respathDic.Clear();
        bool reflashsuccess = true;
        string[] allBundleNames = AssetDatabase.GetAllAssetBundleNames();

        for (int i = 0; i < allBundleNames.Length; i++)
        {
            List<string> assetnamelist = new List<string>();
            string abpath = "";
            m_AllFileDir.TryGetValue(allBundleNames[i], out abpath);
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundleNames[i]);
            for (int j = 0; j < allBundlePath.Length; j++)
            {
                if (allBundlePath[j].EndsWith(".cs"))
                    continue;
                string assetname = GetAssetName(allBundlePath[j]);
                if (!string.IsNullOrEmpty(abpath) && allBundlePath[j].Contains(abpath) && assetnamelist.Contains(assetname))
                {
                    MyDebuger.LogError(string.Format("此AB包:{0} 资源名重复: {1} 资源路径:{2}", allBundleNames[i], assetname, allBundlePath[j]));
                    reflashsuccess = false;
                }
                else
                {
                    assetnamelist.Add(assetname);
                    m_respathDic.Add(allBundlePath[j], allBundleNames[i]);
                }
            }
        }
        if (!reflashsuccess)
            MyDebuger.LogError("Build AB File Fail");
        return reflashsuccess;
    }

    //============================================================

    /// <summary>
    /// 获取需要打包的ab配置文件信息
    /// </summary>
    /// <returns></returns>
    static AssetBundleConfig GeneralAssetBundleConfigfiles()
    {
        AssetBundleConfig assetBundleConfig = new AssetBundleConfig();
        assetBundleConfig.ABList = new List<ABBase>();

        foreach (string path in m_respathDic.Keys)
        {
            if (!ValidPath(path))
                continue;
            ABBase aBBase = new ABBase();
            aBBase.Path = path;
            aBBase.Crc = Crc32.GetCrc32(path);
            aBBase.ABName = m_respathDic[path].ToLower();
            aBBase.AssetName = GetAssetName(path);
            aBBase.ABDependences = new List<string>();
            //获取所有依赖项
            string[] resdependence = AssetDatabase.GetDependencies(path);

            if (resdependence == null)
                continue;
            for (int i = 0; i < resdependence.Length; i++)
            {
                string tempPath = resdependence[i];
                //排除自己和 cs脚本
                if (tempPath == path || tempPath.EndsWith(".cs"))
                    continue;
                string abName = "";
                //依赖项处理 获取所在的AB包
                if (m_respathDic.TryGetValue(tempPath, out abName))
                {
                    //如果依赖包就是自身 依赖项不包含自身
                    if (abName == m_respathDic[path])
                        continue;
                    //对多个依赖的文件源自同一个AB包中进行赛选 只添加一次
                    if (!aBBase.ABDependences.Contains(abName))
                        aBBase.ABDependences.Add(abName);
                }
            }
            assetBundleConfig.ABList.Add(aBBase);
        }
        return assetBundleConfig;
    }


    static string GetAssetName(string path)
    {
        return path.Remove(0, path.LastIndexOf("/") + 1);
    }


    /// <summary>
    /// 将配置信息写入AB配置文件
    /// </summary>
    public static void WriteABByteInfoConfig()
    {
        //写入Xml
        string xmlpath = ABFrameConfigGeter.Config.AssetbundleXMLConfigPath;
        //写入二进制
        string bytepath = ABFrameConfigGeter.Config.AssetbundleBytesConfigPath;
        AssetBundleConfig abconfig = GeneralAssetBundleConfigfiles();
        //写入Xml
        EditorTool.WriteXMLFileInfo(xmlpath, abconfig);
        //对于二进制数据不要path数据 path数是用来观察的
        for (int i = 0; i < abconfig.ABList.Count; i++)
            abconfig.ABList[i].Path = "";
        //写入二进制
        EditorTool.WriteBinaryFileInfo(bytepath, abconfig);

        //刷新才会写入AB 包
        AssetDatabase.Refresh();

        MyDebuger.Log("ab配置文件写入完毕");
    }


    /// <summary>
    /// 删除没有用的AB包 为了减少打包时间 有时候不是所有资源都要重新打包 要重新打包的资源手动删除或者修改配置文件
    /// </summary>
    static void DeleteUnUseAsseBundle()
    {
        string[] allbundlesNames = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo directoryInfo = new DirectoryInfo(AssetBundlePath);
        //获取bunlde目录下的所有文件
        FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);

        for (int i = 0; i < files.Length; i++)
        {
            bool containAbName = false;
            for (int j = 0; j < allbundlesNames.Length; j++)
            {
                //打包后的ab资源包名等于文件名
                if (allbundlesNames[j] == files[i].Name)
                {
                    containAbName = true;
                    break;
                }
            }
            if (containAbName || files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest") || files[i].Name.EndsWith(ABManager.ABConfigABName) || files[i].Name.EndsWith(EditorUserBuildSettings.activeBuildTarget.ToString()))
                continue;
            else
            {
                MyDebuger.Log("删除无效或过时资源: " + files[i].Name);
                if (File.Exists(files[i].FullName))
                    File.Delete(files[i].FullName);
            }
        }
    }


    [MenuItem("MyTools/AB/清除ab包名", false, 30)]
    public static void ClearABNamesAndProgress()
    {
        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();

        for (int i = 0; i < oldABNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            EditorUtility.DisplayProgressBar("清除AB包名:", oldABNames[i], i * 1.0f / oldABNames.Length);
        }
        AssetDatabase.SaveAssets();
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }


    //===============================设置AB包名===============================

    /// <summary>
    /// 设置配置好的AB名字 ishot 是否是热更 热更方式会清除设置好的ab包名
    /// </summary>
    private static void SetABNames()
    {
        int count = 0;
        //设置文件夹ab包名
        foreach (var name in m_AllFileDir.Keys)
        {
            SetABName(name, m_AllFileDir[name]);
            EditorUtility.DisplayProgressBar("设置文件夹内部资源包名:", name, count * 1.0f / m_AllFileDir.Keys.Count);
            count++;
        }
        count = 0;

        for (int i = 0; i < m_CommonDepends.Count; i++)
        {
            SetABName("commonpendences", m_CommonDepends[i]);
            EditorUtility.DisplayProgressBar("设置共同依赖资源包名:", "commonpendences", count * 1.0f / m_CommonDepends.Count);
            count++;
        }

        count = 0;
        //设置prefab ab 包名
        foreach (var name in m_PrefabAllDependsDic.Keys)
        {
            SetABName(name, m_PrefabAllDependsDic[name],true);
            EditorUtility.DisplayProgressBar("设置预制体依赖包名:", name, count * 1.0f / m_PrefabAllDependsDic.Keys.Count);
            count++;
        }
        EditorUtility.ClearProgressBar();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }



    /// <summary>
    /// 设置Ab配置文件包
    /// </summary>
    static void SetAssetbundleBytesConfigAB()
    {
        string bytepath = ABFrameConfigGeter.Config.AssetbundleBytesConfigPath;
        int index = bytepath.IndexOf("Assets");
        string newbytepath = bytepath.Remove(0, index);
        SetABName(ABManager.ABConfigABName, newbytepath);
    }

    /// <summary>
    /// 通过代码设置AB包的名字 
    /// </summary>
    static void SetABName(string abName, List<string> pathList, bool needCheckCommDependices = false)
    {
        for (int i = 0; i < pathList.Count; i++)
        {
            if (needCheckCommDependices)
            {
                if (m_CommonDepends.Contains(pathList[i]))
                    m_CommonDepends.Remove(pathList[i]);
                else
                    SetABName(abName, pathList[i]);
            }
            else
            {
                SetABName(abName, pathList[i]);
            }
        }


    }

    /// <summary>
    /// 通过代码设置AB包的名字 
    /// </summary>
    static void SetABName(string abName, string path)
    {
        string lowerabName = abName.ToLower();
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter == null)
            MyDebuger.LogError("不存在此路径文件" + path);
        else
            assetImporter.assetBundleName = lowerabName;
    }


    //=============================================================

    /// <summary>
    /// Ab资源是否已经包含
    /// </summary>
    /// <returns></returns>
    static bool IsContainAllFileAB(string abPath)
    {
        for (int i = 0; i < m_AllFileAB.Count; i++)
        {
            //Assets/Data/Test  Asset/Data/TestTTT/Test.prefab
            if (m_AllFileAB[i] == abPath || (abPath.Contains(m_AllFileAB[i]) && (abPath.Replace(m_AllFileAB[i], "")[0] == '/')))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 是否是有效路径 判断是否是需要动态加载的文件
    /// </summary>
    /// <returns></returns>
    static bool ValidPath(string path)
    {
        for (int i = 0; i < m_ValueableABFilePaths.Count; i++)
        {
            if (path.Contains(m_ValueableABFilePaths[i]))
                return true;
        }
        return false;
    }

    //=============================HotUpdate===================

    /// <summary>
    /// 写入Md5文件
    /// </summary>
    public static void WriteABMD5()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(AssetBundlePath);
        FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
        //ABMD5管理信息类
        ABMD5 abMd5 = new ABMD5();
        abMd5.ABMD5List = new List<ABMD5Base>();
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest"))
                continue;
            ABMD5Base aBMD5Base = new ABMD5Base();
            aBMD5Base.Name = files[i].Name;
            aBMD5Base.Md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
            aBMD5Base.Size = files[i].Length / 1024.0f;//kb
            abMd5.ABMD5List.Add(aBMD5Base);
        }
        ///本地的md5配置文件信息
        string ABMD5Path = Application.dataPath + "/Resources/ABMD5.bytes";
        BinarySerializeOpt.BinarySerilize(ABMD5Path, abMd5);


        if (!Directory.Exists(m_VersionMd5Path))
            Directory.CreateDirectory(m_VersionMd5Path);

        //=========在外部保存MD5资源版本信息=================
        string xmlpath = m_VersionMd5Path + "/ABMD5_" + PlayerSettings.bundleVersion + ".xml";
        EditorTool.WriteXMLFileInfo(xmlpath, abMd5);

        //存储不同版本 不同平台 md5文件信息
        string targetPath = m_VersionMd5Path + "/ABMD5_" + PlayerSettings.bundleVersion + ".bytes";
        if (File.Exists(targetPath))
            File.Delete(targetPath);
        File.Copy(ABMD5Path, targetPath);
    }



    /// <summary>
    /// 读取热更ABMd5配置文件信息 进行对比分析
    /// </summary>
    /// <param name="abMd5Path"></param>
    /// <param name="hotcount"></param>
    static void ReadMD5Info(string abMd5Path, string hotcount,string version=null,string versiondesc=null)
    {
        m_PackageMd5Dic.Clear();
        using (FileStream fileStream = new FileStream(abMd5Path, FileMode.Open, FileAccess.Read))
        {
            BinaryFormatter bf = new BinaryFormatter();
            ABMD5 aBMD5 = bf.Deserialize(fileStream) as ABMD5;
            foreach (ABMD5Base adBase in aBMD5.ABMD5List)
                m_PackageMd5Dic.Add(adBase.Name, adBase);
        }
        //========判断ab资源哪些改变了================
        List<string> changeList = new List<string>();
        DirectoryInfo directory = new DirectoryInfo(AssetBundlePath);
        FileInfo[] files = directory.GetFiles("*");
        for (int i = 0; i < files.Length; i++)
        {
            if (files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest"))
                continue;
            string name = files[i].Name;
            string md5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
            ABMD5Base aBMD5Base = null;
            //原热更配置文件不包含该文件 直接添加
            if (!m_PackageMd5Dic.ContainsKey(name))
                changeList.Add(name);
            else
            {
                if (m_PackageMd5Dic.TryGetValue(name, out aBMD5Base))
                {
                    //对比md5信息 原热更配置文件和当前文件不一样 添加
                    if (md5 != aBMD5Base.Md5)
                        changeList.Add(name);
                }
            }
        }
        CopyABAndGeneratXml(changeList, hotcount,version,versiondesc);
    }


    /// <summary>
    /// 复制需要热更的ab包和构建xml配置表文本信息
    /// </summary>
    static void CopyABAndGeneratXml(List<string> changeList, string hotcount,string version,string versiondesc)
    {
        if (!Directory.Exists(m_HotPath))
            Directory.CreateDirectory(m_HotPath);
        EditorTool.DeleAllFile(m_HotPath);

        //======拷贝要热更的文件到指定目录======
        foreach (string str in changeList)
        {
            if (!str.EndsWith(".manifes"))
                File.Copy(AssetBundlePath + "/" + str, m_HotPath + "/" + str);

        }
        //=============生成服务器Patch===================
        DirectoryInfo directory = new DirectoryInfo(m_HotPath);
        FileInfo[] files = directory.GetFiles("*", SearchOption.AllDirectories);
        GameVersion versionInfo = new GameVersion();
        Pathces patches = new Pathces();
        patches.Version = hotcount;
        if (!string.IsNullOrEmpty(versiondesc))
            patches.Des = versiondesc;
        else
            patches.Des = "待写入";
        patches.Files = new List<Patch>();
        for (int i = 0; i < files.Length; i++)
        {
            Patch patch = new Patch();
            patch.MD5 = MD5Manager.Instance.BuildFileMd5(files[i].FullName);
            patch.Name = files[i].Name;
            patch.Size = files[i].Length / 1024.0f;
            patch.Platform = EditorUserBuildSettings.activeBuildTarget.ToString();
            if (string.IsNullOrEmpty(version))
                patch.Version = BuildApk.TReadVersion();
            else
                patch.Version = version;
          
            ///下载路径
            patch.Url = hotcount + "/" + files[i].Name;
            patches.Files.Add(patch);
        }
        if (string.IsNullOrEmpty(version))
            versionInfo.Version = BuildApk.TReadVersion();
        else
            versionInfo.Version = version;
        versionInfo.Pathces = new Pathces[1];
        versionInfo.Pathces[0] = patches;
        BinarySerializeOpt.Xmlserialize(m_HotPath + "/PatchVersion.xml", versionInfo);
    }






}

