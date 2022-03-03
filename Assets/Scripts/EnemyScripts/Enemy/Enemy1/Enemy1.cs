namespace Enemy
{
    public class Enemy1 : Enemy // 첫번째 적
    {
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
            };

            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(transform, enemyData.PlayerObject.transform, rb, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, false);
            base.OnEnable();
        }

        public void InitData(out EnemyController controller, out int damage)
        {
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }
    }
}