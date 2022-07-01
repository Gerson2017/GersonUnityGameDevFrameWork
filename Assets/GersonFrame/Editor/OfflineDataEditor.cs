using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GersonFrame.Tool;

namespace GersonFrame.ABFrame
{
#if UNITY_EDITOR


    public class OfflineDataEditor
    {

        [MenuItem("Assets/生成离线数据")]
        public static void AssetCreateOffLineData()
        {
            GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加/刷新离线数据 正在修改:", objects[i].name, 1.0f / objects.Length * i);
                CreateOffLineData(objects[i]);
            }
            EditorUtility.ClearProgressBar();
        }

        [MenuItem("Assets/生成UI离线数据")]
        public static void AssetCreateUIOffLineData()
        {
            GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加/刷新UI离线数据 正在修改:", objects[i].name, 1.0f / objects.Length * i);
                CreateUIOfflineData(objects[i]);
            }
            EditorUtility.ClearProgressBar();
        }


        [MenuItem("Assets/生成特效离线数据")]
        public static void AssetCreateEffectOfflineData()
        {
            GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                EditorUtility.DisplayProgressBar("添加特效离线数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
                CreateEffectOfflineData(objects[i]);
            }
            EditorUtility.ClearProgressBar();
        }

        //===========================================================
        public static void CreateOffLineData(GameObject obj)
        {
            OffLineData offLineData = obj.GetComponent<OffLineData>();
            if (offLineData == null)
            {
                offLineData = obj.AddComponent<OffLineData>();
            }
            offLineData.BindData();
            //保存了预制体信息
            EditorUtility.SetDirty(obj);
            MyDebuger.Log("修改了 " + obj.name + " prefab");
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }


        public static void CreateUIOfflineData(GameObject obj)
        {
            obj.layer = LayerMask.NameToLayer("UI");

            UIOffLineData uidata = obj.GetComponent<UIOffLineData>();
            if (uidata == null)
                uidata = obj.AddComponent<UIOffLineData>();
            uidata.BindData();
            EditorUtility.SetDirty(obj);
            MyDebuger.Log("修改了 " + obj.name + "UI prefab");
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }



        public static void CreateEffectOfflineData(GameObject obj)
        {
            EffectOffLineData effectData = obj.GetComponent<EffectOffLineData>();
            if (effectData == null)
                effectData = obj.AddComponent<EffectOffLineData>();

            effectData.BindData();
            EditorUtility.SetDirty(obj);
            MyDebuger.Log("修改了" + obj.name + " 特效 prefab!");
            Resources.UnloadUnusedAssets();
            AssetDatabase.Refresh();
        }

        //=================================================

        [MenuItem("MyTools/UI/生成所有UI离线数据")]
        public static void AllCreateUIData()
        {
            string path = "Assets/Prefabs/UGUI";
            string[] allstr = AssetDatabase.FindAssets("t:Prefab", new string[] { path });
            if (allstr.Length <= 0)
            {
                MyDebuger.LogWarning("not found any ui prefab!");
                return;
            }
            for (int i = 0; i < allstr.Length; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(allstr[i]);
                EditorUtility.DisplayProgressBar("添加UI 离线数据", "正在扫描路径:" + prefabPath + "....", 1.0f / allstr.Length * i);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (obj == null) continue;
                CreateUIOfflineData(obj);
            }
            EditorUtility.ClearProgressBar();
            MyDebuger.Log("所有UI离线数据生成完毕");
        }

        [MenuItem("MyTools/UI/生成所有特效 prefab离线数据")]
        public static void AllCreateEffectOfflineData()
        {
            string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/Prefabs/Effect" });
            if (allStr.Length <= 0)
            {
                MyDebuger.LogWarning(" not found any effect prefab ");
                return;
            }
            for (int i = 0; i < allStr.Length; i++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
                EditorUtility.DisplayProgressBar("添加特效离线数据", "正在扫描路径：" + prefabPath + "......", 1.0f / allStr.Length * i);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (obj == null)
                    continue;
                CreateEffectOfflineData(obj);
            }
            MyDebuger.Log("特效离线数据全部生成完毕！");
            EditorUtility.ClearProgressBar();
        }
    }

#endif
}
