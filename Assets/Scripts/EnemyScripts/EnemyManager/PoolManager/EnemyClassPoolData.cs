public class EnemyClassPoolData
{
    public void Destroy()
    {
        EnemyClassPoolManager.AddPool(this);
    }
}
