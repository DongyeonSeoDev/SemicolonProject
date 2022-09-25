using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Enemy6Mage : Enemy
    {
        [SerializeField] private EnemyBullet2 bullet;

        [SerializeField] private float[] startAngles;
        [SerializeField] private bool[] isUseAngles;

        private List<EnemyBullet2> bullets = new List<EnemyBullet2>();

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed);

            StartCoroutine(SpawnBullet());
        }

        private IEnumerator SpawnBullet()
        {
            while (true)
            {
                if (bullets.Count >= 6)
                {
                    break;
                }

                yield return new WaitForSeconds(1f);

                EnemyBullet2 b = Instantiate(bullet, transform);
                int i = 0;

                for (; i < isUseAngles.Length; i++)
                {
                    if (!isUseAngles[i])
                    {
                        isUseAngles[i] = true;

                        break;
                    }
                }

                b.Init(enemyData.eEnemyController, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.criticalDamagePercent, UnityEngine.Color.white, transform, bullets.Count == 0 ? startAngles[i] : bullets[0].currentAngle + startAngles[i], this);

                bullets.Add(b);
            }
        }
    }
}