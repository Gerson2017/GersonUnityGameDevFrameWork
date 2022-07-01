using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class LinkXmlGenerator
{

    [MenuItem("MyTools/CreateLink.xml")]
    public static void Create()
    {
        var generator = new LinkXmlGenerator();

        generator.SetTypeConversion(typeof(UnityEditor.Animations.AnimatorController), typeof(RuntimeAnimatorController));

        int i = 0;
        string[] bundlenames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var bundleName in bundlenames)
        {
            EditorUtility.DisplayProgressBar("设置link.xml ", bundleName, i * 1.0f / bundlenames.Length);
            var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleName);
            generator.AddAssets(assetPaths);
            i++;
     
        }
        EditorUtility.ClearProgressBar();
        generator.Save("Assets/link.xml");
        Debug.Log("link 文件构建完毕");
        AssetDatabase.Refresh();
    }

    Dictionary<Type, Type> m_TypeConversion = new Dictionary<Type, Type>();
    HashSet<Type> m_Types = new HashSet<Type>();


    public void AddType(Type type)
    {
        if (type == null)
            return;
        AddTypeInternal(type);
    }


    public void AddTypes(params Type[] types)
    {
        if (types == null)
            return;
        foreach (var t in types)
            AddTypeInternal(t);
    }


    public void AddTypes(IEnumerable<Type> types)
    {
        if (types == null)
            return;
        foreach (var t in types)
            AddTypeInternal(t);
    }

    private void AddTypeInternal(Type t)
    {
        if (t == null)
            return;

        Type convertedType;
        if (m_TypeConversion.TryGetValue(t, out convertedType))
            m_Types.Add(convertedType);
        else
            m_Types.Add(t);
    }


    public void SetTypeConversion(Type a, Type b)
    {
        m_TypeConversion[a] = b;
    }



    public void AddAsset(string assetpath)
    {
        var assets = AssetDatabase.GetDependencies(assetpath);

        List<Type> types = new List<Type>();
        foreach (var asset in assets)
        {
            var type = AssetDatabase.GetMainAssetTypeAtPath(asset);
            if (type == typeof(GameObject))
            {
                var obj = (GameObject)AssetDatabase.LoadAssetAtPath(asset, typeof(GameObject));
                Component[] childrencompnets = obj.GetComponentsInChildren<Component>(true);
                if (childrencompnets != null && childrencompnets.Length > 0)
                {
                    try
                    {
                        types.AddRange(childrencompnets.Select(c => c.GetType()));
                    }
                    catch (Exception e)
                    {

                        Debug.LogError($"AddAsset {assetpath} has error {e.ToString()}");
                    }

                }


            }
            else
            {
                types.Add(type);
            }
        }
        AddTypes(types);
    }


    public void AddAssets(string[] assetPaths)
    {
        foreach (var assetPath in assetPaths)
            AddAsset(assetPath);
    }


    public void Save(string path)
    {
        var assemblyMap = new Dictionary<Assembly, List<Type>>();
        foreach (var t in m_Types)
        {
            var a = t.Assembly;
            List<Type> types;
            if (!assemblyMap.TryGetValue(a, out types))
                assemblyMap.Add(a, types = new List<Type>());
            types.Add(t);
        }
        XmlDocument doc = new XmlDocument();
        var linker = doc.AppendChild(doc.CreateElement("linker"));
        foreach (var k in assemblyMap)
        {
            if (k.Key.FullName.Contains("UnityEditor"))
                continue;

            var assembly = linker.AppendChild(doc.CreateElement("assembly"));
            var attr = doc.CreateAttribute("fullname");
            attr.Value = k.Key.FullName;
            if (assembly.Attributes != null)
            {
                assembly.Attributes.Append(attr);

                foreach (var t in k.Value)
                {
                    var typeEl = assembly.AppendChild(doc.CreateElement("type"));
                    var tattr = doc.CreateAttribute("fullname");
                    tattr.Value = t.FullName;
                    if (typeEl.Attributes != null)
                    {
                        typeEl.Attributes.Append(tattr);
                        var pattr = doc.CreateAttribute("preserve");
                        pattr.Value = "all";
                        typeEl.Attributes.Append(pattr);
                    }
                }
            }
        }
        doc.Save(path);

    }
}