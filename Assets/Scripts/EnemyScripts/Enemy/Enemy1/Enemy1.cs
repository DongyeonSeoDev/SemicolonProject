namespace Enemy
{
    public class Enemy1 : Enemy // ù��° ��
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

            base.OnEnable();
        }

        public void InitData(out EnemyController controller, out int damage)
        {
            controller = enemyData.eEnemyController;
            damage = enemyData.attackDamage;
        }
    }
}