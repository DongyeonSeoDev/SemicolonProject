using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Enemy
{
    public class Boss1SkeletonKing : Enemy
    {
        public List<float> specialAttack3HPPercent = new List<float>();
        public Transform movePivot;
        public PlayableDirector playableDirector;

        public int fireCount = 0;
        public int maxAttackCount = 0;
        public float specialAttackTime = 6f;
        public float bossAttackMoveSpeed = 0f;
        public float fireDistance = 0f;
        public float fireSpawnTime = 0f;
        public float targetMoveSpeed = 0f;
        public float attackSpeedUpPercent = 0f;
        public float startSpeed = 5f;
        public float speedUpValue = 0.5f;
        public float speedUpTime = 1f;
        public float fireDamage = 0f;

        [Header("Distance")]
        public float fireLimitDistance = 1.5f;
        public float fireLimitSpawnDistance = 0.5f;

        [HideInInspector] public Vector2 limitMinPosition;
        [HideInInspector] public Vector2 limitMaxPosition;
        [HideInInspector] public Vector2 limitMinFirePosition;
        [HideInInspector] public Vector2 limitMaxFirePosition;
        [HideInInspector] public LayerMask whatIsWall;
        [HideInInspector] public float currentSpeed = 0f;

        private EnemyCommand enemyMoveCommand;
        private EnemyCommand enemySpecialAttackMoveCommand;
        private EnemyCommand attackMoveCommand;
        private EnemyCommand rushAttackReadyCommand;
        private EnemyCommand rushAttackCommand;
        private WaitForSeconds fireSpawnTimeSeconds;
        private WaitForSeconds fireSpawnTimeSeconds2 = new WaitForSeconds(0.1f);

        private BossCanvas bossHPBar;

        private List<float> specialAttack3Check = new List<float>();
        private int attackCount = 0;
        private float currentTime = 0f;
        private float currentMoveTime = 0f;
        private bool isAttack = false;
        private bool isSpecialAttack1 = false;
        private bool isSpecialAttack3 = false;
        private bool isSpeedUpTime = true;

        public readonly int hashSpecialAttack1 = Animator.StringToHash("specialAttack1");
        public readonly int hashSpecialAttack1End = Animator.StringToHash("specialAttack1End");
        public readonly int hashAttackEnd = Animator.StringToHash("attackEnd");

        private readonly int hashMove = Animator.StringToHash("move");
        private readonly int hashAttack1 = Animator.StringToHash("attack");
        private readonly int hashAttack2 = Animator.StringToHash("attack2");
        private readonly int hashSpecialAttack2 = Animator.StringToHash("specialAttack2");
        private readonly int hashSpecialAttack3 = Animator.StringToHash("specialAttack3");

        protected override void Awake()
        {
            base.Awake();

            fireSpawnTimeSeconds = new WaitForSeconds(fireSpawnTime);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            bossHPBar = GetComponentInChildren<BossCanvas>();
            bossHPBar.Init(enemyData);

            enemyData.enemyCanvas = bossHPBar.gameObject;

            enemyData.attackDelay = 1.8f;
            enemyData.isAttackPlayerDistance = 4f;
            enemyData.isNoKnockback = true;
            enemyData.isNoStun = true;

            enemyMoveCommand = new BossMoveCommand(enemyData, movePivot, rb, this);
            enemySpecialAttackMoveCommand = new EnemyTargetMoveCommand(enemyData, targetMoveSpeed);
            attackMoveCommand = new EnemyTargetMoveCommand(enemyData, bossAttackMoveSpeed);
            rushAttackReadyCommand = new BossRushAttackCommand(enemyData, movePivot, rb, -1f, false);
            rushAttackCommand = new BossRushAttackCommand(enemyData, movePivot, rb, 50f, true);

            enemyData.enemyMoveCommand = enemyMoveCommand;
            enemyData.enemySpriteRotateCommand = new EnemySpriteRotateCommand(enemyData);
            enemyData.attackTypeCheckCondition = SpecialAttackCheck;
            enemyData.addAIAttackStateChangeCondition = AttackStateChangeCondition;
            enemyData.addChangeAttackCondition = ChangeAttackCondition;

            currentTime = 0f;
            attackCount = 0;

            isAttack = false;
            isSpecialAttack1 = false;
            isSpecialAttack3 = false;

            whatIsWall = LayerMask.GetMask("NOTMOVEZONE");

            #region SetLimitPosition

            limitMaxFirePosition.y = CheckPosition(Vector2.up).y - fireLimitSpawnDistance;
            limitMinFirePosition.y = CheckPosition(Vector2.down).y + fireLimitSpawnDistance;
            limitMaxFirePosition.x = CheckPosition(Vector2.right).x - fireLimitSpawnDistance;
            limitMinFirePosition.x = CheckPosition(Vector2.left).x + fireLimitSpawnDistance;

            limitMaxPosition.y = limitMaxFirePosition.y - 0.9f;
            limitMinPosition.y = limitMinFirePosition.y + 1.42f;
            limitMaxPosition.x = limitMaxFirePosition.x - 2f;
            limitMinPosition.x = limitMinFirePosition.x + 2f;

            #endregion

            specialAttack3Check.Clear();

            for (int i = 0; i < specialAttack3HPPercent.Count; i++)
            {
                specialAttack3Check.Add(specialAttack3HPPercent[i]);
            }

            specialAttack3Check.Sort((x, y) => y.CompareTo(x));

            EventManager.StartListening("PlayerDead", StopAttack);
            EventManager.StartListening("BossDead", StopAttack);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            EventManager.StopListening("PlayerDead", StopAttack);
            EventManager.StopListening("BossDead", StopAttack);
        }

        protected override void SetHP(bool useTween)
        {
            if (bossHPBar != null)
            {
                bossHPBar.SetFill();
            }

            if (enemyData.hp <= 0)
            {
                EventManager.TriggerEvent("BossDead");
            }
        }

        public override void MoveEnemy()
        {
            EventManager.TriggerEvent("StartCutScene");

            playableDirector.Play();
        }

        public void EndCutScene()
        {
            EventManager.TriggerEvent("EndCutScene");

            SpeedReset();

            base.MoveEnemy();
        }

        private Vector2 CheckPosition(Vector2 direction)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 100, whatIsWall);

            if (hit.collider == null)
            {
                return Vector2.zero;
            }

            return hit.point;
        }

        protected override void Update()
        {
            base.Update();

            if (isStop || !enemyData.isEnemyMove)
            {
                return;
            }

            if (attackCount >= maxAttackCount)
            {
                isSpecialAttack1 = true;
                attackCount = 0;
            }

            if (!isAttack)
            {
                currentTime += Time.deltaTime;
            }

            if (isSpeedUpTime)
            {
                currentMoveTime += Time.deltaTime;

                if (currentMoveTime >= speedUpTime)
                {
                    currentMoveTime = 0f;
                    currentSpeed += speedUpValue;
                }
            }
        }

        public void StopAttack() // EventManager���� ���� - �� ���� ����
        {
            StopAllCoroutines();

            bossHPBar.SetActiveHPBar(false);
        }

        public void AttackReady() // �ִϸ��̼ǿ��� ���� - �����ϱ��� �̵�
        {
            rb.velocity = Vector2.zero;
            attackCount++;

            SpeedReset();

            rushAttackReadyCommand.Execute();
            enemyData.enemySpriteRotateCommand.Execute();
        }

        public void SetAttackMovePosition() // �ִϸ��̼ǿ��� ���� - ������ ��ġ ����
        {
            enemyData.moveVector = (EnemyManager.Player.transform.position - transform.position).normalized;
        }

        public void AttackMove() // �ִϸ��̼ǿ��� ���� - �����ϸ鼭 �����̴� �ڵ�
        {
            rb.velocity = Vector2.zero;
            attackCount++;

            SpeedReset();

            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }

            attackMoveCommand.Execute();
            enemyData.enemySpriteRotateCommand.Execute();
        }

        public void RushAttack() // �ִϸ��̼ǿ��� ���� - ���� ����
        {
            rb.velocity = Vector2.zero;

            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }

            rushAttackCommand.Execute();
            enemyData.enemySpriteRotateCommand.Execute();
        }

        private void SpecialAttack1() // Ư�� ���� ����
        {
            isSpecialAttack1 = false;
            isAttack = true;

            enemyData.moveVector = Vector2.left;

            enemyData.animationDictionary[EnemyAnimationType.Move] = hashSpecialAttack1;
            enemyData.enemyMoveCommand = enemySpecialAttackMoveCommand;
            enemyData.enemyChaseStateChangeCondition = SpecialAttack1ChangeCondition;

            for (int i = 0; i < enemyAttackCheck.Length; i++)
            {
                enemyAttackCheck[i].AttackObjectReset();
            }

            EnemyManager.AnimatorSet(enemyData.animationDictionary, EnemyAnimationType.AttackEnd, enemyData.enemyAnimator, TriggerType.ResetTrigger);
        }

        private EnemyState SpecialAttack1ChangeCondition() // Ư�� ���� 1 �ߵ� ����
        { 
            if ((transform.position.x - limitMinPosition.x) <= 0.1f)
            {
                return new BossSpecialAttack1Status(enemyData, this);
            }

            return null;
        }

        public void SpecialAttack1End() // Ư�� ���� 1 ����
        {
            isAttack = false;

            enemyData.animationDictionary[EnemyAnimationType.Move] = hashMove;
            enemyData.enemyMoveCommand = enemyMoveCommand;
            enemyData.enemyChaseStateChangeCondition = null;

            SpeedReset();
        }

        public void SpecialAttack2Start() // �ִϸ��̼ǿ��� ���� - Ư������2 ����
        {
            isSpeedUpTime = false;

            StartCoroutine(SpecialAttack2());
        }

        private IEnumerator SpecialAttack2() // Ư������2 �ڷ�ƾ
        {
            List<Fire> fireList = new List<Fire>();
            Vector3 playerPosition = EnemyManager.Player.transform.position;

            playerPosition.x = playerPosition.x < limitMinFirePosition.x + fireLimitDistance ? limitMinFirePosition.x + fireLimitDistance : playerPosition.x;
            playerPosition.x = playerPosition.x > limitMaxFirePosition.x - fireLimitDistance ? limitMaxFirePosition.x - fireLimitDistance : playerPosition.x;
            playerPosition.y = playerPosition.y < limitMinFirePosition.y + fireLimitDistance ? limitMinFirePosition.y + fireLimitDistance : playerPosition.y;
            playerPosition.y = playerPosition.y > limitMaxFirePosition.y - fireLimitDistance ? limitMaxFirePosition.y - fireLimitDistance : playerPosition.y;

            Fire.checkAttackObjectTogether.Clear();
            attackCount++;

            for (int i = 0; i < fireCount - 1; i++)
            {
                Fire fire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, AnglePosition(playerPosition, (360 / (fireCount - 1)) * i)).GetComponent<Fire>();
                fire.Spawn(this, enemyData.eEnemyController, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.criticalDamagePercent, - 1f, true);

                fireList.Add(fire);

                yield return fireSpawnTimeSeconds;
            }

            Fire playerAttackFire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, playerPosition).GetComponent<Fire>();
            playerAttackFire.Spawn(this, enemyData.eEnemyController, enemyData.minAttackPower, enemyData.maxAttackPower, enemyData.randomCritical, enemyData.criticalDamagePercent, -1f, true);

            fireList.Add(playerAttackFire);

            yield return fireSpawnTimeSeconds;

            for (int i  = 0; i < fireList.Count; i++)
            {
                fireList[i].Attack();
            }
        }

        private Vector3 AnglePosition(Vector3 startPosition, float angle) // ������ ������ �÷��̾� ��ġ���� ������ŭ�� ��ġ�� �˷��ִ� �Լ�
        {
            Vector3 position = Vector3.zero;

            position.x = Mathf.Sin(angle * Mathf.Deg2Rad) * fireDistance;
            position.y = Mathf.Cos(angle * Mathf.Deg2Rad) * fireDistance;

            return startPosition + position;
        }

        public void SpecialAttack3Start() // �ִϸ��̼ǿ��� ���� - Ư������3 ����
        {
            StartCoroutine(SpecialAttack3());
        }

        private IEnumerator SpecialAttack3() // Ư������3 �ڷ�ƾ
        {
            isSpecialAttack3 = true;
            attackCount++;

            SpeedReset();

            isSpeedUpTime = false;

            for (int i = 0; i < 150; i++)
            {
                Fire fire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, RandomPosition()).GetComponent<Fire>();
                fire.Spawn(this, enemyData.eEnemyController, enemyData.minAttackPower + fireDamage, enemyData.maxAttackPower + fireDamage, enemyData.randomCritical, enemyData.criticalDamagePercent, 1f, false);

                yield return fireSpawnTimeSeconds2;
            }

            isSpecialAttack3 = false;
            currentTime = 0f;
        }

        public Vector2 RandomPosition()
        {
            Vector2 randomPosition = Vector2.zero;

            randomPosition.x = Random.Range(limitMinFirePosition.x, limitMaxFirePosition.x);
            randomPosition.y = Random.Range(limitMinFirePosition.y, limitMaxFirePosition.y);

            return randomPosition;
        }

        public void SpecialAttackEnd() // �ִϸ��̼ǿ��� ���� - Ư������2 ����
        {
            isAttack = false;
            isSpeedUpTime = true;
        }

        public void SpecialAttackCheck() // �̺�Ʈ ������ ���� - Ư������ ��� Ȯ��
        {
            if (!isSpecialAttack3 && specialAttack3Check.Count > 0 && specialAttack3Check[0] >= EnemyHpPercent())
            {
                specialAttack3Check.RemoveAt(0);
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashSpecialAttack3;
                currentTime = 0;
                enemyData.attackDelay = 4f;
                isAttack = true;
            }
            else if (!isSpecialAttack3 && currentTime >= specialAttackTime)
            {
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashSpecialAttack2;
                isAttack = true;
                currentTime = 0f;
                enemyData.attackDelay = 2.1f;
            }
            else
            {
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashAttack1;

                if (EnemyHpPercent() <= attackSpeedUpPercent)
                {
                    enemyData.attackDelay = 1.3f;
                }
                else
                {
                    enemyData.attackDelay = 1.8f;
                }
            }
        }

        public EnemyState AttackStateChangeCondition() // �̺�Ʈ ������ ���� - ����2 ��� ���� Ȯ��
        {
            if (enemyData.animationDictionary[EnemyAnimationType.Attack] == hashAttack1)
            {
                int random = Random.Range(0, 10);

                if (attackCount + 1 >= maxAttackCount)
                {
                    attackCount = maxAttackCount;
                }
                else if (random < 6)
                {
                    enemyData.animationDictionary[EnemyAnimationType.Attack] = hashAttack2;

                    return new EnemyAIAttackState(enemyData);
                }
                else if (random < 8)
                {
                    isSpecialAttack1 = true;
                    attackCount = attackCount - 3 < 0 ? 0 : attackCount - 3;
                }

                return new EnemyChaseState(enemyData);
            }
            else
            {
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashAttack1;

                return new EnemyChaseState(enemyData);
            }
        }

        public EnemyState ChangeAttackCondition() // �̺�Ʈ ������ ���� - ������ �ؾ��ϴ��� Ȯ��
        {
            if (!isSpecialAttack3 && (currentTime >= specialAttackTime || (specialAttack3Check.Count > 0 && specialAttack3Check[0] >= EnemyHpPercent())))
            {
                SpecialAttackCheck();

                return new EnemyAIAttackState(enemyData);
            }

            if (!isSpecialAttack3 && isSpecialAttack1)
            {
                SpecialAttack1();

                return new EnemyChaseState(enemyData);
            }

            return null;
        }

        public override void SetColor(float time) { }
        private void SpeedReset() => currentSpeed = startSpeed;
    }
}