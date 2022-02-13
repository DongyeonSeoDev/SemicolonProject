using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // �ι�° ��
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            enemyData = new EnemyData()
            {
                eEnemyController = EnemyController.AI,
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyLootList = enemyLootListSO,
                enemyAnimator = anim,
                enemySpriteRenderer = sr,
                enemyRigidbody2D = rb,
                hpBarFillImage = hpBarFillImage,
                damagedColor = Util.Change255To1Color(255f, 180f, 180f),
                enemyDeadEffectColor = Util.Change255To1Color(155f, 173f, 183f),
                normalColor = Color.white,
                enemyType = EnemyType.Rat_02,
                isHitAnimation = true,
                isAttackCommand = true,
                isLongDistanceAttack = true,
                isRotate = true,
                damageDelay = 0.4f,
                chaseSpeed = 3f,
                isSeePlayerDistance = 10f
            };

            base.OnEnable();
        }

        public void EnemyAttack()
        {
            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, enemyData.eEnemyController, enemyData.attackDamage);

                enemyAttackPlayerCommand.Execute();
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyAttackCommand = new EnemyAttackCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.eEnemyController, enemyData.attackDamage);

                enemyAttackCommand.Execute();
            }
        }
    }
}