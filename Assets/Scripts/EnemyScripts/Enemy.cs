using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // 적 관리 클래스
    {
        public EnemyMoveListSO enemyMoveListSO; // 적 움직임 관리 리스트

        public bool isMove = true; // 움직임 가능

        private Command enemyMoveCommand; // 적 명령어

        private int currrentPositionNumber = 0; // 현재 위치 숫자

        private void Start()
        {
            enemyMoveCommand = new EnemyMove(); // 적 움직임으로 초기화

            StartCoroutine(EnmeyMove());
        }

        private IEnumerator EnmeyMove() // 적 움직임
        {
            var enemyMoveList = enemyMoveListSO.enemyMoveList;

            while (isMove) // 움직일 수 있다면
            {
                enemyMoveCommand.Execute(transform, enemyMoveList[currrentPositionNumber]); // 적 움직임 커멘드 실행

                // 움직이는 시간 + 기다리는 시간 대기
                yield return new WaitForSeconds(enemyMoveList[currrentPositionNumber].moveTime + enemyMoveList[currrentPositionNumber].moveDelay);

                currrentPositionNumber = (currrentPositionNumber + 1) % enemyMoveList.Count; // 위치 숫자 변경
            }
        }
    }
}