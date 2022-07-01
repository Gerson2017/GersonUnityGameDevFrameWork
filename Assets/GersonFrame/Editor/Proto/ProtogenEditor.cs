#if UNITY_2018_1_OR_NEWER
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;

public class ProtogenEditor
{

    public static readonly string AllConfigDataName="ConfigDatabase";
    public static string InputProtoFloder = Application.dataPath + "/../ProtoFiles/";
    public static string InPutExcelFloder = Application.dataPath + "/../ExcelFiles/";
    public static string SampleExcelFloder = Application.dataPath + "/../ExcelFiles/Sample";
    /// <summary>
    /// 总的bytes文件
    /// </summary>
    public static string OutAllByteFloder = Application.dataPath + "/Configs/ProtoBytes/";
    /// <summary>
    ///散bytes文件
    /// </summary>
    public static string OutSmallByteFloder = Application.dataPath + "/../SmallBytes/";
    public static string OutJsonFloder = Application.dataPath + "/../JsonFiles/";
    public static string OutPutProtoCsFloder = Application.dataPath + "/HotFix_Dragon~/ProtoCsFiles/";
    public static string ProtoTempFiledir = Application.dataPath + "/GersonFrame/Editor/Proto/ConfigProto/";

    [MenuItem("MyTools/ProtoBuf/打开.proto文件夹")]
    public static void OpenPotoFloder()
    {
        EditorTool.OpenFloder(InputProtoFloder);
    }

    [MenuItem("MyTools/ProtoBuf/打开protoCs文件夹")]
    public static void OpenPotoCSFloder()
    {
        EditorTool.OpenFloder(OutPutProtoCsFloder);
    }

    [MenuItem("MyTools/ProtoBuf/打开Excel文件夹")]
    public static void OpenExcelFloder()
    {
        Excel2ProtoViewTool.SetFloderNameByPlayerPrefabs();
        EditorTool.OpenFloder(InPutExcelFloder);
    }

    [MenuItem("MyTools/ProtoBuf/打开Json文件夹")]
    public static void OpenJsonFloder()
    {
        Excel2ProtoViewTool.SetFloderNameByPlayerPrefabs();
        EditorTool.OpenFloder(OutJsonFloder);
    }

    [MenuItem("MyTools/ProtoBuf/.Proto转成.CS")]
    public static void AllProto2CS()
    {
        RunBat("_GenAllC#.bat", "", InputProtoFloder);
        EditorCoroutineRunner.StartCoroutine(SealdToParticle(0.35f));
        AutoAddHotCsReference();
    }

    public static IEnumerator SealdToParticle(float waittime)
    {
        yield return new WaitForSeconds(waittime);
        Excel2ProtoViewTool.SetFloderNameByPlayerPrefabs();
        if (!Directory.Exists(OutPutProtoCsFloder))
            Directory.CreateDirectory(OutPutProtoCsFloder);
        
        DirectoryInfo csFileDirectoryInfo = new DirectoryInfo(OutPutProtoCsFloder);
        FileInfo[] csfiles = csFileDirectoryInfo.GetFiles("*.cs", SearchOption.TopDirectoryOnly);
        foreach (FileInfo fileInfo in csfiles)
        {
            ReplaceValue(fileInfo.FullName,12, "sealed class", "partial class");
        }

        UnityEngine.Debug.LogWarning("=========AllProto2CS 完毕============");
    }



    /// <param name="strIndex">索引的字符串，定位到某一行</param>
    /// <param name="newValue">替换新值</param>
    public static void ReplaceValue(string filepath, int linnum, string orilstr, string replacestr)
    {
        if (File.Exists(filepath))
        {
            string[] lines = System.IO.File.ReadAllLines(filepath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (i == linnum - 1)
                {
                    lines[i] = lines[i].Replace(orilstr, replacestr);
                    break;
                }
            }
            File.WriteAllLines(filepath, lines);
        }
    }
    [MenuItem("MyTools/ProtoBuf/自动添加热更工程CS文件引用")]
    public static void AutoAddHotCsReference()
    {
        EditorTool.GenerateHotPorjectCsprojXml(Application.dataPath+ "/HotFix_Dragon~/HotFix_Dragon.csproj");
    }

    [MenuItem("MyTools/ProtoBuf/清理多余的数据")]
    public static void ClearData()
    {
        Excel2ProtoViewTool.SetFloderNameByPlayerPrefabs();

        List<string> excelfiles = new List<string>();
        DirectoryInfo excelFileDirectoryInfo = new DirectoryInfo(InPutExcelFloder);
        FileInfo[] excelsFile = excelFileDirectoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);

        foreach (FileInfo fileInfo in excelsFile)
        {
            if (!UsefulExcel(fileInfo)) continue;
            excelfiles.Add(GetFileNameByExcelDirectory(fileInfo));
        }
        if (excelfiles.Count < 1)
        {
            UnityEngine.Debug.Log("没有需要清理的文件 完毕");
            return;
        }
        //过滤总数据
        excelfiles.Add($"{AllConfigDataName}.xlsx");
        //过滤获取配置文件脚本
        excelfiles.Add("GetProtoConfig.xlsx");

