using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyMovePlayerControllerCommand : EnemyCommand
    {
        private EnemyData enemyData;
        private PlayerInput playerInput = null;
        private Stat playerStat = null;
        private float lastSpeed = 0f;
        private float speed = 0f;
        private int wallCheck = LayerMask.GetMask("WALL");

        public EnemyMovePlayerControllerCommand(EnemyData data)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
            playerStat = SlimeGameManager.Instance.Player.PlayerStat;

            enemyData = data;

            speed = playerStat.Speed;
        }

        public override void Execute()
        {
            if (!enemyData.isPlayerControllerMove)
            {
                lastSpeed = 0f;
                enemyData.isPlayerControllerMove = true;
            }
            if (playerInput.MoveVector * speed != Vector2.zero)
            {
                lastSpeed = speed;
            }
            else
            {
                lastSpeed = Mathf.Lerp(lastSpeed, 0f, Time.deltaTime * speed / 2f);
            }

            var ray = Physics2D.Raycast(enemyData.enemyRigidbody2D.transform.position, playerInput.MoveVector, lastSpeed / 10f, wallCheck);

            if (ray.collider != null)
            {
                float distance = Vector2.Distance(ray.point, enemyData.enemyRigidbody2D.transform.position) * 10f;
                enemyData.enemyRigidbody2D.velocity = playerInput.MoveVector * distance;
            }
            else
            {
                enemyData.enemyRigidbody2D.velocity = playerInput.LastMoveVector * lastSpeed;
            }

            enemyData.moveVector = playerInput.MoveVector;
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

            enemyRunAwayCommand = new EnemyFollowPlayerCommand(enemyData, enemyData.enemyObject.transform, enemyData.enemyRigidbody2D, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, true, positionCheckData);

            currentMoveTime = 0f;
        }

        public override void Execute()
        {
            if (EnemyManager.IsRunAway(enemyData))
            {
                enemyRunAwayCommand.Execute();
            }
            else if (positionCheckData.isWall)
            {
                targetPosition = positionCheckData.oppositeDirectionWall * 5f;
                enemyData.moveVector = positionCheckData.oppositeDirectionWall;
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

                    enemyData.moveVector = targetPosition;

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
        private EnemyData enemyData;
        private Transform enemyObject;
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

        public EnemyFollowPlayerCommand(EnemyData data, Transform enemyObject, Rigidbody2D rigid, float followSpeed, float followDistance, bool isLongDistanceAttack, EnemyPositionCheckData positionCheckData = null)
        {
            enemyData = data;
            this.enemyObject = enemyObject;
            this.rigid = rigid;

            this.followSpeed = followSpeed;
            this.followDistance = followDistance;
            this.isLongDistanceAttack = isLongDistanceAttack;

            this.positionCheckData = positionCheckData;

            currentTime = 0;
        }

        public override void Execute()
        {
            if (isLongDistanceAttack)
            {
                // 이동
                currentTime -= Time.deltaTime;

                if (positionCheckData != null && positionCheckData.isWall)
                {
                    currentTime = 0.5f;
                    targetPosition = positionCheckData.oppositeDirectionWall * followSpeed;

                    enemyData.moveVector = positionCheckData.oppositeDirectionWall;

                    positionCheckData.isWall = false;
                }
                else if (currentTime <= 0)
                {
                    targetPosition = enemyObject.transform.position - EnemyManager.Player.transform.position;

                    angle = Mathf.Atan2(targetPosition.x, targetPosition.y) * Mathf.Rad2Deg + 90f;

                    addAngle = Random.Range(-60f, 60f);
                    currentTime = angleChangeTime;

                    angle = (angle + addAngle) % 360;

                    targetPosition.x = EnemyManager.Player.transform.position.x + (followDistance * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
                    targetPosition.y = EnemyManager.Player.transform.position.y + (followDistance * Mathf.Sin(angle * Mathf.Deg2Rad));
                    targetPosition.z = EnemyManager.Player.transform.position.z;

                    targetPosition = (targetPosition - enemyObject.position).normalized;

                    enemyData.moveVector = targetPosition;

                    targetPosition *= followSpeed;
                }
            }
            else
            {
                // 이동
                targetPosition = (EnemyManager.Player.transform.position - enemyObject.position).normalized;

                enemyData.moveVector = targetPosition;

                targetPosition *= followSpeed;
            }

            rigid.velocity = targetPosition;
        }
    }

    public class BossRushAttackCommand : EnemyCommand // 보스 X축과 Y축 돌진 공격
    {
        private EnemyData enemyData;
        private Transform enemyObject;
        private Rigidbody2D rigid;
        private Vector3 targetPosition;
        private float followSpeed;
        private bool isXMove;

        public BossRushAttackCommand(EnemyData data, Transform enemyObject, Rigidbody2D rigid, float followSpeed, bool isXMove)
        {
            enemyData = data;
            this.enemyObject = enemyObject;
            this.rigid = rigid;
            this.followSpeed = followSpeed;
            this.isXMove = isXMove;
        }

        public override void Execute()
        {
            // 이동
            if (isXMove)
            {
                targetPosition = (new Vector3(EnemyManager.Player.transform.position.x, enemyObject.position.y, EnemyManager.Player.transform.position.z) - enemyObject.position).normalized;
            }
            else
            {
                targetPosition = (new Vector3(enemyObject.position.x, EnemyManager.Player.transform.position.y, EnemyManager.Player.transform.position.z) - enemyObject.position).normalized;
            }

            enemyData.moveVector = targetPosition;

            if (followSpeed < 0)
            {
                float distance;

                if (isXMove)
                {
                    distance = Vector2.Distance(new Vector2(EnemyManager.Player.transform.position.x, enemyObject.position.y), enemyObject.transform.position);
                }
                else
                {
                    distance = Vector2.Distance(new Vector2(enemyObject.position.x, EnemyManager.Player.transform.position.y), enemyObject.transform.position);
                }

                targetPosition *= distance * 5.5f;
            }
            else
            {
                targetPosition *= followSpeed;
            }

            rigid.velocity = targetPosition;
        }
    }

    public class EnemyGetDamagedCommand : EnemyCommand // 적이 데미지를 받음
    {
        private EnemyData enemyData;

        public EnemyGetDamagedCommand(EnemyData enemyData)
        {
            this.enemyData = enemyData;
        }

        public override void Execute()
        {
            if (enemyData.eEnemyController == EnemyController.AI)
            {
                if (enemyData.isDamaged) // 색깔 변경
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
                if (enemyData.isDamaged) // 색깔 변경
                {
                    enemyData.enemySpriteRenderer.color = enemyData.playerDamagedColor;
                }
                else // 색깔 변경 해제
                {
                    enemyData.enemySpriteRenderer.color = enemyData.playerNormalColor;
                }
            }
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
        public int attackDamage;

        private Enemy enemy;

        public EnemyAttackCommand(Enemy enemy, Transform enemyPosition, int damage)
        {
            enemyTransform = enemyPosition;
            attackDamage = damage;
            this.enemy = enemy;
        }

        public override void Execute()
        {
            EnemyPoolData bullet = EnemyPoolManager.Instance.GetPoolObject(Type.Bullet, enemyTransform.position);

            bullet.GetComponent<EnemyBullet>().Init(enemy.GetEnemyController(), attackDamage, (EnemyManager.Player.transform.position - enemyTransform.position).normalized);
        }
    }

    public class EnemyAttackPlayerCommand : EnemyCommand
    {
        PlayerInput playerInput;
        Transform transform;
        Enemy enemy;

        int attackDamage;

        public EnemyAttackPlayerCommand(Transform transform, Enemy enemy, int attackDamage)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
            this.transform = transform;
            this.attackDamage = attackDamage;
            this.enemy = enemy;
        }

        public override void Execute()
        {
            EnemyPoolData bullet = EnemyPoolManager.Instance.GetPoolObject(Type.Bullet, transform.position);

            bullet.GetComponent<EnemyBullet>().Init(enemy.GetEnemyController(), attackDamage, (playerInput.AttackMousePosition - (Vector2)transform.position).normalized, enemy);
        }
    }

    public class EnemyAddForceCommand : EnemyCommand
    {
        private Rigidbody2D rigidboyd2D;
        private EnemyPositionCheckData positionCheckData;
        private Enemy enemy;
        Vector2? direction;
        private int wallCheck = LayerMask.GetMask("WALL");
        private float force;

        public EnemyAddForceCommand(Rigidbody2D rigid, Enemy enemy, float rushAttack = 0f, EnemyPositionCheckData positionData = null)
        {
            rigidboyd2D = rigid;
            this.enemy = enemy;
            positionCheckData = positionData;

            force = rushAttack == 0 ? enemy.GetEnemyAttackPower() : rushAttack;
        }

        public override void Execute()
        {
            if (positionCheckData != null)
            {
                direction = positionCheckData.position;
            }
            else
            {
                direction = enemy.GetKnockBackDirection();

                if (direction == null)
                {
                    direction = rigidboyd2D.transform.position - EnemyManager.Player.transform.position;
                }
            }

            direction = direction.Value.normalized;

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

    public class EnemySpriteFlipCommand : EnemyCommand
    {
        private EnemyData enemyData;

        public EnemySpriteFlipCommand(EnemyData data)
        {
            enemyData = data;
        }

        public override void Execute()
        {
            if (enemyData.moveVector.x < 0)
            {
                enemyData.enemySpriteRenderer.flipX = true;
            }
            else if (enemyData.moveVector.x > 0)
            {
                enemyData.enemySpriteRenderer.flipX = false;
            }
        }
    }

    public class EnemySpriteRotateCommand : EnemyCommand
    {
        private EnemyData enemyData;

        public EnemySpriteRotateCommand(EnemyData data)
        {
            enemyData = data;
        }

        public override void Execute()
        {
            if (enemyData.moveVector.x < 0)
            {
                enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (enemyData.moveVector.x > 0)
            {
                enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}