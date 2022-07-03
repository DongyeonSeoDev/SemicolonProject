using UnityEngine;

namespace Enemy
{
    public class Enemy2 : Enemy // �ι�° ��
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        public Transform shootPosition;

        public Color playerAttackColor;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isLongDistanceAttack = true;
            enemyData.attackDelay = 1.5f;
            enemyData.minRunAwayTime = 2f;
            enemyData.maxRunAwayTime = 4f;
            enemyData.playerAnimationTime = 1.05f;

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData); // MoveCommand ����
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyAttackPlayerCommand = new PlayerlongRangeAttackCommand(transform, shootPosition, this, Type.Bullet, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.randomCritical, playerAttackColor);
            enemyAttackCommand = new EnemylongRangeAttackCommand(this, enemyData.enemyObject.transform, Type.Bullet, transform, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.randomCritical);
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