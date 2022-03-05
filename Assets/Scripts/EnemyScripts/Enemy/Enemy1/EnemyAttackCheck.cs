using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackCheck : MonoBehaviour
    {
        private Enemy enemy;
        private Rigidbody2D enemyRigidbody;
        private PlayerStatusEffect playerStatusEffect;
        private EnemyCommand knockBackCommand;

        private EnemyController eEnemyController;

        private bool isAttackInit = false;
        private bool isKnockBack = false;
        private int attackDamage = 0;

        public Action<EnemyController> enemyControllerChange = null;

        private void Start()
        {
            if (SlimeGameManager.Instance.CurrentPlayerBody != null)
            {
                playerStatusEffect = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerStatusEffect>();
            }
            
            AddEnemyController();
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
            Enemy1 enemy1 = GetComponentInParent<Enemy1>();

            if (enemy1 != null)
            {
                enemy1.InitData(out eEnemyController, out attackDamage);

                enemy = enemy1;

                isKnockBack = false;
            }
            else
            {
                Enemy3 enemy3 = GetComponentInParent<Enemy3>();

                enemy3.InitData(out eEnemyController, out attackDamage);

                enemy = enemy3;
                enemyRigidbody = enemy.GetComponent<Rigidbody2D>();

                isKnockBack = true;
            }

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

            if (isKnockBack && collision.CompareTag("Wall"))
            {
                Debug.Log(collision.name);

                enemyRigidbody.velocity = Vector2.zero;
                enemyRigidbody.angularVelocity = 0f;
            }

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(UnityEngine.Random.Range(attackDamage - 5, attackDamage + 6));

                var enemy = collision.GetComponent<Enemy>();

                if (isKnockBack)
                {
                    Debug.Log(collision.name);

                    enemyRigidbody.velocity = Vector2.zero;
                    enemyRigidbody.angularVelocity = 0f;

                    if (playerStatusEffect != null)
                    {
                        playerStatusEffect.KnockBack(collision.transform.position - transform.position, 20f);
                        playerStatusEffect.Sturn(1f);
                    }
                    else
                    {
                        if (enemy != null)
                        {
                            enemy.GetDamage(0, true, 20f, 1f, collision.transform.position - transform.position);
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
                    enemy.GetDamage(UnityEngine.Random.Range(attackDamage - 5, attackDamage + 6), true);
                }
            }
        }
    }
}