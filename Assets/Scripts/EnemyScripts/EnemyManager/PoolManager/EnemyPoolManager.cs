using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    public enum Type
    {
        Slime_01 = 1,
        Rat_02,
        Slime_03,
        SkeletonArcher_04,
        SkeletonGhost_05,
        SkeletonMage_06,
        Boss_SkeletonKing_50 = 50,
        Bullet = 100,
        EnemyLoot,
        DeadEffect,
        BulletEffect,
        Fire,
        EnemySpawnEffect,
        EnemyRushAttackRange,
        Boss1Clone,
        EnemyAfterImage,
        Arrow,
        HitEffect,
        ReflectionBullet,
        ReflectionBulletEffect,
        StunEffect
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

        public List<EnemyPoolData> poolList = new List<EnemyPoolData>();

        private Dictionary<Type, EnemyPoolData> poolDictionary = new Dictionary<Type, EnemyPoolData>();
        private Dictionary<Type, Queue<EnemyPoolData>> poolQueueDictionary = new Dictionary<Type,Queue<EnemyPoolData>>();

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
                poolQueueDictionary[type] = new Queue<EnemyPoolData>();

                if (poolDictionary.ContainsKey(type))
                {
                    for (int j = 0; j < poolDictionary[type].count; j++)
                    {
                        poolQueueDictionary[type].Enqueue(MakePoolObject(type));
                    }
                }
            }
        }

        public EnemyPoolData GetPoolObject(Type poolType, Vector2 position)
        {
            EnemyPoolData pool;

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
            pool.transform.rotation = Quaternion.identity;

            poolQueueDictionary[poolType].Enqueue(pool);

            return pool;
        }

        private EnemyPoolData MakePoolObject(Type poolType)
        {
            EnemyPoolData pool = Instantiate(poolDictionary[poolType], transform);
            pool.gameObject.SetActive(false);

            return pool;
        }
    }
}