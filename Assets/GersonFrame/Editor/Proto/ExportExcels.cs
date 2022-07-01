
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Collections;
using System;
using OfficeOpenXml;
using System.Linq;
using System.Xml.Serialization;
using GersonFrame.ABFrame;
using DragonRun.Proto;

public class ExcelFileToMd5
{
    [XmlAttribute("FileName")]
    public string FileName;
    [XmlAttribute("Md5Code")]
    public string Md5Code;
    [XmlAttribute("Size")]
    public float Size;
}


[System.Serializable]
public class ExcelMd5List
{
    [XmlElement]
    public List<ExcelFileToMd5> ExcelMd5InfoList { get; set; }
}
/// <summary>
/// excel转proto界面逻辑
/// </summary>
public class Excel2ProtoViewTool : BaseEditor
{

    public enum ProtoType
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 被共用的
        /// </summary>
        Common,
        /// <summary>
        /// 包含公用类型
        /// </summary>
        ContainSub
    }

    static bool m_canClick = true;
    /// <summary>
    /// 已经设置过的共同类型的excel
    /// </summary>
    public static List<string> mAlreadySetCommonExcels { get; private set; } = new List<string>();

    /// <summary>
    /// ExcelMD5信息字典
    /// </summary>
    public static Dictionary<string, ExcelFileToMd5> mExcelMd5InfoDic = new Dictionary<string, ExcelFileToMd5>();


    [MenuItem("MyTools/ProtoBuf/Proto导表工具")]
    static void Open()
    {
        GetWindow<Excel2ProtoViewTool>("Proto导表工具");
        ResetConfig();
    }


    /// <summary>
    /// 已经导过的配置表（已经导出bytes）
    /// </summary>
    static List<string> HasExportExcelList = new List<string>();

    /// <summary>
    /// 所有配置表
    /// </summary>
    static List<string> AllExcelList = new List<string>();

    /// <summary>
    /// 新的配置表（没导出bytes）
    /// </summary>
    static List<string> NewExcelList = new List<string>();

    /// <summary>
    /// 程序员模式
    /// </summary>
    //static bool admin;

    /// <summary>
    /// 是都检测当前打开excel
    /// </summary>
    static bool CheckExcel;

    /// <summary>
    /// 是否使用增量模式
    /// </summary>
   public static bool IsAddModel { get; private set; }
    /// <summary>
    /// 是否导出.Bytes散文件和Json文件
    /// </summary>
    public static bool IsOutSmallBytesAndJson { get; private set; }

    public override void OnGUI()
    {
        base.OnGUI();
        GUILayout.BeginHorizontal();
        SetAccess();
        SetCheckState();
        GUILayout.EndHorizontal();
        RefreshInfo();
        SetPath();
        RefreshConfig();
        ShowInfo();
        ShowOtherConfig();
    }

    public override void OnEnable()
    {
        m_canClick = true;
        SetFloderNameByPlayerPrefabs();
        ResetConfig();
    }

    public static void SetFloderNameByPlayerPrefabs()
    {
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(Excel2ProtoTool.ExcelPathKey)))
            ProtogenEditor.InPutExcelFloder = PlayerPrefs.GetString(Excel2ProtoTool.ExcelPathKey);
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(Excel2ProtoTool.ExportBytesPathKey)))
            ProtogenEditor.OutAllByteFloder = PlayerPrefs.GetString(Excel2ProtoTool.ExportBytesPathKey);
        if (!string.IsNullOrEmpty(PlayerPrefs.GetString(Excel2ProtoTool.JsonPathKey)))
            ProtogenEditor.OutJsonFloder = PlayerPrefs.GetString(Excel2ProtoTool.JsonPathKey);
    }

    /// <summary>
    /// 设置权限
    /// </summary>
    static void SetAccess()
    {
        //admin = PlayerPrefs.HasKey("Progromer") ? PlayerPrefs.GetInt("Progromer") == 1 ? true : false : false;
        //admin = GUILayout.Toggle(admin, "程序员");
        //PlayerPrefs.SetInt("Progromer", admin ? 1 : 0);
    }

    /// <summary>
    /// 检测是否打开excel
    /// </summary>
    static void SetCheckState()
    {
        CheckExcel = PlayerPrefs.HasKey("Progromerexcel") ? PlayerPrefs.GetInt("Progromerexcel") == 1 ? true : false : true;
        IsAddModel = PlayerPrefs.HasKey("IsAddModel") ? PlayerPrefs.GetInt("IsAddModel") == 1 ? true : false : true;
        IsOutSmallBytesAndJson = PlayerPrefs.HasKey("IsOutSmallBytesAndJson") ? PlayerPrefs.GetInt("IsOutSmallBytesAndJson") == 1 ? true : false : true;
        CheckExcel = GUILayout.Toggle(CheckExcel, "是否检测excel是否打开");
        IsAddModel = GUILayout.Toggle(IsAddModel, "是否使用增量模式");
        IsOutSmallBytesAndJson = GUILayout.Toggle(IsOutSmallBytesAndJson, "是否导出json和Bytes散文件");
        PlayerPrefs.SetInt("Progromerexcel", CheckExcel ? 1 : 0);
        PlayerPrefs.SetInt("IsAddModel", IsAddModel ? 1 : 0);
        PlayerPrefs.SetInt("IsOutSmallBytesAndJson", IsOutSmallBytesAndJson ? 1 : 0);
    }

    /// <summary>
    /// 获取proto类型
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public static ProtoType GetProtoType(FileInfo file, out string commonprotoName)
    {
        string commonproyname = GetCommonProtoName(file);
        commonprotoName = commonproyname;
        if (string.IsNullOrEmpty(commonproyname))
            return ProtoType.Normal;
        else
        {
            if (mAlreadySetCommonExcels.Contains(commonproyname))
                return ProtoType.ContainSub;
            return ProtoType.Common;
        }

    }


    public static string GetCommonProtoName(FileInfo file)
    {
        if (Directory.Exists(ProtogenEditor.SampleExcelFloder))
        {
            DirectoryInfo serverDirectoryInfo = new DirectoryInfo(ProtogenEditor.SampleExcelFloder);
            if (file.Directory.FullName == serverDirectoryInfo.FullName)
                return Path.GetFileNameWithoutExtension(file.Name);
        }
        int leftindex = file.Name.IndexOf("{");
        int rightindex = file.Name.IndexOf("}");
        if (leftindex < 0 || rightindex < 0 || rightindex < leftindex)
            return string.Empty;
        return file.Name.Substring(leftindex + 1, rightindex - leftindex - 1);

    }

    /// <summary>
    /// 注意
    /// </summary>
    void RefreshInfo()
    {
        if (!DrawHeader("注意事项"))
        {
            return;
        }
        BeginContents();
        ShowLabel("1.使用前关闭excel相关软件");
        ShowLabel("2.新表导出，有两步操作，首先“导出Proto”，导出后等编译完（右下角圈圈转完）再操作“导出.Bytes文件”");
        ShowLabel("3.旧表改了字段，首先“导出Proto”，导出后等编译完（右下角圈圈转完）再操作“导出.Bytes文件”");
        ShowLabel("3.xxx{abc}.xlsl  {}中的内容为共同类型 共同类型Excel文件存放在Sample目录下");
        ShowLabel("4.修改xxx{abc}.xlsl 的类型 需要同同时修改Sample目录下的共同类型的 abc.xlsl 及其它共用共同类型的Excel");
        EndContents();

    }

    /// <summary>
    /// 刷新
    /// </summary>
    void RefreshConfig()
    {
        if (!DrawHeader("刷新配置表"))
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("刷新", GUILayout.Width(70), GUILayout.Height(50)))
                ResetConfig();
            GUILayout.EndHorizontal();
        }
    }

    static void ResetConfig()
    {
        mExcelMd5InfoDic.Clear();
        mAlreadySetCommonExcels.Clear();
        HasExportExcelList.Clear();
        AllExcelList.Clear();
        NewExcelList.Clear();
        BuildConfigIndexObject();
    }

    /// <summary>
    /// 设置路径
    /// </summary>
    private void SetPath()
    {
        if (!DrawHeader("设置配置表路径（必选）"))
        {

        }
        BeginContents();

        GUILayout.BeginVertical();

        ShowLabel("Proto 路径 " + ProtogenEditor.InputProtoFloder);
        ShowLabel("CS 路径 " + ProtogenEditor.OutPutProtoCsFloder);
        ShowLabel("ConfigProto编译路径 " + ProtogenEditor.ProtoTempFiledir);
        GUILayout.EndVertical();
        GUILayout.BeginHorizontal();
        TextField("Excel 路径 ", ref ProtogenEditor.InPutExcelFloder);
        if (GUILayout.Button("设置"))
        {
            ProtogenEditor.InPutExcelFloder = EditorUtility.OpenFolderPanel("open res path", ProtogenEditor.InPutExcelFloder, "");
            if (!string.IsNullOrEmpty(ProtogenEditor.InPutExcelFloder))
            {
                PlayerPrefs.SetString(Excel2ProtoTool.ExcelPathKey, ProtogenEditor.InPutExcelFloder);
            }
            HasExportExcelList.Clear();
            AllExcelList.Clear();
            NewExcelList.Clear();
            BuildConfigIndexObject();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        TextField("Json路径 ", ref ProtogenEditor.OutJsonFloder);
        if (GUILayout.Button("设置"))
        {
            ProtogenEditor.OutJsonFloder = EditorUtility.OpenFolderPanel("open res path", ProtogenEditor.OutJsonFloder, "");
            if (!string.IsNullOrEmpty(ProtogenEditor.OutJsonFloder))
            {
                PlayerPrefs.SetString(Excel2ProtoTool.JsonPathKey, ProtogenEditor.OutJsonFloder);
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        TextField("Bytes路径 ", ref ProtogenEditor.OutAllByteFloder);
        if (GUILayout.Button("设置"))
        {
            ProtogenEditor.OutAllByteFloder = EditorUtility.OpenFolderPanel("open res path", ProtogenEditor.OutAllByteFloder, "");
            if (!string.IsNullOrEmpty(ProtogenEditor.OutAllByteFloder))
            {
                PlayerPrefs.SetString(Excel2ProtoTool.ExportBytesPathKey, ProtogenEditor.OutAllByteFloder);
            }
        }
        GUILayout.EndHorizontal();
        EndContents();
    }

    static void BuildConfigIndexObject()
    {
        //  string serverConfigProtoPath = PlayerPrefs.GetString(Excel2ProtoTool.ExcelPathKey);
        if (string.IsNullOrEmpty(ProtogenEditor.InPutExcelFloder))
        {
            Debug.Log("没有设置Excel目录");
            return;
        }

        if (!Directory.Exists(ProtogenEditor.InputProtoFloder))
            Directory.CreateDirectory(ProtogenEditor.InputProtoFloder);
        
        DirectoryInfo BytesFileDirectoryInfo = new DirectoryInfo(ProtogenEditor.InputProtoFloder);
        FileInfo[] protosFile = BytesFileDirectoryInfo.GetFiles("*.proto", SearchOption.TopDirectoryOnly);

        foreach (FileInfo fileInfo in protosFile)
        {
            string str = fileInfo.Name.Replace(".proto", "");
            HasExportExcelList.Add(str);
        }
        if (!Directory.Exists(ProtogenEditor.InPutExcelFloder))
            Directory.CreateDirectory(ProtogenEditor.InPutExcelFloder);
        DirectoryInfo serverDirectoryInfo = new DirectoryInfo(ProtogenEditor.InPutExcelFloder);
        FileInfo[] serverFileInfos = serverDirectoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);

        DirectoryInfo sampledirect = null;
        if (Directory.Exists(ProtogenEditor.SampleExcelFloder))
            sampledirect = new DirectoryInfo(ProtogenEditor.SampleExcelFloder);


        foreach (FileInfo fileInfo in serverFileInfos)
        {
            if (!ProtogenEditor.UsefulExcel(fileInfo)) continue;
            string str = ProtogenEditor.GetFileNameByExcelDirectory(fileInfo);
            if (AllExcelList.Contains(str))
                Debug.LogError("Excel表名重复 " + str + " 文件 " + fileInfo.FullName);
            else
                AllExcelList.Add(str);
            if (!HasExportExcelList.Contains(str))
            {
                if (sampledirect != null && fileInfo.Directory.FullName == sampledirect.FullName)
                    continue;
                NewExcelList.Add(str);
            }
        }
    }

    void ShowInfo()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("一键导出proto") && m_canClick)
        {
            m_canClick = false;
            if (NewExcelList.Count>0)
                ProtogenEditor.ClearAllProtoFile();
            ExportVersionRes_ExcelSelect(AllExcelList);
            AssetDatabase.Refresh();
            m_canClick = true;
        }
        if (NewExcelList.Count < 1)
        {
            if (GUILayout.Button("一键导出.Bytes文件") && m_canClick)
            {
                if (CheckExcelIsOpen())
                    return;
                m_canClick = false;
                Excel2ProtoTool.ExportExcelConfigAll();
                if (IsOutSmallBytesAndJson)
                    Excel2ProtoTool.ExportExcelConfig();
                Excel2ProtoTool.WriteExcelMd5Info();
                UnityEngine.Debug.Log("===========.bytes文件生成 完毕 生成路径 " + ProtogenEditor.OutAllByteFloder + " ===============");
                if (!IsAddModel)//不是增量模式才需要重新导出CS文件
                    ProtogenEditor.AllProto2CS();
                m_canClick = true;
            }
        }
        GUILayout.EndHorizontal();
    }

    void ShowOtherConfig()
    {
        if (!DrawHeader("所有表"))
        {
            return;
        }
        BeginScroll(0, false, true);
        foreach (string s in AllExcelList)
        {
            EditorGUILayout.BeginHorizontal();
            if (NewExcelList.Contains(s))
            {
                GUI.contentColor = Color.green;
                GUILayout.Label("NEW ", GUILayout.Width(40));
                GUI.contentColor = Color.white;
            }
            GUILayout.Label(s, GUILayout.Width(250));
            EditorGUILayout.EndHorizontal();
        }
        EndScroll();
    }

    /// <summary>
    /// 导出多个excel proto,生成cs文件
    /// </summary>
    /// <param name="list"></param>
    static void ExportVersionRes_ExcelSelect(List<string> list)
    {
        if (CheckExcelIsOpen())
        {
            return;
        }
        Excel2ProtoTool.ExportProto(list);
        AssetDatabase.Refresh();
        //将生成的proto导出到cs文件
        if (Directory.Exists(ProtogenEditor.ProtoTempFiledir))
            Directory.CreateDirectory(ProtogenEditor.ProtoTempFiledir);

        //使用ProtogenEditor.ProtoTempFiledir 里面的Proto文件转换ConfigProto.Cs文件 使用  ProtogenEditor.InputProtoFloder转换热更CS文件
        GenerateProtoCSfile.GenProtoCSFile(ProtogenEditor.ProtoTempFiledir,
               ProtogenEditor.ProtoTempFiledir + "ConfigProto.cs");
        AssetDatabase.Refresh();
        //    Excel2ProtoTool.CopyConfigProtoInExcelPath();

    }

    /// <summary>
    /// 检查excel是否打开
    /// </summary>
    /// <returns></returns>
    static bool CheckExcelIsOpen()
    {
        bool isopen = false;
        if (!CheckExcel)
        {
            return false;
        }
        foreach (System.Diagnostics.Process p in System.Diagnostics.Process.GetProcesses())
        {
            if (p != null && (p.ProcessName == "et" || p.ProcessName == "Excel" || p.ProcessName.Contains("Wps")))
            {
                EditorUtility.DisplayDialog("警告", "excel已经打开,请关闭再操作", "确定");
                isopen = true;
            }
        }
        return isopen;
    }
}

