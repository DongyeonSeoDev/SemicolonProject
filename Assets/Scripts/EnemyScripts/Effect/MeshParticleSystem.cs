using UnityEngine;

namespace Enemy
{
    public class MeshParticleSystem : MonoBehaviour
    {
        private const int MAX_QUAD_COUNT = 1000;

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

        private int quadIndex = 0;
        private bool updateVertices = false;
        private bool updateUV = false;
        private bool updateTriangles = false;

        private void Awake()
        {
            mesh = new Mesh();
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            vertices = new Vector3[MAX_QUAD_COUNT * 4];
            uv = new Vector2[MAX_QUAD_COUNT * 4];
            triangles = new int[MAX_QUAD_COUNT * 6];

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100f);

            meshFilter.mesh = mesh;

            meshRenderer.sortingLayerName = "BackgroundEffect";
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

        public int GetRandomBloodUVIndex() => Random.Range(0, 4);

        public int AddQuad(Vector3 position, float rotation, Vector3 size, int uvIndex)
        {
            UpdateQuad(position, rotation, size, uvIndex, quadIndex);

            var index = quadIndex;

            quadIndex = (quadIndex + 1) % MAX_QUAD_COUNT;

            return index;
        }

        public void UpdateQuad(Vector3 position, float rotation, Vector3 size, int uvIndex, int quadIndex)
        {
            int index0 = quadIndex * 4;
            int index1 = index0 + 1;
            int index2 = index0 + 2;
            int index3 = index0 + 3;

            vertices[index0] = position + Quaternion.Euler(0f, 0f, rotation - 180) * size;
            vertices[index1] = position + Quaternion.Euler(0f, 0f, rotation - 270) * size;
            vertices[index2] = position + Quaternion.Euler(0f, 0f, rotation - 90) * size;
            vertices[index3] = position + Quaternion.Euler(0f, 0f, rotation - 0) * size;

            UVCoords uvCoord = uvArray[uvIndex];

            uv[index0] = uvCoord.uv00;
            uv[index1] = new Vector2(uvCoord.uv00.x, uvCoord.uv11.y);
            uv[index2] = new Vector2(uvCoord.uv11.x, uvCoord.uv00.y);
            uv[index3] = uvCoord.uv11;

            int trianglesIndex = quadIndex * 6;

            triangles[trianglesIndex] = index0;
            triangles[trianglesIndex + 1] = index1;
            triangles[trianglesIndex + 2] = index2;
            triangles[trianglesIndex + 3] = index2;
            triangles[trianglesIndex + 4] = index1;
            triangles[trianglesIndex + 5] = index3;

            updateVertices = true;
            updateUV = true;
            updateTriangles = true;
        }

        public void ClearAllQuad()
        {
            System.Array.Clear(vertices, 0, vertices.Length);
            quadIndex = 0;
            updateVertices = true;
        }
    }
}