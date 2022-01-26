using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawn : MonoBehaviour
    {
        public List<EnemySpawnSO> enemySpawnList = new List<EnemySpawnSO>();
        public List<List<Enemy>> enemyList;

        private void Start()
        {
            enemyList = EnemyManager.Instance.enemyList;

            SpawnEnemy();

            SlimeEventManager.StartListening("PlayerDead", () =>
            {
                EnemyManager.Instance.PlayerDeadEvent();
                SpawnEnemy();
            });
        }

        private void SpawnEnemy()
        {
            for (int i = 0; i < enemySpawnList.Count; i++)
            {
                PoolManager enemy = EnemyPoolManager.Instance.GetPoolObject(enemySpawnList[i].enemyType, enemySpawnList[i].spawnPosition);

                while (enemyList.Count <= enemySpawnList[i].stageNumber)
                {
                    enemyList.Add(new List<Enemy>());
                }

                enemyList[enemySpawnList[i].stageNumber].Add(enemy.GetComponent<Enemy>());
            }
        }
    }
}