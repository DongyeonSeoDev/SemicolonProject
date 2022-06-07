namespace Enemy
{
    public class EnemyReflectionBulletEffect : EnemyPoolData
    {
        public void Remove()
        {
            gameObject.SetActive(false);
        }
    }
}