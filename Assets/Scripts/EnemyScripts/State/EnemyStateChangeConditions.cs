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

    public partial class EnemyMoveState : EnemyState // 움직임 상태
    {
        protected override void StateChangeCondition()
        {
            AnyStateChangeState();
        }
    }

    public partial class EnemyChaseState : EnemyState // 추격 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if (EnemyManager.IsAttackPlayer(enemyData))
            {
                ChangeState(new EnemyAttackState(enemyData));
            }
        }
    }

    public partial class EnemyAttackState : EnemyState // 공격 상태
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState())
            {
                return;
            }
            else if (enemyData.eEnemyController == EnemyController.AI && enemyData.addAIAttackStateChangeCondition != null)
            {
                EnemyState state = enemyData.addAIAttackStateChangeCondition.Invoke();

                if (state != null)
                {
                    ChangeState(state);
                    return;
                }
            }
            
            if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                ChangeState(new EnemyMoveState(enemyData));
            }
            else if (enemyData.eEnemyController == EnemyController.AI && !EnemyManager.IsAttackPlayer(enemyData))
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
        }
    }

    public partial class EnemyStunStatus : EnemyState
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState())
            {
                return;
            }
            else if (enemyData.eEnemyController == EnemyController.AI)
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
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
            if (AlwaysCheckStateChangeCondition()) { }
            else if (enemyData.eEnemyController == EnemyController.PLAYER && enemyData.isAttack)
            {
                enemyData.isAttack = false;

                ChangeState(new EnemyAttackState(enemyData));
            }
            else
            {
                return false;
            }

            return true;
        }

        public bool AlwaysCheckStateChangeCondition()
        {
            if (enemyData.hp <= 0)
            {
                ChangeState(new EnemyDeadState(enemyData));
            }
            else if (enemyData.stunTime > 0 && stateName != eState.STUN)
            {
                ChangeState(new EnemyStunStatus(enemyData));
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}