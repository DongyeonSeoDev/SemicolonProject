using System.Collections;
using System.Collections.Generic;
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
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Player, enemyData.enemyAnimator, enemyData.isUseIdleAnimation);

            base.Start();
        }

        protected override void Update()
        {
            enemyData.playerControllerMove.Execute();

            if (enemyData.enemySpriteRotateCommand != null)
            {
                enemyData.enemySpriteRotateCommand.Execute();
            }

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Idle, enemyData.enemyAnimator, enemyData.enemyRigidbody2D.velocity.magnitude < 1.2f);

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

            if (enemyData.moveEvent != null)
            {
                enemyData.moveEvent.Invoke();
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
        private bool isUseStartDelay = false;
        private bool isStartDelay = false;
        private int enemyAttackCount = 0;

        public EnemyAIAttackState(EnemyData enemyData) : base(eState.ATTACK, enemyData) { }

        protected override void Start()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.ResetTrigger);

            if (enemyData.startAttackDelay > 0)
            {
                isUseStartDelay = true;
                isStartDelay = true;

                Util.DelayFunc(() =>
                {
                    isStartDelay = false;
                }, enemyData.startAttackDelay);
            }

            if (!isUseStartDelay)
            {
                EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.SetTrigger);
                EnemyManager.SpriteFlipCheck(enemyData);
            }

            currentTime = 0f;
            enemyAttackCount = 0;

            base.Start();
        }

        protected override void Update()
        {
            if (isUseStartDelay && !isStartDelay)
            {
                isUseStartDelay = false;

                EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.SetTrigger);
                EnemyManager.SpriteFlipCheck(enemyData);
            }
            else
            {
                currentTime += Time.deltaTime;
            }

            if (currentTime >= enemyData.attackDelay) // ���� ����
            {
                currentTime = 0f;
                enemyAttackCount++;

                EnemyManager.SpriteFlipCheck(enemyData);

                base.Update();
            }

            AlwaysCheckStateChangeCondition();
        }

        protected override void End()
        {
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.ResetTrigger);
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.SetTrigger);

            if (enemyData.endAttack != null)
            {
                enemyData.endAttack.Invoke();
            }
        }
    }

    public partial class BossSpecialAttack1Status : EnemyState
    {
        private Boss1SkeletonKing boss = null;

        private Boss1Clone[] bossCloneArray;
        private AttackBoss1Clone[] attackBoss1CloneArray;
        private RushAttackRange[] attackRangeArray;
        private Vector2[] bossPositionArray;

        private WaitForSeconds bossAfterImageTime;

        private Color afterImageColor;
        private Vector2 bossPosition;

        private int moveCount = 3;
        private int enemyCount = 4;
        private int randomNum;

        private float moveSpeed = 35f;
        private float attackDistance = 5.8f;
        private float afterImageTime = 0.7f;
        private float spawnAfterImageTime = 0.07f;

        private bool isEnd = false;
        private bool isMove = false;
        private bool isRightAttack = true;
        private bool isAfterImage = false;

        public BossSpecialAttack1Status(EnemyData enemyData, Boss1SkeletonKing boss) : base(eState.ATTACK, enemyData) => this.boss = boss;

        protected override void Start()
        {
            // �迭 ����
            bossCloneArray = new Boss1Clone[enemyCount - 1];
            attackBoss1CloneArray = new AttackBoss1Clone[enemyCount - 1];
            attackRangeArray = new RushAttackRange[enemyCount];
            bossPositionArray = new Vector2[enemyCount];

            // ���� �ʱ�ȭ
            isEnd = false;
            isRightAttack = true;

            BossSpecialAttack1Position();
            GetAttackRangePoolObject();
            SetBossAttack(false);

            Util.DelayFunc(() =>
            {
                AttackObjectReset();
                SetBossAttack(true);
                SetBossCloneData();
            }, 1f);

            bossAfterImageTime = new WaitForSeconds(spawnAfterImageTime);
            afterImageColor = Color.white;
            afterImageColor.a = 0.3f;

            base.Start();
        }

        protected override void Update()
        {
            if (isMove)
            {
                // ���� ������
                enemyData.enemyRigidbody2D.velocity = enemyData.moveVector * moveSpeed;

                // ���� �н� ������
                for (int i = 0; i < enemyCount - 1; i++)
                {
                    bossCloneArray[i].MovePosition(enemyData.moveVector * moveSpeed);
                    attackBoss1CloneArray[i].direction = enemyData.moveVector;
                }

                // ���� �����ߴ��� Ȯ��
                if (isRightAttack ? enemyData.enemyObject.transform.position.x >= boss.limitMaxPosition.x : enemyData.enemyObject.transform.position.x <= boss.limitMinPosition.x)
                {
                    SetBossAttack(false);

                    // ���� �н� ����
                    for (int i = 0; i < enemyCount - 1; i++)
                    {
                        bossCloneArray[i].gameObject.SetActive(false);
                    }
                }

                if (!isMove)
                {
                    Util.DelayFunc(() =>
                    {
                        // ������ Ƚ�� ����
                        moveCount--;

                        // ���� ����
                        isRightAttack = !isRightAttack;

                        if (moveCount <= 0) // ����
                        {
                            enemyData.enemyAnimator.ResetTrigger(boss.hashSpecialAttack1End);
                            enemyData.enemyAnimator.SetTrigger(boss.hashAttackEnd);

                            enemyData.moveVector = Vector2.left;

                            if (enemyData.enemySpriteRotateCommand != null)
                            {
                                enemyData.enemySpriteRotateCommand.Execute();
                            }

                            enemyData.enemySpriteRenderer.enabled = true;

                            isEnd = true;
                            isAfterImage = false;

                            base.Update();

                            return;
                        }

                        BossSpecialAttack1Position();
                        GetAttackRangePoolObject();

                        Util.DelayFunc(() =>
                        {
                            AttackObjectReset();
                            SetBossAttack(true);
                            SetBossCloneData();
                        }, 1f);
                    }, 1f);
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

        /// <summary>
        /// ���� ��ȯ ��ġ�� ���ϴ� �Լ�
        /// </summary>
        private void BossSpecialAttack1Position()
        {
            if (Mathf.CeilToInt((EnemyManager.Player.transform.position.y + 9) / 3f) % 2 == 1)
            {
                SetStartUpPosition();
            }
            else
            {
                SetStartDownPosition();
            }
        }

        private void SetStartUpPosition()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                bossPositionArray[i] = new Vector2(boss.transform.position.x, boss.limitMaxPosition.y - (i * attackDistance));
            }

            SetRandomBossPosition();
        }

        private void SetStartDownPosition()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                bossPositionArray[i] = new Vector2(boss.transform.position.x, boss.limitMinPosition.y + (i * attackDistance));
            }

            SetRandomBossPosition();
        }

        private void SetRandomBossPosition()
        {
            randomNum = Random.Range(0, enemyCount);

            bossPosition = bossPositionArray[randomNum];
            bossPositionArray[randomNum] = bossPositionArray[0];
            bossPositionArray[0] = bossPosition;
        }
        
        /// <summary>
        /// ���� ��ġ�� �����ִ� ������Ʈ�� Ǯ���� ����ؼ� ��ȯ
        /// </summary>
        private void GetAttackRangePoolObject()
        {
            for (int i = 0; i < enemyCount; i++)
            {
                attackRangeArray[i] = (RushAttackRange)EnemyPoolManager.Instance.GetPoolObject(Type.EnemyRushAttackRange, new Vector2(0f, bossPositionArray[i].y));
                attackRangeArray[i].AnimationReset();
            }
        }

        /// <summary>
        /// ���� �н��� ��ȯ�ϰ� �����͸� ����
        /// </summary>
        private void SetBossCloneData()
        {
            boss.transform.position = bossPositionArray[0];

            for (int i = 0; i < enemyCount - 1; i++)
            {
                bossCloneArray[i] = (Boss1Clone)EnemyPoolManager.Instance.GetPoolObject(Type.Boss1Clone, bossPositionArray[i + 1]);
                attackBoss1CloneArray[i] = bossCloneArray[i].GetComponentInChildren<AttackBoss1Clone>();
                attackBoss1CloneArray[i].Init(enemyData.eEnemyController, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.criticalDamagePercent, enemyData.criticalDamagePercent);
                attackBoss1CloneArray[i].AttackObjectReset();
            }
        }

        /// <summary>
        /// ���� ���� ������Ʈ�� ����
        /// </summary>
        private void AttackObjectReset()
        {
            for (int i = 0; i < boss.enemyAttackCheck.Length; i++)
            {
                boss.enemyAttackCheck[i].AttackObjectReset();
            }
        }

        /// <summary>
        /// ���� ���� ����
        /// </summary>
        private void SetBossAttack(bool isAttack)
        {
            isMove = isAttack;
            enemyData.enemySpriteRenderer.enabled = isAttack;

            enemyData.enemyAnimator.ResetTrigger(isAttack ? boss.hashSpecialAttack1End : boss.hashSpecialAttack1);
            enemyData.enemyAnimator.SetTrigger(isAttack ? boss.hashSpecialAttack1 : boss.hashSpecialAttack1End);

            isAfterImage = isAttack;

            if (isAttack)
            {
                enemyData.moveVector = isRightAttack ? Vector2.right : Vector2.left;
                enemyData.enemySpriteRotateCommand.Execute();

                // ���� ���� ����
                for (int i = 0; i < enemyCount; i++)
                {
                    attackRangeArray[i].gameObject.SetActive(false);
                }

                boss.StartCoroutine(StartSpawnAfterImage());
            }
        }

        /// <summary>
        /// ���� �ܻ� �ڵ�
        /// </summary>
        private IEnumerator StartSpawnAfterImage()
        {
            while (isAfterImage)
            {
                yield return bossAfterImageTime;

                if (enemyData.enemySpriteRenderer.enabled)
                {
                    EnemyAfterImage afterImage = EnemyPoolManager.Instance.GetPoolObject(Type.EnemyAfterImage, boss.transform.position).GetComponent<EnemyAfterImage>();
                    afterImage.Init(enemyData.enemySpriteRenderer.sprite, afterImageColor, afterImageTime, !isRightAttack);
                }

                for (int i = 0; i < enemyCount - 1; i++)
                {
                    if (bossCloneArray[i].sr.enabled)
                    {
                        EnemyAfterImage bossClonefterImage = EnemyPoolManager.Instance.GetPoolObject(Type.EnemyAfterImage, bossCloneArray[i].transform.position).GetComponent<EnemyAfterImage>();
                        bossClonefterImage.Init(bossCloneArray[i].sr.sprite, afterImageColor, afterImageTime, !isRightAttack);
                    }
                }
            }
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
                SlimeGameManager.Instance.CurrentSkillDelayTimer[0] = SlimeGameManager.Instance.SkillDelays[0];

                EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Attack, enemyData.enemyAnimator, TriggerType.SetTrigger);
                enemyData.enemyAnimator.speed = enemyData.playerAnimationSpeed;

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
            Debug.Log("���� ����!");

            if (isNoAttack)
            {
                base.Update();
            }

            if (SlimeGameManager.Instance.SkillDelays[0] - SlimeGameManager.Instance.CurrentSkillDelayTimer[0] > enemyData.playerAnimationTime) // ���� ����
            {
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

            if (enemyData.currentStunEffect == null)
            {
                enemyData.currentStunEffect = EnemyPoolManager.Instance.GetPoolObject(Type.StunEffect, enemyData.stunEffectPosition).GetComponent<StunEffect>();
                enemyData.currentStunEffect.transform.SetParent(enemyData.enemyObject.transform, false);
            }

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

        protected override void End()
        {
            if (enemyData.currentStunEffect != null)
            {
                enemyData.currentStunEffect.gameObject.SetActive(false);
                enemyData.currentStunEffect.transform.SetParent(EnemyPoolManager.Instance.transform, false);
                enemyData.currentStunEffect = null;
            }
            
            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.Hit, enemyData.enemyAnimator, TriggerType.ResetTrigger);
        }
    }

    public partial class EnemyDeadState : EnemyState // �׾�����
    {
        private float currentTime = 0f;
        private float deadTime = 1f;

        public EnemyDeadState(EnemyData enemyData) : base(eState.DEAD, enemyData) { }

        protected override void Start()
        {
            enemyData.enemyRigidbody2D.velocity = Vector2.zero;

            if (enemyData.deadEvent != null)
            {
                enemyData.deadEvent.Invoke();
            }

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

            EnemyDead();
        }

        private void EnemyDead()
        {
            for (int i = 0; i < enemyData.enemyLootList.Count; i++)
            {
                for (int j = 0; j < enemyData.enemyLootList[i].count; j++)
                {
                    if (CSVEnemyLoot.Instance.itemDictionary.ContainsKey(enemyData.enemyLootList[i].lootName))
                    {
                        Water.PoolManager.GetItem("Item").GetComponent<Item>().SetData(CSVEnemyLoot.Instance.itemDictionary[enemyData.enemyLootList[i].lootName].id, enemyData.enemyObject.transform.position);
                    }
                    else
                    {
                        Debug.LogError(enemyData.enemyLootList[i].lootName + "�� �����ϴ�.");
                    }
                }
            }

            EnemyPoolManager.Instance.GetPoolObject(Type.DeadEffect, enemyData.enemyObject.transform.position).GetComponent<EnemyDeadEffect>().Play(enemyData.enemyDeadEffectColor);
            enemyData.enemyObject.GetComponent<Enemy>().EnemyDestroy();
        }
    }
}