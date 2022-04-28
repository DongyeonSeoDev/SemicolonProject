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

        private void Start()
        {
            EventManager.StartListening("PlayerDead", Remove);
            EventManager.StartListening("ExitCurrentMap", Remove);
        }

        public void Play()
        {
            particle.Play();

            Util.DelayFunc(() =>
            {
                Remove();
            }, 4f, this);
        }

        private void Remove()
        {
            gameObject.SetActive(false);
        }
    }
}