using UnityEngine;

namespace Enemy
{
    public partial class EnemyIdleState : EnemyState
    {
        public EnemyIdleState(EnemyData enemyData) : base(eState.IDLE, enemyData) { }
    }

    public partial class EnemyMoveState : EnemyState // 움직임 상태
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

    public partial class EnemyChaseState : EnemyState // 추격 상태
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

    public partial class EnemyAIAttackState : EnemyState // 적 공격 상태
    {
        private float currentTime;
        private bool isDelay = false;

        public EnemyAIAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.ResetTrigger);

            if (enemyData.isUseDelay) // 공격 쿨타임 확인
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
            if (!isDelay) // 공격 시간 확인
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

            if (currentTime >= enemyData.attackDelay) // 공격 종료
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
        private AttackBoss1Clone[] attackBoss1CloneArray = new AttackBoss1Clone[2];
        private RushAttackRange[] attackRangeArray = new RushAttackRange[3];
        private Vector2[] bossPositionArray = new Vector2[3];

        private int moveCount = 0;

        private bool isEnd = false;
        private bool isMove = false;

        public BossSpecialAttack1Status(EnemyData enemyData, Boss1SkeletonKing boss) : base(eState.ATTACK, enemyData) => this.boss = boss;

        protected override void Start()
        {
            moveCount = 0;
            isEnd = false;

            //
            BossSpecialAttack1Position(ref bossPositionArray);

            attackRangeArray[0] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[0].y));
            attackRangeArray[1] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[1].y));
            attackRangeArray[2] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[2].y));

            enemyData.enemyAnimator.ResetTrigger(boss.hashSpecialAttack1);
            enemyData.enemyAnimator.SetTrigger(boss.hashSpecialAttack1End);

            Util.DelayFunc(() =>
            {
                for (int i = 0; i < boss.enemyAttackCheck.Length; i++)
                {
                    boss.enemyAttackCheck[i].AttackObjectReset();
                }

                enemyData.enemySpriteRenderer.enabled = true;
                enemyData.moveVector = Vector2.right;

                boss.transform.position = bossPositionArray[0];
                bossCloneArray[0] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, bossPositionArray[1]);
                bossCloneArray[1] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, bossPositionArray[2]);
                attackBoss1CloneArray[0] = bossCloneArray[0].GetComponentInChildren<AttackBoss1Clone>();
                attackBoss1CloneArray[1] = bossCloneArray[1].GetComponentInChildren<AttackBoss1Clone>();

                attackBoss1CloneArray[0].Init(enemyData.eEnemyController, enemyData.attackPower);
                attackBoss1CloneArray[1].Init(enemyData.eEnemyController, enemyData.attackPower);
                attackBoss1CloneArray[0].AttackObjectReset();
                attackBoss1CloneArray[1].AttackObjectReset();

                enemyData.enemySpriteRotateCommand.Execute();
                isMove = true;

                enemyData.enemyAnimator.ResetTrigger(boss.hashSpecialAttack1End);
                enemyData.enemyAnimator.SetTrigger(boss.hashSpecialAttack1);
            }, 2f);

            base.Start();
        }

        protected override void Update()
        {
            if (isMove)
            {
                enemyData.enemyRigidbody2D.velocity = enemyData.moveVector * 20f;
                bossCloneArray[0].MovePosition(enemyData.moveVector * 20f);
                bossCloneArray[1].MovePosition(enemyData.moveVector * 20f);

                if ((moveCount % 2 == 0 && enemyData.enemyObject.transform.position.x >= boss.limitMaxPosition.x) || (moveCount % 2 == 1 && enemyData.enemyObject.transform.position.x <= boss.limitMinPosition.x))
                {
                    isMove = false;
                    enemyData.enemySpriteRenderer.enabled = false;

                    bossCloneArray[0].gameObject.SetActive(false);
                    bossCloneArray[1].gameObject.SetActive(false);

                    attackRangeArray[0].gameObject.SetActive(false);
                    attackRangeArray[1].gameObject.SetActive(false);
                    attackRangeArray[2].gameObject.SetActive(false);

                    enemyData.enemyAnimator.ResetTrigger(boss.hashSpecialAttack1);
                    enemyData.enemyAnimator.SetTrigger(boss.hashSpecialAttack1End);
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
                            BossSpecialAttack1Position(ref bossPositionArray);

                            attackRangeArray[0] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[0].y));
                            attackRangeArray[1] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[1].y));
                            attackRangeArray[2] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[2].y));

                            Util.DelayFunc(() => 
                            {
                                // 왼쪽 이동
                                enemyData.moveVector = Vector2.left;
                                
                                boss.transform.position = bossPositionArray[0];
                                bossCloneArray[0] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, bossPositionArray[1]);
                                bossCloneArray[1] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, bossPositionArray[2]);

                                attackBoss1CloneArray[0] = bossCloneArray[0].GetComponentInChildren<AttackBoss1Clone>();
                                attackBoss1CloneArray[1] = bossCloneArray[1].GetComponentInChildren<AttackBoss1Clone>();

                                attackBoss1CloneArray[0].Init(enemyData.eEnemyController, enemyData.attackPower);
                                attackBoss1CloneArray[1].Init(enemyData.eEnemyController, enemyData.attackPower);
                                attackBoss1CloneArray[0].AttackObjectReset();
                                attackBoss1CloneArray[1].AttackObjectReset();

                                isMove = true;
                                enemyData.enemySpriteRenderer.enabled = true;
                                enemyData.enemySpriteRotateCommand.Execute();

                                enemyData.enemyAnimator.ResetTrigger(boss.hashSpecialAttack1End);
                                enemyData.enemyAnimator.SetTrigger(boss.hashSpecialAttack1);
                            }, 2f);
                        }
                        else if (moveCount == 2)
                        {
                            BossSpecialAttack1Position(ref bossPositionArray);

                            attackRangeArray[0] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[0].y));
                            attackRangeArray[1] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[1].y));
                            attackRangeArray[2] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[2].y));

                            Util.DelayFunc(() => 
                            {
                                // 오른쪽 이동
                                enemyData.moveVector = Vector2.right;

                                boss.transform.position = bossPositionArray[0];
                                bossCloneArray[0] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, bossPositionArray[1]);
                                bossCloneArray[1] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, bossPositionArray[2]);

                                attackBoss1CloneArray[0] = bossCloneArray[0].GetComponentInChildren<AttackBoss1Clone>();
                                attackBoss1CloneArray[1] = bossCloneArray[1].GetComponentInChildren<AttackBoss1Clone>();

                                attackBoss1CloneArray[0].Init(enemyData.eEnemyController, enemyData.attackPower);
                                attackBoss1CloneArray[1].Init(enemyData.eEnemyController, enemyData.attackPower);
                                attackBoss1CloneArray[0].AttackObjectReset();
                                attackBoss1CloneArray[1].AttackObjectReset();

                                isMove = true;
                                enemyData.enemySpriteRenderer.enabled = true;
                                enemyData.enemySpriteRotateCommand.Execute();

                                enemyData.enemyAnimator.ResetTrigger(boss.hashSpecialAttack1End);
                                enemyData.enemyAnimator.SetTrigger(boss.hashSpecialAttack1);
                            }, 2f);                            
                        }
                        else
                        {
                            // 종료
                            isEnd = true;
                            isMove = true;
                            enemyData.enemySpriteRenderer.enabled = true;
                            enemyData.enemySpriteRotateCommand.Execute();

                            enemyData.enemyAnimator.ResetTrigger(boss.hashSpecialAttack1End);
                            enemyData.enemyAnimator.SetTrigger(boss.hashSpecialAttack1);
                        }
                    }, 2f);
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

        private void BossSpecialAttack1Position(ref Vector2[] positionArray)
        {
            if (EnemyManager.Player.transform.position.y <= boss.limitMinPosition.y + 5f)
            {
                if (EnemyManager.Player.transform.position.y <= boss.limitMinPosition.y)
                {
                    positionArray[0] = new Vector2(boss.transform.position.x, boss.limitMinPosition.y);
                    positionArray[1] = new Vector2(boss.transform.position.x, boss.limitMinPosition.y + 5f);
                    positionArray[2] = new Vector2(boss.transform.position.x, boss.limitMinPosition.y + 10f);
                }
                else
                {
                    positionArray[0] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y);
                    positionArray[1] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y + 5f);
                    positionArray[2] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y + 10f);
                }
            }
            else if (EnemyManager.Player.transform.position.y >= boss.limitMaxPosition.y - 5f)
            {
                if (EnemyManager.Player.transform.position.y >= boss.limitMaxPosition.y)
                {
                    positionArray[0] = new Vector2(boss.transform.position.x, boss.limitMaxPosition.y);
                    positionArray[1] = new Vector2(boss.transform.position.x, boss.limitMaxPosition.y - 5f);
                    positionArray[2] = new Vector2(boss.transform.position.x, boss.limitMaxPosition.y - 10f);
                }
                else
                {
                    positionArray[0] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y);
                    positionArray[1] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y - 5f);
                    positionArray[2] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y - 10f);
                }
            }
            else
            {
                positionArray[0] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y);
                positionArray[1] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y + 5f);
                positionArray[2] = new Vector2(boss.transform.position.x, EnemyManager.Player.transform.position.y - 5f);
            }
        }
    }

    public partial class EnemyPlayerControllerAttackState : EnemyState // 적으로 변신 후 공격 상태
    {
        private bool isNoAttack = false;

        public EnemyPlayerControllerAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.ResetTrigger);

            if (SlimeGameManager.Instance.CurrentSkillDelayTimer[0] <= 0) // 공격 쿨타임 확인
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

            if (SlimeGameManager.Instance.CurrentSkillDelayTimer[0] <= enemyData.playerAnimationDelay) // 공격 종료
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

    public partial class EnemyDeadState : EnemyState // 죽었을때
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