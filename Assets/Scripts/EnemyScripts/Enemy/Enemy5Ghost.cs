namespace Enemy
{
    public class Enemy5Ghost : Enemy
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 0.5f;

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyMoveCommand(enemyData, transform, SlimeGameManager.Instance.CurrentPlayerBody.transform, enemyData.chaseSpeed);
        }

        public void ReadyAttack() // 애니메이션에서 실행
        {
            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }
        }
    }
}