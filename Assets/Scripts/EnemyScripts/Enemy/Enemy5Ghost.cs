namespace Enemy
{
    public class Enemy5Ghost : Enemy
    {
        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 0.5f;
        }
    }
}