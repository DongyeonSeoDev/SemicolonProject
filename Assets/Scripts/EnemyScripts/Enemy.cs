using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // �� ���� Ŭ����
    {
        public EnemyMoveSO enemyMoveSO; // �� ������ ����

        public Transform followObject;

        public float followSpeed = 5f;

        private Command enemyMoveCommand; // �� ������ ��ɾ�
        private Command enemyFollowPlayerCommand; // ���� �÷��̾� ���󰡴� ��ɾ�

        private Vector3 targetPosition;

        private bool isFollow = false;

        private void Start()
        {
            enemyMoveCommand = new EnemyMove(enemyMoveSO, transform);
            enemyFollowPlayerCommand = new EnemyFollowPlayer(transform, followObject, followSpeed);
        }

        private void Update()
        {
            if (isFollow)
            {
                enemyFollowPlayerCommand.Execute();
            }
            else
            {
                enemyMoveCommand.Execute();
            }
        }

        public void StartFollow()
        {
            isFollow = true;
        }

        public void EndFollow()
        {
            isFollow = false;
        }
    }
}