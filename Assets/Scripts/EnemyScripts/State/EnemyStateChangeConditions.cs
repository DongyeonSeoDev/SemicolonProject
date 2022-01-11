using UnityEngine;

namespace Enemy
{
    public partial class Move : State // 움직임 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (enemyData.IsSeePlayer())
            {
                ChangeState(new Chase(enemyData));
            }
            else if (enemyData.IsAttackPlayer())
            {
                ChangeState(new Attack(enemyData));
            }
        }
    }

    public partial class Chase : State // 추격 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (!enemyData.IsSeePlayer())
            {
                ChangeState(new Move(enemyData));
            }
            else if (enemyData.IsAttackPlayer())
            {
                ChangeState(new Attack(enemyData));
            }
        }
    }

    public partial class Attack : State // 공격 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (!enemyData.IsSeePlayer())
            {
                ChangeState(new Move(enemyData));
            }
            else if (!enemyData.IsAttackPlayer())
            {
                ChangeState(new Chase(enemyData));
            }
        }
    }

    public partial class GetDamaged : State // 공격 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (enemyData.IsSeePlayer())
            {
                ChangeState(new Chase(enemyData));
            }
            else if (enemyData.IsAttackPlayer())
            {
                ChangeState(new Attack(enemyData));
            }
            else
            {
                ChangeState(new Move(enemyData));
            }
        }
    }

    public partial class State
    {
        public bool AnyStateChangeState()
        {
            if (enemyData.isDamaged)
            {
                ChangeState(new GetDamaged(enemyData));
                return true;
            }

            return false;
        }
    }
}