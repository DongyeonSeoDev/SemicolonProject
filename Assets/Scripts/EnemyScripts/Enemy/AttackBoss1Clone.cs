using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class AttackBoss1Clone : MonoBehaviour
    {
        private List<GameObject> attackObject = new List<GameObject>();
        private EnemyController eEnemyController;
        private float attackPower;

        public Vector3 direction;

        public void Init(EnemyController controllerm, float power)
        {
            eEnemyController = controllerm;
            attackPower = power;
        }

        public void AttackObjectReset()
        {
            attackObject.Clear();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (attackObject.Find(x => x == collision.gameObject) != null)
            {
                return;
            }

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(gameObject, Random.Range(attackPower - 5, attackPower + 6), transform.position, direction);

                if (enemy != null)
                {
                    enemy.AttackInit(0, false, false);
                }

                attackObject.Add(collision.gameObject);
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                if (enemy != null)
                {
                    (float, bool) damage;
                    damage.Item1 = Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);

                    enemy.GetDamage(damage.Item1, damage.Item2, false, false, transform.position, direction);

                    attackObject.Add(collision.gameObject);
                }
            }
        }
    }
}