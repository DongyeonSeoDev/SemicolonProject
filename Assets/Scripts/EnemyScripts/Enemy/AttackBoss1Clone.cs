using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class AttackBoss1Clone : MonoBehaviour
    {
        private List<GameObject> attackObject = new List<GameObject>();
        private EnemyController eEnemyController;
        private int attackPower;

        public void Init(EnemyController controllerm, int power)
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

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(Random.Range(attackPower - 5, attackPower + 6));

                attackObject.Add(collision.gameObject);
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null)
                {
                    (int, bool) damage;
                    damage.Item1 = Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);

                    enemy.GetDamage(damage.Item1, damage.Item2);

                    attackObject.Add(collision.gameObject);
                }
            }
        }
    }
}