        DeleteNoUseFileByExcel(InputProtoFloder, excelfiles, ".proto");
        DeleteNoUseFileByExcel(ProtoTempFiledir, excelfiles, ".proto");
        DeleteNoUseFileByExcel(OutPutProtoCsFloder, excelfiles, ".cs");
        UnityEngine.Debug.Log($"=======清理文件完毕==========");
        AssetDatabase.Refresh();
    }




    [MenuItem("MyTools/ProtoBuf/清除proto文件")]
    public static void ClearAllProtoFile()
    {
        int count = 0;
        EditorUtility.DisplayProgressBar("清理文件", "请等待", 0.5f);
        DirectoryInfo inputFileDirectoryInfo = new DirectoryInfo(InputProtoFloder);
        FileInfo[] inportprotofiles = inputFileDirectoryInfo.GetFiles("*.proto", SearchOption.TopDirectoryOnly);
        for (int i = inportprotofiles.Length - 1; i >= 0; i--)
        {
            EditorUtility.DisplayProgressBar("清理文件", inportprotofiles[i].FullName, count * 1.0f / inportprotofiles.Length);
            inportprotofiles[i].Delete();
        }

        DirectoryInfo tempprotoDirectoryInfo = new DirectoryInfo(ProtoTempFiledir);
        FileInfo[] temprptofiles = tempprotoDirectoryInfo.GetFiles("*.proto", SearchOption.TopDirectoryOnly);
         count = 0;
        for (int i = temprptofiles.Length - 1; i >= 0; i--)
        {
            EditorUtility.DisplayProgressBar("清理文件", temprptofiles[i].FullName, count * 1.0f / temprptofiles.Length);
            temprptofiles[i].Delete();
        }

        EditorUtility.ClearProgressBar();
        UnityEngine.Debug.Log($"=======清理proto散文件完毕==========");
        AssetDatabase.Refresh();
    }

    public static bool UsefulExcel(FileInfo excelfile)
    {
        return !excelfile.Name.StartsWith("~");
    }


    /// <summary>
    /// 根据Excel文件路径获取 .ptoto .cs .bytes 名字
    /// </summary>
    /// <param name="excelfile"></param>
    public static string GetFileNameByExcelDirectory(FileInfo excelfile,bool issampleexcel=false)
    {
        string filename = "";
        if (issampleexcel)
        {
            filename = excelfile.Name;
         return   filename = filename.Replace(".xlsx", "");
        }
        DirectoryInfo excelFileDirectoryInfo = new DirectoryInfo(InPutExcelFloder);
        if (excelfile.Directory.FullName != excelFileDirectoryInfo.FullName)
            filename= (excelfile.Directory.Name + excelfile.Name);
        else
            filename = excelfile.Name;

        filename= filename.Replace(".xlsx", "");
        //检查是否有花括号
        int leftindex= filename.IndexOf("{");
        if (leftindex > -1)
        {
          int rightindex=  filename.IndexOf("}");
            if (rightindex<leftindex)
                UnityEngine.Debug.LogError($"花括号配置错误{excelfile.FullName}");
            else
                filename= filename.Remove(leftindex, rightindex-leftindex+1);
        }
        //检查开头有几个数字
        Regex regNum = new Regex("^[0-9]");
        int startcount = 0;
        if (regNum.IsMatch(filename))
        {
            for (int i = 0; i < filename.Length; i++)
            {
                string str = filename.Substring(i,1);
                if (IsNumeric(str))
                    startcount++;
                else
                    break;
            }
        }
        if (startcount > 0)
            return filename.Remove(0, startcount).Replace("_", "");

        return filename.Replace("_", "");
    }


    /// <summary>
    /// 判断是否是数字  2019年9月22日20:54:40  Dennyhui
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNumeric(string value)
    {
        return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
    }

    static void DeleteNoUseFileByExcel(string directory, List<string> excelfiles, string fileextension)
    {
        DirectoryInfo protoFileDirectoryInfo = new DirectoryInfo(directory);
        FileInfo[] files = protoFileDirectoryInfo.GetFiles("*" + fileextension, SearchOption.TopDirectoryOnly);
        int count = 0;
        for (int i = files.Length - 1; i >= 0; i--)
        {
            EditorUtility.DisplayProgressBar("清理文件", files[i].FullName, count*1.0f/ files.Length);
            count++;
            string str = files[i].Name.Replace(fileextension, ".xlsx");
            if (!excelfiles.Contains(str))
            {
                UnityEngine.Debug.Log($"清理文件 { files[i].FullName}");
                files[i].Delete();
            }
        }
        EditorUtility.ClearProgressBar();
    }


    /// <summary>
    /// 首字母大写
    /// </summary>
    /// <returns></returns>
    public static string FirstCharBig(string str,string excelname)
    {
            if (string.IsNullOrEmpty(str))
            {
            MyDebuger.LogError(excelname+" 存在空字段名 ");
                return string.Empty;
            }
            return str.First().ToString().ToUpper() + str.Substring(1);
    }


    /// <summary>
    /// batfile bat文件名 args 参数  workingDir bat文件存放的路径
    /// </summary>
    /// <param name="batfile"></param>
    /// <param name="args"></param>
    /// <param name="workingDir"></param>
    public static void RunBat(string batfile, string args, string workingDir = "")
    {
       EditorTool.CreateShellExProcess(batfile, args, workingDir);

    }





    /// <summary>
    /// 是否是sample文件夹的文件
    /// </summary>
    /// <param name="fileInfo"></param>
    /// <returns></returns>
    public static bool IsSampleFloderFile(FileInfo fileInfo)
    {
        DirectoryInfo sampledirect = null;
        if (Directory.Exists(ProtogenEditor.SampleExcelFloder))
            sampledirect = new DirectoryInfo(ProtogenEditor.SampleExcelFloder);
        if (sampledirect != null && fileInfo.Directory.FullName == sampledirect.FullName)
            return true;

        return false;
    }

}
#endif