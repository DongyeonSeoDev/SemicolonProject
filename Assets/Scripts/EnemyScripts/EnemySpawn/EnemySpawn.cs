using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawn : MonoSingleton<EnemySpawn>
    {
        public List<List<Enemy>> enemyList;

        private void Start()
        {
            enemyList = EnemyManager.Instance.enemyList;

            PlayerRespawnEvent();

            EventManager.StartListening("AfterPlayerRespawn", PlayerRespawnEvent);
            EventManager.StartListening("SpawnEnemy", SpawnEnemy);
            EventManager.StartListening("EnemyMove", EnemyMove);

            CSVEnemySpawn.Instance.GetData("");
            
            for (int i = 0; i < 3; i++)
            {
                EventManager.TriggerEvent("SpawnEnemy", i);
            }
        }

        private void PlayerRespawnEvent()
        {
            EnemyManager.Instance.PlayerDeadEvent();

            for (int i = 0; i < 3; i++)
            {
                EventManager.TriggerEvent("SpawnEnemy", i);
            }
        }

        private void SpawnEnemy(int stageNumber)
        {
            if (CSVEnemySpawn.Instance.enemySpawnDatas.Count <= stageNumber)
            {
                Debug.LogError("잘못된 stageNumber 입니다. SpawnEnemy 실행 실패");
            }

            for (int i = 0; i < CSVEnemySpawn.Instance.enemySpawnDatas[stageNumber].Count; i++)
            {
                EnemyPoolData enemy = EnemyPoolManager.Instance.GetPoolObject((Type)CSVEnemySpawn.Instance.enemySpawnDatas[stageNumber][i].enemyId, CSVEnemySpawn.Instance.enemySpawnDatas[stageNumber][i].position);

                while (enemyList.Count <= stageNumber)
                {
                    enemyList.Add(new List<Enemy>());
                }

                enemyList[stageNumber].Add(enemy.GetComponent<Enemy>());
            }
        }

        private void EnemyMove(int stageNumber)
        {
            if (enemyList.Count <= stageNumber)
            {
                Debug.LogError("잘못된 stageNumber 입니다. EnemyMove 실행 실패");
            }

            for (int i = 0; i < enemyList[stageNumber].Count; i++)
            {
                enemyList[stageNumber][i].MoveEnemy();
            }
        }
    }
}