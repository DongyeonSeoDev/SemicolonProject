using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Boss1SkeletonKing : Enemy
    {
        public List<float> specialAttack3HPPercent = new List<float>();
        public Transform movePivot;
        public float specialAttackTime = 6f;
        public int fireCount = 0;
        public int maxAttackCount = 0;
        public float bossMoveSpeed = 0f;
        public float fireDistance = 0f;
        public float fireSpawnTime = 0f;
        public float targetMoveSpeed = 0f;
        public float attackSpeedUpPercent = 0f;

        private EnemyCommand enemyMoveCommand;
        private EnemyCommand enemySpecialAttackMoveCommand;
        private EnemyCommand attackMoveCommand;
        private EnemyCommand rushAttackReadyCommand;
        private EnemyCommand rushAttackCommand;
        private WaitForSeconds fireSpawnTimeSeconds;
        private WaitForSeconds fireSpawnTimeSeconds2 = new WaitForSeconds(0.1f);

        public Vector2 limitMinPosition;
        public Vector2 limitMaxPosition;
        public LayerMask whatIsWall;

        private BossHPBar bossHPBar;

        private List<float> specialAttack3Check = new List<float>();
        private int attackCount = 0;
        private float currentTime = 0f;
        private bool isAttack = false;
        private bool isSpecialAttack1 = false;
        private bool isSpecialAttack3 = false;

        private readonly int hashMove = Animator.StringToHash("move");
        private readonly int hashAttack1 = Animator.StringToHash("attack");
        private readonly int hashAttack2 = Animator.StringToHash("attack2");
        public readonly int hashSpecialAttack1 = Animator.StringToHash("specialAttack1");
        public readonly int hashSpecialAttack1End = Animator.StringToHash("specialAttack1End");
        private readonly int hashSpecialAttack2 = Animator.StringToHash("specialAttack2");
        private readonly int hashSpecialAttack3 = Animator.StringToHash("specialAttack3");

        protected override void Awake()
        {
            base.Awake();

            fireSpawnTimeSeconds = new WaitForSeconds(fireSpawnTime);
        }

        protected override void OnEnable()
        {
            bossHPBar = FindObjectOfType<BossHPBar>();
            bossHPBar.transform.GetChild(0).gameObject.SetActive(true);
            hpBarFillImage = bossHPBar.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            hpBarFillImage.fillAmount = 1;

            base.OnEnable();

            enemyData.attackDelay = 1.8f;
            enemyData.isAttackPlayerDistance = 3.5f;
            enemyData.attackPower = 30;
            enemyData.maxHP = 500;
            enemyData.hp = 500;
            enemyData.isNoKnockback = true;
            enemyData.isNoStun = true;

            enemyMoveCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 5f, 0f, false);
            enemySpecialAttackMoveCommand = new EnemyTargetMoveCommand(enemyData, targetMoveSpeed);
            attackMoveCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, bossMoveSpeed, 0f, false);
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

            whatIsWall = LayerMask.GetMask("WALL");

            limitMaxPosition.y = CheckPosition(Vector2.up).y - 3f;
            limitMinPosition.y = CheckPosition(Vector2.down).y + 3f;
            limitMaxPosition.x = CheckPosition(Vector2.right).x - 3f;
            limitMinPosition.x = CheckPosition(Vector2.left).x + 3f;

            specialAttack3Check.Clear();

            for (int i = 0; i < specialAttack3HPPercent.Count; i++)
            {
                specialAttack3Check.Add(specialAttack3HPPercent[i]);
            }

            specialAttack3Check.Sort((x, y) => y.CompareTo(x));

            EventManager.StartListening("PlayerDead", StopAttack);
        }

        protected override void OnDisable()
        {
            bossHPBar.transform.GetChild(0).gameObject.SetActive(false);
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

        private void OnDestroy()
        {
            EventManager.StopListening("PlayerDead", StopAttack);
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
        }

        public void StopAttack() // EventManager���� ���� - �� ���� ����
        {
            StopAllCoroutines();
        }

        public void AttackReady() // �ִϸ��̼ǿ��� ���� - �����ϱ��� �̵�
        {
            rb.velocity = Vector2.zero;
            attackCount++;

            rushAttackReadyCommand.Execute();
            enemyData.enemySpriteRotateCommand.Execute();
        }

        public void AttackMove() // �ִϸ��̼ǿ��� ���� - �����ϸ鼭 �����̴� �ڵ�
        {
            rb.velocity = Vector2.zero;
            attackCount++;

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

            if (Random.Range(0, 10) < 2)
            {
                isSpecialAttack1 = true;
                attackCount = attackCount - 3 < 0 ? 0 : attackCount - 3;
            }
        }

        private void SpecialAttack1() // Ư�� ���� ����
        {
            isSpecialAttack1 = false;
            isAttack = true;

            enemyData.moveVector = (new Vector3(limitMinPosition.x, transform.position.y, transform.position.z) - transform.position).normalized;

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
                sr.enabled = false;
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
        }

        public void SpecialAttack2Start() // �ִϸ��̼ǿ��� ���� - Ư������2 ����
        {
            StartCoroutine(SpecialAttack2());
        }

        private IEnumerator SpecialAttack2() // Ư������2 �ڷ�ƾ
        {
            List<Fire> fireList = new List<Fire>();
            Vector3 playerPosition = EnemyManager.Player.transform.position;

            Fire.checkAttackObjectTogether.Clear();
            attackCount++;
 
            for (int i = 0; i < fireCount - 1; i++)
            {
                Fire fire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, AnglePosition(playerPosition, (360 / (fireCount - 1)) * i)).GetComponent<Fire>();
                fire.Spawn(this, enemyData.eEnemyController, enemyData.attackPower, -1f, true);

                fireList.Add(fire);

                yield return fireSpawnTimeSeconds;
            }

            Fire playerAttackFire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, playerPosition).GetComponent<Fire>();
            playerAttackFire.Spawn(this, enemyData.eEnemyController, enemyData.attackPower, -1f, true);

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

            for (int i = 0; i < 150; i++)
            {
                Fire fire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, RandomPosition()).GetComponent<Fire>();
                fire.Spawn(this, enemyData.eEnemyController, enemyData.attackPower, 1f, false);

                yield return fireSpawnTimeSeconds2;
            }

            isSpecialAttack3 = false;
            currentTime = 0f;
        }

        public Vector2 RandomPosition()
        {
            Vector2 randomPosition = Vector2.zero;

            randomPosition.x = Random.Range(limitMinPosition.x, limitMaxPosition.x);
            randomPosition.y = Random.Range(limitMinPosition.y, limitMaxPosition.y);

            return randomPosition;
        }

        public void SpecialAttackEnd() // �ִϸ��̼ǿ��� ���� - Ư������2 ����
        {
            isAttack = false;
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
                if (Random.Range(0, 10) < 6)
                {
                    enemyData.animationDictionary[EnemyAnimationType.Attack] = hashAttack2;

                    return new EnemyAIAttackState(enemyData);
                }

                return null;
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
    }
}