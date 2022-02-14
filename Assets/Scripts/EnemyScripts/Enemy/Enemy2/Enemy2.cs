using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // 두번째 적
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
                enemyDeadEffectColor = Util.Change255To1Color(112f, 173f, 183),
                normalColor = Color.white,
                playerNormalColor = Util.Change255To1Color(200f, 255f, 200f),
                playerDamagedColor = Util.Change255To1Color(255f, 200f, 200f),
                playerDeadEffectColor = Util.Change255To1Color(200f, 255f, 200f),
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
                enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, this, enemyData.eEnemyController, enemyData.attackDamage);

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