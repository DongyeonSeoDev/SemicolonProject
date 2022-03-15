using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawn : MonoSingleton<EnemySpawn>
    {
        public Dictionary<string, List<Enemy>> enemyDictionary = new Dictionary<string, List<Enemy>>();

        private void Start()
        {
            enemyDictionary = EnemyManager.Instance.enemyDictionary;

            PlayerRespawnEvent();

            EventManager.StartListening("AfterPlayerRespawn", PlayerRespawnEvent);
            EventManager.StartListening("SpawnEnemy", SpawnEnemy);
            EventManager.StartListening("EnemyMove", EnemyMove);

            CSVEnemySpawn.Instance.GetData("");
            
            /*for (int i = 0; i < 3; i++)
            {
                EventManager.TriggerEvent("SpawnEnemy", i);
            }*/
        }

        private void PlayerRespawnEvent()
        {
            EnemyManager.Instance.PlayerDeadEvent();

           /* for (int i = 0; i < 3; i++)
            {
                EventManager.TriggerEvent("SpawnEnemy", i);
            }*/
        }

        private void SpawnEnemy(string stageId)
        {
            if (!CSVEnemySpawn.Instance.enemySpawnDatas.ContainsKey(stageId))
            {
                //CSVEnemySpawn.Instance.enemySpawnDatas.Keys.ForEach(x => Debug.Log(x.Trim() == stageId.Trim()));
                Debug.LogError("잘못된 stageId 입니다. SpawnEnemy 실행 실패 : " + stageId);
            }

            for (int i = 0; i < CSVEnemySpawn.Instance.enemySpawnDatas[stageId].Count; i++)
            {
                EnemyPoolData enemy = EnemyPoolManager.Instance.GetPoolObject((Type)CSVEnemySpawn.Instance.enemySpawnDatas[stageId][i].enemyId, CSVEnemySpawn.Instance.enemySpawnDatas[stageId][i].position);

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

        private void EnemyMove(string stageId)
        {
            if (!enemyDictionary.ContainsKey(stageId))
            {
                Debug.LogError("잘못된 stageId 입니다. EnemyMove 실행 실패 : " + stageId);
            }

            for (int i = 0; i < enemyDictionary[stageId].Count; i++)
            {
                enemyDictionary[stageId][i].MoveEnemy();
            }
        }
    }
}