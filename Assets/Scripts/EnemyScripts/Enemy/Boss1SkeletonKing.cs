using UnityEngine;

namespace Enemy
{
    public class Boss1SkeletonKing : Enemy
    {
        public Transform movePivot;

        private EnemyCommand attackMoveCommand;
        private EnemyCommand rushAttackCommand;

        private readonly int hashAttack1 = Animator.StringToHash("attack");
        private readonly int hashAttack2 = Animator.StringToHash("attack2");

        protected override void Start()
        {
            attackMoveCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 15f, 0f, false);
            rushAttackCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 30f, 0f, false); // TODO

            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            enemyData.attackDelay = 1.8f;
            enemyData.isAttackPlayerDistance = 3.5f;
            enemyData.attackPower = 30;
            enemyData.maxHP = 500;
            enemyData.hp = 500;

            enemyData.enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 5f, 0f, false);
            enemyData.enemySpriteRotateCommand = new EnemySpriteFlipCommand(enemyData);
            enemyData.addAIAttackStateChangeCondition = attackStateChangeCondition;
        }

        public void AttackMove() // �ִϸ��̼ǿ��� ����
        {
            rb.velocity = Vector2.zero;
            enemyAttackCheck.AttackObjectReset();

            attackMoveCommand.Execute();
            enemyData.enemySpriteRotateCommand.Execute();
        }

        public void RushAttack() // �ִϸ��̼ǿ��� ����
        {
            rb.velocity = Vector2.zero;
            enemyAttackCheck.AttackObjectReset();

            rushAttackCommand.Execute();
            enemyData.enemySpriteRotateCommand.Execute();
        }

        public EnemyState attackStateChangeCondition() // �̺�Ʈ ������ ����
        {
            if (enemyData.animationDictionary[EnemyAnimationType.Attack] == hashAttack1)
            {
                if (Random.Range(0, 10) < 6)
                {
                    enemyData.animationDictionary[EnemyAnimationType.Attack] = hashAttack2;
                    return new EnemyAttackState(enemyData);
                }

                return null;
            }
            else
            {
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashAttack1;
                return new EnemyChaseState(enemyData);
            }
            
        }
    }
}