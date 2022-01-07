using UnityEngine;

namespace Enemy
{
    public abstract class Command // 적 관리 부모 클래스 (커멘드)
    {
        public abstract void Execute(Transform enemyPosition, EnemyMoveSO enemyMoveSO);
    }
}