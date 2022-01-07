using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMove", menuName = "EnemyMove")]
public class EnemyMoveSO : ScriptableObject // �� ������ ����
{
    public Vector2 targetPosition;

    public float moveTime;
    public float moveDelay;
}