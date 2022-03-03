namespace Enemy
{
    public class Enemy2 : Enemy // �ι�° ��
    {
        private EnemyCommand enemyAttackCommand;
        private EnemyCommand enemyAttackPlayerCommand;

        protected override void OnEnable()
        {
            enemyData = new EnemyData(enemyDataSO)
            {
                enemyObject = gameObject,
                enemyLootList = enemyLootListSO,
                enemyAnimator = anim,
                enemySpriteRenderer = sr,
                enemyRigidbody2D = rb,
                hpBarFillImage = hpBarFillImage,
                isHitAnimation = true,
                isAttackCommand = true,
                isLongDistanceAttack = true,
                isRotate = true,
                damageDelay = 0.4f,
                chaseSpeed = 7f,
                isSeePlayerDistance = 10f,
                attackDelay = 2f
            };

            enemyData.enemyMoveCommand = new EnemyRandomMoveCommand(enemyData);
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