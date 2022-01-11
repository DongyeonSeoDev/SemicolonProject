using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // 적 관리 클래스
    {
        public EnemyMoveSO enemyMoveSO; // 적 움직임 관리

        public Transform followObject;

        public float followSpeed = 5f;

        private Command enemyMoveCommand; // 적 움직임 명령어
        private Command enemyFollowPlayerCommand; // 적이 플레이어 따라가는 명령어

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