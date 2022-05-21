using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Enemy
{
    public abstract class TutorialEnemy : MonoBehaviour, ICanGetDamagableEnemy
    {
        private Animator anim = null;

        public float maxHP = 10;
        public float hp = 10;

        protected bool destroyTimerStarted = false;
        protected readonly float destroyTime = 1f;
        protected float destroyTimer = 0f;

        public virtual void Start()
        {
            anim = GetComponent<Animator>();
            anim.Play("Move");

            if(hp > maxHP)
            {
                hp = maxHP;
            }
        }
        public virtual void Update()
        {
            CheckDead();

            if (destroyTimerStarted && destroyTimer > 0f)
            {
                destroyTimer -= Time.deltaTime;

                if(destroyTimer < 0f)
                {
                    Destroy(gameObject);
                }
            }
        }

        public virtual void GetDamage(float damage, bool critical, bool isKnockBack, bool isStun, Vector2 effectPosition, Vector2 direction, float knockBackPower = 20f, float stunTime = 1f, Vector3? effectSize = null)
        {
        }

        public void CheckDead()
        {
            if(!destroyTimerStarted && hp <= 0)
            {
                destroyTimerStarted = true;

                anim.Play("Dead");

                EventManager.TriggerEvent("Tuto_EnemyDeathCheck");

                destroyTimer = destroyTime;
            }
        }
        public Transform GetTransform() => transform;
        public GameObject GetGameObject() => gameObject;
        public float EnemyHpPercent() => ((float)hp / maxHP) * 100f;
        public string GetEnemyName()
        {
            return "TutorialEnemy";
        }
    }
}
