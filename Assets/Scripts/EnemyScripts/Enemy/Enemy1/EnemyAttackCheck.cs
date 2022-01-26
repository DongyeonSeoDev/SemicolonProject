using UnityEngine;

namespace Enemy
{
    public class EnemyAttackCheck : MonoBehaviour
    {
        private EnemyController eEnemyController = EnemyController.AI;

        private bool isAttackInit = false;
        private int attackDamage = 0;

        public void Init()
        {
            Enemy1 enemy1 = GetComponentInParent<Enemy1>();

            if (enemy1 != null)
            {
                enemy1.InitData(out eEnemyController, out attackDamage);
            }
            else
            {
                Enemy3 enemy3 = GetComponentInParent<Enemy3>();

                enemy3.InitData(out eEnemyController, out attackDamage);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isAttackInit)
            {
                isAttackInit = true;

                Init();
            }

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