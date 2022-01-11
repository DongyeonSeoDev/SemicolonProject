using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    [CreateAssetMenu(fileName = "EnemyMove", menuName = "EnemyMove")]
    public class EnemyMoveSO : ScriptableObject
    {
        public List<Vector3> targetPositions = new List<Vector3>(); // 이동할 위치들

        public int currentPositionNumber;  // 현재 위치 번호
        public float targetPositionChangeDistance;  // 이 거리보다 더 가까이 있으면 다음 위치로 이동
        public float moveSpeed;  // 이동할때 속도
    }
}