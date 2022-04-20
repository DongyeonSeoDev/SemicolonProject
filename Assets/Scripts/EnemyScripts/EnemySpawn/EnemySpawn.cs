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
            EventManager.StartListening("BossSpawn", BossSpawn);

            CSVEnemySpawn.Instance.GetData();
        }

        private void PlayerRespawnEvent()
        {
            EnemyManager.Instance.PlayerDeadEvent();
        }

        private void SpawnEnemy(string stageId)
        {
            if (!CSVEnemySpawn.Instance.enemySpawnDatas.ContainsKey(stageId))
            {
                Debug.LogError("잘못된 stageId 입니다. SpawnEnemy 실행 실패 : " + stageId);
            }

            for (int i = 0; i < CSVEnemySpawn.Instance.enemySpawnDatas[stageId].Count; i++)
            {
                EnemySpawnData data = CSVEnemySpawn.Instance.enemySpawnDatas[stageId][i];

                if (enemySpawnEffectPositionDic.ContainsKey(data.enemyId))
                {
                    EnemyPoolData effect = EnemyPoolManager.Instance.GetPoolObject(Type.EnemySpawnEffect, (Vector2)data.position + enemySpawnEffectPositionDic[data.enemyId]);
                    effect.transform.localScale = enemySpawnEffectScaleDic[data.enemyId];

                    effect.GetComponent<EnemySpawnEffect>().Play();
                }
            }

            Util.DelayFunc(() =>
            {
                for (int i = 0; i < CSVEnemySpawn.Instance.enemySpawnDatas[stageId].Count; i++)
                {
                    EnemyPoolData enemy = EnemyPoolManager.Instance.GetPoolObject(CSVEnemySpawn.Instance.enemySpawnDatas[stageId][i].enemyId, CSVEnemySpawn.Instance.enemySpawnDatas[stageId][i].position);

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

                EnemyManager.Instance.enemyCount = CSVEnemySpawn.Instance.enemySpawnDatas[stageId].Count;  //Set Enemy Count
                EventManager.TriggerEvent("EnemySpawnAfter");

                Util.DelayFunc(() => 
                {
                    for (int i = 0; i < enemyDictionary[stageId].Count; i++)
                    {
                        enemyDictionary[stageId][i].MoveEnemy();
                    }
                }, 1f);
            }, 2.5f);
        }

        private void BossSpawn(string stageId)
        {
            SpawnEnemy(stageId);
        }
    }
}