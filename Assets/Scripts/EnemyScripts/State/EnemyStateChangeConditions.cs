using UnityEngine;

namespace Enemy
{
    public partial class EnemyMoveState : EnemyState // 움직임 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (enemyData.IsSeePlayer())
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
            else if (enemyData.IsAttackPlayer())
            {
                ChangeState(new EnemyAttackState(enemyData));
            }
        }
    }

    public partial class EnemyChaseState : EnemyState // 추격 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (!enemyData.IsSeePlayer())
            {
                ChangeState(new EnemyMoveState(enemyData));
            }
            else if (enemyData.IsAttackPlayer())
            {
                ChangeState(new EnemyAttackState(enemyData));
            }
        }
    }

    public partial class EnemyAttackState : EnemyState // 공격 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (!enemyData.IsSeePlayer())
            {
                ChangeState(new EnemyMoveState(enemyData));
            }
            else if (!enemyData.IsAttackPlayer())
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
        }
    }

    public partial class EnemyGetDamagedState : EnemyState // 데미지를 받았을때
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (enemyData.IsSeePlayer())
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
            else if (enemyData.IsAttackPlayer())
            {
                ChangeState(new EnemyAttackState(enemyData));
            }
            else
            {
                ChangeState(new EnemyMoveState(enemyData));
            }
        }
    }

    public partial class EnemyDeadState : EnemyState // 죽었을때
    {
        protected override void StateChangeCondition() 
        {
            ChangeState(null);
        }
    }

    public partial class EnemyState
    {
        public bool AnyStateChangeState()
        {
            if (enemyData.isDamaged)
            {
                ChangeState(new EnemyGetDamagedState(enemyData));
            }
            else if (enemyData.hp <= 0)
            {
                ChangeState(new EnemyDeadState(enemyData));
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}