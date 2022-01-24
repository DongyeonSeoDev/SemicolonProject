using UnityEngine;

namespace Enemy
{
    public partial class EnemySpawnSO : ScriptableObject
    {
        public Vector3 spawnPosition;
        public Type enemyType;
    }
}