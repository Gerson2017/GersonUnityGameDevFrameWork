
using GersonFrame.ABFrame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;


public class BuildApk : MonoBehaviour
{

    public static string m_BuildPath = Application.dataPath + "/../BuildTarget";
    public static string m_AndroidPath = m_BuildPath + "/Android/";
    public static string m_IOSPath = m_BuildPath + "/IOS/";
    public static string m_WindowsPath = m_BuildPath + "/Windows/";

    public static string m_AppType = "母包";

    public static BuildTarget BulidTarget { get; private set; } = BuildTarget.NoTarget;


    [MenuItem("Tools/BuildAPK")]
    public static void BuildAPK()
    {
        SetProjectKey();
        BulidTarget = BuildTarget.Android;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        BuildReflashAB();
    }

    [MenuItem("MyTools/打包/Android/更新版本号和版本名称", priority = 10)]
    public static void BuildReflashAB()
    {
        BulidTarget = BuildTarget.Android;
        BuildHotfixDll();
        ILRuntimeEditor.GenerateCLRBindingByAnalysis();
        OpenStoryPanelPrefab();
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        //打AB包
        BundleEditor.BuildAB(BulidTarget);
        BuildAPKByNowAB(true);
    }

    [MenuItem("MyTools/打包/Android/不更新版本名称", priority = 15)]
    public static void BuildAPKByNowVersion()
    {
        BulidTarget = BuildTarget.Android;
        BuildHotfixDll();
        ILRuntimeEditor.GenerateCLRBindingByAnalysis();
        OpenStoryPanelPrefab();
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        //打AB包
        BundleEditor.BuildAB(BulidTarget);
        BuildAPKByNowAB(false);
    }


    [MenuItem("MyTools/打包/Android/不更新AB不更新版本名称", priority = 20)]
    public static void BuildAPKByNowABAndNoUptateVersion()
    {
        BulidTarget = BuildTarget.Android;
        BuildAPKByNowAB(false);
    }


    [MenuItem("MyTools/AB/BulidAB/Android", false, 25)]
    public static void BuildAndroidAB()
    {
        BulidTarget = BuildTarget.Android;
        BundleEditor.BuildAB(BulidTarget);
    }


