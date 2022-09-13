namespace Enemy
{
    public class Enemy6Mage : Enemy
    {
        public EnemyBullet2[] bullets;

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, transform, rb, enemyData.chaseSpeed);

            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i].Init(enemyData.eEnemyController, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.criticalDamagePercent, UnityEngine.Color.white, this);
            }
        }
    }
}