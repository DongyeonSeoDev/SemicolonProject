using UnityEngine;
using DG.Tweening;
using Enemy;

public class EnemyMove : Command // �� ������
{
    public override void Execute(Transform enemyPosition, EnemyMoveSO enemyMoveSO)
    {
        // �� �����̴� �Լ�
        enemyPosition.DOMove(enemyMoveSO.targetPosition, enemyMoveSO.moveTime); // TODO .SetEase ����ؼ� �� �������� �ε巴�� ������ ����
    }
}
