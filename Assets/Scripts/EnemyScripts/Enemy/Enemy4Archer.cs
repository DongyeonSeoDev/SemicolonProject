using UnityEngine;

namespace Enemy
{
    public class Enemy4Archer : Enemy
    {
        public Transform shotPosition;
        public float moveDelay = 1f;

        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        public Color playerAttackColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 0.42f;
            enemyData.playerAnimationTime = 0.45f;
            enemyData.isAttackPlayerDistance = 12f;
            enemyData.playerAnimationDelay = 0.1f;
            enemyData.playerAnimationSpeed = 1.0f;
            enemyData.startAttackDelay = 0.5f;
            enemyData.minAttackCount = 3;
            enemyData.maxAttackCount = 5;
            enemyData.isUseIdleAnimation = true;

            enemyData.enemyMoveCommand = new EnemyLongDistanceFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isAttackPlayerDistance - 2f, moveDelay);
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);

            enemyAttackPlayerCommand = new PlayerlongRangeAttackCommand(transform, shotPosition, this, Type.Arrow, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.randomCritical, playerAttackColor);
            enemyAttackCommand = new EnemylongRangeAttackCommand(this, enemyData.enemyObject.transform, Type.Arrow, shotPosition, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.randomCritical);
        }

        public void EnemyAttack() // 애니메이션에서 실행 - 적 공격
        {
            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyAttackPlayerCommand.Execute();
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyAttackCommand.Execute();
            }
        }
    }
}