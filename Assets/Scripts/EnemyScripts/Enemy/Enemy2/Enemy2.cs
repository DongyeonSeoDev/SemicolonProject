using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // 두번째 적
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
                isLongDistanceAttack = true,
                damageDelay = 0.4f,
                chaseSpeed = 3f,
                isSeePlayerDistance = 10f
            };

            base.Awake();
        }
    }
}