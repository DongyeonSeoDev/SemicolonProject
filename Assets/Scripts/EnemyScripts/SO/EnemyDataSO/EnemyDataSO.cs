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
        AttackEnd,
        Idle,
        Player
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

        [Header("스피드")]
        public float speed;
        [Header("최소 공격력")]
        public float minAttack;
        [Header("최대 공격력")]
        public float maxAttack;
        [Header("치명타 확률")]
        public float critical;
        [Header("치명타 추가 데미지 퍼센트")]
        public float criticalDamage;
        [Header("방어력")]
        public float defense;
        [Header("체력")]
        public float hp;
        [Header("경험치")]
        public float addExperience;
    }
}