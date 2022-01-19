using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // �ι�° ��
    {
        protected override void Awake()
        {
            enemyData = new EnemyData()
            {
                eEnemyController = EnemyController.AI,
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyAnimator = GetComponent<Animator>(),
                enemySpriteRenderer = GetComponent<SpriteRenderer>(),
                isHitAnimation = true,
                isAttackCommand = true,
                damageDelay = 0.4f
            };

            base.Awake();
        }
    }
}