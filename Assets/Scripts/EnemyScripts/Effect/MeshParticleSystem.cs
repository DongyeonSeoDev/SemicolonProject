using UnityEngine;

namespace Enemy
{
    public class MeshParticleSystem : MonoBehaviour
    {
        private const int LIMIT_MAX_PARTICLE_COUNT = 1000;

        [System.Serializable]
        public struct UVPixel
        {
            public Vector2 uv00Pixel;
            public Vector2 uv11Pixel;
        }

        public struct UVCoords
        {
            public Vector2 uv00;
            public Vector2 uv11;
        }

        [SerializeField]
        private UVPixel[] uvPixelArray;
        private UVCoords[] uvArray;

        private Mesh mesh = null;
        private MeshFilter meshFilter = null;
        private MeshRenderer meshRenderer = null;

        private Vector3[] vertices;
        private Vector2[] uv;
        private int[] triangles;

        private bool updateVertices = false;
        private bool updateUV = false;
        private bool updateTriangles = false;

        private void Awake()
        {
            mesh = new Mesh();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            vertices = new Vector3[LIMIT_MAX_PARTICLE_COUNT * 4];
            uv = new Vector2[LIMIT_MAX_PARTICLE_COUNT * 4];
            triangles = new int[LIMIT_MAX_PARTICLE_COUNT * 6];

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f);

            meshFilter.mesh = mesh;

            meshRenderer.sortingLayerName = "Object";
            meshRenderer.sortingOrder = 0;

            Texture texture = meshRenderer.material.mainTexture;
            int textureWidth = texture.width;
            int textureHeight = texture.height;

            uvArray = new UVCoords[uvPixelArray.Length];

            for (int i = 0; i < uvPixelArray.Length; i++)
            {
                uvArray[i] = new UVCoords 
                { 
                    uv00 = new Vector2(uvPixelArray[i].uv00Pixel.x / textureWidth, uvPixelArray[i].uv00Pixel.y / textureHeight),
                    uv11 = new Vector2(uvPixelArray[i].uv11Pixel.x / textureWidth, uvPixelArray[i].uv11Pixel.y / textureHeight)
                };
            }

            //TestMesh();
        }

        private void LateUpdate()
        {
            if (updateVertices)
            {
                mesh.vertices = vertices;
                updateVertices = false;
            }

            if (updateUV)
            {
                mesh.uv = uv;
                updateUV = false;
            }

            if (updateTriangles)
            {
                mesh.triangles = triangles;
                updateTriangles = false;
            }
        }

        private void TestMesh()
        {
            vertices[0] = new Vector3(0f, 0f, 0f);
            vertices[1] = new Vector3(0f, 1f, 0f);
            vertices[2] = new Vector3(1f, 0f, 0f);
            vertices[3] = new Vector3(1f, 1f, 0f);

            uv[0] = uvArray[0].uv00;
            uv[1] = new Vector2(uvArray[0].uv00.x, uvArray[0].uv11.y);
            uv[2] = new Vector2(uvArray[0].uv11.x, uvArray[0].uv00.y);
            uv[3] = uvArray[0].uv11;

            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 2;
            triangles[4] = 1;
            triangles[5] = 3;

            updateVertices = true;
            updateUV = true;
            updateTriangles = true;
        }
    }
}