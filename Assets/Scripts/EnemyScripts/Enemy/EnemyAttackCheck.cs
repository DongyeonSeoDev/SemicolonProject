using System;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackCheck : MonoBehaviour
    {
        private Enemy enemy;
        private Rigidbody2D enemyRigidbody;
        private PlayerStatusEffect playerStatusEffect;
        private EnemyPositionCheckData positionCheckData;

        private List<GameObject> attackObject = new List<GameObject>();

        private EnemyController eEnemyController;

        private bool isAttackInit = false;
        private bool isUseKnockBack = false;
        private int attackPower = 0;

        public Action<EnemyController> enemyControllerChange = null;

        private void Start()
        {
            if (SlimeGameManager.Instance.CurrentPlayerBody != null)
            {
                playerStatusEffect = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerStatusEffect>();
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
            attackPower = enemy.GetEnemyAttackPower();
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
                SlimeGameManager.Instance.Player.GetDamage(this.enemy.gameObject, UnityEngine.Random.Range(attackPower - 5, attackPower + 6));
                attackObject.Add(collision.gameObject);

                var enemy = collision.GetComponent<Enemy>();

                if (isUseKnockBack)
                {
                    enemyRigidbody.velocity = Vector2.zero;
                    enemyRigidbody.angularVelocity = 0f;

                    if (playerStatusEffect != null)
                    {
                        playerStatusEffect.KnockBack(positionCheckData.position, 50f, 0.1f);
                        playerStatusEffect.Sturn(1f);
                    }
                    else
                    {
                        if (enemy != null)
                        {
                            enemy.GetDamage(0, false, false, 30f, 1f, positionCheckData.position);
                        }
                    }
                }
                else
                {
                    if (enemy != null)
                    {
                        enemy.GetDamage(0);
                    }
                }
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null && enemy != this.enemy)
                {
                    (int, bool) damage;
                    damage.Item1 = UnityEngine.Random.Range(SlimeGameManager.Instance.Player.PlayerStat.MaxDamage, SlimeGameManager.Instance.Player.PlayerStat.MaxDamage + 1);
                    damage = SlimeGameManager.Instance.Player.CriticalCheck(damage.Item1);
                    
                    enemy.GetDamage(damage.Item1, damage.Item2);
                    attackObject.Add(collision.gameObject);
                }
            }
        }
    }
}