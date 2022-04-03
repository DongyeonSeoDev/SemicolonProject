using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Fire : EnemyPoolData
    {
        private Animator animator;
        private Collider2D attackCollider;
        private Enemy enemyCheck;
        private EnemyController eEnemyController;
        private int attackDamage;

        private readonly int hashAttack = Animator.StringToHash("Attack");
        private readonly int hashReset = Animator.StringToHash("Reset");

        private List<GameObject> attackObject = new List<GameObject>();

        private void Awake()
        {
            animator = GetComponent<Animator>();
            attackCollider = GetComponent<Collider2D>();
        }

        private void Start()
        {
            Spawn(null, EnemyController.AI, 30, 3f); // Debug Code
        }

        public void Spawn(Enemy enemy, EnemyController controller, int damage, float attackTime)
        {
            animator.ResetTrigger(hashAttack);
            animator.SetTrigger(hashReset);

            enemyCheck = enemy;
            eEnemyController = controller;
            attackDamage = damage;

            AttackObjectReset();

            Util.DelayFunc(Attack, attackTime);
        }

        public void Attack()
        {
            animator.ResetTrigger(hashReset);
            animator.SetTrigger(hashAttack);
        }

        public void AnimationEnd()
        {
            gameObject.SetActive(false);
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
                SlimeGameManager.Instance.Player.GetDamage(Random.Range(attackDamage - 5, attackDamage + 6));

                attackObject.Add(collision.gameObject);
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null && enemy != enemyCheck)
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
