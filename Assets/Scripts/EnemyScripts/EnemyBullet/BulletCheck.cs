using UnityEngine;

namespace Enemy
{
    public class BulletCheck : MonoBehaviour
    {
        private EnemyData enemyData;

        private int bulletLayer;
        public bool isCheck = false;

        public void Init(EnemyData enemyData)
        {
            this.enemyData = enemyData;

            enemyData.addChangeAttackCondition = IsAttackCheck;
            enemyData.endAttack = EndAttack;
        }

        private EnemyState IsAttackCheck()
        {
            if (isCheck)
            {
                return new EnemyAIAttackState(enemyData);
            }

            return null;
        }

        private void EndAttack()
        {
            isCheck = false;
        }

        private void Awake()
        {
            bulletLayer = LayerMask.NameToLayer("PLAYERPROJECTILE");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareLayer(bulletLayer))
            {
                isCheck = true;
            }
        }
    }
}