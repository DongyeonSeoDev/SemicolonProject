namespace Enemy
{
    public class Enemy6Mage : Enemy
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed);
        }
    }
}