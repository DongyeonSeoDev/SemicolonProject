using UnityEngine;

namespace Enemy
{
    public class Enemy3 : Enemy
    {
        private EnemyCommand enemyAttackCommand;
        private Rigidbody2D rb2d;

        protected override void Awake()
        {
            enemyData = new EnemyData()
            {
                eEnemyController = EnemyController.AI,
                enemyObject = gameObject,
                enemyMoveSO = enemyMoveSO,
                enemyLootList = enemyLootListSO,
                enemyAnimator = GetComponent<Animator>(),
                enemySpriteRenderer = GetComponent<SpriteRenderer>(),
                hpBarFillImage = hpBarFillImage,
                normalColor = Color.white,
                damagedColor = Color.red,
                enemyId = "Slime_03",
                isAnimation = false,
                isEndAttackAnimation = true,
                isSeePlayerDistance = 20f,
                isAttackPlayerDistance = 7f,
                attackDelay = 3f,
                chaseSpeed = 2f,
                attackDamage = 20,
                maxHP = 50,
                hp = 50
            };

            rb2d = GetComponent<Rigidbody2D>();

            enemyAttackCommand = new EnemyRushAttackCommand(rb2d, transform, enemyData.PlayerObject.transform, enemyData.rushForce);

            base.Awake();
        }

        private void Start()
        {
            EnemyAttackCheck enemyAttackCheck = transform.GetComponentInChildren<EnemyAttackCheck>();

            if (enemyAttackCheck != null)
            {
                enemyAttackCheck.Init(enemyData.eEnemyController, enemyData.attackDamage);
                enemyAttackCheck.gameObject.SetActive(false);
            }
        }

        public void EnemyAttack()
        {
            enemyAttackCommand.Execute();
        }
    }
}