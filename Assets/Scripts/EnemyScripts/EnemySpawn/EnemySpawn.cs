using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemySpawn : MonoBehaviour
    {
        public List<EnemySpawnSO> enemySpawnList = new List<EnemySpawnSO>();

        private void Start()
        {
            for (int i = 0; i < enemySpawnList.Count; i++)
            {
                EnemyPoolManager.Instance.GetPoolObject(enemySpawnList[i].enemyType, enemySpawnList[i].spawnPosition);
            }
        }
    }
}