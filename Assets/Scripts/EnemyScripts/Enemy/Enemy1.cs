namespace Enemy
{
    public class Enemy1 : Enemy // ù��° ��
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isAttackPlayerDistance = 1.7f;
            enemyData.isParrying = true;

            enemyData.playerAnimationTime = 0.85f;
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed); // MoveCommand �߰�
            enemyData.enemySpriteRotateCommand = new EnemySpriteFlipCommand(enemyData);

            GetComponentInChildren<BulletCheck>().Init(enemyData);
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