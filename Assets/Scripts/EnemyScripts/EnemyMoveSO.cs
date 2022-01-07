using UnityEngine;

[CreateAssetMenu(fileName = "EnemyMove", menuName = "EnemyMove")]
public class EnemyMoveSO : ScriptableObject // 利 框流烙 包府
{
    public Vector2 targetPosition;

    public float moveTime;
    public float moveDelay;
}