
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;
using System;
using UnityEditor;
using System.Reflection;
using System.Collections;
using UnityEngine.Networking;
using GersonFrame;
using GersonFrame.ABFrame;

public enum LogLevel
{
    NoLog = 0,
    Error = 1,
    Waring = 2,
    All = 3
}

public class MyDebuger
{

    [System.Serializable]
    public class LogLevelInfo
    {
        public string id;
        public int level;
    }

    private static string fullPath;
    private static bool m_hasInit = false;
    private static LogLevel m_logLevel = LogLevel.NoLog;

    public static void InitLogger(LogLevel logLevel)
    {
        if (m_hasInit) return;
        if (logLevel == LogLevel.NoLog) return;
        m_logLevel = logLevel;
        CheckLogFile();
        m_hasInit = true;
    }

    static void CheckLogFile()
    {
        if (m_hasInit) 
            return;
        string logfloder = Application.persistentDataPath + "/LogFloder";
        fullPath = logfloder + "/output.txt";
        if (File.Exists(fullPath))
            File.Delete(fullPath);
        if (!Directory.Exists(logfloder))
            Directory.CreateDirectory(logfloder);
        FileStream fileIo = File.Create(fullPath);
        fileIo.Dispose();
        fileIo.Close();
        Application.logMessageReceived += logCallBack;
    }

    public static void InitByNetWork(MonoBehaviour mono, Enviormnet env)
    {
        m_hasInit = false;
        CheckLogFile();
        switch (env)
        {
            case Enviormnet.Product:
                mono.StartCoroutine(InitLogIe("http://123.56.146.26:21102/"));
                break;
            default:
                mono.StartCoroutine(InitLogIe("http://127.0.0.1:21102/"));
                break;
        }
    }



    static IEnumerator InitLogIe(string ip)
    {
        string deviceId = SystemInfo.deviceUniqueIdentifier;
        Debug.Log(deviceId);
        string url = ip + "gm?op=getlog&id=" + deviceId;
        UnityWebRequest unityWeb = UnityWebRequest.Get(url);
        yield return unityWeb.SendWebRequest();
        if (unityWeb.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.LogError("InitLogIe error " + unityWeb.error);
            InitLogger(LogLevel.Error);
        }
        else
        {
            string text = unityWeb.downloadHandler.text;
            LogLevelInfo data = LitJson.JsonMapper.ToObject<LogLevelInfo>(text);
            LogLevel logLevel = LogLevel.All;
            if (HotPatchManager.Instance.mIsBusiness)
                Debug.Log(" initlogle url=" + url + " setvergetid=" + data.id + " level " + data.level);
            else
                logLevel = (LogLevel)data.level;
            InitLogger(logLevel);
     
        }
        m_hasInit = true;
    }


    private static void logCallBack(string condition, string stackTrace, LogType type)
    {
        if (m_hasInit)
        {
            if (type == LogType.Error || type == LogType.Warning || type == LogType.Exception)
            {
                using (StreamWriter sw = File.AppendText(fullPath))
                {
                    sw.WriteLine(condition);
                    sw.WriteLine(stackTrace);
                }
            }
        }
    }



    /// <summary>
    /// 本次log文本信息
    /// </summary>
    /// <returns></returns>
    public static string GetLogInfo()
    {
        if (File.Exists(fullPath))
        {
            string loginfo = File.ReadAllText(fullPath);
            return loginfo;
        }
        return "";
    }


    public static void SetUseDebuger(LogLevel logLevel)
    {
        m_logLevel = logLevel;
    }

    #region Debuge


    public static void Log(object message)
    {
        if (m_logLevel >= LogLevel.All)
            Debug.Log("[MyLog]" + message);
    }

    public static void Log(object message, UnityEngine.Object context)
    {
        if (m_logLevel >= LogLevel.All) 
            Debug.Log("[MyLog]" + message, context);
    }



    public static void LogError(object message)
    {
        if (m_logLevel >= LogLevel.Error)
            Debug.LogError("[MyLog]" + message);
    }

    public static void LogError(object message, UnityEngine.Object context)
    {
        if (m_logLevel >= LogLevel.Error)
            Debug.LogError("[MyLog]" + message, context);
    }

    public static void LogErrorFormat(string format, params object[] args)
    {
        if (m_logLevel >= LogLevel.Error)
            Debug.LogErrorFormat("[MyLog]" + format, args);
    }

