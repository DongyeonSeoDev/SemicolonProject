namespace Enemy
{
    public class Enemy1 : Enemy // 첫번째 적
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.isAttackPlayerDistance = 1.7f;
            enemyData.isParrying = true;

            enemyData.playerAnimationTime = 0.85f;
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed); // MoveCommand 추가
            enemyData.enemySpriteRotateCommand = new EnemySpriteFlipCommand(enemyData);

            GetComponentInChildren<BulletCheck>().Init(enemyData);
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