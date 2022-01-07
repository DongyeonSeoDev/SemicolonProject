using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMove", menuName = "EnemyMove")]
public class EnemyMoveSO : ScriptableObject // 적 움직임 관리
{
    public Vector2 targetPosition;

    public float moveTime;
    public float moveDelay;
}