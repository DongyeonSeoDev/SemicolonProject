using UnityEngine;
using DG.Tweening;
using Enemy;

public class EnemyMove : Command // 적 움직임
{
    public override void Execute(Transform enemyPosition, EnemyMoveSO enemyMoveSO)
    {
        // 적 움직이는 함수
        enemyPosition.DOMove(enemyMoveSO.targetPosition, enemyMoveSO.moveTime); // TODO .SetEase 사용해서 적 움직임을 부드럽게 변경할 예정
    }
}
