using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackCheck : MonoBehaviour
    {
        public int addAttackValue = 0;
        public float stunTime = 1f;

        private Enemy enemy;
        private Rigidbody2D enemyRigidbody;
        private PlayerStatusEffect playerStatusEffect;
        private EnemyPositionCheckData positionCheckData;

        private List<GameObject> attackObject = new List<GameObject>();

        private EnemyController eEnemyController;

        private bool isAttackInit = false;
        private bool isUseKnockBack = false;
        private float attackPower = 0;

        public Action<EnemyController> enemyControllerChange = null;

        private void Start()
        {
            if (SlimeGameManager.Instance.CurrentPlayerBody != null)
            {
                playerStatusEffect = SlimeGameManager.Instance.Player.GetComponent<PlayerStatusEffect>();
            }
            
            AddEnemyController();
        }

        public void AttackObjectReset()
        {
            attackObject.Clear();
        }

        public void AddEnemyController()
        {
            if (enemyControllerChange == null)
            {
                enemyControllerChange = e =>
                {
                    eEnemyController = e;

                    if (e == EnemyController.PLAYER)
                    {
                        gameObject.layer = LayerMask.NameToLayer("PLAYERPROJECTILE");
                    }
                    else if (e == EnemyController.AI)
                    {
                        gameObject.layer = LayerMask.NameToLayer("ENEMYPROJECTILE");
                    }
                };
            }
        }

        public void Init()
        {
            enemy = GetComponentInParent<Enemy>();

            isUseKnockBack = enemy.GetIsKnockBack();
            positionCheckData = enemy.positionCheckData;
            eEnemyController = enemy.GetEnemyController();
            attackPower = enemy.GetEnemyAttackPower() + addAttackValue;
            enemyRigidbody = enemy.GetComponent<Rigidbody2D>();

            if (eEnemyController == EnemyController.PLAYER)
            {
                gameObject.layer = LayerMask.NameToLayer("PLAYERPROJECTILE");
            }
            else if (eEnemyController == EnemyController.AI)
            {
                gameObject.layer = LayerMask.NameToLayer("ENEMYPROJECTILE");
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isAttackInit)
            {
                isAttackInit = true;

                Init();
            }
            else if (attackObject.Find(x => x == collision.gameObject) != null)
            {
                return;
            }

            if (isUseKnockBack && collision.CompareTag("Wall"))
            {
                enemyRigidbody.velocity = Vector2.zero;
                enemyRigidbody.angularVelocity = 0f;

                positionCheckData.isWall = true;
                positionCheckData.oppositeDirectionWall = (collision.transform.position - transform.position).normalized;
            }

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                attackObject.Add(collision.gameObject);

                var enemy = collision.GetComponent<Enemy>();
                var hit = Physics2D.Raycast(transform.position, (collision.transform.position - transform.position).normalized, Vector2.Distance(collision.transform.position, transform.position) + 1f, EnemyManager.Instance.whatIsPlayer);

                if (isUseKnockBack)
                {
                    SlimeGameManager.Instance.Player.GetDamage(gameObject, UnityEngine.Random.Range(attackPower - 5, attackPower + 6), hit.point, positionCheckData.position, new Vector3(1.5f, 1.5f, 1.5f));

                    enemyRigidbody.velocity = Vector2.zero;
                    enemyRigidbody.angularVelocity = 0f;

                    if (enemy != null)
                    {
                        enemy.AttackInit(0, true, true, positionCheckData.position, 30f, 0.7f);
                    }
                    else
                    {
                        playerStatusEffect.KnockBack(positionCheckData.position, 50f, 0.1f);
                        playerStatusEffect.Sturn(stunTime);
                    }
                }
                else
                {
                    if (enemy != null)
                    {
                        SlimeGameManager.Instance.Player.GetDamage(gameObject, UnityEngine.Random.Range(attackPower - 5, attackPower + 6), hit.point, enemy.transform.position - this.enemy.transform.position);
                        enemy.AttackInit(0, false, false);
                    }
                    else
                    {
                        SlimeGameManager.Instance.Player.GetDamage(gameObject, UnityEngine.Random.Range(attackPower - 5, attackPower + 6), hit.point, EnemyManager.Player.transform.position - this.enemy.transform.position);
                    }
                }
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                var hit = Physics2D.Raycast(transform.position, (collision.transform.position - transform.position).normalized, Vector2.Distance(collision.transform.position, transform.position) + 1f, EnemyManager.Instance.whatIsEnemy);

                if (enemy != null && enemy != this.enemy)
                {
                    (float, bool) damage;
                    damage.Item1 = UnityEngine.Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MinDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);
                    
                    enemy.GetDamage(damage.Item1, damage.Item2, false, false, hit.point, enemy.transform.position - this.enemy.transform.position);
                    attackObject.Add(collision.gameObject);

                    EventManager.TriggerEvent("OnEnemyAttack");
                }
                else
                {
                    EventManager.TriggerEvent("OnAttackMiss");
                }
            }
        }
    }
}