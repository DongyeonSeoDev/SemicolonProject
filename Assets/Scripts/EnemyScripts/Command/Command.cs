using UnityEngine;

namespace Enemy
{
    public abstract class Command // �� ���� �θ� Ŭ���� (Ŀ���)
    {
        public abstract void Execute(Transform enemyPosition, EnemyMoveSO enemyMoveSO);
    }
}