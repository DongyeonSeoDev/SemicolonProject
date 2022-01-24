using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    public enum Type
    {
        Slime_01,
        Rat_02,
        Bullet,
        EnemyLoot
    }

    public class EnemyPoolManager : MonoBehaviour
    {
        private static EnemyPoolManager instance;

        public static EnemyPoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogWarning("EnemyPoolManager의 instance가 null 입니다.");

                    return null;
                }

                return instance;
            }
        }

        public List<PoolManager> poolList = new List<PoolManager>();

        private Dictionary<Type, PoolManager> poolDictionary = new Dictionary<Type, PoolManager>();
        private Dictionary<Type, Queue<PoolManager>> poolQueueDictionary = new Dictionary<Type,Queue<PoolManager>>();

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("EnemyPoolManager의 instance가 이미 있습니다.");
            }

            instance = this;

            poolDictionary = poolList.ToDictionary(x => x.poolType, x => x);

            foreach (Type type in Enum.GetValues(typeof(Type)))
            {
                poolQueueDictionary[type] = new Queue<PoolManager>();

                if (poolDictionary.ContainsKey(type))
                {
                    for (int j = 0; j < poolDictionary[type].count; j++)
                    {
                        poolQueueDictionary[type].Enqueue(MakePoolObject(type));
                    }
                }
            }
        }

        public PoolManager GetPoolObject(Type poolType, Vector2 position)
        {
            PoolManager pool;

            if (poolQueueDictionary[poolType].Peek().gameObject.activeSelf)
            {
                pool = MakePoolObject(poolType);   
            }
            else
            {
                pool = poolQueueDictionary[poolType].Dequeue();
            }

            pool.gameObject.SetActive(true);
            pool.transform.position = position;

            poolQueueDictionary[poolType].Enqueue(pool);

            return pool;
        }

        private PoolManager MakePoolObject(Type poolType)
        {
            PoolManager pool = Instantiate(poolDictionary[poolType], transform);
            pool.gameObject.SetActive(false);

            return pool;
        }
    }
}