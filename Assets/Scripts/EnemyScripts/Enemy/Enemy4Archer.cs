using UnityEngine;

namespace Enemy
{
    public class Enemy4Archer : Enemy
    {
        public Transform shotPosition;
        public float moveDelay = 1f;

        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 0.42f;
            enemyData.playerAnimationTime = 0.45f;
            enemyData.isAttackPlayerDistance = 12f;
            enemyData.maxHP = 50;
            enemyData.hp = 50;
            enemyData.attackPower = 15;
            enemyData.playerAnimationDelay = 0.2f;
            enemyData.playerAnimationSpeed = 1.0f;
            enemyData.startAttackDelay = 0.5f;
            enemyData.minAttackCount = 3;
            enemyData.maxAttackCount = 5;

            enemyData.enemyMoveCommand = new EnemyLongDistanceFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isAttackPlayerDistance - 2f, moveDelay);
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);

            enemyAttackPlayerCommand = new PlayerlongRangeAttackCommand(transform, shotPosition, this, Type.Arrow, enemyData.attackPower);
            enemyAttackCommand = new EnemylongRangeAttackCommand(this, enemyData.enemyObject.transform, Type.Arrow, shotPosition, enemyData.attackPower);
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