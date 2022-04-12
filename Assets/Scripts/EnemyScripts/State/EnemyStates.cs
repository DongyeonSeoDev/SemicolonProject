using UnityEngine;

namespace Enemy
{
    public partial class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyData enemyData) : base(eState.IDLE, enemyData) { }
    }

    public partial class EnemyMoveState : EnemyState // ������ ����
    {
        public EnemyMoveState(EnemyData enemyData) : base(eState.MOVE, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Move, enemyData.enemyAnimator, TriggerType.SetTrigger);
            base.Start();
        }

        protected override void Update()
        {
            enemyData.playerControllerMove.Execute();

            if (enemyData.enemySpriteRotateCommand != null)
            {
                enemyData.enemySpriteRotateCommand.Execute();
            }

            base.Update();
        }

        protected override void End()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Move, enemyData.enemyAnimator, TriggerType.ResetTrigger);
            enemyData.isPlayerControllerMove = false;
        }
    }

    public partial class EnemyChaseState : EnemyState // �߰� ����
    {
        public EnemyChaseState(EnemyData enemyData) : base(eState.CHASE, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Move, enemyData.enemyAnimator, TriggerType.SetTrigger);
            base.Start();
        }

        protected override void Update()
        {
            enemyData.enemyMoveCommand.Execute();

            if (enemyData.enemySpriteRotateCommand != null)
            {
                enemyData.enemySpriteRotateCommand.Execute();
            }

            base.Update();
        }

        protected override void End()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Move, enemyData.enemyAnimator, TriggerType.ResetTrigger);
        }
    }

    public partial class EnemyAIAttackState : EnemyState // �� ���� ����
    {
        private float currentTime;
        private bool isDelay = false;

        public EnemyAIAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.ResetTrigger);

            if (enemyData.isUseDelay) // ���� ��Ÿ�� Ȯ��
            {
                isDelay = EnemyManager.IsAttackDelay(enemyData);
            }

            if (!isDelay)
            {
                EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.SetTrigger);
            }

            EnemyManager.SpriteFlipCheck(enemyData);
            currentTime = 0f;

            base.Start();
        }

        protected override void Update()
        {
            if (!isDelay) // ���� �ð� Ȯ��
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                isDelay = EnemyManager.IsAttackDelay(enemyData);

                if (!isDelay)
                {
                    EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.SetTrigger);
                    EnemyManager.SpriteFlipCheck(enemyData);
                }
            }

            if (currentTime >= enemyData.attackDelay) // ���� ����
            {
                currentTime = 0f;

                EnemyManager.SpriteFlipCheck(enemyData);

                base.Update();
            }

            AlwaysCheckStateChangeCondition();
        }

        protected override void End()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.ResetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.SetTrigger);
        }
    }

    public partial class BossSpecialAttack1Status : EnemyState
    {
        private Boss1SkeletonKing boss = null;
        private Boss1Clone[] bossCloneArray = new Boss1Clone[2];
        private RushAttackRange[] attackRangeArray = new RushAttackRange[3];

        private int moveCount = 0;

        private bool isEnd = false;
        private bool isActive = false; 
        private bool isMove = false;

        public BossSpecialAttack1Status(EnemyData enemyData, Boss1SkeletonKing boss) : base(eState.ATTACK, enemyData) => this.boss = boss;

        protected override void Start()
        {
            moveCount = 0;
            isEnd = false;

            base.Start();
        }

        protected override void Update()
        {
            if (!isActive)
            {
                isActive = true;

                Util.DelayFunc(() =>
                {
                    for (int i = 0; i < boss.enemyAttackCheck.Length; i++)
                    {
                        boss.enemyAttackCheck[i].AttackObjectReset();
                    }

                    enemyData.enemySpriteRenderer.enabled = true;
                    enemyData.moveVector = (new Vector3(12f, enemyData.enemyObject.transform.position.y) - enemyData.enemyObject.transform.position).normalized;

                    bossCloneArray[0] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, new Vector2(boss.transform.position.x, boss.transform.position.y + 3f));
                    bossCloneArray[1] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, new Vector2(boss.transform.position.x, boss.transform.position.y - 3f));

                    attackRangeArray[0] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, boss.transform.position.y));
                    attackRangeArray[1] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossCloneArray[0].transform.position.y));
                    attackRangeArray[2] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossCloneArray[1].transform.position.y));

                    enemyData.enemySpriteRotateCommand.Execute();
                    isMove = true;
                }, 2f);
            }

            if (isMove)
            {
                enemyData.enemyRigidbody2D.velocity = enemyData.moveVector * 20f;
                bossCloneArray[0].rigid.velocity = enemyData.moveVector * 20f;
                bossCloneArray[1].rigid.velocity = enemyData.moveVector * 20f;

                if ((moveCount % 2 == 0 && enemyData.enemyObject.transform.position.x >= boss.limitMaxPosition.x) || (moveCount % 2 == 1 && enemyData.enemyObject.transform.position.x <= boss.limitMinPosition.x))
                {
                    isMove = false;
                    enemyData.enemySpriteRenderer.enabled = false;

                    bossCloneArray[0].gameObject.SetActive(false);
                    bossCloneArray[1].gameObject.SetActive(false);

                    attackRangeArray[0].gameObject.SetActive(false);
                    attackRangeArray[1].gameObject.SetActive(false);
                    attackRangeArray[2].gameObject.SetActive(false);
                }

                if (!isMove)
                {
                    Util.DelayFunc(() =>
                    {
                        moveCount++;

                        for (int i = 0; i < boss.enemyAttackCheck.Length; i++)
                        {
                            boss.enemyAttackCheck[i].AttackObjectReset();
                        }

                        if (moveCount == 1)
                        {
                            // ���� �̵�
                            enemyData.moveVector = (new Vector3(boss.limitMinPosition.x, enemyData.enemyObject.transform.position.y) - enemyData.enemyObject.transform.position).normalized;

                            bossCloneArray[0] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, new Vector2(boss.transform.position.x, boss.transform.position.y + 3f));
                            bossCloneArray[1] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, new Vector2(boss.transform.position.x, boss.transform.position.y - 3f));

                            attackRangeArray[0] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, boss.transform.position.y));
                            attackRangeArray[1] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossCloneArray[0].transform.position.y));
                            attackRangeArray[2] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossCloneArray[1].transform.position.y));
                        }
                        else if (moveCount == 2)
                        {
                            // ������ �̵�
                            enemyData.moveVector = (new Vector3(boss.limitMaxPosition.x, enemyData.enemyObject.transform.position.y) - enemyData.enemyObject.transform.position).normalized;

                            bossCloneArray[0] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, new Vector2(boss.transform.position.x, boss.transform.position.y + 3f));
                            bossCloneArray[1] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, new Vector2(boss.transform.position.x, boss.transform.position.y - 3f));

                            attackRangeArray[0] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, boss.transform.position.y));
                            attackRangeArray[1] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossCloneArray[0].transform.position.y));
                            attackRangeArray[2] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossCloneArray[1].transform.position.y));
                        }
                        else
                        {
                            // ����
                            isEnd = true;
                        }

                        isMove = true;
                        enemyData.enemySpriteRenderer.enabled = true;

                        enemyData.enemySpriteRotateCommand.Execute();
                    }, 3f);
                }
            }

            base.Update();
        }

        protected override void End()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.ResetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.SetTrigger);

            base.End();
        }
    }

    public partial class EnemyPlayerControllerAttackState : EnemyState // ������ ���� �� ���� ����
    {
        private bool isNoAttack = false;

        public EnemyPlayerControllerAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.ResetTrigger);

            if (SlimeGameManager.Instance.CurrentSkillDelayTimer[0] <= 0) // ���� ��Ÿ�� Ȯ��
            {
                SlimeGameManager.Instance.SetSkillDelay(0, enemyData.playerAnimationDelay + enemyData.playerAnimationTime);
                SlimeGameManager.Instance.CurrentSkillDelayTimer[0] = SlimeGameManager.Instance.SkillDelays[0];

                EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.SetTrigger);
                enemyData.enemyAnimator.speed = 1.2f;

                EnemyManager.SpriteFlipCheck(enemyData);
            }
            else
            {
                isNoAttack = true;
            }

            base.Start();
        }

        protected override void Update()
        {
            if (isNoAttack)
            {
                base.Update();
            }

            if (SlimeGameManager.Instance.CurrentSkillDelayTimer[0] <= enemyData.playerAnimationDelay) // ���� ����
            {
                EnemyManager.SpriteFlipCheck(enemyData);
                base.Update();
            }

            AlwaysCheckStateChangeCondition();
        }

        protected override void End()
        {
            enemyData.enemyAnimator.speed = 1.0f;

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.ResetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.SetTrigger);
        }
    }

    public partial class EnemyStunStatus : EnemyState
    {
        public EnemyStunStatus(EnemyData enemyData) : base(eState.STUN, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Hit, enemyData.enemyAnimator, TriggerType.SetTrigger);

            base.Start();
        }

        protected override void Update()
        {
            enemyData.stunTime -= Time.deltaTime;

            if (enemyData.stunTime <= 0)
            {
                enemyData.stunTime = 0;

                base.Update();
            }

            AlwaysCheckStateChangeCondition();
        }

        protected override void End() => EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Hit, enemyData.enemyAnimator, TriggerType.ResetTrigger);
    }

    public partial class EnemyDeadState : EnemyState // �׾�����
    {
        private EnemyCommand deadCommand;

        private float currentTime = 0f;
        private float deadTime = 1f;

        public EnemyDeadState(EnemyData enemyData) : base(eState.DEAD, enemyData)
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                deadCommand = new EnemyDeadAIControllerCommand(enemyData.enemyObject, enemyData.enemyLootList, enemyData.enemyDeadEffectColor);
            }
        }

        protected override void Start()
        {
            enemyData.enemyRigidbody2D.velocity = Vector2.zero;

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Reset, enemyData.enemyAnimator, TriggerType.ResetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Die, enemyData.enemyAnimator, TriggerType.SetTrigger);

            currentTime = 0f;

            enemyData.enemyObject.layer = LayerMask.NameToLayer("ENEMYDEAD");

            base.Start();
        }

        protected override void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime > deadTime)
            {
                base.Update();
            }
        }

        protected override void End()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Die, enemyData.enemyAnimator, TriggerType.ResetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.IsDead, enemyData.enemyAnimator, true);

            deadCommand.Execute();
        }
    }
}