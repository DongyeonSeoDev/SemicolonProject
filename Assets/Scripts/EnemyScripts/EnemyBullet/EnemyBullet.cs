using UnityEngine;

namespace Enemy
{
    public class EnemyBullet : MonoBehaviour
    {
        public float speed;

        private EnemyController eEnemyController;

        private Vector3 distance = Vector3.zero;

        private int attackDamage;

        private void Update()
        {
            transform.position += distance * speed * Time.deltaTime;
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

        public void Init(EnemyController controller, int damage)
        {
            eEnemyController = controller;
            attackDamage = damage;
        }
    }
}