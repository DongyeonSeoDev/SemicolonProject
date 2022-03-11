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

            //EventManager.StartListening("SpawnEnemy", SpawnEnemy);
            EventManager.StartListening("SpawnEnemy", SpawnEnemy);

            CSVEnemySpawn.Instance.GetData("");
            
            for (int i = 0; i < 3; i++)
            {
                EventManager.TriggerEvent("SpawnEnemy", "", i);
            }
        }

        private void PlayerRespawnEvent()
        {
            EnemyManager.Instance.PlayerDeadEvent();
        }

        //private void SpawnEnemy(int stageNumber)
        //{
        //    for (int i = 0; i < CSVEnemySpawn.Instance.enemySpawnDatas[stageNumber].Count; i++)
        //    {
        //        EnemyPoolData enemy = EnemyPoolManager.Instance.GetPoolObject((Type)CSVEnemySpawn.Instance.enemySpawnDatas[stageNumber][i].enemyId, CSVEnemySpawn.Instance.enemySpawnDatas[stageNumber][i].position);

        //        while (enemyList.Count <= stageNumber)
        //        {
        //            enemyList.Add(new List<Enemy>());
        //        }

        //        enemyList[stageNumber].Add(enemy.GetComponent<Enemy>());
        //    }
        //}

        private void SpawnEnemy(string temp, int stageNumber)
        {
            if (CSVEnemySpawn.Instance.enemySpawnDatas.Count <= stageNumber)
            {
                return;
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
    }
}