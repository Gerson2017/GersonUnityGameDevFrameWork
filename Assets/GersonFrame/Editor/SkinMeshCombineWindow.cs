using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class SkinnedMesCombineWindow : EditorWindow
{
    private enum SaveStrategy
    {
        AnimMap, //only anim map
        Mat, //with shader
        Prefab //prefab with mat
    }

    #region FIELDS

    private static GameObject _targetGo;
    private Transform transform;
    private GameObject gameObject;
    private byte[] m_textureByte;
    private static string _path = "SkinMeshCombine";
    private static string _subPath = "SubPath";


    public bool m_IsdestoryChildren = true;

    private SerializedObject m_obj;
    private SerializedProperty m_destoryChildren;

    public SkinnedMeshRenderer[] renders;
    Vector2[] uvs;

    // 合并后使用的材质
    public Material material;

    #endregion


    #region  METHODS

    [MenuItem("MyTools/Combine SkinMeshRender")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SkinnedMesCombineWindow));
    }

    private void OnGUI()
    {
        _targetGo = (GameObject)EditorGUILayout.ObjectField(_targetGo, typeof(GameObject), true);
        _subPath = _targetGo == null ? _subPath : _targetGo.name;
        EditorGUILayout.LabelField(string.Format($"output path:{Path.Combine(_path, _subPath)}"));
        _path = EditorGUILayout.TextField(_path);
        _subPath = EditorGUILayout.TextField(_subPath);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.LabelField("合并完成后是否销毁子节点:");
        EditorGUILayout.PropertyField(m_destoryChildren, true);
        if (EditorGUI.EndChangeCheck())
            m_obj.ApplyModifiedProperties();


        if (!GUILayout.Button("Bake")) return;
        if (_targetGo == null)
        {
            EditorUtility.DisplayDialog("err", "targetGo is null！", "OK");
            return;
        }

        GameObject go = GameObject.Instantiate(_targetGo);
        transform = go.transform;
        gameObject = go;
        Start();
        Save();
    }

    void OnEnable()
    {
        m_obj = new SerializedObject(this);
        m_destoryChildren = m_obj.FindProperty("m_IsdestoryChildren");
    }

    private void Save()
    {
        string path = CreateFolder();
        SkinnedMeshRenderer smg = gameObject.GetComponent<SkinnedMeshRenderer>();
        string na = _targetGo.name;
        AssetDatabase.CreateAsset(smg.sharedMesh, string.Format("{0}/{1}mesh.mesh", path, na));
        if (smg.sharedMaterial != null)
            AssetDatabase.CreateAsset(smg.sharedMaterial, string.Format("{0}/{1}mat.mat", path, na));
        if (m_textureByte != null)
        {
            FileStream file = File.Create(string.Format("{0}/{1}png.png", path, na));
            file.Write(m_textureByte, 0, m_textureByte.Length);
            file.Close();
        }
        PrefabUtility.SaveAsPrefabAsset(gameObject, string.Format("{0}/{1}.prefab", path, na));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    private string CreateFolder()
    {
        string folderPath = Path.Combine("Assets/" + _path, _subPath);

        if (!AssetDatabase.IsValidFolder(folderPath))
            Directory.CreateDirectory(folderPath);

        return folderPath;
    }

    #endregion

    void Start()
    {
        renders = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        // 先记录当前预制件的变换矩阵，合并之后再赋值回来
        Vector3 l_position = transform.position;
        Quaternion l_rotation = transform.rotation;
        Vector3 l_scale = transform.localScale;

        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        // 待合并的skinnedrender需要记录的信息
        List<Transform> bones = new List<Transform>();
        List<BoneWeight> boneWeights = new List<BoneWeight>();
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        CombineTexture(renders);

        int length = renders.Length;
        int boneOffset = 0;
        for (int i = 0; i < length; i++)
        {
            SkinnedMeshRenderer oneRender = renders[i];
            oneRender.transform.localScale = Vector3.one;
            // 记录骨骼
            bones.AddRange(oneRender.bones);
            // 记录权重
            BoneWeight[] meshBoneweight = oneRender.sharedMesh.boneWeights;
            for (int j = 0; j < meshBoneweight.Length; ++j)
            {
                BoneWeight bw = meshBoneweight[j];
                BoneWeight bWeight = bw;
                bWeight.boneIndex0 += boneOffset;
                bWeight.boneIndex1 += boneOffset;
                bWeight.boneIndex2 += boneOffset;
                bWeight.boneIndex3 += boneOffset;
                boneWeights.Add(bWeight);
            }

            // offset是为了合并之后BoneWeight.boneIndex还能正确定向骨骼
            boneOffset += oneRender.bones.Length;
            // 记录网格相关信息
            CombineInstance combineInstance = new CombineInstance();
            Mesh mesh = new Mesh();
            oneRender.BakeMesh(mesh);
            mesh.uv = oneRender.sharedMesh.uv;
            combineInstance.mesh = mesh;
            combineInstance.transform = oneRender.localToWorldMatrix;
            combineInstances.Add(combineInstance);
            if (m_IsdestoryChildren)
                DestroyImmediate(oneRender.gameObject);
            else
                oneRender.gameObject.SetActive(false);
        }

        // 将所有的骨骼变换矩阵从自身转换到当前预制件下
        List<Matrix4x4> bindposes = new List<Matrix4x4>();
        int boneLength = bones.Count;
        for (int i = 0; i < boneLength; i++)
        {
            bindposes.Add(bones[i].worldToLocalMatrix * transform.worldToLocalMatrix);
        }

        // 在当前预制下创建新的蒙皮渲染器,设置属性
        SkinnedMeshRenderer combinedSkinnedRenderer = gameObject.AddComponent<SkinnedMeshRenderer>();
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combineInstances.ToArray(), true, true);
        combinedSkinnedRenderer.sharedMesh = combinedMesh;
        combinedSkinnedRenderer.bones = bones.ToArray();
        combinedSkinnedRenderer.sharedMesh.boneWeights = boneWeights.ToArray();
        combinedSkinnedRenderer.sharedMesh.bindposes = bindposes.ToArray();
        combinedSkinnedRenderer.sharedMesh.RecalculateBounds();
        combinedSkinnedRenderer.material = material;
        combinedSkinnedRenderer.sharedMesh.uv = uvs;

        // 还原自身的变换矩阵
        transform.position = l_position;
        transform.rotation = l_rotation;
        transform.localScale = l_scale;
    }


    void CombineTexture(SkinnedMeshRenderer[] skinmeshrenders)
    {
        m_textureByte = null;
        if (skinmeshrenders == null|| skinmeshrenders.Length<1)
            return;
        
        Material[] skinmeraials = new Material[skinmeshrenders.Length];
        Texture2D[] textures = new Texture2D[skinmeshrenders.Length];
        int uvcount = 0;
        //存储模型的uv
        List<Vector2[]> uvlist = new List<Vector2[]>();
        for (int i = 0; i < skinmeshrenders.Length; i++)
        {
            skinmeraials[i] = skinmeshrenders[i].sharedMaterial;

            if (skinmeshrenders[i]==null)
            {
                Debug.LogWarning("未找到材质球");
                continue;
            }
            Texture tex = skinmeraials[i].GetTexture("_MainTex");
            if (tex == null)
            {
                Debug.LogError("此处可以根据具体的shader使用的贴图属性来改写");
                continue;
            }
            Texture2D tx =tex as Texture2D;

            //Texture2D tx = skinmeraials[i].mainTexture as Texture2D;
            Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);
            uvlist.Add(skinmeshrenders[i].sharedMesh.uv);
            uvcount += skinmeshrenders[i].sharedMesh.uv.Length;
            tx2D.SetPixels(tx.GetPixels(0, 0, tx.width, tx.height));
            tx2D.Apply();
            textures[i] = tx2D;
        }

        material = new Material(skinmeraials[0].shader);
        material.CopyPropertiesFromMaterial(skinmeraials[0]);

        Texture2D texture = new Texture2D(1024,1024);
        Rect[] rects = texture.PackTextures(textures,10,1024);
        uvs = new Vector2[uvcount];
        int j = 0;
        //遍历 rects rects 的数量就是filters 的数量
        for (int i = 0; i < skinmeshrenders.Length; i++)
        {
            //遍历物体uv 未合并之前的uv
            foreach (Vector2 uv in uvlist[i])
            {
                //对新的uv进行插值计算 
                uvs[j].x = Mathf.Lerp(rects[i].xMin, rects[i].xMax, uv.x);
                uvs[j].y = Mathf.Lerp(rects[i].yMin, rects[i].yMax, uv.y);
                j++;
            }
        }
        material.SetTexture("_MainTex",texture);
        m_textureByte = texture.EncodeToPNG();
    }
}