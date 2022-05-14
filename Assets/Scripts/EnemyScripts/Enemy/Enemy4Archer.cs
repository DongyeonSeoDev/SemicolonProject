namespace Enemy
{
    public class Enemy4Archer : Enemy
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.chaseSpeed = 10f;
            enemyData.attackDelay = 1f;
            enemyData.playerAnimationTime = 0.55f;
            enemyData.isAttackPlayerDistance = 5f;
            enemyData.maxHP = 50;
            enemyData.hp = 50;

            enemyData.enemyMoveCommand = new EnemyLongDistanceFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isAttackPlayerDistance);
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