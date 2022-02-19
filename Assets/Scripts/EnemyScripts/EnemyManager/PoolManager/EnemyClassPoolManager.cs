using System.Collections.Generic;

public static class EnemyClassPoolManager
{
    private static Dictionary<string, Queue<EnemyClassPoolData>> pool = new Dictionary<string, Queue<EnemyClassPoolData>>();

    public static EnemyClassPoolData GetPool<T>() where T : EnemyClassPoolData
    {
        if (!pool.ContainsKey(nameof(T)))
        {
            pool.Add(nameof(T), new Queue<EnemyClassPoolData>());
            return new EnemyClassPoolData();
        }
        else if (pool[nameof(T)].Count == 0)
        {
            return new EnemyClassPoolData();
        }
        else
        {
            if (pool[nameof(T)].Peek() == null)
            {
                pool[nameof(T)].Dequeue();
                return new EnemyClassPoolData();
            }
            else
            {
                return pool[nameof(T)].Dequeue();
            }
        }
    }

    public static void AddPool<T>(T classData) where T : EnemyClassPoolData
    {
        if (classData == null)
        {
            return;
        }

        if (!pool.ContainsKey(nameof(T)))
        {
            pool.Add(nameof(T), new Queue<EnemyClassPoolData>());
        }

        pool[nameof(T)].Enqueue(classData);
    }
}
