using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // 두번째 적
    {
        private EnemyCommand enemyAttackCommand;

        protected override void Awake()
        {
            enemyData = new EnemyData()
            {
                eEnemyController = EnemyController.AI,
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyLootList = enemyLootListSO,
                enemyAnimator = GetComponent<Animator>(),
                enemySpriteRenderer = GetComponent<SpriteRenderer>(),
                hpBarFillImage = hpBarFillImage,
                enemyId = "Rat_02",
                isHitAnimation = true,
                isAttackCommand = true,
                isLongDistanceAttack = true,
                damageDelay = 0.4f,
                chaseSpeed = 3f,
                isSeePlayerDistance = 10f
            };

            enemyAttackCommand = new EnemyAttackCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.eEnemyController, enemyData.attackDamage);

            base.Awake();
        }

        public void EnemyAttack()
        {
            enemyAttackCommand.Execute();
        }
    }
}