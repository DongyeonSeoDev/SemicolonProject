using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackCheck : MonoBehaviour
    {
        private int attackDamage = 0;

        public void SetAttackDamage(int damage)
        {
            attackDamage = damage;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<EnemyAttackTest>().EnemyAttack(attackDamage);
            }
        }
    }
}