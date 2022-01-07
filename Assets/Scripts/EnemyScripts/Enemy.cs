using System.Collections;
using UnityEngine;

namespace Enemy
{
    public class Enemy : MonoBehaviour // �� ���� Ŭ����
    {
        public EnemyMoveListSO enemyMoveListSO; // �� ������ ���� ����Ʈ

        public bool isMove = true; // ������ ����

        private Command enemyMoveCommand; // �� ��ɾ�

        private int currrentPositionNumber = 0; // ���� ��ġ ����

        private void Start()
        {
            enemyMoveCommand = new EnemyMove(); // �� ���������� �ʱ�ȭ

            StartCoroutine(EnmeyMove());
        }

        private IEnumerator EnmeyMove() // �� ������
        {
            var enemyMoveList = enemyMoveListSO.enemyMoveList;

            while (isMove) // ������ �� �ִٸ�
            {
                enemyMoveCommand.Execute(transform, enemyMoveList[currrentPositionNumber]); // �� ������ Ŀ��� ����

                // �����̴� �ð� + ��ٸ��� �ð� ���
                yield return new WaitForSeconds(enemyMoveList[currrentPositionNumber].moveTime + enemyMoveList[currrentPositionNumber].moveDelay);

                currrentPositionNumber = (currrentPositionNumber + 1) % enemyMoveList.Count; // ��ġ ���� ����
            }
        }
    }
}