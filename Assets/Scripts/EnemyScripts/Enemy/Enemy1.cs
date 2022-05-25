namespace Enemy
{
    public class Enemy1 : Enemy // ù��° ��
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.maxHP = 30;
            enemyData.hp = 30;

            enemyData.playerAnimationTime = 0.85f;
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance); // MoveCommand �߰�
            enemyData.enemySpriteRotateCommand = new EnemySpriteFlipCommand(enemyData);
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