using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public enum EnemyAnimationType
    {
        Reset,
        Die,
        IsDead,
        Hit,
        Move,
        Attack,
        AttackEnd
    }

    [System.Serializable]
    public struct EnemyAnimation
    {
        public EnemyAnimationType type;
        public string animationName;
    }

    [CreateAssetMenu(fileName = "EnemyDataSO", menuName = "EnemySO/EnemyDataSO", order = int.MaxValue)]
    public class EnemyDataSO : ScriptableObject
    {
        public EnemyType enemyType;

        public List<EnemyAnimation> animationList = new List<EnemyAnimation>();

        public Color normalColor;
        public Color damagedColor;
        public Color enemyDeadEffectColor;
        public Color playerNormalColor;
        public Color playerDamagedColor;
        public Color playerDeadEffectColor;
    }
}