using UnityEngine;

namespace Enemy
{
    public class EnemyMovePlayerControllerCommand : EnemyCommand
    {
        private PlayerInput playerInput = null;
        private Stat playerStat = null;
        private Vector2 lastMoveVec = Vector2.zero;

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

            if(MoveVec != Vector2.zero)
            {
                lastMoveVec = MoveVec;
            }
            else
            {
                lastMoveVec = Vector2.Lerp(lastMoveVec, Vector2.zero, Time.deltaTime * playerStat.Speed / 2f);
            }

            rigid.velocity = lastMoveVec;
        }
    }

    public class EnemyRandomMoveCommand : EnemyCommand
    {
        private EnemyData enemyData = null;
        private EnemyCommand enemyRunAwayCommand;

        public EnemyRandomMoveCommand(EnemyData enemyData)
        {
            this.enemyData = enemyData;
            enemyRunAwayCommand = new EnemyFollowPlayerCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.enemyRigidbody2D, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, true);
        }

        public override void Execute()
        {
            if (enemyData.IsRunAway())
            {
                enemyRunAwayCommand.Execute();
            }
            else
            {
                float angle = Random.Range(0f, 360f);
                Vector2 targetPosition = Vector2.zero;

                targetPosition.y = Mathf.Sin(angle * Mathf.Deg2Rad);
                targetPosition.x = Mathf.Cos(angle * Mathf.Deg2Rad);

                targetPosition *= 10f;

                enemyData.enemyRigidbody2D.velocity = targetPosition;
            }
        }
    }

    public class EnemyFollowPlayerCommand : EnemyCommand // 적 움직임
    {
        private Transform enemyObject;
        private Transform followObject;
        private Rigidbody2D rigid;

        private float followSpeed;
        private float followDistance;
        private bool isLongDistanceAttack;

        private Vector3 targetPosition;
        private float angle;

        public EnemyFollowPlayerCommand(Transform enemyObject, Transform followObject, Rigidbody2D rigid, float followSpeed, float followDistance, bool isLongDistanceAttack)
        {
            this.enemyObject = enemyObject;
            this.followObject = followObject;
            this.rigid = rigid;

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
                targetPosition *= followSpeed;
            }
            else
            {
                // 이동
                targetPosition = (followObject.position - enemyObject.position).normalized;
                targetPosition *= followSpeed;
            }

            rigid.velocity = targetPosition;
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
            EnemyPoolData bullet = EnemyPoolManager.Instance.GetPoolObject(Type.Bullet, enemyTransform.position);

            bullet.GetComponent<EnemyBullet>().Init(eEnemyController, attackDamage, (targetTransform.position - enemyTransform.position).normalized);
        }
    }

    public class EnemyAttackPlayerCommand : EnemyCommand // 적으로 변신한 플레이어가 공격
    {
        PlayerInput playerInput;
        Transform transform;
        Enemy enemy;

        EnemyController enemyController;

        int attackDamage;

        public EnemyAttackPlayerCommand(Transform transform, Enemy enemy, EnemyController controller, int attackDamage)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
            this.transform = transform;
            enemyController = controller;
            this.attackDamage = attackDamage;
            this.enemy = enemy;
        }

        public override void Execute()
        {
            EnemyPoolData bullet = EnemyPoolManager.Instance.GetPoolObject(Type.Bullet, transform.position);

            bullet.GetComponent<EnemyBullet>().Init(enemyController, attackDamage, (playerInput.MousePosition - (Vector2)transform.position).normalized, enemy);
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

    public class EnemyKnockBackAICommand : EnemyCommand
    {
        private Rigidbody2D rigid;
        private Vector2 direction;

        public EnemyKnockBackAICommand(Rigidbody2D rigid, Vector2 direction)
        {
            this.rigid = rigid;
            this.direction = direction;
        }

        public override void Execute()
        {
            rigid.AddForce(direction, ForceMode2D.Impulse);
        }
    }

    public class EnemyKnockBackPlayerCommand : EnemyCommand
    {
        public override void Execute()
        {
            throw new System.NotImplementedException("플레이어 넉백 구현해야함");
        }
    }
}