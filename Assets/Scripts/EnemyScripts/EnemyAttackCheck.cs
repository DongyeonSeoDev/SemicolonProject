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
                Player player = collision.GetComponent<Player>();
                
                if (player != null)
                {
                    collision.GetComponent<Player>().GetDamage(attackDamage);
                }
                else
                {
                    collision.GetComponent<EnemyAttackTest>().EnemyAttack(attackDamage);
                }
            }
        }
    }
}