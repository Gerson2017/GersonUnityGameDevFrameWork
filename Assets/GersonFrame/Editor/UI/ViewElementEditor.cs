using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;

public class ViewElementEditor : EditorWindow
{
    public enum ElementType
    {
        UI,
        Enemy,
        Player
    }

    private string Fight_HotNameSpace="HotFix_Dragon.Fight_Hot";
    private string Fight_UINameSpace = "HotFix_Dragon.UI_Hot";

    public string HotFightEnemyCsFilePath = "/HotFix_Dragon~/Fight_Hot/Role/Enemy/ViewElement/";
    public string HotFightPlayerCsFilePath = "/HotFix_Dragon~/Fight_Hot/ViewElement/";
    public string HotUICsFilePath = "/HotFix_Dragon~/UI_Hot/ViewElement/";
    public string UICsFilePath = "/Scripts/ViewElemnet/";

    private string m_csFilePath = "";

    public string ViewMudelFilePath = "Assets/GersonFrame/Editor/UI/ViewElementModel.txt";
    private static GameObject m_viewGoRoot;
    private string GetCompentStr = "GetComponent<{0}>();\n";
    private bool m_click;
    [Header("是否包含自身")]
    public bool dealSelf = true;
    public bool dealChild = true;
    public ElementType m_ElementType = ElementType.UI;

    private List<string> m_properityList = new List<string>();
    private List<string> m_getCompentList = new List<string>();
    private SerializedObject m_obj;

    private SerializedProperty m_UICsFilePathProperity;
    private SerializedProperty m_EnemyCsFilePathProperity;
    private SerializedProperty m_PlayerCsFilePathProperity;

    private SerializedProperty m_ElementTypeProperity;


    private SerializedProperty m_DealChild;
    private SerializedProperty m_DealSelf;

    [MenuItem("GameObject/生成ViewElement", priority = 0)]
    static void Test()
    {
        m_viewGoRoot = Selection.activeObject as GameObject;
        MyDebuger.InitLogger(LogLevel.All);
        ShowEditor();
    }


    [MenuItem("MyTools/创建ViewElement数据")]
    static void ShowEditor()
    {
        ViewElementEditor combinewindow = GetWindow<ViewElementEditor>();
        combinewindow.minSize = new Vector2(370, 370);
    }

