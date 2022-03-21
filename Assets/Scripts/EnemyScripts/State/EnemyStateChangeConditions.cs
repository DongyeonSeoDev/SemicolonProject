using UnityEngine;

namespace Enemy
{
    public partial class EnemyIdleState : EnemyState
    {
        protected override void StateChangeCondition()
        {
            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                ChangeState(new EnemyMoveState(enemyData));
            }
            else if (enemyData.isEnemyMove)
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
        }
    }

    public partial class EnemyMoveState : EnemyState // ������ ����
    {
        protected override void StateChangeCondition()
        {
            AnyStateChangeState();
        }
    }

    public partial class EnemyChaseState : EnemyState // �߰� ����
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (enemyData.IsAttackPlayer())
            {
                ChangeState(new EnemyAttackState(enemyData));
            }
        }
    }

    public partial class EnemyAttackState : EnemyState // ���� ����
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                ChangeState(new EnemyMoveState(enemyData));
            }
            else if (enemyData.eEnemyController == EnemyController.AI && !enemyData.IsAttackPlayer())
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
        }
    }

    public partial class EnemyGetDamagedState : EnemyState // �������� �޾�����
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                ChangeState(new EnemyMoveState(enemyData));
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                if (enemyData.IsAttackPlayer())
                {
                    ChangeState(new EnemyAttackState(enemyData));
                }
                else
                {
                    ChangeState(new EnemyChaseState(enemyData));
                }
            }
        }
    }

    public partial class EnemyDeadState : EnemyState // �׾�����
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
            if (enemyData.hp <= 0)
            {
                ChangeState(new EnemyDeadState(enemyData));
            }
            else if (enemyData.isDamaged)
            {
                ChangeState(new EnemyGetDamagedState(enemyData));
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER && enemyData.isAttack)
            {
                enemyData.isPlayerAttacking = true;
                enemyData.isAttack = false;

                ChangeState(new EnemyAttackState(enemyData));
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}