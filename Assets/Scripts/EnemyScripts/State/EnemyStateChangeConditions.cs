using UnityEngine;

namespace Enemy
{
    public partial class Move : State // ������ ����
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

    public partial class Chase : State // �߰� ����
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

    public partial class Attack : State // ���� ����
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

    public partial class GetDamaged : State // �������� �޾�����
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

    public partial class Dead : State // �׾�����
    {
        protected override void StateChangeCondition() 
        {
            ChangeState(null);
        }
    }

    public partial class State
    {
        public bool AnyStateChangeState()
        {
            if (enemyData.isDamaged)
            {
                ChangeState(new GetDamaged(enemyData));
            }
            else if (enemyData.hp <= 0)
            {
                ChangeState(new Dead(enemyData));
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}