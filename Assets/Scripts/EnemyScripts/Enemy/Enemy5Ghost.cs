namespace Enemy
{
    public class Enemy5Ghost : Enemy
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 0.5f;

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyMoveCommand();
        }

        public void ReadyAttack() // �ִϸ��̼ǿ��� ����
        {
            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }
        }
    }
}