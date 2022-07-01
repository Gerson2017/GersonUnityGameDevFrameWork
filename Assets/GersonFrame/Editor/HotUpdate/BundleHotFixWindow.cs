using UnityEngine;
using UnityEditor;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using GersonFrame.ABFrame;

namespace GersonFrame
{


    public class BundleHotFixWindow : EditorWindow
    {
        string md5Path = "";
        string hotCunt = "1";
        string hotDesc = "";
        string curhotversion = "";
        BuildTargetGroup buildTargetGroup = BuildTargetGroup.Android;
        BuildTarget m_buildTarget = BuildTarget.Android;

        private SerializedObject serializedObject;


        OpenFileName m_openfileName = null;
        [MenuItem("MyTools/热更/打开热更包窗口")]
        static void Init()
        {
            BundleHotFixWindow window =GetWindow<BundleHotFixWindow>(false, "热更包窗口(选择的ABMd5配置文件进行打包)", true);
            window.minSize = new Vector2(370, 370);
            window.Show();
        }


        private void OnEnable()
        {
            serializedObject = new SerializedObject(this);
            string filepath = BundleEditor.m_HotPath + "/PatchVersion.xml";
            if (File.Exists(filepath))
            {
                GameVersion version= BinarySerializeOpt.XmlDeserialize<GameVersion>(filepath);
                if (version!=null&& version.Pathces!=null)
                {
                    hotCunt =(int.Parse(version.Pathces[version.Pathces.Length - 1].Version) +1).ToString();
                }
            }
        }


        private void OnGUI()
        {
            serializedObject.Update();
            GUILayout.BeginHorizontal();
            md5Path = EditorGUILayout.TextField("ABMD5路径", md5Path, GUILayout.Width(350), GUILayout.Height(20));
            if (GUILayout.Button("选择ABMD5版本文件", GUILayout.Width(150), GUILayout.Height(30)))
            {
                m_openfileName = new OpenFileName();
                m_openfileName.structSize = Marshal.SizeOf(m_openfileName);
                m_openfileName.filter = "ABMD5文件(*.bytes)\0*.bytes";
                m_openfileName.file = new string(new char[256]);
                m_openfileName.maxFile = m_openfileName.file.Length;
                m_openfileName.fileTitle = new string(new char[64]);
                m_openfileName.maxFileTitle = m_openfileName.fileTitle.Length;
                m_openfileName.initialDir = (Application.dataPath + "/../Version").Replace("/", "\\");//默认打开路径
                m_openfileName.title = "请选择对应版本的MD5文件";
                m_openfileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

                if (LocalDialog.GetSaveFileName(m_openfileName))
                {
                    Debug.Log(m_openfileName.file);
                    md5Path = m_openfileName.file;
                }
            }
            if (!string.IsNullOrEmpty(md5Path) && md5Path.EndsWith(".bytes"))
            {
                int last_Index = md5Path.LastIndexOf('_');
                string patharray = md5Path.Substring(last_Index + 1);
                int lastpointindex = patharray.LastIndexOf('.');
                curhotversion= patharray.Substring(0, lastpointindex);
            }
            GUILayout.EndHorizontal();
            //============================
            GUILayout.BeginVertical();

            hotCunt = EditorGUILayout.TextField("热更补丁版本(第几次热更)", hotCunt, GUILayout.Width(350), GUILayout.Height(20));
            EditorGUILayout.TextField("本次热更的版本号:", curhotversion, GUILayout.Width(210), GUILayout.Height(20));
            hotDesc = EditorGUILayout.TextField("本次热更描述:", hotDesc, GUILayout.Width(420), GUILayout.Height(20));

            buildTargetGroup = EditorGUILayout.BeginBuildTargetSelectionGrouping();
            EditorGUILayout.LabelField($"热更平台 {buildTargetGroup}");

            switch (buildTargetGroup)
            {
                case BuildTargetGroup.Standalone:
                    m_buildTarget = BuildTarget.StandaloneWindows;
                    break;
                case BuildTargetGroup.WebGL:
                    m_buildTarget = BuildTarget.WebGL;
                    break;
                case BuildTargetGroup.iOS:
                    m_buildTarget = BuildTarget.iOS;
                    break;
                case BuildTargetGroup.Android:
                    m_buildTarget = BuildTarget.Android;
                    break;
                default:
                    m_buildTarget = BuildTarget.NoTarget;
                    break;
            }
            EditorGUILayout.EndBuildTargetSelectionGrouping();

            EditorGUI.BeginChangeCheck();
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
            GUILayout.EndVertical();
            if (GUILayout.Button("更新热更包", GUILayout.Width(100), GUILayout.Height(50)))
            {
                if (!string.IsNullOrEmpty(md5Path) && md5Path.EndsWith(".bytes")&&m_buildTarget!= BuildTarget.NoTarget)
                {
                    // MyDebuger.Log("m_AssignAbs count "+ m_AssignAbs.Count);
                    HotUpdateConfig updateConfig = new HotUpdateConfig();
                    updateConfig.adm5path = md5Path;
                    updateConfig.hotcount = hotCunt;
                    updateConfig.HotDesc = hotDesc;
                    updateConfig.VersionName = curhotversion;

                    Debug.Log($"md5Path {md5Path}        updateConfig.VersionName {updateConfig.VersionName}"  );
                    //构建热更包
                    BundleEditor.BuildABHot(updateConfig, m_buildTarget);
                }
                else
                {
                    if (m_buildTarget== BuildTarget.NoTarget)
                        Debug.LogError("请检查热更平台:" + buildTargetGroup);
                    else 
                        Debug.LogError("请检查文件路径:" + md5Path);
                }
            }
        }

    }
}
