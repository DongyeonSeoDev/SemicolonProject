using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Enemy6Mage : Enemy
    {
        [SerializeField] private EnemyBullet2 bullet;

        [SerializeField] private float[] startAngles;
        
        public List<EnemyBullet2> bullets = new List<EnemyBullet2>();
        public bool[] isUseAngles;

        private Coroutine coroutine;
        private float currentTime = 0f;
        private float defaultAngle = 0f;
        private bool isAttack;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 0.4f;
            enemyData.playerAnimationSpeed = 1f;
            enemyData.playerAnimationTime = 0.4f;

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, 3f);
            coroutine = StartCoroutine(SpawnBullet());

            enemyData.enemyChaseStateChangeCondition = ChaseStateChangeCondition;

            defaultAngle = 0f;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            RemoveBullet();
        }

        protected override void Update()
        {
            base.Update();

            if (isStop)
            {
                return;
            }

            currentTime += Time.deltaTime;

            if (!isAttack && bullets.Count == 6)
            {
                isAttack = true;
            }

            if (isAttack && bullets.Count == 0)
            {
                isAttack = false;
            }

            defaultAngle += Time.deltaTime * 100f;
        }

        private EnemyState ChaseStateChangeCondition()
        {
            if (!isAttack)
            {
                return null;
            }
            else
            {
                return new EnemyAIAttackState(enemyData);
            }
        }

        private IEnumerator SpawnBullet()
        {
            while (true)
            {
                yield return new WaitUntil(() => currentTime >= 2f);

                if (!isAttack && bullets.Count < 6)
                {
                    currentTime = 0f;

                    EnemyBullet2 b = Instantiate(bullet, transform.position, Quaternion.identity);
                    int i = 0;

                    for (; i < isUseAngles.Length; i++)
                    {
                        if (!isUseAngles[i])
                        {
                            isUseAngles[i] = true;

                            break;
                        }
                    }

                    b.Init(enemyData.eEnemyController, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.criticalDamagePercent, UnityEngine.Color.white, transform, defaultAngle + startAngles[i], this, i, this);

                    bullets.Add(b);
                }
            }
        }

        private void RemoveBullet() // 애니메이션에서 실행
        {
            foreach (EnemyBullet2 g in bullets)
            {
                g.isConnectMage = false;
                g.gameObject.SetActive(false);
            }

            bullets.Clear();

            for (int i = 0; i < isUseAngles.Length; i++)
            {
                isUseAngles[i] = false;
            }

            StopCoroutine(coroutine);
        }

        private void FireBullet() // 애니메이션에서 실행
        {
            if (bullets.Count > 0)
            {
                if (GetEnemyController() == EnemyController.AI)
                {
                    bullets[0].Fire((EnemyManager.Player.transform.position - transform.position).normalized);
                }
                else if (GetEnemyController() == EnemyController.PLAYER)
                {
                    bullets[0].Fire((playerInput.AttackMousePosition - (Vector2)transform.position).normalized);
                }
            }
        }
    }
}