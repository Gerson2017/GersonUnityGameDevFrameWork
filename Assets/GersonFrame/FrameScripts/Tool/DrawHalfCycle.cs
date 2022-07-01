using UnityEngine;


namespace GersonFrame.Tool
{


    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class DrawHalfCycle : MonoBehaviour
    {
        public float Radius = 6;          //外半径  
        public float InnerRadius = 1;          //外半径  
        public float angleDegree = 360;   //扇形或扇面的角度
        public int Segments = 10;         //分割数  

        private MeshFilter meshFilter;

        private Mesh mesh;



        void Start()
        {
            meshFilter = GetComponent<MeshFilter>();
            mesh = new Mesh();
            meshFilter.mesh = mesh;
        }

        private void Update()
        {
            CreateMesh();
        }

        Mesh CreateMesh()
        {
            //vertices(顶点):
            int vertices_count = Segments * 2 + 2;              //因为vertices(顶点)的个数与triangles（索引三角形顶点数）必须匹配
            Vector3[] vertices = new Vector3[vertices_count];
            float angleRad = Mathf.Deg2Rad * angleDegree;
            float angleCur = angleRad;
            float angledelta = angleRad / Segments;
            for (int i = 0; i < vertices_count; i += 2)
            {
                float cosA = Mathf.Cos(angleCur);
                float sinA = Mathf.Sin(angleCur);

                vertices[i] = new Vector3(Radius * cosA, InnerRadius, Radius * sinA);
                angleCur -= angledelta;
            }

            //triangles:
            int triangle_count = Segments * 6;
            int[] triangles = new int[triangle_count];
            for (int i = 0, vi = 0; i < triangle_count; i += 6, vi += 2)
            {
                triangles[i] = vi;
                triangles[i + 1] = vi + 3;
                triangles[i + 2] = vi + 1;
                triangles[i + 3] = vi + 2;
                triangles[i + 4] = vi + 3;
                triangles[i + 5] = vi;
            }

            //uv:
            Vector2[] uvs = new Vector2[vertices_count];
            for (int i = 0; i < vertices_count; i++)
            {
                uvs[i] = new Vector2(vertices[i].x / Radius / 2 + 0.5f, vertices[i].z / Radius / 2 + 0.5f);
            }

            //负载属性与mesh
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x, angleDegree / 2 - 90, this.transform.eulerAngles.z);
            return mesh;
        }
    }
}