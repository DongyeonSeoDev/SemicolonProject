using UnityEngine;

namespace Enemy
{
    public partial class Move : State // ������ ����
    {
        protected override void StateChangeCondition()
        {
            if (enemyData.IsSeePlayer())
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
            if (!enemyData.IsSeePlayer())
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
            if (!enemyData.IsSeePlayer())
            {
                ChangeState(new Move(enemyData));
            }
            else if (!enemyData.IsAttackPlayer())
            {
                ChangeState(new Chase(enemyData));
            }
        }
    }
}