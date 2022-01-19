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
        public override void Execute()
        {
            Debug.Log("플레이어 움직임 코드 작성");
        }
    }

    public class EnemyFollowPlayerCommand : EnemyCommand // 적 움직임
    {
        private Transform enemyObject;
        private Transform followObject;

        private Vector3 targetPosition;

        private float followSpeed;

        public EnemyFollowPlayerCommand(Transform enemyObject, Transform followObject, float followSpeed)
        {
            this.enemyObject = enemyObject;
            this.followObject = followObject;
            this.followSpeed = followSpeed;
        }

        public override void Execute()
        {
            if (Vector3.Distance(enemyObject.position, followObject.position) >= 1f) // 거리 확인
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
            if (!isWorking) // 색깔 변경
            {
                enemyData.enemySpriteRenderer.color = Color.green;
            }
            else // 색깔 변경 해제
            {
                enemyData.enemySpriteRenderer.color = Color.magenta;
            }

            isWorking = !isWorking;
        }
    }

    public class EnemyGetDamagedPlayerControllerCommand : EnemyCommand
    {
        public override void Execute()
        {
            Debug.Log("플레이어가 데미지를 입는 코드 작성");   
        }
    }

    public class EnemyDeadAIControllerCommand : EnemyCommand // 적이 죽음
    {
        private GameObject enemyObject; 

        public EnemyDeadAIControllerCommand(GameObject enemyObj)
        {
            enemyObject = enemyObj;
        }

        public override void Execute()
        {
            GameObject.Destroy(enemyObject);
        }
    }

    public class EnemyDeadPlayerControllerCommand : EnemyCommand // 적이 죽음
    {
        public override void Execute()
        {
            Debug.Log("플레이어가 죽는 코드 작성");
        }
    }

    public class EnemyAttackAIControllerCommand : EnemyCommand // 적 공격
    {
        public Vector3 spawnPosition;
        public EnemyController eEnemyController;
        public int attackDamage;

        public EnemyAttackAIControllerCommand(Vector3 position, EnemyController controller, int damage)
        {
            spawnPosition = position;
            eEnemyController = controller;
            attackDamage = damage;
        }

        public override void Execute()
        {
            EnemyPoolManager.Instance.GetEnemyBullet(spawnPosition, eEnemyController, attackDamage);
        }
    }

    public class EnemyAttackPlayerControllerCommand : EnemyCommand // 플레이어가 변신했을때 공격
    {
        public override void Execute()
        {
            Debug.Log("플레이어 공격 코드 작성");
        }
    }
}