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
                    Debug.Log("EnemyManager의 instance가 null입니다.");

                    return null;
                }

                return instance;
            }
        }

        public StageCheck stageCheck;
        public int enemyCount;

        public Dictionary<string, List<Enemy>> enemyDictionary = new Dictionary<string, List<Enemy>>();

        public Transform player;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.Log("EnemyManager의 instance가 중복입니다.");

                Destroy(gameObject);
            }

            instance = this;
        }

        public void PlayerDeadEvent()
        {
            foreach(List<Enemy> enemyList in enemyDictionary.Values)
            {
                for (int i = 0; i < enemyList.Count; i++)
                {
                    if (enemyList[i].gameObject.activeSelf)
                    {
                        enemyList[i].gameObject.SetActive(false);
                    }
                }
            }

            enemyDictionary.Clear();
        }

        public void EnemyDestroy()
        {
            enemyCount--;

            if (enemyCount == 0)
            {
                /*if (stageCheck != null)
                {
                    stageCheck.StageClear();
                }*/

                StageManager.Instance.StageClear();
            }
        }
    }
}