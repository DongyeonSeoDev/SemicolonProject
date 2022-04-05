using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Boss1SkeletonKing : Enemy
    {
        public List<float> specialAttack3HPPercent = new List<float>();
        public Transform movePivot;
        public float specialAttackTime = 6f;
        public int fireCount = 0;
        public float fireDistance = 0f;
        public float fireSpawnTime = 0f;
        public Vector2 limitMinPosition;
        public Vector2 limitMaxPosition;

        private EnemyCommand attackMoveCommand;
        private EnemyCommand rushAttackCommand;
        private WaitForSeconds fireSpawnTimeSeconds;
        private WaitForSeconds fireSpawnTimeSeconds2 = new WaitForSeconds(0.1f);

        private List<float> specialAttack3Check = new List<float>();
        private float currentTime = 0f;
        private bool isAttack = false;

        private readonly int hashAttack1 = Animator.StringToHash("attack");
        private readonly int hashAttack2 = Animator.StringToHash("attack2");
        private readonly int hashSpecialAttack2 = Animator.StringToHash("specialAttack2");
        private readonly int hashSpecialAttack3 = Animator.StringToHash("specialAttack3");

        protected override void Awake()
        {
            base.Awake();

            fireSpawnTimeSeconds = new WaitForSeconds(fireSpawnTime);
        }

        protected override void Start()
        {
            attackMoveCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 15f, 0f, false);
            rushAttackCommand = new EnemyFollowPlayerCommand(enemyData, movePivot, rb, 50f, 0f, false);

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
            enemyData.attackTypeCheckCondition = SpecialAttackCheck;
            enemyData.addAIAttackStateChangeCondition = AttackStateChangeCondition;
            enemyData.addChangeAttackCondition = ChangeAttackCondition;

            currentTime = 0f;
            isAttack = false;

            specialAttack3Check.Clear();

            for (int i = 0; i < specialAttack3HPPercent.Count; i++)
            {
                specialAttack3Check.Add(specialAttack3HPPercent[i]);
            }

            specialAttack3Check.Sort((x, y) => y.CompareTo(x));

            EventManager.StartListening("PlayerDead", StopAttack);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            EventManager.StopListening("PlayerDead", StopAttack);
        }

        protected override void Update()
        {
            base.Update();

            if (!isAttack)
            {
                currentTime += Time.deltaTime;
            }
        }

        public void StopAttack() // EventManager���� ���� - �� ���� ����
        {
            StopAllCoroutines();
        }

        public void AttackMove() // �ִϸ��̼ǿ��� ���� - �����ϸ鼭 �����̴� �ڵ�
        {
            rb.velocity = Vector2.zero;

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

        public void SpecialAttack2Start() // �ִϸ��̼ǿ��� ���� - Ư������2 ����
        {
            StartCoroutine(SpecialAttack2());
        }

        private IEnumerator SpecialAttack2() // Ư������2 �ڷ�ƾ
        {
            List<Fire> fireList = new List<Fire>();
            Vector3 playerPosition = EnemyManager.Player.transform.position;

            Fire.checkAttackObjectTogether.Clear();

            for (int i = 0; i < fireCount - 1; i++)
            {
                Fire fire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, anglePosition(playerPosition, (360 / (fireCount - 1)) * i)).GetComponent<Fire>();
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

        private Vector3 anglePosition(Vector3 startPosition, float angle) // ������ ������ �÷��̾� ��ġ���� ������ŭ�� ��ġ�� �˷��ִ� �Լ�
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
            Debug.Log("����");

            for (int i = 0; i < 150; i++)
            {
                Fire fire = EnemyPoolManager.Instance.GetPoolObject(Type.Fire, RandomPosition()).GetComponent<Fire>();
                fire.Spawn(this, enemyData.eEnemyController, enemyData.attackPower, 1f, false);

                yield return fireSpawnTimeSeconds2;
            }
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
            if (specialAttack3Check.Count > 0 && specialAttack3Check[0] >= EnemyHpPercent())
            {
                specialAttack3Check.RemoveAt(0);
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashSpecialAttack3;
                currentTime = 0;
                enemyData.attackDelay = 15f;
                isAttack = true;
            }
            else if (currentTime >= specialAttackTime)
            {
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashSpecialAttack2;
                isAttack = true;
                currentTime = 0f;
                enemyData.attackDelay = 2.1f;
            }
            else
            {
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashAttack1;
                enemyData.attackDelay = 1.8f;
            }
        }

        public EnemyState AttackStateChangeCondition() // �̺�Ʈ ������ ���� - ����2 ��� ���� Ȯ��
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

        public EnemyState ChangeAttackCondition() // �̺�Ʈ ������ ���� - ������ �ؾ��ϴ��� Ȯ��
        {
            if (currentTime >= specialAttackTime || (specialAttack3Check.Count > 0 && specialAttack3Check[0] >= EnemyHpPercent()))
            {
                SpecialAttackCheck();

                return new EnemyAttackState(enemyData);
            }

            return null;
        }
    }
}