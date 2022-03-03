using System;
using UnityEngine;

namespace Enemy
{
    public class EnemyAttackCheck : MonoBehaviour
    {
        private Enemy enemy;
        private PlayerStatusEffect playerStatusEffect;

        private EnemyController eEnemyController;

        private bool isAttackInit = false;
        private bool isKnockBack = false;
        private int attackDamage = 0;

        public Action<EnemyController> enemyControllerChange = null;

        private void Start()
        {
            playerStatusEffect = SlimeGameManager.Instance.CurrentPlayerBody.GetComponent<PlayerStatusEffect>();

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

            if (eEnemyController == EnemyController.AI && collision.CompareTag("Player"))
            {
                SlimeGameManager.Instance.Player.GetDamage(UnityEngine.Random.Range(attackDamage - 5, attackDamage + 6));

                if (isKnockBack)
                {
                    playerStatusEffect.KnockBack((Vector2)(collision.transform.position - transform.position), 20f);
                    playerStatusEffect.Sturn(1f);
                }
            }
            else if (eEnemyController == EnemyController.PLAYER)
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();

                if (enemy != null && enemy != this.enemy)
                {
                    enemy.GetDamage(UnityEngine.Random.Range(attackDamage - 5, attackDamage + 6));
                }
            }
        }
    }
}