/// <summary>
/// excel转proto工具，主要逻辑在这里
/// </summary>
public class Excel2ProtoTool
{

    public static readonly string ConfigAdd = "Configs";

    /// <summary>
    /// excel路径key
    /// </summary>
    public static string ExcelPathKey
    {
        get
        {
            return Application.dataPath + "ExcelPathKey";
        }
    }

    /// <summary>
    /// 放热更cs文件路径key
    /// </summary>
    public static string JsonPathKey
    {
        get
        {
            return Application.dataPath + "JsonPathKey";
        }
    }

    public static string ExportBytesPathKey
    {
        get
        {
            return Application.dataPath + "ExportBytesPathKey";
        }
    }


    /// <summary>
    /// 就是导出cs的命名空间，可以自定义
    /// </summary>
    public const string PackageName = "DragonRun.Proto";

    /// <summary>
    /// 导出所有配置前端用
    /// </summary>
    public static void ExportExcelConfigAll()
    {
        string m_ExcelPath = ProtogenEditor.InPutExcelFloder;
        if (string.IsNullOrEmpty(m_ExcelPath))
        {
            Debug.Log("没有设置服务器SNV目录");
            return;
        }

        #region 设置好需要导出数据的配置表相关信息（因为变量名是驼峰命名，所以提前存起来）

        DirectoryInfo serverDirectoryInfo = new DirectoryInfo(m_ExcelPath);
        FileInfo[] serverFileInfos = serverDirectoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
        Dictionary<string, string> dicPath = new Dictionary<string, string>();
        Dictionary<string, string> classNameDic = new Dictionary<string, string>();

        int count = 0;
        foreach (FileInfo fileInfo in serverFileInfos)
        {
            if (!ProtogenEditor.UsefulExcel(fileInfo)) continue;
            if (ProtogenEditor.IsSampleFloderFile(fileInfo))
                continue;
            string name = ProtogenEditor.GetFileNameByExcelDirectory(fileInfo);
            classNameDic[name] = name;
            dicPath[name] = fileInfo.FullName;
            EditorUtility.DisplayProgressBar("读取Excel文件信息:", name, count * 1.0f / serverFileInfos.Length);
            count++;
        }
        #endregion

        #region 实例化总表数据
        Type AllConfigType = SysUtil.GetTypeByName(PackageName + "." + ProtogenEditor.AllConfigDataName);
        if (AllConfigType == null)
            return;

        var pathConfig = string.Format("{0}/{1}.bytes", ProtogenEditor.OutAllByteFloder, ProtogenEditor.AllConfigDataName);
        System.Object AllConfigData = null;
        try
        {
            if (File.Exists(pathConfig))
            {
                using (FileStream stream = File.OpenRead(pathConfig))
                    AllConfigData = ProtoBuf.Serializer.Deserialize<ConfigDatabase>(stream);
            }
            else
                AllConfigData = Activator.CreateInstance(AllConfigType);
        }
        catch (Exception e)
        {
            AllConfigData = Activator.CreateInstance(AllConfigType);
        }
    
        #endregion

        #region 遍历总表属性，把对应的数据赋值到总表实例里面（通过属性设值）

        Dictionary<string, ExcelFileToMd5> excelMd5Dic = ReadExcelMd5InfoDic();

        count = 0;
        System.Reflection.PropertyInfo[] fields = AllConfigType.GetProperties();
        foreach (System.Reflection.PropertyInfo f in fields)
        {
            if (f.Name.EndsWith("Config"))
            {
                //var start = Time.realtimeSinceStartup;//System.DateTime.Now.Ticks;
                int startremoveindex = f.Name.LastIndexOf("Config");
                string name = f.Name.Remove(startremoveindex);
                if (!dicPath.ContainsKey(name))
                    continue;
                EditorUtility.DisplayProgressBar("生成Excel字段信息:", name, count * 1.0f / serverFileInfos.Length);
                count++;

                string filePath = dicPath[name];
                if (excelMd5Dic.ContainsKey(filePath))
                {
                    if (excelMd5Dic[filePath].Md5Code == MD5Manager.Instance.BuildFileMd5(filePath))
                        continue;
                }

                System.Object tempData = CreateData(dicPath[name], classNameDic[name]);

                if (null == tempData) { continue; }

                f.SetValue(AllConfigData, tempData, null);
            }
        }
        #endregion

        System.IO.MemoryStream MstreamConfig = new System.IO.MemoryStream();
        ProtoBuf.Serializer.Serialize(MstreamConfig, AllConfigData);
        byte[] dataConfig = MstreamConfig.ToArray();

        System.IO.FileStream FstreamConfig = System.IO.File.Create(pathConfig);
        System.IO.BinaryWriter bwConfig = new System.IO.BinaryWriter(FstreamConfig);
        bwConfig.Write(dataConfig);
        FstreamConfig.Close();
        bwConfig.Close();
        MstreamConfig.Close();
        Debug.Log(pathConfig + "生成完毕");
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 每个表单独导出一份bytes和Json，可以给后端用
    /// </summary>
    public static void ExportExcelConfig()
    {
        if (string.IsNullOrEmpty(ProtogenEditor.InPutExcelFloder))
        {
            Debug.Log("没有设置Excel目录");
            return;
        }
        Dictionary<string, ExcelFileToMd5> excelmd5Dic = ReadExcelMd5InfoDic();

        DirectoryInfo serverDirectoryInfo = new DirectoryInfo(ProtogenEditor.InPutExcelFloder);
        FileInfo[] serverFileInfos = serverDirectoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);


        int count = 0;
        foreach (FileInfo fileInfo in serverFileInfos)
        {
            if (!ProtogenEditor.UsefulExcel(fileInfo))
                continue;

            if (ProtogenEditor.IsSampleFloderFile(fileInfo))
                continue;

            if (excelmd5Dic.ContainsKey(fileInfo.FullName))
            {
                string filemd5 = MD5Manager.Instance.BuildFileMd5(fileInfo.FullName);
                if (string.Equals(filemd5, excelmd5Dic[fileInfo.FullName].Md5Code))
                    continue;
            }

            string name = ProtogenEditor.GetFileNameByExcelDirectory(fileInfo);
            System.Object configObj = CreateData(fileInfo.FullName, name);

            string jsonstr = LitJson.JsonMapper.ToJson(configObj);
            jsonstr = jsonstr.Remove(0, jsonstr.IndexOf(":")+1);
            jsonstr = jsonstr.Remove( jsonstr.LastIndexOf("}"),1);


            if (!Directory.Exists(ProtogenEditor.OutJsonFloder))
                Directory.CreateDirectory(ProtogenEditor.OutJsonFloder);

            EditorUtility.DisplayProgressBar("生成.byte和json文件:", name, count * 1.0f / serverFileInfos.Length);
            using (StreamWriter filestream = File.CreateText(ProtogenEditor.OutJsonFloder + name + ".json"))
            {
                filestream.Write(jsonstr);
            }
         
            System.IO.MemoryStream MstreamConfig = new System.IO.MemoryStream();
            ProtoBuf.Serializer.Serialize(MstreamConfig, configObj);
            byte[] dataConfig = MstreamConfig.ToArray();
            var pathConfig = ProtogenEditor.OutSmallByteFloder + "/" + name + ".bytes";

            if (!Directory.Exists(ProtogenEditor.OutSmallByteFloder))
                Directory.CreateDirectory(ProtogenEditor.OutSmallByteFloder);

            FileStream FstreamConfig = File.Create(pathConfig);
            BinaryWriter bwConfig = new BinaryWriter(FstreamConfig);
            bwConfig.Write(dataConfig);
            FstreamConfig.Close();
            bwConfig.Close();
            MstreamConfig.Close();
            count++;
            Debug.Log(pathConfig + "生成完毕");
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 根据excel路径和对应的类名来实例化数据
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    static System.Object CreateData(string filePath, string name)
    {
        ExcelWorksheet workSheet = null;
        using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            ExcelPackage excelReader = new ExcelPackage(stream);
            ExcelWorkbook result = excelReader.Workbook;
            workSheet = result.Worksheets.First();
        }

        FileInfo fileInfo = new FileInfo(filePath);
        string comname = Excel2ProtoViewTool.GetCommonProtoName(fileInfo);
        Type dataType = null;
        if (string.IsNullOrEmpty(comname))
            //单个数据
            dataType = SysUtil.GetTypeByName(PackageName + "." + name + "Config");
        else
            dataType = SysUtil.GetTypeByName(PackageName + "." + comname + "Config");


        if (dataType == null)
        {
            Debug.LogError("type====" + name + "===is not find");
            return null;
        }

        //列表数据
        Type configType = SysUtil.GetTypeByName(PackageName + "." + name + "ConfigData");
        if (configType == null)
        {
            Debug.LogError("type=====" + name + "Config=======is not find");
            return null;
        }

        //获取列表变量
        System.Reflection.FieldInfo field = configType.GetField($"_{name}{ConfigAdd}",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.FlattenHierarchy |
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.GetField);


        if (field == null)
        {
            Debug.LogError("field not find !!! ======" + configType.Name + $"._{name}{ConfigAdd}");
            return null;
        }

        #region 遍历整个excel表读取每一行数据（可以扩展列表，枚举，其他表数据，这里只列出基本数据类型）
        //列
        int columns = workSheet.Dimension.End.Column;
        //行
        int rows = workSheet.Dimension.End.Row;
        System.Reflection.PropertyInfo[] tmpFileds = dataType.GetProperties(System.Reflection.BindingFlags.SetField | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        System.Object configObj = Activator.CreateInstance(configType);

        IList m_DataList = field.GetValue(configObj) as IList;
        try
        {
            //行
            for (int i = 0; i < rows; i++)
            {
                if (i > 2)
                {
                    System.Object target = Activator.CreateInstance(dataType);
                    //列
                    for (int j = 0, FiledsIndex = 0; j < columns; j++)
                    {
                        string fieldname = workSheet.GetValue<string>(3, j + 1);
                        string kk = workSheet.GetValue<string>(i + 1, j + 1);
                        System.Reflection.PropertyInfo property = GetFiledByName(tmpFileds, fieldname, FiledsIndex);

                        if (property == null)
                        {
                            Debug.LogWarning("not found filed " + fieldname + " file " + filePath);
                            continue;
                        }
                        if (FiledsIndex >= tmpFileds.Length)
                        {
                            continue;
                        }

                        TypeCode tpy = Type.GetTypeCode(property.PropertyType);

                        string value = workSheet.GetValue<string>(i + 1, j + 1);
                        if (string.IsNullOrEmpty(value))
                        {
                            value = "";
                        }
                        value = value.TrimEnd(' ');
                        if (!tmpFileds[FiledsIndex].CanWrite)
                        {
                            continue;
                        }
                        switch (tpy)
                        {
                            case TypeCode.Int32:

                                if (kk != null)
                                {
                                    if (string.IsNullOrEmpty(value))
                                    {
                                        value = "0";
                                    }
                                    try
                                    {
                                        property?.SetValue(target, Int32.Parse(value), null);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Debug.LogError(ex.ToString());
                                        Debug.LogError(string.Format("Data error: {0} : {2}:[{1}] is not int", name, workSheet.GetValue<string>(i + 1, j + 1), tmpFileds[j].Name));

                                        string key = workSheet.GetValue<string>(i + 1, 1);
                                        int keyValue;
                                        if (int.TryParse(key, out keyValue))
                                        {
                                            Debug.LogError("上条错误对应的ID：" + keyValue);
                                        }
                                    }

                                }
                                else
                                    property?.SetValue(target, 0, null);

                                break;

                            case TypeCode.Int64:

                                if (kk != null)
                                {
                                    if (string.IsNullOrEmpty(value))
                                        value = "0";
                                    try
                                    {
                                        property?.SetValue(target, Int64.Parse(value), null);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Debug.LogError(ex.ToString());
                                        Debug.LogError(string.Format("Data error: {0} : {2}:[{1}] is not long", name, workSheet.GetValue<string>(i + 1, j + 1), tmpFileds[j].Name));

                                        string key = workSheet.GetValue<string>(i + 1, 1);
                                        int keyValue;
                                        if (int.TryParse(key, out keyValue))
                                            Debug.LogError("上条错误对应的ID：" + keyValue);
                                    }

                                }
                                else
                                    property?.SetValue(target, 0, null);
                                break;

                            case TypeCode.String:
                                if (kk != null)
                                    property?.SetValue(target, workSheet.GetValue<string>(i + 1, j + 1), null);
                                else
                                    property?.SetValue(target, "", null);
                                break;

                            case TypeCode.Single:
                                if (kk != null)
                                {
                                    try
                                    {
                                        if (string.IsNullOrEmpty(value))
                                            value = "0";
                                        property?.SetValue(target, float.Parse(value), null);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        Debug.LogError(ex.ToString());
                                        Debug.LogError(string.Format("Data error: {0} : {2}:[{1}] is not float", name, workSheet.GetValue<string>(i + 1, j + 1), tmpFileds[j].Name));
                                    }
                                }
                                else
                                    property?.SetValue(target, 0, null);

                                break;
                            case TypeCode.Boolean:
                                bool bvalue = workSheet.GetValue<bool>(i + 1, j + 1);
                                property?.SetValue(target, bvalue, null);
                                break;
                            default:
                                break;
                        }

                        FiledsIndex++;
                    }

                    m_DataList.Add(target);

                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(filePath + " error:" + e.ToString());
        }

        #endregion

        #region 校验数据
        #endregion
        return configObj;
    }


    static System.Reflection.PropertyInfo GetFiledByName(System.Reflection.PropertyInfo[] tmpFileds, string filedname, int index)
    {
        if (tmpFileds != null)
        {
            if (tmpFileds.Length > index && tmpFileds[index].Name == filedname)
            {
                return tmpFileds[index];
            }
            for (int i = 0; i < tmpFileds.Length; i++)
            {
                if (tmpFileds[i].Name == filedname)
                    return tmpFileds[i];
            }
        }
        return null;

    }

    static void ExportSampleProto()
    {
        if (Directory.Exists(ProtogenEditor.SampleExcelFloder))
        {
            DirectoryInfo sampleDirectoryInfo = new DirectoryInfo(ProtogenEditor.SampleExcelFloder);
            FileInfo[] serverFileInfos = sampleDirectoryInfo.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly);
            foreach (System.IO.FileInfo fileInfo in serverFileInfos)
            {
                if (!ProtogenEditor.UsefulExcel(fileInfo)) continue;
                string name = ProtogenEditor.GetFileNameByExcelDirectory(fileInfo, true);
                //单个配置相关数据
                CreateConfigScrptData scriptData = new CreateConfigScrptData();

                ExcelWorksheet workSheet = null;
                using (FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    ExcelPackage excelReader = new ExcelPackage(stream);
                    ExcelWorkbook result = excelReader.Workbook;
                    workSheet = result.Worksheets.First();
                }

                scriptData.excelName = name;
                scriptData.ExcelNameContainFloder = name;

                GennerateProtoFile(workSheet, fileInfo, scriptData, false);
                GennerateProtoFile(workSheet, fileInfo, scriptData, true);
                Excel2ProtoViewTool.mAlreadySetCommonExcels.Add(name);

                Debug.Log("build Proto:" + name + " .xlsx");
            }
        }

    }


    /// <summary>
    /// 导出proto文件
    /// </summary>
    /// <param name="platform"></param>
    /// <param name="selectList"></param>
    public static void ExportProto(List<string> selectList)
    {
        if (string.IsNullOrEmpty(ProtogenEditor.InPutExcelFloder))
        {
            Debug.Log("没有设置Excel目录");
            return;
        }
        if (selectList == null) return;
        //先导出样板
        ExportSampleProto();

        DirectoryInfo serverDirectoryInfo = new DirectoryInfo(ProtogenEditor.InPutExcelFloder);
        FileInfo[] serverFileInfos = serverDirectoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);

        //可导配置列表
        List<string> excelNames = new List<string>();
        //可导配置相关信息（用于创建初始化代码）
        List<CreateConfigScrptData> scriptDataList = new List<CreateConfigScrptData>();


        DirectoryInfo sampledir = null;
        FileInfo[] samplefiles = null;
        if (Directory.Exists(ProtogenEditor.SampleExcelFloder))
        {
            sampledir = new DirectoryInfo(ProtogenEditor.SampleExcelFloder);
            samplefiles = sampledir.GetFiles();
        }

        #region 遍历所有配置导出单个proto文件
        int progress = 0;
        foreach (System.IO.FileInfo fileInfo in serverFileInfos)
        {
            if (!ProtogenEditor.UsefulExcel(fileInfo)) continue;
            if (samplefiles!=null)
            {
                bool issample = false;
                for (int i = 0; i < samplefiles.Length; i++)
                {
                    if (samplefiles[i].FullName== fileInfo.FullName)
                    {
                        issample = true;
                        continue;
                    }
                }
                if (issample) continue;
            }
            string name = ProtogenEditor.GetFileNameByExcelDirectory(fileInfo);

            //单个配置相关数据
            CreateConfigScrptData scriptData = new CreateConfigScrptData();

            scriptDataList.Add(scriptData);
            excelNames.Add(name);

            Debug.Log("build Proto:" + name + " .xlsx");

            EditorUtility.DisplayProgressBar("生成.proto文件:", name, progress * 1.0f / serverFileInfos.Length);
            progress++;
            ExcelWorksheet workSheet = null;
            using (FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read))
            {
                ExcelPackage excelReader = new ExcelPackage(stream);
                ExcelWorkbook result = excelReader.Workbook;
                workSheet = result.Worksheets.First();
            }

            Excel2ProtoViewTool.ProtoType protoType = Excel2ProtoViewTool.GetProtoType(fileInfo, out string commonname);
            switch (protoType)
            {
                case Excel2ProtoViewTool.ProtoType.Common:
                    Debug.LogError($"{commonname} excel样板未设置");
                    return;
                case Excel2ProtoViewTool.ProtoType.ContainSub:
                    scriptData.excelName = commonname;
                    scriptData.ExcelNameContainFloder = name;
                    break;
                default:
                    scriptData.excelName = name;
                    scriptData.ExcelNameContainFloder = "";
                    break;
            }

            GennerateProtoFile(workSheet, fileInfo, scriptData, false);
            GennerateProtoFile(workSheet, fileInfo, scriptData, true);
        }
        #endregion

        #region 创建一个总表proto文件

        string protoNewValue = "package " + PackageName + ";\r\n\r\n";
        foreach (string str in excelNames)
        {
            protoNewValue += " import \"" + str + ".proto\";\r\n";
        }

        protoNewValue += "\r\nmessage " + ProtogenEditor.AllConfigDataName + " {\r\n";
        for (int i = 0; i < excelNames.Count; i++)
        {
            protoNewValue += "\trequired " + excelNames[i] + "ConfigData " + excelNames[i] + "Config = " + (i + 1) + ";\r\n";
        }

        protoNewValue += "}\r\n";

        CreateProtoFile(protoNewValue, $"{ProtogenEditor.ProtoTempFiledir}{ProtogenEditor.AllConfigDataName}.proto");

        protoNewValue = protoNewValue.Replace(PackageName, "Hot" + PackageName);
        protoNewValue = protoNewValue.Replace("required", "");
        protoNewValue = protoNewValue.Insert(0, "syntax = \"proto3\";\n");
        CreateProtoFile(protoNewValue, $"{ProtogenEditor.InputProtoFloder}{ProtogenEditor.AllConfigDataName}.proto");
        #endregion
        EditorUtility.ClearProgressBar();
        //自动生成配置代码
        CreateConfigScrpt(scriptDataList, true);

    }

    /// <summary>
    /// 构建proto文件
    /// </summary>
    /// <param name="workSheet"></param>
    /// <param name="fileInfo"></param>
    /// <param name="name"></param>
    /// <param name="scriptData"></param>
    static void GennerateProtoFile(ExcelWorksheet workSheet, FileInfo fileInfo, CreateConfigScrptData scriptData, bool isproto3)
    {     //列
        int columns = workSheet.Dimension.End.Column;
        ///行
        int rows = workSheet.Dimension.End.Row;

        if (rows < 3)
        {
            Debug.LogError("选择的excel表行数小于3");
            return;
        }

        string comproname = "";
        Excel2ProtoViewTool.ProtoType prototype = Excel2ProtoViewTool.GetProtoType(fileInfo, out comproname);

        string prototypeName = ProtogenEditor.GetFileNameByExcelDirectory(fileInfo);

        string SaveValue = "package " + PackageName + ";\n\n";
        switch (prototype)
        {
            case Excel2ProtoViewTool.ProtoType.ContainSub:
                SaveValue += " import \"" + comproname + ".proto\";\r\n\n";
                SaveValue += "message " + prototypeName + "ConfigData{\n";
                SaveValue += "\trepeated " + comproname + "Config " + prototypeName + ConfigAdd + " = 1;\n";
                SaveValue += "}\n";
                SetSheetInfo(columns, workSheet, fileInfo, scriptData, prototype);

                break;
            case Excel2ProtoViewTool.ProtoType.Normal:
                SaveValue += "message " + prototypeName + "Config{\n";
                SaveValue += SetSheetInfo(columns, workSheet, fileInfo, scriptData, prototype);

                SaveValue += "}\n\n";
                SaveValue += "message " + prototypeName + "ConfigData{\n";
                SaveValue += "\trepeated " + prototypeName + "Config " + prototypeName + ConfigAdd + " = 1;\n";
                SaveValue += "}\n";
                break;
            case Excel2ProtoViewTool.ProtoType.Common:
                prototypeName = comproname;
                SaveValue += "message " + comproname + "Config{\n";
                SaveValue += SetSheetInfo(columns, workSheet, fileInfo, scriptData, prototype);

                SaveValue += "}\n\n";
                break;
            default:
                MyDebuger.LogError("Not found type " + prototype);
                break;
        }

        //包含共同proto
        if (isproto3)
        {
            SaveValue = SaveValue.Replace(PackageName, "Hot" + PackageName);
            SaveValue = SaveValue.Replace("required", "");
            SaveValue = SaveValue.Insert(0, "syntax = \"proto3\";\n");
            CreateProtoFile(SaveValue, ProtogenEditor.InputProtoFloder + prototypeName + ".proto");
        }
        else
        {
            CreateProtoFile(SaveValue, ProtogenEditor.ProtoTempFiledir + prototypeName + ".proto");
        }
    }

    static string SetSheetInfo(int columns, ExcelWorksheet workSheet, FileInfo fileInfo, CreateConfigScrptData scriptData, Excel2ProtoViewTool.ProtoType protoType)
    {
        string SaveValue = "";

        for (int j = 0; j <= columns; j++)
        {
            if (j == 0) { continue; }
            string valueName = workSheet.GetValue<string>(3, j);
            if (string.IsNullOrEmpty(valueName))
            {
                Debug.LogError(fileInfo.FullName + "第" + (j + 1) + "列变量名为空");
                continue;
            }
            valueName = valueName.TrimEnd(' ');
            string explain = workSheet.GetValue<string>(1, j);
            if (!string.IsNullOrEmpty(explain))
            {
                explain = explain.Replace("\n"," ");
            }
            string type = workSheet.GetValue<string>(2, j);
            //保存第一个字段的类型以及变量名
            if (j == 1)
            {
                scriptData.VariableName = valueName;
                scriptData.TypeName = type;
            }
            switch (protoType)
            {
                case Excel2ProtoViewTool.ProtoType.Normal:
                case Excel2ProtoViewTool.ProtoType.Common:
                    if (type == "int")
                        type = "int32";
                    else if (type == "long")
                        type = "int64";
                    SaveValue += "\trequired " + type + " " + valueName + " = " + (j) + ";\t\t//" + explain + "\n";
                    break;
            }
        }
        return SaveValue;
    }

    static void CreateProtoFile(string protoStr, string protoFieInfo)
    {
        System.IO.FileStream Fstream = System.IO.File.Create(protoFieInfo);
        char[] data = protoStr.ToCharArray();
        System.IO.BinaryWriter bw = new System.IO.BinaryWriter(Fstream);
        bw.Write(data);
        bw.Close();
        Fstream.Close();
    }

    public static void CreateConfigScrpt(List<CreateConfigScrptData> scriptDataList, bool isHot)
    {
        string temp = "";
        temp += "using System.Collections.Generic;\n";
        if (isHot)
        {
            temp += "using Google.Protobuf.Collections;\n";
            temp += "namespace Hot" + PackageName + "{\n";
            temp += "public class GetProtoConfig :HotSingleton<GetProtoConfig>{ \n";
        }
        else
        {
            temp += "namespace " + PackageName + "{\n";
            temp += "public class GetProtoConfig{ \n";
        }

        temp += "private " + ProtogenEditor.AllConfigDataName + " AllConfig;\n";
        temp += "public void InitConfig(" + ProtogenEditor.AllConfigDataName + " data){\n";
        temp += "AllConfig = data;\n";
        temp += "}\n";


        // tempData.excelName 如果不是共同类型则没有问题 如果是共同类型或者则需要添加文件夹信息

        for (int i = 0; i < scriptDataList.Count; i++)
        {
            CreateConfigScrptData tempData = scriptDataList[i];

            string camelcaseStr = "";
            string dataListName = "";
            string configName = "";
            string dicname = "";
            ///共用类型
            if (!string.IsNullOrEmpty(tempData.ExcelNameContainFloder))
            {
                camelcaseStr = tempData.ExcelNameContainFloder;
                dicname = "m_" + tempData.ExcelNameContainFloder + "Dic";
                configName = tempData.excelName + "Config";
                dataListName = $"AllConfig.{ tempData.ExcelNameContainFloder}Config.{tempData.ExcelNameContainFloder}{ConfigAdd}";
            }

            else
            {
                camelcaseStr = tempData.excelName;
                configName = tempData.excelName + "Config";
                dicname = "m_" + tempData.excelName + "Dic";
                dataListName = $"AllConfig.{ configName}.{tempData.excelName}{ConfigAdd}";
            }

            temp += string.Format("private Dictionary<{0}, {1}> {2} ", tempData.TypeName, configName, dicname) + ";\n";
            temp += string.Format("public Dictionary<{0}, {1}> {2} =>Get{3}();\n", tempData.TypeName, configName, dicname.Replace("m_", ""), dicname.Replace("m_", ""));

            //增加填充字典数据方法
            temp += string.Format("private Dictionary<{0}, {1}> Get{2}()\n", tempData.TypeName, configName, dicname.Replace("m_", ""));

            temp += "{\n";
            temp += string.Format("if ({0}==null)", dicname);
            temp += "{\n";
            temp += string.Format("{0} = new Dictionary<{1}, {2}>();\n", dicname, tempData.TypeName, configName);
            temp += string.Format(" foreach({0} oneConfig in {1})\n  {2}[oneConfig.{3}] = oneConfig;\n", configName, dataListName, dicname, ProtogenEditor.FirstCharBig(tempData.VariableName,tempData.excelName));
            temp += "\n}\n";
            temp += string.Format("return {0};\n", dicname);
            temp += "}\n";


            temp += string.Format("public {0} Get{1}({2} id)\n", configName, camelcaseStr, tempData.TypeName);
            temp += "{\n";
            temp += string.Format("if({0}.ContainsKey(id))\n  return {1}[id];  \n", dicname.Replace("m_", ""), dicname.Replace("m_", ""));
            temp += $"MyDebuger.LogError(\"{ dicname.Replace("m_", "")} not found \"+ id);\n \nreturn null;";
            temp += "\n}\n";

            if (isHot)
            {
                ///共用类型
                if (!string.IsNullOrEmpty(tempData.ExcelNameContainFloder))
                    temp += string.Format("public RepeatedField<{0}> Get{1}ConfigList()\n", configName, tempData.ExcelNameContainFloder);
                else
                    temp += string.Format("public RepeatedField<{0}> Get{1}List()\n", configName, configName);
            }
            else
            {   ///共用类型
                if (!string.IsNullOrEmpty(tempData.ExcelNameContainFloder))
                    temp += string.Format("public List<{0}> Get{1}ConfigList()\n", configName, tempData.ExcelNameContainFloder);
                else
                    temp += string.Format("public List<{0}> Get{1}List()\n", configName, configName);
            }

            temp += "{\n";
            temp += string.Format("return {0};\n", dataListName);
            temp += "}\n";

        }
        temp += "}\n}\n";
        string filePath = ProtogenEditor.OutPutProtoCsFloder + "/GetProtoConfig.cs";
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
        //获得字节数组
        byte[] wiritedata = System.Text.Encoding.Default.GetBytes(temp);
        //开始写入
        fs.Write(wiritedata, 0, wiritedata.Length);
        //清空缓冲区、关闭流
        fs.Flush();
        fs.Close();
    }


    /// <summary>
    /// 将Excel信息写入MD5 是否是用来对proto判断
    /// </summary>
    public static void WriteExcelMd5Info()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(ProtogenEditor.InPutExcelFloder);
        FileInfo[] excelfiles = directoryInfo.GetFiles("*.xlsx", SearchOption.AllDirectories);
        //ABMD5管理信息类
        ExcelMd5List md5List = new ExcelMd5List();
        md5List.ExcelMd5InfoList = new List<ExcelFileToMd5>();
        for (int i = 0; i < excelfiles.Length; i++)
        {
            ExcelFileToMd5 excelmd5info = new ExcelFileToMd5();
            excelmd5info.FileName = excelfiles[i].FullName;
            excelmd5info.Md5Code = MD5Manager.Instance.BuildFileMd5(excelfiles[i].FullName);
            excelmd5info.Size = excelfiles[i].Length / 1024.0f;//kb
            md5List.ExcelMd5InfoList.Add(excelmd5info);
        }
        string xmlpath = ProtogenEditor.InPutExcelFloder + "/ExcelMD5Info.xml";
        xmlpath = xmlpath.Replace("//", "/");
        //=========在外部保存MD5资源版本信息=================
        EditorTool.WriteXMLFileInfo(xmlpath, md5List);
        Debug.Log($"{xmlpath} 创建文件成功!");
    }



