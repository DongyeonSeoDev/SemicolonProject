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

        public EnemyMovePlayerControllerCommand(EnemyData data)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();
            playerStat = SlimeGameManager.Instance.Player.PlayerStat;

            enemyData = data;
        }

        public override void Execute()
        {
            if (!enemyData.isPlayerControllerMove)
            {
                lastSpeed = 0f;
                enemyData.isPlayerControllerMove = true;
            }

            if (playerInput.MoveVector * playerStat.Speed != Vector2.zero)
            {
                lastSpeed = playerStat.Speed;
            }
            else
            {
                lastSpeed = Mathf.Lerp(lastSpeed, 0f, Time.deltaTime * playerStat.Speed / 2f);
            }

            enemyData.enemyRigidbody2D.velocity = playerInput.LastMoveVector * lastSpeed;
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

            enemyRunAwayCommand = new EnemyRunAwayCommand(enemyData, enemyData.enemyObject.transform, enemyData.enemyRigidbody2D, enemyData.chaseSpeed, enemyData.isMinAttackPlayerDistance, positionCheckData);

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
                    enemyData.moveVector = Vector2.zero;
                    currentMoveTime -= Time.deltaTime;
                }
                else
                {
                    enemyData.enemyRigidbody2D.velocity = targetPosition;
                    enemyData.moveVector = targetPosition;
                    currentMoveTime -= Time.deltaTime;
                }
            }
        }
    }

    public class EnemyTargetMoveCommand : EnemyCommand
    {
        private EnemyData enemyData;
        private float moveSpeed;

        public EnemyTargetMoveCommand(EnemyData data, float speed)
        {
            enemyData = data;
            moveSpeed = speed;
        }

        public override void Execute()
        {
            enemyData.enemyRigidbody2D.velocity = enemyData.moveVector * moveSpeed;
        }
    }

    public class EnemyLongDistanceFollowPlayerCommand : EnemyCommand
    {
        private EnemyData enemyData;
        private Transform enemyTransform;
        private Rigidbody2D rigid;
        private Vector3 targetPosition;
        private float followSpeed;
        private float followDistance;
        private float angle;
        private float moveDelay;
        private float currentTime = 0f;

        public EnemyLongDistanceFollowPlayerCommand(EnemyData data, Transform enemyTransform, Rigidbody2D rigid, float followSpeed, float followDistance, float moveDelay)
        {
            enemyData = data;
            this.enemyTransform = enemyTransform;
            this.rigid = rigid;

            this.followSpeed = followSpeed;
            this.followDistance = followDistance;

            this.moveDelay = moveDelay;
            
            currentTime = moveDelay;
        }

        public override void Execute()
        {
            if (currentTime >= moveDelay)
            {
                currentTime = 0f;

                targetPosition = enemyTransform.transform.position - EnemyManager.Player.transform.position;

                angle = Mathf.Atan2(targetPosition.x, targetPosition.y) * Mathf.Rad2Deg + 90f;

                targetPosition.x = EnemyManager.Player.transform.position.x + (followDistance * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
                targetPosition.y = EnemyManager.Player.transform.position.y + (followDistance * Mathf.Sin(angle * Mathf.Deg2Rad));
                targetPosition.z = EnemyManager.Player.transform.position.z;

                targetPosition = (targetPosition - enemyTransform.position).normalized;
                targetPosition *= followSpeed;

                enemyData.moveVector = targetPosition;
                rigid.velocity = targetPosition;
            }
            else
            {
                currentTime += Time.deltaTime;

                enemyData.moveVector = targetPosition;
                rigid.velocity = targetPosition;
            }
        }
    }

    public class EnemyRunAwayCommand : EnemyCommand
    {
        private EnemyData enemyData;
        private EnemyPositionCheckData positionCheckData;
        private Transform enemyTransform;
        private Rigidbody2D rigid;
        private Vector3 targetPosition;
        private float followSpeed;
        private float followDistance;
        private float lastTime;
        private float angle;
        private float addAngle;
        private float angleChangeTime;

        public EnemyRunAwayCommand(EnemyData data, Transform enemyTransform, Rigidbody2D rigid, float followSpeed, float followDistance, EnemyPositionCheckData positionCheckData = null, float angleChangeTime = 1f)
        {
            enemyData = data;
            this.enemyTransform = enemyTransform;
            this.rigid = rigid;

            this.followSpeed = followSpeed;
            this.followDistance = followDistance;

            this.positionCheckData = positionCheckData;

            this.angleChangeTime = angleChangeTime;

            lastTime = 0;
        }

        public override void Execute()
        {
            if (positionCheckData != null && positionCheckData.isWall)
            {
                lastTime = Time.time - 0.5f;
                targetPosition = positionCheckData.oppositeDirectionWall * followSpeed;
                positionCheckData.isWall = false;
            }
            else if (lastTime + angleChangeTime <= Time.time)
            {
                targetPosition = enemyTransform.transform.position - EnemyManager.Player.transform.position;

                angle = Mathf.Atan2(targetPosition.x, targetPosition.y) * Mathf.Rad2Deg + 90f;

                addAngle = Random.Range(-60f, 60f);
                lastTime = Time.time;

                angle = (angle + addAngle) % 360;

                targetPosition.x = EnemyManager.Player.transform.position.x + (followDistance * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
                targetPosition.y = EnemyManager.Player.transform.position.y + (followDistance * Mathf.Sin(angle * Mathf.Deg2Rad));
                targetPosition.z = EnemyManager.Player.transform.position.z;

                targetPosition = (targetPosition - enemyTransform.position).normalized;
                targetPosition *= followSpeed;
            }

            enemyData.moveVector = targetPosition;
            rigid.velocity = targetPosition;
        }
    }

    public class EnemyFollowPlayerCommand : EnemyCommand
    {
        private EnemyData enemyData;
        private Rigidbody2D rigid;
        private Transform enemyTransform;
        private Vector2 targetPosition;
        private Vector2Int? position;

        private Stack<Vector2Int> nextPosition = new Stack<Vector2Int>();

        private float followSpeed;
        private int moveCount = 0;

        public EnemyFollowPlayerCommand(EnemyData data, Transform enemyTransform, Rigidbody2D rigid, float followSpeed)
        {
            enemyData = data;
            this.enemyTransform = enemyTransform;
            this.rigid = rigid;

            this.followSpeed = followSpeed;

            nextPosition = null;
            position = null;
        }

        public override void Execute()
        {
            if (enemyData.isMoveStop)
            {
                return;
            }

            if (enemyData.isMovePositionReset || moveCount > 100 || nextPosition == null || nextPosition.Count < 1)
            {
                nextPosition = EnemyManager.NextPosition(enemyTransform.position, EnemyManager.Player.transform.position);

                position = null;
                enemyData.isMovePositionReset = false;
            }

            if (position == null || Vector2.Distance(new Vector2(enemyTransform.position.x, enemyTransform.position.y), position.Value) < 0.4f)
            {
                if (nextPosition.Count < 1)
                {
                    return;
                }
                else
                {
                    position = nextPosition.Pop();
                    moveCount = 0;
                }
            }

            targetPosition = (new Vector3(position.Value.x, position.Value.y, enemyTransform.position.z) - enemyTransform.position).normalized;
            targetPosition *= followSpeed;
            enemyData.moveVector = targetPosition;
            rigid.velocity = targetPosition;

            moveCount++;
        }
    }

    public class BossMoveCommand : EnemyCommand // ?????? ?????????
    {
        private EnemyData enemyData;
        private Transform enemyObject;
        private Rigidbody2D rigid;
        private Boss1SkeletonKing boss;

        private Vector3 targetPosition;

        public BossMoveCommand(EnemyData data, Transform enemyObject, Rigidbody2D rigid, Boss1SkeletonKing boss1SkeletonKing)
        {
            enemyData = data;

            this.enemyObject = enemyObject;
            this.rigid = rigid;
            boss = boss1SkeletonKing;
        }

        public override void Execute()
        {
            targetPosition = (EnemyManager.Player.transform.position - enemyObject.position).normalized;
            targetPosition *= boss.currentSpeed;
            enemyData.moveVector = targetPosition;
            rigid.velocity = targetPosition;
        }
    }

    public class BossRushAttackCommand : EnemyCommand // ?????? X?????? Y??? ?????? ??????
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
            // ??????
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

    public class EnemylongRangeAttackCommand : EnemyCommand // ??? ??????
    {
        private Transform enemyTransform;
        private Transform shotTransform;
        private Enemy enemy;

        private Type objectType;

        private float minAttack;
        private float maxAttack;
        private float critical;
        private float criticalPower;

        public EnemylongRangeAttackCommand(Enemy enemy, Transform enemyPosition, Type objectType, Transform shotTransform, float minAttack, float maxAttack, float critical, float criticalPower)
        {
            this.minAttack = minAttack;
            this.maxAttack = maxAttack;
            this.critical = critical;
            this.criticalPower = criticalPower;

            this.enemy = enemy;
            enemyTransform = enemyPosition;
            this.objectType = objectType;
            this.shotTransform = shotTransform;
        }

        public override void Execute()
        {
            EnemyPoolData spawnObject = EnemyPoolManager.Instance.GetPoolObject(objectType, shotTransform.position);

            spawnObject.GetComponent<EnemyBullet>().Init(enemy.GetEnemyController(), (EnemyManager.Player.transform.position - enemyTransform.position).normalized, minAttack, maxAttack, critical, criticalPower, Color.white, null);
        }
    }

    public class PlayerlongRangeAttackCommand : EnemyCommand
    {
        private PlayerInput playerInput;
        private Transform transform;
        private Transform shotTransform;
        private Enemy enemy;

        private Type objectType;
        private Color color;

        private float minAttack;
        private float maxAttack;
        private float critical;
        private float criticalPower;

        public PlayerlongRangeAttackCommand(Transform transform, Transform shotTransform, Enemy enemy, Type objectType, float minAttack, float maxAttack, float critical, float criticalPower, Color color)
        {
            playerInput = SlimeGameManager.Instance.Player.GetComponent<PlayerInput>();

            this.minAttack = minAttack;
            this.maxAttack = maxAttack;
            this.critical = critical;
            this.criticalPower = criticalPower;

            this.transform = transform;
            this.shotTransform = shotTransform;
            this.enemy = enemy;
            this.objectType = objectType;
            this.color = color;
        }

        public override void Execute()
        {
            EnemyPoolData spawnObject = EnemyPoolManager.Instance.GetPoolObject(objectType, shotTransform.position);

            spawnObject.GetComponent<EnemyBullet>().Init(enemy.GetEnemyController(), (playerInput.AttackMousePosition - (Vector2)transform.position).normalized, minAttack, maxAttack, critical, criticalPower, color, enemy);
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

                if (enemyData.enemyCanvas != null)
                {
                    enemyData.enemyCanvas.transform.rotation = Quaternion.identity;
                }
            }
            else if (enemyData.moveVector.x > 0)
            {
                enemyData.enemySpriteRenderer.flipX = false;

                if (enemyData.enemyCanvas != null)
                {
                    enemyData.enemyCanvas.transform.rotation = Quaternion.identity;
                }
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

                if (enemyData.enemyCanvas != null)
                {
                    enemyData.enemyCanvas.transform.rotation = Quaternion.identity;
                }
            }
            else if (enemyData.moveVector.x > 0)
            {
                enemyData.enemyObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

                if (enemyData.enemyCanvas != null)
                {
                    enemyData.enemyCanvas.transform.rotation = Quaternion.identity;
                }
            }
        }
    }
}