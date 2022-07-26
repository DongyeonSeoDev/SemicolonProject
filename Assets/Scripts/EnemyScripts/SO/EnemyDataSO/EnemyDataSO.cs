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

        [Header("���ǵ�")]
        public float speed;
        [Header("�ּ� ���ݷ�")]
        public float minAttack;
        [Header("�ִ� ���ݷ�")]
        public float maxAttack;
        [Header("ġ��Ÿ Ȯ��")]
        public float critical;
        [Header("ġ��Ÿ �߰� ������ �ۼ�Ʈ")]
        public float criticalDamage;
        [Header("����")]
        public float defense;
        [Header("ü��")]
        public float hp;
        [Header("����ġ")]
        public float addExperience;
    }
}