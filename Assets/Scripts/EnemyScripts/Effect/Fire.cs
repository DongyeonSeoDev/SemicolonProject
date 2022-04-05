using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class Fire : EnemyPoolData
    {
        public static List<GameObject> checkAttackObjectTogether = new List<GameObject>();

        private Animator animator;
        private Collider2D attackCollider;
        private Enemy enemyCheck;
        private EnemyController eEnemyController;
        private int attackDamage;
        private bool checkTogether;

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
            EventManager.StartListening("PlayerSetActiveFalse", PlayerDeadEvent);
        }

        public void Spawn(Enemy enemy, EnemyController controller, int damage, float attackTime, bool checkTogether)
        {
            animator.ResetTrigger(hashAttack);
            animator.SetTrigger(hashReset);

            enemyCheck = enemy;
            eEnemyController = controller;
            attackDamage = damage;
            this.checkTogether = checkTogether;

            if (!checkTogether)
            {
                AttackObjectReset();
            }

            if (attackTime > 0)
            {
                Util.DelayFunc(Attack, attackTime);
            }
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

        public void PlayerDeadEvent()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (checkTogether)
            {
                if (checkAttackObjectTogether.Find(x => x == collision.gameObject) != null)
                {
                    return;
                }
            }
            else
            {
                if (attackObject.Find(x => x == collision.gameObject) != null)
                {
                    return;
                }
            }

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(Random.Range(attackDamage - 5, attackDamage + 6));

                if (checkTogether)
                {
                    checkAttackObjectTogether.Add(collision.gameObject);
                }
                else
                {
                    attackObject.Add(collision.gameObject);
                }
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

                    if (checkTogether)
                    {
                        checkAttackObjectTogether.Add(collision.gameObject);
                    }
                    else
                    {
                        attackObject.Add(collision.gameObject);
                    }
                }
            }
        }
    }
}
