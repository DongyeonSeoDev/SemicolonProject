using UnityEngine;

namespace Enemy
{
    public class Particle
    {
        private MeshParticleSystem mesh;
        private Vector3 position;
        private Vector3 direction;
        private Vector3 size;
        private float rotation;
        private float speed;
        private int uvIndex;
        private int quadIndex;

        public Particle(MeshParticleSystem m)
        {
            mesh = m;
        }

        public Particle Init(Vector3 pos, Vector3 dir, Vector3 s, float rot, float sp, int uv)
        {
            position = pos;
            direction = dir;
            size = s;
            rotation = rot;
            speed = sp;
            uvIndex = uv;

            quadIndex = mesh.AddQuad(position, rotation, size, uvIndex);

            return this;
        }

        public void Update()
        {
            position += direction * speed * Time.deltaTime;
            rotation += 360f * (speed * 0.1f) * Time.deltaTime;

            mesh.UpdateQuad(position, rotation, size, uvIndex, quadIndex);

            speed -= speed * 2f * Time.deltaTime;
        }

        public bool IsComplete() => speed < 0.05f;

        public Particle Clone()
        {
            return MemberwiseClone() as Particle;
        }
    }
}