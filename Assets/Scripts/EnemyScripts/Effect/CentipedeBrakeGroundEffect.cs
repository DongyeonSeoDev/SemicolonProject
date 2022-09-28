using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class CentipedeBrakeGroundEffect : EnemyPoolData
    {
        private ParticleSystem particle;

        [SerializeField]
        private float duration = 1f;

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
            }, duration, this);
        }

        private void Remove()
        {
            gameObject.SetActive(false);
        }
    }
}
