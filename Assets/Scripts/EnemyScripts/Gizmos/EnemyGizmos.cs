using UnityEngine;

public class EnemyAngleGizmos : MonoBehaviour
{
    private Transform playerTransform;
    private Vector3 targetPosition;
    private float angle;

    public float addAngle = 10f;

    private void Start()
    {
        playerTransform = SlimeGameManager.Instance.CurrentPlayerBody.transform;
    }

    private void OnDrawGizmos()
    {
        targetPosition = transform.position - playerTransform.position;

        angle = Mathf.Atan2(targetPosition.x, targetPosition.y) * Mathf.Rad2Deg + 90f;

        targetPosition.x = playerTransform.position.x + (10000 * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
        targetPosition.y = playerTransform.position.y + (10000 * Mathf.Sin(angle * Mathf.Deg2Rad));
        targetPosition.z = playerTransform.position.z;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, ((targetPosition - transform.position).normalized * 100f));

        angle = (angle + addAngle) % 360;

        targetPosition.x = playerTransform.position.x + (10000 * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
        targetPosition.y = playerTransform.position.y + (10000 * Mathf.Sin(angle * Mathf.Deg2Rad));
        targetPosition.z = playerTransform.position.z;

        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, ((targetPosition - transform.position).normalized * 100f));

        angle = (angle - (addAngle * 2)) % 360;

        targetPosition.x = playerTransform.position.x + (10000 * Mathf.Cos(angle * Mathf.Deg2Rad) * -1f);
        targetPosition.y = playerTransform.position.y + (10000 * Mathf.Sin(angle * Mathf.Deg2Rad));
        targetPosition.z = playerTransform.position.z;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, ((targetPosition - transform.position).normalized * 100f));
    }
}