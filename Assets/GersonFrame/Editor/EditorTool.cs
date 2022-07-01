using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

public class EditorTool 
{


    /// <summary>
    /// 写入指定xml文本信息
    /// </summary>
    /// <param name="xmlsavepath"></param>
    /// <param name="data"></param>
   public static void WriteXMLFileInfo(string xmlsavepath, object data)
    {
        if (File.Exists(xmlsavepath))
            File.Delete(xmlsavepath);
        using (FileStream fs = new FileStream(xmlsavepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(data.GetType());
            xs.Serialize(sw, data);
            sw.Close();
        }
    }


    /// <summary>
    /// 读取XML 文本信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xmlfilepath"></param>
    /// <returns></returns>
    public static T ReadXMLFileInfo<T>(string xmlfilepath) where T:class
    {
        T t = null;
        if (!File.Exists(xmlfilepath))
            return t;
        using (FileStream fileStream = new FileStream(xmlfilepath, FileMode.Open, FileAccess.Read))
        {
            StreamReader sw = new StreamReader(fileStream, Encoding.UTF8);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            t = xs.Deserialize(fileStream) as T;
        }
        return t;
    }

    /// <summary>
    /// 写入指定二进制文本信息
    /// </summary>
    /// <param name="xmlsavepath"></param>
    /// <param name="data"></param>
    public    static void WriteBinaryFileInfo(string bytepath, object data)
    {
        FileStream fbs = new FileStream(bytepath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        //清空文件流
        fbs.Seek(0, SeekOrigin.Begin);
        fbs.SetLength(0);
        BinaryFormatter bfm = new BinaryFormatter();
        bfm.Serialize(fbs, data);
        fbs.Close();
    }


    /// <summary>
    /// 删除指定目录下的所有文件
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public static bool DeleAllFile(string fullPath)
    {
        if (Directory.Exists(fullPath))
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".meta")) continue;
                File.Delete(files[i].FullName);
            }
            return true;
        }
        return false;
    }


    public static void Copy(string srcpath, string targetpath)
    {
        try
        {
            if (!Directory.Exists(targetpath))
            {
                Directory.CreateDirectory(targetpath);
            }
            string srcdir = Path.Combine(targetpath, Path.GetFileName(srcpath));
            if (Directory.Exists(srcpath))
                srcdir += Path.DirectorySeparatorChar;

            if (!Directory.Exists(srcdir))
                Directory.CreateDirectory(srcdir);
            //获取所有文件名
            string[] files = Directory.GetFileSystemEntries(srcpath);

            foreach (string file in files)
            {
                //是否为文件夹
                if (Directory.Exists(file))
                    Copy(file, srcdir);
                //为文件
                else
                    File.Copy(file, srcdir + Path.GetFileName(file), true);
            }
        }
        catch (Exception e)
        {
            MyDebuger.LogError("无法复制：" + srcpath + " 到" + targetpath + " error:" + e);
        }
    }



    public static void DeleteDir(string srcPath)
    {
        try
        {
            DirectoryInfo dirinfo = new DirectoryInfo(srcPath);
            if (dirinfo.Exists)
                dirinfo.Delete(true);
        }
        catch (Exception e)
        {
            MyDebuger.LogError(string.Format(" 删除文件失败 {0} error {1}", srcPath, e.ToString()));
        }
    }


    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="floderPath"></param>
    public static void OpenFloder(string floderPath)
    {
        if (!Directory.Exists(floderPath))
            Directory.CreateDirectory(floderPath);
        Application.OpenURL("file://" + floderPath);
    }

    /// <summary>
    /// 文件夹移动
    /// </summary>
    /// <param name="srcFolderPath"></param>
    /// <param name="destFolderPath"></param>
    public static void FolderMove(string srcFolderPath, string destFolderPath)
    {
        //检查目标目录是否以目标分隔符结束，如果不是则添加之
        if (destFolderPath[destFolderPath.Length - 1] != Path.DirectorySeparatorChar)
            destFolderPath += Path.DirectorySeparatorChar;

        //判断目标目录是否存在，如果不在则创建之
        if (!Directory.Exists(destFolderPath))
            Directory.CreateDirectory(destFolderPath);
        string[] fileList = Directory.GetFileSystemEntries(srcFolderPath);
        foreach (string file in fileList)
        {
            if (Directory.Exists(file))
                FolderMove(file, destFolderPath + Path.GetFileName(file));
            else
                File.Move(file, destFolderPath + Path.GetFileName(file));
        }
        Directory.Delete(srcFolderPath);
    }


    /// <summary>
    /// 构建项目Cs文件引用
    /// </summary>
    /// <param name="Csprojectfilepath"></param>
    public static void GenerateHotPorjectCsprojXml(string Csprojectfilepath)
    {
        string xmlstr = File.ReadAllText(Csprojectfilepath);
        int lastfloderindex = Csprojectfilepath.LastIndexOf("/");
       string direcotrypath= Csprojectfilepath.Remove(lastfloderindex+1);

        DirectoryInfo hotprojectfloder = new DirectoryInfo(direcotrypath);
        FileInfo[] csfiles = hotprojectfloder.GetFiles("*.cs", SearchOption.AllDirectories);
        string compilestr = "";
        string tempfilepath = "";
        int count = 0;
        foreach (var item in csfiles)
        {
            if (item.Directory.Name == "Debug" || item.Directory.Name == "Release")
                continue;
            if (item.Name.Contains("SystemFunctionConfigAudioConfig"))
                continue;
            tempfilepath = item.FullName.Replace(hotprojectfloder.FullName, "");
            if (count == 0)
                compilestr += $"<Compile Include=\"{ tempfilepath }\" />\n";
            else
                compilestr += $"    <Compile Include=\"{ tempfilepath }\" />\n";
            count++;
        }

        int firstcompileindex = xmlstr.IndexOf("<Compile");
        int lastcompileindex = xmlstr.IndexOf("</ItemGroup>", firstcompileindex);
        xmlstr = xmlstr.Remove(firstcompileindex, lastcompileindex - firstcompileindex);
        xmlstr = xmlstr.Insert(firstcompileindex, compilestr);
        File.WriteAllText(Csprojectfilepath, xmlstr);

        UnityEngine.Debug.LogWarning("修改 HotFix_Dragon.csproj 完成");
    }



    /// <summary>
    /// 创建Cmd/bat 进程信息
    /// </summary>
    /// <param name="cmd"></param>
    /// <param name="args"></param>
    /// <param name="workingDir"></param>
    /// <returns></returns>
    public static Process CreateShellExProcess(string cmd, string args, string workingDir = "")
    {
        var pStartInfo = new ProcessStartInfo(cmd);
        pStartInfo.Arguments = args;
        pStartInfo.CreateNoWindow = false;
        pStartInfo.UseShellExecute = true;
        // pStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        pStartInfo.RedirectStandardError = false;
        pStartInfo.RedirectStandardInput = false;
        pStartInfo.RedirectStandardOutput = false;
        if (!string.IsNullOrEmpty(workingDir))
            pStartInfo.WorkingDirectory = workingDir;
        Process process = Process.Start(pStartInfo);
        //该方法可以让进程等待
        process.WaitForExit();
        process.Close();
        return process;
    }



}
