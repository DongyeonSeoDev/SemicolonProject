using UnityEngine;

namespace Enemy
{
    public class EnemyHitEffect : EnemyPoolData
    {
        public ParticleSystem particle = null;

        public void Play()
        {
            particle.Play();

            Util.DelayFunc(() =>
            {
                gameObject.SetActive(false);
            }, 0.4f, this);
        }
    }
}