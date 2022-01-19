using UnityEngine;

public class EnemyDamagedTest : MonoBehaviour // 적이 데미지를 받는것 테스트
{
    private int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy.Enemy enemy = collision.gameObject.GetComponent<Enemy.Enemy>();

        if (enemy != null)
        {
            enemy.GetDamage(damage);
            Debug.Log("적이 " + damage + "의 데미지를 받았습니다.");
        }
    }
}
