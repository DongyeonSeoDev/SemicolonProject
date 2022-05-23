using UnityEngine;

namespace Enemy
{
    public class MeshParticleSystem : MonoBehaviour
    {
        private const int LIMIT_MAX_PARTICLE_COUNT = 1000;

        private Mesh mesh = null;

        private Vector3[] vertices;
        private Vector2[] uv;
        private int[] triangles;

        private void Awake()
        {
            mesh = GetComponent<Mesh>();

            vertices = new Vector3[LIMIT_MAX_PARTICLE_COUNT * 4];
            uv = new Vector2[LIMIT_MAX_PARTICLE_COUNT * 4];
            triangles = new int[LIMIT_MAX_PARTICLE_COUNT * 6];

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
        }
    }
}