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

    public partial class EnemyLootListSO : ScriptableObject
    {
        public List<EnemyLootList> enemyLootList = new List<EnemyLootList>();
    }
}