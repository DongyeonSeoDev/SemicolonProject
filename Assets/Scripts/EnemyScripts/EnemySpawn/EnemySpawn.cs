using System.Collections.Generic;

namespace Enemy
{
    public class EnemySpawn : MonoSingleton<EnemySpawn>
    {
        public List<EnemySpawnSO> enemySpawnList = new List<EnemySpawnSO>();
        public List<List<Enemy>> enemyList;

        private void Start()
        {
            enemyList = EnemyManager.Instance.enemyList;

            PlayerRespawnEvent();

            EventManager.StartListening("AfterPlayerRespawn", PlayerRespawnEvent);
        }

        private void PlayerRespawnEvent()
        {
            EnemyManager.Instance.PlayerDeadEvent();
            SpawnEnemy(enemySpawnList);
        }

        public void SpawnEnemy(List<EnemySpawnSO> enemySpawnSO)
        {
            for (int i = 0; i < enemySpawnSO.Count; i++)
            {
                EnemyPoolData enemy = EnemyPoolManager.Instance.GetPoolObject(enemySpawnSO[i].enemyType, enemySpawnSO[i].spawnPosition);

                while (enemyList.Count <= enemySpawnSO[i].stageNumber)
                {
                    enemyList.Add(new List<Enemy>());
                }

                enemyList[enemySpawnSO[i].stageNumber].Add(enemy.GetComponent<Enemy>());
            }
        }
    }
}