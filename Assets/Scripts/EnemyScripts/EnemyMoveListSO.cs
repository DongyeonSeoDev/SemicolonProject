using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMoveList", menuName = "EnemyMoveList")]
public class EnemyMoveListSO : ScriptableObject
{
    public List<EnemyMoveSO> enemyMoveList; // �� ������ ����Ʈ ����
}