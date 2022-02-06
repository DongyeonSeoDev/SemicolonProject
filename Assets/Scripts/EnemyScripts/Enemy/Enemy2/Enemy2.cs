using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // 두번째 적
    {
        private EnemyCommand enemyAttackCommand;

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
                damagedColor = new Color(1f, 180f / 255f, 180f / 255f),
                normalColor = Color.white,
                enemyId = "Rat_02",
                isHitAnimation = true,
                isAttackCommand = true,
                isLongDistanceAttack = true,
                isRotate = true,
                damageDelay = 0.4f,
                chaseSpeed = 3f,
                isSeePlayerDistance = 10f
            };

            enemyAttackCommand = new EnemyAttackCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.eEnemyController, enemyData.attackDamage);

            base.OnEnable();
        }

        public void EnemyAttack()
        {
            enemyAttackCommand.Execute();
        }
    }
}