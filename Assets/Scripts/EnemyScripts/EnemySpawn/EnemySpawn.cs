using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public struct EnemySpawnEffectPosition
    {
        public Type enemyType;
        public Vector2 spawnPosition;
        public Vector3 spawnScale;
    }

    public class EnemySpawn : MonoSingleton<EnemySpawn>
    {
#if UNITY_EDITOR
        public List<EnemySpawnData> enemySpawnData = new List<EnemySpawnData>();
        public Queue<EnemySpawnData> addSpawnDataQueue = new Queue<EnemySpawnData>();
        public Queue<EnemySpawnData> removeSpawnDataQueue = new Queue<EnemySpawnData>();
#endif

        public List<EnemySpawnEffectPosition> effectPositionList = new List<EnemySpawnEffectPosition>();

        public Dictionary<string, List<Enemy>> enemyDictionary = new Dictionary<string, List<Enemy>>();

         private Dictionary<Type, Vector2> enemySpawnEffectPositionDic = new Dictionary<Type, Vector2>();
         private Dictionary<Type, Vector3> enemySpawnEffectScaleDic = new Dictionary<Type, Vector3>();

        private void Start()
        {
            enemyDictionary = EnemyManager.Instance.enemyDictionary;
            enemySpawnEffectPositionDic = effectPositionList.ToDictionary(x => x.enemyType, x => x.spawnPosition);
            enemySpawnEffectScaleDic = effectPositionList.ToDictionary(x => x.enemyType, x => x.spawnScale);

            PlayerRespawnEvent();

            EventManager.StartListening("AfterPlayerRespawn", PlayerRespawnEvent);
            EventManager.StartListening("SpawnEnemy", SpawnEnemy);

            CSVEnemySpawn.Instance.GetData();
        }

        private void PlayerRespawnEvent()
        {
            EnemyManager.Instance.PlayerDeadEvent();
        }

        private void SpawnEnemy(string stageId)
        {
            var spawnData = CSVEnemySpawn.Instance.enemySpawnDatas;

            if (!spawnData.ContainsKey(stageId))
            {
                Debug.LogError("잘못된 stageId 입니다. SpawnEnemy 실행 실패 : " + stageId);
            }

            for (int i = 0; i < spawnData[stageId].Count; i++)
            {
                EnemySpawnData data = spawnData[stageId][i];

                if (enemySpawnEffectPositionDic.ContainsKey(data.enemyId))
                {
                    EnemyPoolData effect = EnemyPoolManager.Instance.GetPoolObject(Type.EnemySpawnEffect, (Vector2)data.position + enemySpawnEffectPositionDic[data.enemyId]);
                    effect.transform.localScale = enemySpawnEffectScaleDic[data.enemyId];

                    effect.GetComponent<EnemySpawnEffect>().Play();
                }
            }

            for (int i = 0; i < spawnData[stageId].Count; i++) // 보스 라면 대기 없이 바로 소환후 움직임
            {
                if (spawnData[stageId][i].enemyId == Type.Boss_SkeletonKing_50)
                {
                    Spawn(spawnData[stageId], stageId);
                    Move(stageId);

                    return;
                }
            }

            Util.DelayFunc(() =>
            {
                Spawn(spawnData[stageId], stageId);

                EnemyManager.Instance.enemyCount = spawnData[stageId].Count;
                EventManager.TriggerEvent("EnemySpawnAfter");

                Util.DelayFunc(() => 
                {
                    for (int i = 0; i < enemyDictionary[stageId].Count; i++)
                    {
                        Move(stageId);
                    }
                }, 1f);
            }, 2.5f);
        }

        private void Spawn(List<EnemySpawnData> spawnData, string stageId)
        {
            for (int i = 0; i < spawnData.Count; i++)
            {
                EnemyPoolData enemy = EnemyPoolManager.Instance.GetPoolObject(spawnData[i].enemyId, spawnData[i].position);

                if (enemyDictionary.ContainsKey(stageId))
                {
                    enemyDictionary[stageId].Add(enemy.GetComponent<Enemy>());
                }
                else
                {
                    enemyDictionary.Add(stageId, new List<Enemy>());
                    enemyDictionary[stageId].Add(enemy.GetComponent<Enemy>());
                }
            }
        }

        private void Move(string stageId)
        {
            for (int i = 0; i < enemyDictionary[stageId].Count; i++)
            {
                enemyDictionary[stageId][i].MoveEnemy();
            }
        }
    }
}