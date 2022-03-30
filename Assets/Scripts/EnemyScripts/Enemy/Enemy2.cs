using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // 두번째 적
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isAttackCommand = true;
            enemyData.isLongDistanceAttack = true;
            enemyData.damageDelay = 0.4f;
            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 1.5f;
            enemyData.minRunAwayTime = 2f;
            enemyData.maxRunAwayTime = 4f;
            enemyData.playerAnimationTime = 1.05f;

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData); // MoveCommand 생성
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, this, enemyData.attackPower);
            enemyAttackCommand = new EnemyAttackCommand(this, enemyData.enemyObject.transform, enemyData.attackPower);
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