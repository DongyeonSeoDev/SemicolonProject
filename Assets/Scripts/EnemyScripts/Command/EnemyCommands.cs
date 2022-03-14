using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyMovePlayerControllerCommand : EnemyCommand
    {
        private PlayerInput playerInput = null;
        private Stat playerStat = null;
        private float lastForce = 0f;
        private int wallCheck = LayerMask.GetMask("WALL");

        private Rigidbody2D rigid;

        public EnemyMovePlayerControllerCommand(Rigidbody2D rb)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
            playerStat = SlimeGameManager.Instance.Player.PlayerStat;

            rigid = rb;
        }

        public override void Execute()
        {
            float force = 0;

            if (playerInput.MoveVector * (playerStat.Speed) != Vector2.zero)
            {
                lastForce = force = playerStat.Speed;
            }
            else
            {
                lastForce = Mathf.Lerp(lastForce, 0f, Time.deltaTime * playerStat.Speed / 2f);
            }

            var ray = Physics2D.Raycast(rigid.transform.position, playerInput.MoveVector, lastForce / 10f, wallCheck);

            if (ray.collider != null)
            {
                float distance = Vector2.Distance(ray.point, rigid.transform.position) * 10f;
                rigid.velocity = playerInput.MoveVector * distance;
            }
            else
            {
                rigid.velocity = playerInput.LastMoveVector * lastForce;
            }
        }
    }

    public class EnemyRandomMoveCommand : EnemyCommand
    {
        private EnemyData enemyData = null;
        private EnemyCommand enemyRunAwayCommand;
        private EnemyPositionCheckData positionCheckData;

        private Vector2 targetPosition = Vector2.zero;
        private float currentMoveTime;
        private float angle = 0;

        public EnemyRandomMoveCommand(EnemyData enemyData, EnemyPositionCheckData positionCheckData)
        {
            this.enemyData = enemyData;
            this.positionCheckData = positionCheckData;

            enemyRunAwayCommand = new EnemyFollowPlayerCommand(enemyData.enemyObject.transform, enemyData.PlayerObject.transform, enemyData.enemyRigidbody2D, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, true, positionCheckData);

            currentMoveTime = 0f;
        }

        public override void Execute()
        {
            if (enemyData.IsRunAway())
            {
                enemyRunAwayCommand.Execute();
            }
            else if (positionCheckData.isWall)
            {
                targetPosition = positionCheckData.oppositeDirectionWall * 5f;
                enemyData.enemyRigidbody2D.velocity = targetPosition;

                currentMoveTime = 2f;

                positionCheckData.isWall = false;
            }
            else
            {
                if (currentMoveTime <= 0f)
                {
                    angle = Random.Range(0f, 360f);

                    targetPosition.y = Mathf.Sin(angle * Mathf.Deg2Rad);
                    targetPosition.x = Mathf.Cos(angle * Mathf.Deg2Rad);

                    targetPosition *= 5f;

                    enemyData.enemyRigidbody2D.velocity = targetPosition;

                    currentMoveTime = 2f;
                }
                else if (currentMoveTime <= 1f)
                {
                    enemyData.enemyRigidbody2D.velocity = Vector2.zero;
                    currentMoveTime -= Time.deltaTime;
                }
                else
                {
                    enemyData.enemyRigidbody2D.velocity = targetPosition;
                    currentMoveTime -= Time.deltaTime;
                }
            }
        }
    }

    public class EnemyFollowPlayerCommand : EnemyCommand // 적 움직임
    {
        private Transform enemyObject;
        private Transform followObject;
        private Rigidbody2D rigid;
        private EnemyPositionCheckData positionCheckData;

        private float followSpeed;
        private float followDistance;
        private bool isLongDistanceAttack;

        private Vector3 targetPosition;
        private float angle;

        private float currentTime = 0f;
        private float angleChangeTime = 1f;
        private float addAngle = 0;

        public EnemyFollowPlayerCommand(Transform enemyObject, Transform followObject, Rigidbody2D rigid, float followSpeed, float followDistance, bool isLongDistanceAttack, EnemyPositionCheckData positionCheckData = null)
        {
            this.enemyObject = enemyObject;
            this.followObject = followObject;
            this.rigid = rigid;

            this.followSpeed = followSpeed;
            this.followDistance = followDistance;
            this.isLongDistanceAttack = isLongDistanceAttack;

            this.positionCheckData = positionCheckData;

            currentTime = 0;
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
                currentTime -= Time.deltaTime;

                if (positionCheckData != null && positionCheckData.isWall)
                {
                    currentTime = 0.5f;
                    targetPosition = positionCheckData.oppositeDirectionWall * followSpeed;
                    positionCheckData.isWall = false;
                }
                else if (currentTime <= 0)
                {
                    targetPosition = enemyObject.transform.position - followObject.transform.position;

                    angle = Mathf.Atan2(targetPosition.x, targetPosition.y) * Mathf.Rad2Deg + 90f;

                    addAngle = Random.Range(-60f, 60f);
                    currentTime = angleChangeTime;

                    angle = (angle + addAngle) % 360;

                    targetPosition.x = followObject.transform.position.x + (followDistance * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
                    targetPosition.y = followObject.transform.position.y + (followDistance * Mathf.Sin(angle * Mathf.Deg2Rad));
                    targetPosition.z = followObject.transform.position.z;

                    targetPosition = (targetPosition - enemyObject.position).normalized;
                    targetPosition *= followSpeed;
                }
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

    public class EnemyGetDamagedCommand : EnemyCommand // 적이 데미지를 받음
    {
        private EnemyData enemyData;

        private bool isWorking = false;

        public EnemyGetDamagedCommand(EnemyData enemyData)
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

    public class EnemyDeadAIControllerCommand : EnemyCommand // 적이 죽음
    {
        private GameObject enemyObject;
        private List<EnemyLootData> enemyLootList;

        private Color enemyColor;

        public EnemyDeadAIControllerCommand(GameObject enemyObj, List<EnemyLootData> lootList, Color color)
        {
            enemyObject = enemyObj;
            enemyLootList = lootList;
            enemyColor = color;
        }

        public override void Execute()
        {
            for (int i = 0; i < enemyLootList.Count; i++)
            {
                for (int j = 0; j < enemyLootList[i].count; j++)
                {
                    if (CSVEnemyLoot.Instance.itemDictionary.ContainsKey(enemyLootList[i].lootName))
                    {
                        Water.PoolManager.GetItem("Item").GetComponent<Item>().SetData(CSVEnemyLoot.Instance.itemDictionary[enemyLootList[i].lootName].id, enemyObject.transform.position);
                    }
                    else
                    {
                        Debug.Log(enemyLootList[i].lootName);
                    }
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

    public class EnemyAttackPlayerCommand : EnemyCommand
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

    public class EnemyAddForceCommand : EnemyCommand
    {
        private Rigidbody2D rigidboyd2D;
        private float force;
        private EnemyPositionCheckData positionCheckData;
        Vector2? direction;
        private int wallCheck = LayerMask.GetMask("WALL");

        public EnemyAddForceCommand(Rigidbody2D rigid, float force, EnemyPositionCheckData positionData = null, Vector2? direction = null)
        {
            rigidboyd2D = rigid;
            this.force = force;
            positionCheckData = positionData;
            this.direction = direction;
        }

        public override void Execute()
        {
            if (positionCheckData != null)
            {
                direction = positionCheckData.position;
            }

            var ray = Physics2D.Raycast(rigidboyd2D.transform.position, direction.Value, force / 10f, wallCheck);

            if (ray.collider != null)
            {
                float distance = Vector2.Distance(ray.point, rigidboyd2D.transform.position) * 10f;
                rigidboyd2D.AddForce(direction.Value * distance, ForceMode2D.Impulse);
            }
            else
            {
                rigidboyd2D.AddForce(direction.Value * force, ForceMode2D.Impulse);
            }
        }
    }
}