using UnityEngine;

namespace Enemy
{
    public class EnemyMove : Command // 적 움직임
    {
        private EnemyMoveSO enemyMoveSO;
        private Transform enemyPosition;

        private Vector3 targetPosition;

        public EnemyMove(EnemyMoveSO enemyMoveSO, Transform enemyPosition)
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

    public class EnemyFollowPlayer : Command // 적 움직임
    {
        private Transform enemyObject;
        private Transform followObject;

        private Vector3 targetPosition;

        private float followSpeed;

        public EnemyFollowPlayer(Transform enemyObject, Transform followObject, float followSpeed)
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

    public class EnemyGetDamaged : Command // 적이 데미지를 받음
    {
        private EnemyData enemyData;

        private bool isWorking = false;

        public EnemyGetDamaged(EnemyData enemyData)
        {
            this.enemyData = enemyData;
            isWorking = false;

            Debug.Log("생성");
        }

        public override void Execute()
        {
            Debug.Log("실행");

            if (!isWorking) // 데미지를 주고 색깔 변경
            {
                enemyData.enemySpriteRenderer.color = Color.green;
                enemyData.hp -= enemyData.damagedValue;
            }
            else // 색깔 변경 후 데미지 상태 해제
            {
                enemyData.enemySpriteRenderer.color = Color.magenta;
                enemyData.isDamaged = false;
            }

            isWorking = !isWorking;
        }
    }

    public class EnemyDead : Command // 적이 죽음
    {
        private GameObject enemyObject; 

        public EnemyDead(GameObject enemyObj)
        {
            enemyObject = enemyObj;
        }

        public override void Execute()
        {
            GameObject.Destroy(enemyObject);
        }
    }
}