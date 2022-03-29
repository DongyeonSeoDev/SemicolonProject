using UnityEngine;

public class EnemyAttackDistanceGizmos : MonoBehaviour
{
    public float isMinAttackPlayerDistance;
    public float isMaxAttackPlayerDistance;
    public float isRunAwayDistance;

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, isMinAttackPlayerDistance);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, isMaxAttackPlayerDistance);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, isRunAwayDistance);
        }
    }
}