    /// <summary>
    /// updateveroion 是否更新版本名称
    /// </summary>
    /// <param name="updateveroion">是否更新版本名称</param>
    private static void BuildAPKByNowAB(bool updateveroion)
    {
        SetProjectKey();
        OpenStoryPanelPrefab();
        BundleEditor.CopyABFilesToProject();
        //hotupdate 写入版本信息 
        UpdayeVersion(PlayerSettings.applicationIdentifier, updateveroion);

        string aPkName = PlayerSettings.productName;
        string version = PlayerSettings.bundleVersion;
        string apkFloder = "";
        string savepath = "";

        if (version == "7.7.7")
            m_AppType = "商务母包";

        if (BulidTarget == BuildTarget.Android)
        {
            apkFloder = m_AndroidPath;
            savepath = apkFloder + aPkName + BulidTarget + string.Format("{0}_{1:yyyy_MM_dd_HH_mm}_{2}", m_AppType, DateTime.Now, version) + ".apk";
        }
        else if (BulidTarget == BuildTarget.iOS)
        {
            apkFloder = m_IOSPath;
            //打出xcode工程
            savepath = apkFloder + aPkName + BulidTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}_{2}", DateTime.Now);
        }
        else if (BulidTarget == BuildTarget.StandaloneWindows || BulidTarget == BuildTarget.StandaloneWindows64)
        {
            apkFloder = m_WindowsPath;
            savepath = apkFloder + aPkName + BulidTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}/{1}_{2}.exe", DateTime.Now, aPkName);
        }
        ///打包
        BuildPipeline.BuildPlayer(FindEnableEitorScenes(), savepath, BulidTarget, BuildOptions.None);
        Application.OpenURL("file://" + apkFloder);
        MyDebuger.Log("打包完毕");
    }

    [MenuItem("MyTools/打包/打开打包文件文件夹", priority = 30)]
    private static void OpenAppFiel()
    {
        EditorTool.OpenFloder(Application.dataPath + "/../BuildTarget");
    }


    static void OpenStoryPanelPrefab()
    {
        string path = ABFrameConfigGeter.Config.TimeLinePrefabPath;
        if (!string.IsNullOrEmpty(path))
            AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path));
        else
            MyDebuger.LogWarning("未配置timeline预制体 请检查是否需要配置");
    }


    private static string[] FindEnableEitorScenes()
    {
        List<string> editorscenes = new List<string>();
        ///获取编辑器下面设置好的所以场景
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            editorscenes.Add(scene.path);
        }
        return editorscenes.ToArray();
    }


    [MenuItem("Tools/测试Version写入")]
    public static void TVersion()
    {
        UpdayeVersion(PlayerSettings.applicationIdentifier, false);
    }

    [MenuItem("MyTools/构建热更DLL")]
    public static void BuildHotfixDll()
    {
        string msbuildPath = Application.dataPath + "/../Tools/MSBuild/MSBuild.exe";
        string projPath = Application.dataPath + "/HotFix_Dragon~/HotFix_Dragon.csproj";

        Debug.Log(msbuildPath);
        Debug.Log(projPath);
        System.Diagnostics.Process.Start(msbuildPath, projPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/测试Version读取")]
    public static string TReadVersion()
    {
        TextAsset vesiontex = Resources.Load<TextAsset>("Version");
        if (vesiontex == null)
        {
            Debug.LogError("未读取到本地版本");
            return "0.0.0";
        }
        HotUpdateVersionInfo versionInfo = null;
        using (MemoryStream stream = new MemoryStream(vesiontex.bytes))
        {
            BinaryFormatter bf = new BinaryFormatter();
            versionInfo = bf.Deserialize(stream) as HotUpdateVersionInfo;
        }
        return versionInfo.Version;
    }

    /// <summary>
    /// 更新版本号 包名 默认版本号加1  版本次数加1
    /// </summary>
    /// <param name="version"></param>
    /// <param name="package">包名</param>
    /// <param name="updateversion">是否更新版本名称为true时会根据配置文件中的版本号进行加一操作</param>
    static void UpdayeVersion(string package, bool updateversion)
    {
        string savePath = Application.dataPath + "/Resources/VersionJson.json";
        if (!Directory.Exists(Application.dataPath + "/Resources/"))
            Directory.CreateDirectory(Application.dataPath + "/Resources/");

        HotUpdateVersionInfo fileversionInfo = null;
        string newversion = PlayerSettings.bundleVersion;

        using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (StreamReader sr = new StreamReader(fs, System.Text.Encoding.UTF8))
            {
                string jsonstr = sr.ReadToEnd();
                fileversionInfo = JsonUtility.FromJson<HotUpdateVersionInfo>(jsonstr);
            }
        }
        if (fileversionInfo == null)
        {
            fileversionInfo = new HotUpdateVersionInfo();
#if UNITY_ANDROID
            fileversionInfo.BundleVersionCode = PlayerSettings.Android.bundleVersionCode;
#endif
        }
        else
        {
            fileversionInfo.BundleVersionCode++;
            if (updateversion)
            {
                newversion = "";
                string[] versionstrs = fileversionInfo.Version.Split('.');
                int lastversion = int.Parse(versionstrs[versionstrs.Length - 1]);
                lastversion++;
                for (int i = 0; i < versionstrs.Length - 1; i++)
                    newversion += versionstrs[i] + ".";
                newversion += lastversion;
            }

#if UNITY_ANDROID
            PlayerSettings.Android.bundleVersionCode = fileversionInfo.BundleVersionCode;
#endif
        }

        fileversionInfo.Version = newversion;
        fileversionInfo.PackageName = package;

        //写入文件
        using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate))
        {
            using (StreamWriter st = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                string versionInfoStr = JsonUtility.ToJson(fileversionInfo);
                st.Write(versionInfoStr);
            }
        }

        string bytepath = Application.dataPath + "/Resources/Version.bytes";
        FileStream fbs = new FileStream(bytepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        //清空文件流
        fbs.Seek(0, SeekOrigin.Begin);
        fbs.SetLength(0);
        BinaryFormatter bfm = new BinaryFormatter();
        bfm.Serialize(fbs, fileversionInfo);
        fbs.Close();

        //刷新才会写入AB 包
        AssetDatabase.Refresh();

        Debug.Log("版本信息写入成功 "+ savePath);
    }

    [MenuItem("Tools/设置签名密码")]
    public static void SetProjectKey()
    {
        PlayerSettings.keyaliasPass = "pj9s44";
        PlayerSettings.keystorePass = "9r65uy";
        AssetDatabase.Refresh();
    }




}
