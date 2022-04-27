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

        public int maxHP = 10;
        public int hp = 10;

        protected bool destroyTimerStarted = false;
        protected readonly float destroyTime = 1f;
        protected float destroyTimer = 0f;

        public virtual void Start()
        {
            anim = GetComponent<Animator>();
            anim.Play("Move");

            hp = maxHP;
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

        public virtual void GetDamage(int damage, bool critical = false, bool isKnockBack = false, float knockBackPower = 20, float stunTime = 1, Vector2? direction = null)
        {
        }
        public void CheckDead()
        {
            if(!destroyTimerStarted && hp <= 0)
            {
                destroyTimerStarted = true;

                anim.Play("Dead");

                destroyTimer = destroyTime;
            }
        }
        public Transform GetTransform() => transform;
        public GameObject GetGameObject() => gameObject;
        public float EnemyHpPercent() => ((float)hp / maxHP) * 100f;
    }
}
