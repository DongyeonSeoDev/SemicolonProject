using UnityEngine;

public class EnemyBullet2 : MonoBehaviour
{
    public Transform targetTransform;
    public float distance;
    public float startAngle;
    public float angleSpeed;

    private float angle;

    private void Start()
    {
        angle = startAngle;
    }

    private void Update()
    {
        transform.position = targetTransform.position + Quaternion.Euler(0f, 0f, angle) * Vector2.right * distance;

        angle += Time.deltaTime * angleSpeed;
    }
}