    /// <summary>
    /// 获取Excel字典信息
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, ExcelFileToMd5> ReadExcelMd5InfoDic()
    {
        if (!Excel2ProtoViewTool.IsAddModel)
            return new Dictionary<string, ExcelFileToMd5>();
        if (Excel2ProtoViewTool.mExcelMd5InfoDic.Count < 1)
        {
            string xmlpath = ProtogenEditor.InPutExcelFloder + "/ExcelMD5Info.xml";
            xmlpath = xmlpath.Replace("//", "/");
            ExcelMd5List excelMd5 = null;
            try
            {
                 excelMd5 = EditorTool.ReadXMLFileInfo<ExcelMd5List>(xmlpath);
            }
            catch (Exception e)
            {
                Debug.LogWarning("增量模式无效 md5文件出错 "+e.Message);
            }
    
            if (excelMd5 != null && excelMd5.ExcelMd5InfoList != null)
            {
                for (int i = 0; i < excelMd5.ExcelMd5InfoList.Count; i++)
                {
                    ExcelFileToMd5 md5 = excelMd5.ExcelMd5InfoList[i];
                    Excel2ProtoViewTool.mExcelMd5InfoDic[md5.FileName] = md5;
                }
            }
        }
        return Excel2ProtoViewTool.mExcelMd5InfoDic;
    }


}

/// <summary>
/// 脚本数据
/// </summary>
public class CreateConfigScrptData
{
    /// <summary>
    /// excel名 数据类型
    /// </summary>
    public string excelName;
    /// <summary>
    /// 类型名
    /// </summary>
    public string TypeName;
    /// <summary>
    /// 变量名
    /// </summary>
    public string VariableName;
    /// <summary>
    /// 包含文件夹名称的Excel名
    /// </summary>
    public string ExcelNameContainFloder;
}
