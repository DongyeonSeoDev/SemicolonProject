namespace Enemy
{
    public class Enemy2 : Enemy // �ι�° ��
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isLongDistanceAttack = true;
            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 1.5f;
            enemyData.minRunAwayTime = 2f;
            enemyData.maxRunAwayTime = 4f;
            enemyData.playerAnimationTime = 1.05f;
            enemyData.maxHP = 20;
            enemyData.hp = 20;

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData, positionCheckData); // MoveCommand ����
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyAttackPlayerCommand = new PlayerlongRangeAttackCommand(transform, transform, this, Type.Bullet, enemyData.attackPower);
            enemyAttackCommand = new EnemylongRangeAttackCommand(this, enemyData.enemyObject.transform, Type.Bullet, transform, enemyData.attackPower);
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