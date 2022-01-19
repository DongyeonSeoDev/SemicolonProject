using UnityEngine;

public class EnemyDamagedTest : MonoBehaviour // ���� �������� �޴°� �׽�Ʈ
{
    private int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy.Enemy enemy = collision.gameObject.GetComponent<Enemy.Enemy>();

        if (enemy != null)
        {
            enemy.GetDamage(damage);
            Debug.Log("���� " + damage + "�� �������� �޾ҽ��ϴ�.");
        }
    }
}
