using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamagedTest : MonoBehaviour
{
    private int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy.Enemy enemy = collision.gameObject.GetComponent<Enemy.Enemy>();

        if (enemy != null)
        {
            enemy.GetDamage(damage);
        }
    }
}
