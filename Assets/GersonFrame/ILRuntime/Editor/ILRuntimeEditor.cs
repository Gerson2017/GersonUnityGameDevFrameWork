using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using GersonFrame.SelfILRuntime;

public class ILRuntimeEditor 
{
    private const string BindFileFloder = "Assets/ILRuntime/BindScripts";


    [MenuItem("ILRuntime/安装VS调试插件")]
    static void InstallDebugger()
    {
        EditorUtility.OpenWithDefaultApp("Assets/Samples/ILRuntime/2.0.1/Demo/Debugger~/ILRuntimeDebuggerLauncher.vsix");
    }

    [MenuItem("ILRuntime/打开ILRuntime中文文档")]
    static void OpenDocumentation()
    {
        Application.OpenURL("https://ourpalm.github.io/ILRuntime/");
    }

    [MenuItem("ILRuntime/打开ILRuntime Github项目")]
    static void OpenGithub()
    {
        Application.OpenURL("https://github.com/Ourpalm/ILRuntime");
    }


    // [MenuItem("ILRuntime/修改本地dll文文件名")]
    public static void ModifyDllFileName()
    {
        string dllfile = ILRuntimeManager.DllPath + ".bytes";
        string pdbfile = ILRuntimeManager. PDBPath + ".bytes";
        if (File.Exists(ILRuntimeManager.DllPath)) {
            if (File.Exists(dllfile)) 
                File.Delete(dllfile);
            File.Move(ILRuntimeManager.DllPath, dllfile);
        }
        if (File.Exists(ILRuntimeManager.PDBPath)) {
            if (File.Exists(pdbfile))
                File.Delete(pdbfile);
            File.Move(ILRuntimeManager.PDBPath, pdbfile);
        } 
        AssetDatabase.Refresh();
    }

    //[ILRuntimeJIT(ILRuntimeJITFlags.NoJIT)]

    [MenuItem("ILRuntime/根据DLL 生成CLR绑定(建议使用)提高运行效率",priority =17)]
  public static void GenerateCLRBindingByAnalysis()
    {
        if (!File.Exists(ILRuntimeManager.DllPath + ".bytes"))
        {
            Debug.LogError("请先配置好热更工程的Dll存在路径"+ ILRuntimeManager.DllPath);
            return;
        }
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream fs = new System.IO.FileStream(ILRuntimeManager.DllPath + ".bytes", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
            //Crossbind Adapter is needed to generate the correct binding code
            ILRuntimeManager.RegisterClass(domain);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, BindFileFloder);
        }
        if (File.Exists("Assets/GersonFrame/ILRuntime/Data/CLRBindings.cs"))
            File.Delete("Assets/GersonFrame/ILRuntime/Data/CLRBindings.cs");
        AssetDatabase.Refresh();
    }


    [MenuItem("ILRuntime/清除所有绑定的脚本(解决绑定代码错误)", priority = 27)]
    public static void ClearBindScripts()
    {
        string srcPath = BindFileFloder + "/";
        try
        {
            DirectoryInfo dirinfo = new DirectoryInfo(srcPath);
            FileSystemInfo[] fileinfo = dirinfo.GetFileSystemInfos();
             foreach (FileSystemInfo info in fileinfo)
                File.Delete(info.FullName);

            TextAsset clrbidingstr = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/GersonFrame/ILRuntime/Data/CLRBindings.txt");

            using (FileStream fs = new FileStream(srcPath+ "CLRBindings.cs", FileMode.OpenOrCreate))
            {
                using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                {
                    sw.Write(clrbidingstr.text);
                }
            }
        }
        catch (Exception e)
        {
            MyDebuger.LogError(string.Format(" 删除文件失败 {0} error {1}", srcPath, e.ToString()));
        }
        AssetDatabase.Refresh();
    }




    [MenuItem("ILRuntime/打开热更工程",priority =10)]
    static void OpenHotFixProject()
    {
        Application.OpenURL(Application.dataPath+ "/HotFix_Dragon~/HotFixDragon_Project.sln");
    }



}
