using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackCheck : MonoBehaviour
    {
        private EnemyController eEnemyController = EnemyController.AI;

        private int attackDamage = 0;

        public void Init(EnemyController controller, int damage)
        {
            eEnemyController = controller;
            attackDamage = damage;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
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
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.GetDamage(attackDamage);
                }
            }
        }
    }
}