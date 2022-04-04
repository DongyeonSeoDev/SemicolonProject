using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Boss1SkeletonKing : Enemy
    {
        public Transform movePivot;
        public float specialAttackTime = 6f;
        public int fireCount = 0;
        public float fireDistance = 0f;
        public float fireSpawnTime = 0f;

        private EnemyCommand attackMoveCommand;
        private EnemyCommand rushAttackCommand;
        private WaitForSeconds fireSpawnTimeSeconds;

        private float currentTime = 0f;

        private readonly int hashAttack1 = Animator.StringToHash("attack");
        private readonly int hashAttack2 = Animator.StringToHash("attack2");
        private readonly int hashSpecialAttack2 = Animator.StringToHash("specialAttack2");

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
        }

        protected override void Update()
        {
            base.Update();

            currentTime += Time.deltaTime;
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

        public void SpecialAttack2End() // �ִϸ��̼ǿ��� ���� - Ư������2 ����
        {
            currentTime = 0;
        }

        public void SpecialAttackCheck() // �̺�Ʈ ������ ���� - Ư������ ��� Ȯ��
        {
            if (currentTime >= specialAttackTime)
            {
                enemyData.animationDictionary[EnemyAnimationType.Attack] = hashSpecialAttack2;
                currentTime = 0;
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
            if (currentTime >= specialAttackTime)
            {
                SpecialAttackCheck();

                return new EnemyAttackState(enemyData);
            }

            return null;
        }
    }
}