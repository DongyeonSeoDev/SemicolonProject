using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [System.Serializable]
    public class EnemyLootList
    {
        public ItemSO enemyLoot;
        public int lootCount;
    }

    [CreateAssetMenu(fileName = "EnemyLootList", menuName = "EnemyLootList")]
    public class EnemyLootListSO : ScriptableObject
    {
        public List<EnemyLootList> enemyLootList = new List<EnemyLootList>();
    }
}