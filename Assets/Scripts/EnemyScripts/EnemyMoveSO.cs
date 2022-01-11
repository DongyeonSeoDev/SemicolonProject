using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyMove", menuName = "EnemyMove")]
    public class EnemyMoveSO : ScriptableObject
    {
        public List<Vector3> targetPositions = new List<Vector3>(); // �̵��� ��ġ��

        public int currentPositionNumber;  // ���� ��ġ ��ȣ
        public float targetPositionChangeDistance;  // �� �Ÿ����� �� ������ ������ ���� ��ġ�� �̵�
        public float moveSpeed;  // �̵��Ҷ� �ӵ�
    }
}