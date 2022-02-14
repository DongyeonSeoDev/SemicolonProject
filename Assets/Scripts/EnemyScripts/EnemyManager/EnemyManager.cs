using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        private static EnemyManager instance;
        public static EnemyManager Instance
        {
            get
            {
                if (instance == null)
                {
                    Debug.Log("EnemyManager�� instance�� null�Դϴ�.");

                    return null;
                }

                return instance;
            }
        }

        public StageCheck stageCheck;
        public int enemyCount;

        public List<List<Enemy>> enemyList = new List<List<Enemy>>();

        public Transform player;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.Log("EnemyManager�� instance�� �ߺ��Դϴ�.");

                Destroy(gameObject);
            }

            instance = this;
        }

        public void PlayerDeadEvent()
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                for (int j = 0; j < enemyList[i].Count; j++)
                {
                    if (enemyList[i][j].gameObject.activeSelf)
                    {
                        enemyList[i][j].gameObject.SetActive(false);
                    }
                }
            }

            enemyList.Clear();
        }

        public void EnemyDestroy()
        {
            enemyCount--;

            if (enemyCount == 0)
            {
                if (stageCheck != null)
                {
                    stageCheck.StageClear();
                }
            }
        }
    }
}