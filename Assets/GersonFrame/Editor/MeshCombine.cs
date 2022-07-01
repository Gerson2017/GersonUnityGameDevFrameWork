using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GersonFrame.Tool;
using System.IO;

public class MeshCombineWindow : EditorWindow
{

    private  GameObject m_combineGoRoot;
    private GameObject gameObject;
    private byte[] m_textureByte;
    private bool m_click;
    public bool m_IsdestoryChildren=true;

    private SerializedObject m_obj;
    private SerializedProperty m_destoryChildren;


    [MenuItem("MyTools/Combine Mesh")]
        static void ShowEditor()
    {
        MeshCombineWindow combinewindow = GetWindow<MeshCombineWindow>();
        combinewindow.minSize = new Vector2(700,500);
     
    }

    void OnEnable()
    {
        m_obj = new SerializedObject(this);
        m_destoryChildren = m_obj.FindProperty("m_IsdestoryChildren");
    }
    


    private void OnGUI()
    {
        this.DrawWindow();
        if (m_click)
        {
            this.m_click = false;
            if (m_combineGoRoot==null)
            {
                MyDebuger.LogError("要合并的父物体 m_combineGoRoot 不能为空 ");
                return;
            }

            //要合并的网格
            MeshFilter[] filters = m_combineGoRoot.GetComponentsInChildren<MeshFilter>();
           //网格合并实例
            CombineInstance[] combines = new CombineInstance[filters.Length];
            //
            MeshRenderer[] renders = m_combineGoRoot.GetComponentsInChildren<MeshRenderer>();
            MyDebuger.Log(" MeshRenderer count " + filters.Length);
            //存储不同的材质 
            HashSet<Material> materialsHash = new HashSet<Material>();
            //存储所有要合并的贴图
            Texture2D[] textures = new Texture2D[filters.Length];
            //存储模型的uv
            List<Vector2[]> uvlist = new List<Vector2[]>();
            int uvcount = 0;
            for (int i = 0; i < filters.Length; i++)
            {
                combines[i].mesh = filters[i].sharedMesh;//使用编辑器执行只能使用sharemesh
                ///网格坐标转换
                combines[i].transform = m_combineGoRoot.transform.worldToLocalMatrix * filters[i].transform.localToWorldMatrix;
                if (!materialsHash.Contains(renders[i].sharedMaterial)) materialsHash.Add(renders[i].sharedMaterial);
                uvlist.Add(filters[i].sharedMesh.uv);
                uvcount += filters[i].sharedMesh.uv.Length;
                Debug.LogWarning("此处用的是mainTexturep 获取贴图属性 不同的shader可能使用对应的shader中的属性 注意辨别 ");
             //  textures[i]=renders[i].sharedMaterial.GetTexture("Base (RGB)") as Texture2D ;
             textures[i] = renders[i].sharedMaterial.mainTexture as Texture2D;
            }

            //存储材质
            Material[] materials = new Material[materialsHash.Count];
            int index = 0;
            foreach (var mat in materialsHash)
            {
                materials[index] = mat;
                index++;
            }
            ///合并贴图 
            Texture2D combinetext = new Texture2D(512,512);
            //存放每张贴图在新贴图中所占的比例 模型uv比例为 0~1
            Rect[] rects = combinetext.PackTextures(textures,0);
            Vector2[] uvs = new Vector2[uvcount];
            int j = 0;
            //遍历 rects rects 的数量就是filters 的数量
            for (int i = 0; i < filters.Length; i++)
            {
                //遍历物体uv 未合并之前的uv
                foreach (Vector2 uv in uvlist[i])
                {
                    //对新的uv进行插值计算 
                    uvs[j].x = Mathf.Lerp(rects[i].xMin,rects[i].xMax,uv.x);
                    uvs[j].y = Mathf.Lerp(rects[i].yMin, rects[i].yMax, uv.y);
                    j++;
                }
            }
            #region 
            Material newmat = new Material(materials[0]);
            newmat.CopyPropertiesFromMaterial(materials[0]);
            //设置材质贴图 注意材质贴图属性名称
            newmat.SetTexture("_MainTex", combinetext);
            newmat.name = "combineMat";
            #endregion
            m_textureByte= combinetext.EncodeToPNG();

            m_combineGoRoot.gameObject.SetActive(false);
            GameObject go = GameObject.Instantiate(m_combineGoRoot);
            gameObject = go;

            //给父物体添加网格组件
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            Mesh newmesh = new Mesh();
            filter.sharedMesh = newmesh;
            ///合并刚才的所有mesh
            filter.sharedMesh.CombineMeshes(combines);
            filter.sharedMesh.uv = uvs;
            renderer.material = newmat;
            gameObject.SetActive(true);

            Save();

        }
    }


    /// <summary>
    /// 绘制窗口
    /// </summary>
    private void DrawWindow()
    {
        BeginWindows();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("合并网格根节点:");
        m_combineGoRoot = (GameObject)EditorGUILayout.ObjectField(m_combineGoRoot,typeof(GameObject),true);
        EditorGUILayout.LabelField("合并完成后是否销毁子节点:");
       EditorGUILayout.PropertyField(m_destoryChildren, true);
        this.m_click=  GUILayout.Button("Combine");
        if (EditorGUI.EndChangeCheck())
            m_obj.ApplyModifiedProperties();
        EndWindows();
    }

    private string CreateFolder()
    {
        string folderPath = Path.Combine("Assets/CombineMesh/" , m_combineGoRoot.name);

        if (!AssetDatabase.IsValidFolder(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }


    private void Save()
    {
        string path = CreateFolder();
        MeshRenderer mesh = gameObject.GetComponent<MeshRenderer>();
        MeshFilter filiter = gameObject.GetComponent<MeshFilter>();
        string na = gameObject.name;
        if (m_IsdestoryChildren)
        {
            for (int i = gameObject.transform.childCount-1; i >=0 ; i--)
                DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
        }

        AssetDatabase.CreateAsset(filiter.sharedMesh, string.Format("{0}/{1}mesh.mesh", path, na));
        if (mesh.sharedMaterial != null)
        {
             AssetDatabase.CreateAsset(mesh.sharedMaterial, string.Format("{0}/{1}mat.mat", path, na));
        }

        if (m_textureByte!=null)
        {
            FileStream file = File.Create(string.Format("{0}/{1}png.png", path, na));
            file.Write(m_textureByte, 0, m_textureByte.Length);
            file.Close();
        }
        
           
        PrefabUtility.SaveAsPrefabAsset(gameObject, string.Format("{0}/{1}.prefab", path, na));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