    private void OnEnable()
    {
        MyDebuger.InitLogger(LogLevel.All);
        m_click = false;
        m_obj = new SerializedObject(this);

        m_EnemyCsFilePathProperity = m_obj.FindProperty("HotFightEnemyCsFilePath");
        m_UICsFilePathProperity = m_obj.FindProperty("HotUICsFilePath");
        m_PlayerCsFilePathProperity = m_obj.FindProperty("HotFightPlayerCsFilePath");
        m_ElementTypeProperity = m_obj.FindProperty("m_ElementType");

        m_DealSelf = m_obj.FindProperty("dealSelf");
        m_DealChild = m_obj.FindProperty("dealChild");
  
    }
    private void OnGUI()
    {
        BeginWindows();
        m_obj.Update();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(m_ElementTypeProperity, true);

        switch (m_ElementType)
        {
            case ElementType.UI:
                EditorGUILayout.LabelField("UICS文件路径:");
                EditorGUILayout.PropertyField(m_UICsFilePathProperity, true);
                break;
            case ElementType.Enemy:
                EditorGUILayout.LabelField("EnemyCS文件路径:");
                EditorGUILayout.PropertyField(m_EnemyCsFilePathProperity, true);
                break;
            case ElementType.Player:
                EditorGUILayout.LabelField("PlayerCS文件路径:");
                EditorGUILayout.PropertyField(m_PlayerCsFilePathProperity, true);
                break;
            default:
                break;
        }
        EditorGUILayout.LabelField("是否是包含被选中的节点:");
        EditorGUILayout.PropertyField(m_DealSelf, true);

        EditorGUILayout.LabelField("是否是包含子物体:");
        EditorGUILayout.PropertyField(m_DealChild, true);



        if (EditorGUI.EndChangeCheck())
            m_obj.ApplyModifiedProperties();

        EditorGUILayout.LabelField("View根节点:");
        m_viewGoRoot = (GameObject)EditorGUILayout.ObjectField(m_viewGoRoot, typeof(GameObject), true);
        this.m_click = GUILayout.Button("创建");
        EndWindows();
        if (m_click)
        {
            this.m_click = false;
            if (m_viewGoRoot == null)
            {
                MyDebuger.LogError("View根节点不能为空 ");
                return;
            }
            CreateViewElemnet();
        }
    }
    void CreateViewElemnet()
    {
        m_properityList.Clear();
        m_getCompentList.Clear();
        TextAsset basestrTxt = AssetDatabase.LoadAssetAtPath<TextAsset>(ViewMudelFilePath);
        string basestr = basestrTxt.text;
        string allpropertity = "";
        string allgetCompent = "";

        FindGoChild(m_viewGoRoot.transform, dealSelf);

       
        if (m_properityList.Count < 1)
        {
            MyDebuger.LogError("未找可生成的组件物体");
            return;
        }

        m_click = true;
        for (int i = 0; i < this.m_properityList.Count; i++)
            allpropertity += this.m_properityList[i];
        for (int i = 0; i < this.m_getCompentList.Count; i++)
            allgetCompent += this.m_getCompentList[i];
        string elementName = m_viewGoRoot.name + "ViewElement";
        string newbasestr = basestr.Replace("{0}", elementName);
        newbasestr = newbasestr.Replace("{1}", allpropertity);
        string newbasestr2 = newbasestr.Replace("{2}", allgetCompent);


        switch (m_ElementType)
        {
            case ElementType.UI:
                m_csFilePath = HotUICsFilePath;
                newbasestr2 = newbasestr2.Replace("{3}", Fight_UINameSpace);
                break;
            case ElementType.Enemy:
                m_csFilePath = HotFightEnemyCsFilePath;
                newbasestr2 = newbasestr2.Replace("{3}", Fight_HotNameSpace);
                break;
            case ElementType.Player:
                m_csFilePath = HotFightPlayerCsFilePath;
                newbasestr2 = newbasestr2.Replace("{3}", Fight_HotNameSpace);
                break;
            default:
                break;
        }

        string filepath = Application.dataPath + m_csFilePath + elementName + ".cs";
        if (File.Exists(filepath)) File.Delete(filepath);
        using (FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate))
        {
            using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
            {
                sw.Write(Regex.Replace(newbasestr2, "(?<!\r)\n", "\r\n"));
            }
          
        }
        MyDebuger.Log("创建 " + elementName + "成功!");
        ProtogenEditor.AutoAddHotCsReference();
        AssetDatabase.Refresh();
    }
    void FindGoChild(Transform ts,bool containself=true)
    {
        var isDeal = false;
        if (containself)
            isDeal= TsNeedAddInViewElement(ts);

        if (isDeal && !dealChild)
            return;
        for (int i = 0; i < ts.childCount; i++)
            FindGoChild(ts.GetChild(i));
    }
    bool TsNeedAddInViewElement(Transform childts)
    {
        bool isDeal = false;
        string properitystr = "";
        string tempgetCompentstr = "";
        string properityName = "m_" + childts.name;
        if (childts.name.EndsWith("_Txt"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Text");
            properitystr = "public Text " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Btn"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Button");
            properitystr = "public Button " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_RawImg"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "RawImage");
            properitystr = "public RawImage " + properityName + ";\n";
            isDeal = true;
        }
        else if (childts.name.EndsWith("_Img"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Image");
            properitystr = "public Image " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Ts"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Transform");
            properitystr = "public Transform " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_RectTs"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "RectTransform");
            properitystr = "public RectTransform " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_ScRect"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "ScrollRect");
            properitystr = "public ScrollRect " + properityName + ";\n";
        }
        if (childts.name.EndsWith("_InputField"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "InputField");
            properitystr = "public InputField " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Go"))
        {
            tempgetCompentstr = "gameObject;\n";
            properitystr = "public GameObject " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Ps"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "ParticleSystem");
            properitystr = "public ParticleSystem " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Am"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Animator");
            properitystr = "public Animator " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Sp"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "SpriteRenderer");
            properitystr = "public SpriteRenderer " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Rend"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Renderer");
            properitystr = "public Renderer " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_ProTxt"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "TextMeshProUGUI");
            properitystr = "public TextMeshProUGUI " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Col"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Collider");
            properitystr = "public Collider " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Mesh"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "MeshRenderer");
            properitystr = "public MeshRenderer " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Skin"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "SkinnedMeshRenderer");
            properitystr = "public SkinnedMeshRenderer " + properityName + ";\n";
            isDeal = true;
        }
        else if (childts.name.EndsWith("_Slider"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Slider");
            properitystr = "public Slider " + properityName + ";\n";
        }
        else if (childts.name.EndsWith("_Rg"))
        {
            tempgetCompentstr = GetCompentStr.Replace("{0}", "Rigidbody");
            properitystr = "public Rigidbody " + properityName + ";\n";
        }


        isDeal = true;
        if (!string.IsNullOrEmpty(properitystr))
        {
            m_properityList.Add(properitystr);
            string path = childts.GetPath(m_viewGoRoot.transform);
            if (path.Contains(" "))
            {
                MyDebuger.LogError("请确保路径中没有空格  " + path);
            }
            string tempgetCompentNameStr = properityName + "=go.transform.Find(" + '"' + path + '"' + ").";
            m_getCompentList.Add(tempgetCompentNameStr + tempgetCompentstr);
        }
        return isDeal;
    }

}