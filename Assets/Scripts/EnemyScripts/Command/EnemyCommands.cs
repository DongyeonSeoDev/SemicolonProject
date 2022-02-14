using UnityEngine;

namespace Enemy
{
    public class EnemyMoveAIControllerCommand : EnemyCommand // 적 움직임
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
            // 이동
            targetPosition = (enemyMoveSO.targetPositions[enemyMoveSO.currentPositionNumber] - enemyPosition.position).normalized;
            targetPosition *= enemyMoveSO.moveSpeed * Time.deltaTime;

            enemyPosition.position += targetPosition;

            // 거리 확인
            if (Vector3.Distance(enemyPosition.position, enemyMoveSO.targetPositions[enemyMoveSO.currentPositionNumber]) < enemyMoveSO.targetPositionChangeDistance)
            {
                enemyMoveSO.currentPositionNumber = (enemyMoveSO.currentPositionNumber + 1) % enemyMoveSO.targetPositions.Count;
            }
        }
    }

    public class EnemyMovePlayerControllerCommand : EnemyCommand // 적 움직임
    {
        private PlayerInput playerInput = null;
        private Stat playerStat = null;

        private Rigidbody2D rigid;

        public EnemyMovePlayerControllerCommand(Rigidbody2D rb)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
            playerStat = SlimeGameManager.Instance.Player.PlayerStat;

            rigid = rb;
        }

        public override void Execute()
        {
            Vector2 MoveVec = playerInput.MoveVector * (playerStat.Speed);
            rigid.velocity = MoveVec;
        }
    }

    public class EnemyFollowPlayerCommand : EnemyCommand // 적 움직임
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
            if (followObject == null)
            {
                followObject = EnemyManager.Instance.player;

                if (followObject == null)
                {
                    EnemyManager.Instance.player = GameObject.FindGameObjectWithTag("Player").transform;

                    followObject = EnemyManager.Instance.player;

                    if (followObject == null)
                    {
                        Debug.LogError("Player를 찾을 수 없습니다.");

                        return;
                    }
                }
            }

            if (isLongDistanceAttack)
            {
                // 이동
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
                // 이동
                targetPosition = (followObject.position - enemyObject.position).normalized;
                targetPosition *= followSpeed * Time.deltaTime;

                enemyObject.position += targetPosition;
            }
        }
    }

    public class EnemyGetDamagedAIControllerCommand : EnemyCommand // 적이 데미지를 받음
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
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                if (!isWorking) // 색깔 변경
                {
                    enemyData.enemySpriteRenderer.color = enemyData.damagedColor;
                }
                else // 색깔 변경 해제
                {
                    enemyData.enemySpriteRenderer.color = enemyData.normalColor;
                }
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                if (!isWorking) // 색깔 변경
                {
                    enemyData.enemySpriteRenderer.color = enemyData.playerDamagedColor;
                }
                else // 색깔 변경 해제
                {
                    enemyData.enemySpriteRenderer.color = enemyData.playerNormalColor;
                }
            }

            isWorking = !isWorking;
        }
    }

    public class EnemyGetDamagedPlayerControllerCommand : EnemyCommand
    {
        private int damage;

        public EnemyGetDamagedPlayerControllerCommand(int damage)
        {
            this.damage = damage;
        }

        public override void Execute()
        {
            SlimeGameManager.Instance.Player.GetDamage(damage);
        }
    }

    public class EnemyDeadAIControllerCommand : EnemyCommand // 적이 죽음
    {
        private GameObject enemyObject;
        private EnemyLootListSO enemyLootListSO;

        private Color enemyColor;

        public EnemyDeadAIControllerCommand(GameObject enemyObj, EnemyLootListSO lootListSO, Color color)
        {
            enemyObject = enemyObj;
            enemyLootListSO = lootListSO;
            enemyColor = color;
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

            EnemyPoolManager.Instance.GetPoolObject(Type.DeadEffect, enemyObject.transform.position).GetComponent<EnemyEffect>().Play(enemyColor);

            enemyObject.GetComponent<Enemy>().EnemyDestroy();
        }
    }

    public class EnemyAttackCommand : EnemyCommand // 적 공격
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

    public class EnemyAttackPlayerCommand : EnemyCommand // 적 공격
    {
        PlayerInput playerInput;
        Transform transform;

        EnemyController enemyController;

        int attackDamage;

        public EnemyAttackPlayerCommand(Transform transform, EnemyController controller, int attackDamage)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
            this.transform = transform;
            enemyController = controller;
            this.attackDamage = attackDamage;
        }

        public override void Execute()
        {
            PoolManager bullet = EnemyPoolManager.Instance.GetPoolObject(Type.Bullet, transform.position);

            bullet.GetComponent<EnemyBullet>().Init(enemyController, attackDamage, (playerInput.MousePosition - (Vector2)transform.position).normalized);
        }
    }

    public class EnemyRushAttackCommand : EnemyCommand
    {
        public class RushAttackPosition
        {
            public Vector2 position;
        }

        private Rigidbody2D rigidboyd2D;
        private RushAttackPosition rushAttackPosition;

        private float rushForce;

        public EnemyRushAttackCommand(Rigidbody2D rigid, RushAttackPosition attackPosition, float force)
        {
            rigidboyd2D = rigid;
            rushForce = force;
            rushAttackPosition = attackPosition;
        }

        public override void Execute()
        {
            rigidboyd2D.AddForce(rushAttackPosition.position * rushForce, ForceMode2D.Impulse);
        }
    }
}