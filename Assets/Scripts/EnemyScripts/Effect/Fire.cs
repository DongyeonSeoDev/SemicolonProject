using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Enemy
{
    public class Fire : EnemyPoolData
    {
        public static List<GameObject> checkAttackObjectTogether = new List<GameObject>();

        public Transform attackRange = null;
        public Vector3 targetAttackRangeScale = new Vector3(2f, 2f, 1f);

        private Animator animator;
        private SpriteRenderer attackRangeSprite = null;
        private Enemy enemyCheck;

        private EnemyController eEnemyController;
        private Color currentAttackRangeColor;
        private Color targetAttackRangeColor;

        private float attackPower;
        private bool checkTogether;

        private readonly int hashAttack = Animator.StringToHash("Attack");
        private readonly int hashReset = Animator.StringToHash("Reset");

        private List<GameObject> attackObject = new List<GameObject>();

        private void Awake()
        {
            animator = GetComponent<Animator>();
            attackRangeSprite = attackRange.GetComponent<SpriteRenderer>();

            currentAttackRangeColor = attackRangeSprite.color;
            targetAttackRangeColor = currentAttackRangeColor;
            targetAttackRangeColor.a = 1;
        }

        private void Start()
        {
            EventManager.StartListening("PlayerSetActiveFalse", PlayerDeadEvent);
            EventManager.StartListening("BossDead", PlayerDeadEvent);
        }

        public void Spawn(Enemy enemy, EnemyController controller, float power, float attackTime, bool checkTogether)
        {
            animator.ResetTrigger(hashAttack);
            animator.SetTrigger(hashReset);

            enemyCheck = enemy;
            eEnemyController = controller;
            attackPower = power;
            this.checkTogether = checkTogether;

            if (!checkTogether)
            {
                AttackObjectReset();
            }

            if (attackTime > 0)
            {
                attackRange.gameObject.SetActive(true);
                attackRange.localScale = Vector3.zero;

                attackRange.DOScale(targetAttackRangeScale, attackTime - 0.2f).OnComplete(() =>
                {
                    Attack();
                });
            }
            else
            {
                attackRange.gameObject.SetActive(true);
            }
        }

        public void Attack()
        {
            attackRangeSprite.color = targetAttackRangeColor;

            Util.DelayFunc(() =>
            {
                attackRange.gameObject.SetActive(false);
                attackRangeSprite.color = currentAttackRangeColor;

                animator.ResetTrigger(hashReset);
                animator.SetTrigger(hashAttack);
            }, 0.2f);
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

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(gameObject, Random.Range(attackPower - 5, attackPower + 6), transform.position, EnemyManager.Player.transform.position - transform.position, Vector3.one);

                if (enemy != null && enemy != enemyCheck)
                {
                    enemy.GetDamage(0, false, false, false, EnemyManager.Player.transform.position - transform.position, transform.position, false);
                }

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
                if (enemy != null && enemy != enemyCheck)
                {
                    (float, bool) damage;
                    damage.Item1 = Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);

                    enemy.GetDamage(damage.Item1, damage.Item2, false, false, EnemyManager.Player.transform.position - transform.position, transform.position);

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