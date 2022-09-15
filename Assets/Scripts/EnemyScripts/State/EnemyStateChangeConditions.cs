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
            if (AnyStateChangeState())
            {
                return;
            }

            if (enemyData.enemyChaseStateChangeCondition != null)
            {
                EnemyState state = enemyData.enemyChaseStateChangeCondition.Invoke();

                if (state != null)
                {
                    ChangeState(state);
                }

                return;
            }

            if (!enemyData.isNoAttack && EnemyManager.IsAttackPlayer(enemyData))
            {
                if (enemyData.attackTypeCheckCondition != null)
                {
                    enemyData.attackTypeCheckCondition();
                }

                ChangeState(new EnemyAIAttackState(enemyData));
            }
            else if (enemyData.addChangeAttackCondition != null)
            {
                EnemyState state = enemyData.addChangeAttackCondition.Invoke();

                if (state != null)
                {
                    ChangeState(state);
                }
            }
        }
    }

    public partial class EnemyAIAttackState : EnemyState
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState())
            {
                return;
            }
            
            if (enemyData.addAIAttackStateChangeCondition != null)
            {
                EnemyState state = enemyData.addAIAttackStateChangeCondition.Invoke();

                if (state != null)
                {
                    ChangeState(state);
                    return;
                }
            }

            if (enemyData.eEnemyController == EnemyController.AI && !EnemyManager.IsAttackPlayer(enemyData))
            {
                ChangeState(new EnemyChaseState(enemyData));
            }
            else if (enemyData.minAttackCount > 0 && enemyData.maxAttackCount > 0 && (enemyAttackCount > Random.Range(enemyData.minAttackCount, enemyData.maxAttackCount)))
            {
                enemyData.isNoAttack = true;

                Util.DelayFunc(() =>
                {
                    enemyData.isNoAttack = false;
                }, enemyData.noAttackTime);

                ChangeState(new EnemyChaseState(enemyData));
            }
        }
    }

    public partial class BossSpecialAttack1Status : EnemyState
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else if(isEnd)
            {
                boss.SpecialAttack1End();
                ChangeState(new EnemyChaseState(enemyData));
            }
        }
    }

    public partial class EnemyPlayerControllerAttackState : EnemyState
    {
        protected override void StateChangeCondition()
        {
            if (AnyStateChangeState()) { }
            else
            {
                ChangeState(new EnemyMoveState(enemyData));
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
            else if (enemyData.eEnemyController == EnemyController.PLAYER && enemyData.isPlayerAttack)
            {
                enemyData.isPlayerAttack = false;
                ChangeState(new EnemyPlayerControllerAttackState(enemyData));
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
            else if (!enemyData.isNoStun && enemyData.stunTime > 0 && stateName != eState.STUN)
            {
                ChangeState(new EnemyStunStatus(enemyData));
            }
            else if (enemyData.isAttack)
            {
                enemyData.isAttack = false;

                ChangeState(new EnemyAIAttackState(enemyData));
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}