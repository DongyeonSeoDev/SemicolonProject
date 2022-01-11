using UnityEngine;

namespace Enemy
{
    public class EnemyMove : Command // �� ������
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

    public class EnemyFollowPlayer : Command // �� ������
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
            if (Vector3.Distance(enemyObject.position, followObject.position) >= 1f) // �Ÿ� Ȯ��
            {
                // �̵�
                targetPosition = (followObject.position - enemyObject.position).normalized;
                targetPosition *= followSpeed * Time.deltaTime;

                enemyObject.position += targetPosition;
            }
        }
    }

    public class EnemyAttack : Command // �� ���� TODO: ���� ������ ��
    {
        public EnemyAttack()
        {
            
        }

        public override void Execute()
        {
            
        }
    }

    public class EnemyGetDamaged : Command // ���� �������� ���� TODO: ������ �޴°� ������ ��
    {
        public EnemyGetDamaged()
        {

        }

        public override void Execute()
        {

        }
    }

    public class EnemyDead : Command // ���� ���� TODO: ���� �״°� ������ ��
    {
        public EnemyDead()
        {

        }

        public override void Execute()
        {

        }
    }
}