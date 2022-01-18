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
            if (Vector3.Distance(enemyObject.position, followObject.position) >= 1f) // �Ÿ� Ȯ��
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
                enemyData.enemySpriteRenderer.color = Color.green;
            }
            else // ���� ���� ����
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
            Debug.Log("�÷��̾ �������� �Դ� �ڵ� �ۼ�");   
        }
    }

    public class EnemyDeadAIControllerCommand : EnemyCommand // ���� ����
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

    public class EnemyDeadPlayerControllerCommand : EnemyCommand // ���� ����
    {
        public override void Execute()
        {
            Debug.Log("�÷��̾ �״� �ڵ� �ۼ�");
        }
    }
}