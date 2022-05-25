using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class MeshParticleManager : MonoSingleton<MeshParticleManager>
    {
        private MeshParticleSystem mesh = null;

        private List<Particle> particleList = new List<Particle>();

        public float minSize;
        public float maxSize;
        public float minSpeed;
        public float maxSpeed;
        public int minCount;
        public int maxCount;

        private void Awake()
        {
            mesh = GetComponent<MeshParticleSystem>();

            EventManager.StartListening("ExitCurrentMap", ClearAllParticle);
        }

        private void Update()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                particleList[i].Update();

                if (particleList[i].IsComplete())
                {
                    particleList.RemoveAt(i);
                    i--;
                }
            }
        }

        public void SpawnBloodEffect(Vector3 position)
        {
            int count = Random.Range(minCount, maxCount);

            for (int i = 0; i < count; i++)
            {
                var randomAngle = Random.Range(0f, 360f);
                particleList.Add(new Particle(mesh, position, new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad)), Vector3.one * Random.Range(minSize, maxSize), Random.Range(0f, 360f), Random.Range(minSpeed, maxSpeed), mesh.GetRandomBloodUVIndex()));
            }
        }

        public void ClearAllParticle() => mesh.ClearAllQuad();
    }
}