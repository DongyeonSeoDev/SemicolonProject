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
                    Debug.LogWarning("EnemyPoolManager�� instance�� null �Դϴ�.");

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
                Debug.LogWarning("EnemyPoolManager�� instance�� �̹� �ֽ��ϴ�.");
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