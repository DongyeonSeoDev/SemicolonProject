using UnityEngine;

namespace Enemy
{
    public class EnemyEffect : PoolManager
    {
        private ParticleSystem particle = null;

        private void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        public void Play(Color color)
        {
            ParticleSystem.MainModule main = particle.main;
            main.startColor = color;

            particle.Play();

            Util.DelayFunc(() =>
            {
                gameObject.SetActive(false);
            }, 1f, this);
        }
    }
}