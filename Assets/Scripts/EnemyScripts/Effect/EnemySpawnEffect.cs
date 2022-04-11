using UnityEngine;

namespace Enemy
{
    public class EnemySpawnEffect : EnemyPoolData
    {
        private ParticleSystem particle;

        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            particle.Play();

            Util.DelayFunc(() =>
            {
                gameObject.SetActive(false);
            }, 4f, this);
        }
    }
}