using UnityEngine;

namespace Enemy
{
    public class EnemyMoveAIControllerCommand : EnemyCommand // �� ������
    {
        private EnemyMoveSO enemyMoveSO;
        private Transform enemyPosition;

        private Vector3 targetPosition;

        public EnemyMoveAIControllerCommand(EnemyMoveSO enemyMoveSO, Transform enemyPosition)
        {
            this.enemyMoveSO = enemyMoveSO;
            this.enemyPosition = enemyPosition;
        }

        public override void Execute()
        {
            // �̵�
            targetPosition = (enemyMoveSO.targetPositions[enemyMoveSO.currentPositionNumber] - enemyPosition.position).normalized;
            targetPosition *= enemyMoveSO.moveSpeed * Time.deltaTime;

            enemyPosition.position += targetPosition;

            // �Ÿ� Ȯ��
            if (Vector3.Distance(enemyPosition.position, enemyMoveSO.targetPositions[enemyMoveSO.currentPositionNumber]) < enemyMoveSO.targetPositionChangeDistance)
            {
                enemyMoveSO.currentPositionNumber = (enemyMoveSO.currentPositionNumber + 1) % enemyMoveSO.targetPositions.Count;
            }
        }
    }

    public class EnemyMovePlayerControllerCommand : EnemyCommand // �� ������
    {
        public override void Execute()
        {
            Debug.Log("�÷��̾� ������ �ڵ� �ۼ�");
        }
    }

    public class EnemyFollowPlayerCommand : EnemyCommand // �� ������
    {
        private Transform enemyObject;
        private Transform followObject;
        private float followSpeed;
        private float followDistance;
        private bool isLongDistanceAttack;

        private Vector3 targetPosition;
        private float angle;

        public EnemyFollowPlayerCommand(Transform enemyObject, Transform followObject, float followSpeed, float followDistance, bool isLongDistanceAttack)
        {
            this.enemyObject = enemyObject;
            this.followObject = followObject;
            this.followSpeed = followSpeed;
            this.followDistance = followDistance;
            this.isLongDistanceAttack = isLongDistanceAttack;
        }

        public override void Execute()
        {
            if (isLongDistanceAttack)
            {
                // �̵�
                targetPosition = enemyObject.transform.position - followObject.transform.position;

                angle = Mathf.Atan2(targetPosition.x, targetPosition.y) * Mathf.Rad2Deg + 90f;

                targetPosition.x = followObject.transform.position.x + (followDistance * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
                targetPosition.y = followObject.transform.position.y + (followDistance * Mathf.Sin(angle * Mathf.Deg2Rad));
                targetPosition.z = followObject.transform.position.z;

                targetPosition = (targetPosition - enemyObject.position).normalized;
                targetPosition *= followSpeed * Time.deltaTime;

                enemyObject.position += targetPosition;
            }
            else
            {
                // �̵�
                targetPosition = (followObject.position - enemyObject.position).normalized;
                targetPosition *= followSpeed * Time.deltaTime;

                enemyObject.position += targetPosition;
            }
        }
    }

    public class EnemyGetDamagedAIControllerCommand : EnemyCommand // ���� �������� ����
    {
        private EnemyData enemyData;

        private bool isWorking = false;

        public EnemyGetDamagedAIControllerCommand(EnemyData enemyData)
        {
            this.enemyData = enemyData;
            isWorking = false;
        }

        public override void Execute()
        {
            if (!isWorking) // ���� ����
            {
                enemyData.enemySpriteRenderer.color = enemyData.damagedColor;
            }
            else // ���� ���� ����
            {
                enemyData.enemySpriteRenderer.color = enemyData.normalColor;
            }

            isWorking = !isWorking;
        }
    }

    public class EnemyGetDamagedPlayerControllerCommand : EnemyCommand
    {
        public override void Execute()
        {
            Debug.Log("�÷��̾ �������� �Դ� �ڵ� �ۼ�");   
        }
    }

    public class EnemyDeadAIControllerCommand : EnemyCommand // ���� ����
    {
        private GameObject enemyObject;
        private EnemyLootListSO enemyLootListSO;

        public EnemyDeadAIControllerCommand(GameObject enemyObj, EnemyLootListSO lootListSO)
        {
            enemyObject = enemyObj;
            enemyLootListSO = lootListSO;
        }

        public override void Execute()
        {
            for (int i = 0; i < enemyLootListSO.enemyLootList.Count; i++)
            {
                for (int j = 0; j < enemyLootListSO.enemyLootList[i].lootCount; j++)
                {
                    Water.PoolManager.GetItem("Item").GetComponent<Item>().SetData(enemyLootListSO.enemyLootList[i].enemyLoot.id, enemyObject.transform.position);
                }
            }

            enemyObject.GetComponent<Enemy>().EnemyDestroy();
        }
    }

    public class EnemyDeadPlayerControllerCommand : EnemyCommand // ���� ����
    {
        public override void Execute()
        {
            Debug.Log("�÷��̾ �״� �ڵ� �ۼ�");
        }
    }

    public class EnemyAttackCommand : EnemyCommand // �� ����
    {
        public Transform enemyTransform;
        public Transform targetTransform;
        public EnemyController eEnemyController;
        public int attackDamage;

        public EnemyAttackCommand(Transform enemy, Transform target, EnemyController controller, int damage)
        {
            enemyTransform = enemy;
            targetTransform = target;
            eEnemyController = controller;
            attackDamage = damage;
        }

        public override void Execute()
        {
            PoolManager bullet = EnemyPoolManager.Instance.GetPoolObject(Type.Bullet, enemyTransform.position);

            bullet.GetComponent<EnemyBullet>().Init(eEnemyController, attackDamage, (targetTransform.position - enemyTransform.position).normalized);
        }
    }

    public class EnemyRushAttackCommand : EnemyCommand
    {
        private Rigidbody2D rigidboyd2D;
        private Transform enemyTransform;
        private Transform playerTransform;

        private Vector2 targetPosition;

        private float rushForce;
        private bool isReady;

        public EnemyRushAttackCommand(Rigidbody2D rigid, Transform enemy, Transform player, float force)
        {
            rigidboyd2D = rigid;
            enemyTransform = enemy;
            playerTransform = player;
            rushForce = force;
            isReady = false;
        }

        public override void Execute()
        {
            if (!isReady)
            {
                isReady = true;
                targetPosition = (playerTransform.position - enemyTransform.position).normalized;
            }
            else
            {
                isReady = false;
                rigidboyd2D.AddForce(targetPosition * rushForce, ForceMode2D.Impulse);
            }
        }
    }
}