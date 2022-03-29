using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // �ι�° ��
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            enemyData.isHitAnimation = true;
            enemyData.isAttackCommand = true;
            enemyData.isLongDistanceAttack = true;
            enemyData.isRotate = true;
            enemyData.damageDelay = 0.4f;
            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 1.5f;
            enemyData.minRunAwayTime = 2f;
            enemyData.maxRunAwayTime = 4f;
            enemyData.playerAnimationTime = 1.05f;
            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData); // MoveCommand ����

            // ���� Command ����
            enemyAttackPlayerCommand = new EnemyAttackPlayerCommand(transform, this, enemyData.eEnemyController, enemyData.attackPower);
            enemyAttackCommand = new EnemyAttackCommand(enemyData.enemyObject.transform, EnemyManager.Player.transform, enemyData.eEnemyController, enemyData.attackPower);

            base.OnEnable();
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