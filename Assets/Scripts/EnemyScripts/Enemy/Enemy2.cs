// #define ENEMYDEBUG

using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // 두번째 적
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        private EnemyPositionCheckData positionCheckData = new EnemyPositionCheckData();

        protected override void OnEnable()
        {
            enemyData = new EnemyData(enemyDataSO)
            {
                enemyObject = gameObject,
                enemyLootList = enemyLootListSO,
                enemyAnimator = anim,
                enemySpriteRenderer = sr,
                enemyRigidbody2D = rb,
                hpBarFillImage = hpBarFillImage,
                isHitAnimation = true,
                isAttackCommand = true,
                isLongDistanceAttack = true,
                isRotate = true,
                isUseAttacking = true,
                damageDelay = 0.4f,
                chaseSpeed = 10f,
                attackDelay = 1.5f,
                minRunAwayTime = 2f,
                maxRunAwayTime = 4f,
                playerAnimationTime = 1.05f
            };

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData);
            base.OnEnable();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                positionCheckData.isWall = true;
                positionCheckData.oppositeDirectionWall = collision.contacts[0].normal;
            }
        }

        public void EnemyAttack()
        {
            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, this, enemyData.eEnemyController, enemyData.attackDamage);

                enemyAttackPlayerCommand.Execute();
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyAttackCommand = new EnemyAttackCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.eEnemyController, enemyData.attackDamage);

                enemyAttackCommand.Execute();
            }
        }

#if ENEMYDEBUG

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, enemyData.isMinAttackPlayerDistance);

                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, enemyData.isMaxAttackPlayerDistance);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(transform.position, enemyData.isRunAwayDistance);
            }
        }

#endif
    }
}