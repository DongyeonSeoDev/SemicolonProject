using UnityEngine;

namespace Enemy
{
    public partial class EnemyDataSO : ScriptableObject
    {
        public EnemyType enemyType;

        public Color normalColor;
        public Color damagedColor;
        public Color enemyDeadEffectColor;
        public Color playerNormalColor;
        public Color playerDamagedColor;
        public Color playerDeadEffectColor;
    }
}