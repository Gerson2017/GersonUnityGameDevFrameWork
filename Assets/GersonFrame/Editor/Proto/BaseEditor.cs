using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BaseEditor : EditorWindow
{
    Dictionary<string, bool> BoolDic = new Dictionary<string, bool>();

    public bool Foldout(string name)
    {
        if (!BoolDic.ContainsKey(name))
        {
            BoolDic[name] = true;
        }
        BoolDic[name] = EditorGUILayout.Foldout(BoolDic[name], name);
        return BoolDic[name];
    }

    /// <summary>
    /// 用于记录滚动坐标
    /// </summary>
    private List<Vector2> scrollPosList = new List<Vector2>();

    public virtual void OnEnable()
    {

    }

    public virtual void OnGUI()
    {
        
    }

    /// <summary>
    /// 对象输入框
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="anotherLine"></param>
    public void ObjectField(ref UnityEngine.Object obj, System.Type type, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        obj = EditorGUILayout.ObjectField(obj, type, true);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 带名字对象输入框
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="anotherLine"></param>
    /// <param name="name"></param>
    public void ObjectFieldWithName(ref UnityEngine.Object obj, System.Type type, bool anotherLine = true,string name = "")
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        obj = EditorGUILayout.ObjectField(obj, type, true);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    public void ShowLabel(string str, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(str,GUILayout.MinWidth(20f));
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 文字输入框
    /// </summary>
    /// <param name="label"></param>
    /// <param name="text"></param>
    public void TextField(string label, ref string text,bool anotherLine = true)
    {
        if(anotherLine)
        GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        text = EditorGUILayout.TextField(text);
        if (anotherLine)
        GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 整形输入框
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    public void IntField(string label,ref int value, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        value = EditorGUILayout.IntField(value);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    public void FloatField(string label, ref float value, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        value = EditorGUILayout.FloatField(value);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 开始滚动 从0开始每加一个滚动 index 加1
    /// </summary>
    /// <param name="index"></param>
    public void BeginScroll(int index,bool BoolHorizontal = true,bool BoolVertical = true)
    {
        if (scrollPosList.Count < index + 1)
        {
            scrollPosList.Add(Vector2.zero);
        }
        scrollPosList[index] = EditorGUILayout.BeginScrollView(scrollPosList[index], BoolHorizontal, BoolVertical);
    }

    /// <summary>
    /// 滚动结束标志
    /// </summary>
    public void EndScroll()
    {
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 内容范围框
    /// </summary>
    public static void BeginContents()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    public void BeginContents2(float maxWidth = 0)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(4f);
        if(maxWidth == 0)
        {
            GUILayout.BeginVertical("TextArea");
        }
        else
        {
            GUILayout.BeginVertical("TextArea", GUILayout.Width(maxWidth));
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    public void EndContents2()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        GUILayout.EndVertical();
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        GUILayout.Space(3f);
    }

    /// <summary>
    /// 内容结束标志
    /// </summary>
    public static void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    public static void GUIGreen()
    {
        GUI.backgroundColor = Color.green;
    }
    public static void GUIWhite()
    {
        GUI.backgroundColor = Color.white;
    }
    public static void GUIRed()
    {
        GUI.backgroundColor = Color.red;
    }

    /// <summary>
    /// 标题伸缩
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public bool DrawHeader(string text) { return DrawHeader(text, text, false); }
    public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false); }
    public bool DrawHeader(string text, bool forceOn) { return DrawHeader(text, text, forceOn); }
    public bool DrawHeader(string text, string key, bool forceOn)
    {
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        text = "<b><size=11>" + text + "</size></b>";
        if (state) text = "\u25B2 " + text;
        else text = "\u25BC " + text;
        if (!GUILayout.Toggle(true, text, "dragtab")) state = !state;

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);

        return state;
    }

    public static bool DrawHeaderWithLenght(string text,float length) { return DrawHeaderWithLenght(text, text, false,length); }
    public static bool DrawHeaderWithLenght(string text, string key, float length) { return DrawHeaderWithLenght(text, key, false, length); }
    public static bool DrawHeaderWithLenght(string text, bool forceOn, float length) { return DrawHeaderWithLenght(text, text, forceOn, length); }
    public static bool DrawHeaderWithLenght(string text, string key, bool forceOn, float length)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

        GUI.changed = false;

        text = "<b><size=11>" + text + "</size></b>";
        if (state) text = "\u25B2 " + text;
        else text = "\u25BC " + text;
        if (!GUILayout.Toggle(true, text, "dragtab",GUILayout.Width(length))) state = !state;
        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUI.backgroundColor = Color.white;

        return state;
    }

    /// <summary>
    /// unity自带对话框
    /// </summary>
    /// <param name="tips"></param>
    /// <returns></returns>
    public bool DisplayDialog(string tips)
    {
        return EditorUtility.DisplayDialog("提示", tips, "确定", "取消");
    }
}

public class BaseEditorInspector : Editor
{
    Dictionary<string, bool> BoolDic = new Dictionary<string, bool>();

    public bool Foldout(string name)
    {
        if (!BoolDic.ContainsKey(name))
        {
            BoolDic[name] = true;
        }
        BoolDic[name] = EditorGUILayout.Foldout(BoolDic[name], name);
        return BoolDic[name];
    }

    /// <summary>
    /// 用于记录滚动坐标
    /// </summary>
    private List<Vector2> scrollPosList = new List<Vector2>();

    public virtual void OnEnable()
    {

    }

    public virtual void OnGUI()
    {

    }

    /// <summary>
    /// 对象输入框
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="anotherLine"></param>
    public void ObjectField(ref UnityEngine.Object obj, System.Type type, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        obj = EditorGUILayout.ObjectField(obj, type, true);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 带名字对象输入框
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="type"></param>
    /// <param name="anotherLine"></param>
    /// <param name="name"></param>
    public void ObjectFieldWithName(ref UnityEngine.Object obj, System.Type type, bool anotherLine = true, string name = "")
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        obj = EditorGUILayout.ObjectField(obj, type, true);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    public void ObjectTranform(ref Transform obj, string name = "")
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name);
        obj = EditorGUILayout.ObjectField(obj, typeof(Transform), true) as Transform;
        GUILayout.EndHorizontal();
    }

    public void ShowLabel(string str, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(str, GUILayout.MinWidth(20f));
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 文字输入框
    /// </summary>
    /// <param name="label"></param>
    /// <param name="text"></param>
    public void TextField(string label, ref string text, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        text = EditorGUILayout.TextField(text);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 整形输入框
    /// </summary>
    /// <param name="value"></param>
    /// <param name="name"></param>
    public void IntField(string label, ref int value, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        value = EditorGUILayout.IntField(value);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    public void FloatField(string label, ref float value, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        GUILayout.Label(label);
        value = EditorGUILayout.FloatField(value);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }


    public void Vector3Field(string label, ref Vector3 value, bool anotherLine = true)
    {
        if (anotherLine)
            GUILayout.BeginHorizontal();
        value = EditorGUILayout.Vector3Field(label,value);
        if (anotherLine)
            GUILayout.EndHorizontal();
    }

    /// <summary>
    /// 开始滚动 从0开始每加一个滚动 index 加1
    /// </summary>
    /// <param name="index"></param>
    public void BeginScroll(int index, bool BoolHorizontal = true, bool BoolVertical = true)
    {
        if (scrollPosList.Count < index + 1)
        {
            scrollPosList.Add(Vector2.zero);
        }
        scrollPosList[index] = EditorGUILayout.BeginScrollView(scrollPosList[index], BoolHorizontal, BoolVertical);
    }

    /// <summary>
    /// 滚动结束标志
    /// </summary>
    public void EndScroll()
    {
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 内容范围框
    /// </summary>
    public static void BeginContents()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Space(4f);
        EditorGUILayout.BeginHorizontal("TextArea", GUILayout.MinHeight(10f));
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    public void BeginContents2(float maxWidth = 0)
    {
        GUILayout.BeginVertical();
        GUILayout.Space(4f);
        if (maxWidth == 0)
        {
            GUILayout.BeginVertical("TextArea");
        }
        else
        {
            GUILayout.BeginVertical("TextArea", GUILayout.Width(maxWidth));
        }
        GUILayout.BeginVertical();
        GUILayout.Space(2f);
    }

    public void EndContents2()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        GUILayout.EndVertical();
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        GUILayout.Space(3f);
    }

    /// <summary>
    /// 内容结束标志
    /// </summary>
    public static void EndContents()
    {
        GUILayout.Space(3f);
        GUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(3f);
        GUILayout.EndHorizontal();
        GUILayout.Space(3f);
    }

    public static void GUIGreen()
    {
        GUI.backgroundColor = Color.green;
    }
    public static void GUIWhite()
    {
        GUI.backgroundColor = Color.white;
    }
    public static void GUIRed()
    {
        GUI.backgroundColor = Color.red;
    }

    /// <summary>
    /// 标题伸缩
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public bool DrawHeader(string text) { return DrawHeader(text, text, false); }
    public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false); }
    public bool DrawHeader(string text, bool forceOn) { return DrawHeader(text, text, forceOn); }
    public bool DrawHeader(string text, string key, bool forceOn)
    {
        bool state = EditorPrefs.GetBool(key, true);

        GUILayout.Space(3f);
        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
        GUILayout.BeginHorizontal();
        GUILayout.Space(3f);

        GUI.changed = false;

        text = "<b><size=11>" + text + "</size></b>";
        if (state) text = "\u25B2 " + text;
        else text = "\u25BC " + text;
        if (!GUILayout.Toggle(true, text, "dragtab")) state = !state;

        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUILayout.Space(2f);
        GUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;
        if (!forceOn && !state) GUILayout.Space(3f);

        return state;
    }

    public static bool DrawHeaderWithLenght(string text, float length) { return DrawHeaderWithLenght(text, text, false, length); }
    public static bool DrawHeaderWithLenght(string text, string key, float length) { return DrawHeaderWithLenght(text, key, false, length); }
    public static bool DrawHeaderWithLenght(string text, bool forceOn, float length) { return DrawHeaderWithLenght(text, text, forceOn, length); }
    public static bool DrawHeaderWithLenght(string text, string key, bool forceOn, float length)
    {
        bool state = EditorPrefs.GetBool(key, true);

        if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);

        GUI.changed = false;

        text = "<b><size=11>" + text + "</size></b>";
        if (state) text = "\u25B2 " + text;
        else text = "\u25BC " + text;
        if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.Width(length))) state = !state;
        if (GUI.changed) EditorPrefs.SetBool(key, state);

        GUI.backgroundColor = Color.white;

        return state;
    }

    /// <summary>
    /// unity自带对话框
    /// </summary>
    /// <param name="tips"></param>
    /// <returns></returns>
    public bool DisplayDialog(string tips)
    {
        return EditorUtility.DisplayDialog("提示", tips, "确定", "取消");
    }
}