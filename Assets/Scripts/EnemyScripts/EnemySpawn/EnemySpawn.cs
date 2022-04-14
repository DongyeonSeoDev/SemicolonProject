using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawn : MonoSingleton<EnemySpawn>
    {
#if UNITY_EDITOR
        public List<EnemySpawnData> enemySpawnData = new List<EnemySpawnData>();
        public Queue<EnemySpawnData> addSpawnDataQueue = new Queue<EnemySpawnData>();
        public Queue<EnemySpawnData> removeSpawnDataQueue = new Queue<EnemySpawnData>();
#endif
        public Dictionary<string, List<Enemy>> enemyDictionary = new Dictionary<string, List<Enemy>>();

        private void Start()
        {
            enemyDictionary = EnemyManager.Instance.enemyDictionary;

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
            if (!CSVEnemySpawn.Instance.enemySpawnDatas.ContainsKey(stageId))
            {
                Debug.LogError("잘못된 stageId 입니다. SpawnEnemy 실행 실패 : " + stageId);
            }

            for (int i = 0; i < CSVEnemySpawn.Instance.enemySpawnDatas[stageId].Count; i++)
            {
                EnemyPoolData effect = EnemyPoolManager.Instance.GetPoolObject(Type.EnemySpawnEffect, CSVEnemySpawn.Instance.enemySpawnDatas[stageId][i].position);

                effect.GetComponent<EnemySpawnEffect>().Play();
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

                for (int i = 0; i < enemyDictionary[stageId].Count; i++)
                {
                    enemyDictionary[stageId][i].MoveEnemy();
                }

                EventManager.TriggerEvent("EnemySpawnAfter");
            }, 2.5f);
        }
    }
}