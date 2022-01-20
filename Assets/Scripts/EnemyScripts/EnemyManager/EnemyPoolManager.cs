using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyPoolManager : MonoBehaviour
    {
        private static EnemyPoolManager instance;

        public static EnemyPoolManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.LogWarning("EnemyPoolManager의 instance가 null 입니다.");

                    return null;
                }

                return instance;
            }
        }


        public EnemyBullet enemyBullet;
        public int count;

        private Queue<EnemyBullet> enemyBulletQueue = new Queue<EnemyBullet>();

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("EnemyPoolManager의 instance가 이미 있습니다.");
            }

            instance = this;
        }

        private void Start()
        {
            for (int i = 0; i < count; i++)
            {
                enemyBulletQueue.Enqueue(MakeEnemyBullet());
            }
        }

        public EnemyBullet GetEnemyBullet(Vector2 position, EnemyController controller, int damage, Vector3 direction)
        {
            EnemyBullet bullet;

            if (enemyBulletQueue.Peek().gameObject.activeSelf)
            {
                bullet = MakeEnemyBullet();
            }
            else
            {
                bullet = enemyBulletQueue.Dequeue();
            }

            bullet.gameObject.SetActive(true);
            bullet.transform.position = position;
            bullet.Init(controller, damage, direction);

            enemyBulletQueue.Enqueue(bullet);

            return bullet;
        }

        private EnemyBullet MakeEnemyBullet()
        {
            EnemyBullet bullet = Instantiate(enemyBullet, transform);
            bullet.gameObject.SetActive(false);

            return bullet;
        }
    }
}