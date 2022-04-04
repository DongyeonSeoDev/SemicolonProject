using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // �ι�° ��
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isAttackCommand = true;
            enemyData.isLongDistanceAttack = true;
            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 1.5f;
            enemyData.minRunAwayTime = 2f;
            enemyData.maxRunAwayTime = 4f;
            enemyData.playerAnimationTime = 1.05f;

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData); // MoveCommand ����
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, this, enemyData.attackPower);
            enemyAttackCommand = new EnemyAttackCommand(this, enemyData.enemyObject.transform, enemyData.attackPower);
        }

        public void EnemyAttack() // �ִϸ��̼ǿ��� ���� - �� ����
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