    public static void LogErrorFormat(UnityEngine.Object context,
        string format, params object[] args)
    {
        if (m_logLevel >= LogLevel.Error)
            Debug.LogErrorFormat("[MyLog]" + context, format, args);
    }





    public static void LogFormat(string format, params object[] args)
    {
        if (m_logLevel >= LogLevel.All) 
            Debug.LogFormat("[MyLog]" + format, args);
    }

    public static void LogFormat(UnityEngine.Object context,
        string format, params object[] args)
    {
        if (m_logLevel >= LogLevel.All) Debug.LogFormat("[MyLog]" + context, format, args);
    }

    public static void LogWarning(object message)
    {
        if (m_logLevel >= LogLevel.Waring) 
            Debug.LogWarning("[MyLog]" + message);
    }


    public static void LogWarning(object message, object scriptobj)
    {
        if (m_logLevel >= LogLevel.Waring) Debug.LogWarning("[MyLog]" + message+" script "+scriptobj);
    }

    public static void LogWarning(object message,
        UnityEngine.Object context)
    {
        if (m_logLevel >= LogLevel.Waring) Debug.LogWarning("[MyLog]" + message, context);
    }

    public static void LogWarningFormat(string format,
        params object[] args)
    {
        if (m_logLevel >= LogLevel.Waring) 
            Debug.LogWarningFormat("[MyLog]" + format, args);
    }

    public static void LogWarningFormat(UnityEngine.Object context,
        string format, params object[] args)
    {
        if (m_logLevel >= LogLevel.Waring)
            Debug.LogWarningFormat("[MyLog]" + context, format, args);
    }


    public static void LogTodo(string todosomthing)
    {
           LogWarning("==============Todo " + todosomthing);
    }


    #endregion

    #region Editor
#if UNITY_EDITOR


    private static bool m_hasForceMono = false;
    // 处理asset打开的callback函数
    [UnityEditor.Callbacks.OnOpenAssetAttribute(-1)]
    static bool OnOpenAsset(int instance, int line)
    {
        if (m_hasForceMono) return false;

        // 自定义函数，用来获取log中的stacktrace，定义在后面。
        string stack_trace = GetStackTrace();
        // 通过stacktrace来定位是否是我们自定义的log，我的log中有特殊文字[SDebug]，很好识别
        if (!string.IsNullOrEmpty(stack_trace) && stack_trace.Contains("[MyLog]"))
        {
            // 正则匹配at xxx，在第几行
            Match matches = Regex.Match(stack_trace, @"\(at (.+)\)", RegexOptions.IgnoreCase);
            string pathline = "";
            while (matches.Success)
            {
                pathline = matches.Groups[1].Value;

                // 找到不是我们自定义log文件的那行，重新整理文件路径，手动打开
                if (!pathline.Contains("MyDebuger.cs") && !string.IsNullOrEmpty(pathline))
                {
                    int split_startindex = pathline.IndexOf("Assets");
                    int split_index = pathline.LastIndexOf(":");
                    string path = pathline.Substring(split_startindex, split_index- split_startindex);
                    line = Convert.ToInt32(pathline.Substring(split_index + 1));
                    m_hasForceMono = true;
                    //方式一
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path), line);
                    m_hasForceMono = false;
                    //方式二
                    //string fullpath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("Assets"));
                    // fullpath = fullpath + path;
                    //  UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(fullpath.Replace('/', '\\'), line);
                    return true;
                }
                matches = matches.NextMatch();
            }
            return true;
        }
        return false;
    }


    static string GetStackTrace()
    {
        // 找到类UnityEditor.ConsoleWindow
        var type_console_window = typeof(EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        // 找到UnityEditor.ConsoleWindow中的成员ms_ConsoleWindow
        var filedInfo = type_console_window.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic);
        // 获取ms_ConsoleWindow的值
        var ConsoleWindowInstance = filedInfo.GetValue(null);
        if (ConsoleWindowInstance != null)
        {
            if ((object)EditorWindow.focusedWindow == ConsoleWindowInstance)
            {
                // 找到类UnityEditor.ConsoleWindow中的成员m_ActiveText
                filedInfo = type_console_window.GetField("m_ActiveText", BindingFlags.Instance | BindingFlags.NonPublic);
                string activeText = filedInfo.GetValue(ConsoleWindowInstance).ToString();
                return activeText;
            }
        }
        return null;
    }
#endif
    #endregion

}