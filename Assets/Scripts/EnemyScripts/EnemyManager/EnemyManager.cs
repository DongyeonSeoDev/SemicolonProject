using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public enum EnemyController
    {
        AI,
        PLAYER
    }

    public enum EnemyType
    {
        Slime_01,
        Rat_02,
        Slime_03
    }

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

        private static GameObject player;
        public static GameObject Player
        {
            get
            {
                if (player == null)
                {
                    player = GameObject.FindGameObjectWithTag("Player");
                }

                return player;
            }

            set => player = value;
        }

        public static readonly int hashIsDie = Animator.StringToHash("isDie");
        public static readonly int hashIsDead = Animator.StringToHash("isDead");
        public static readonly int hashMove = Animator.StringToHash("Move");
        public static readonly int hashAttack = Animator.StringToHash("Attack");
        public static readonly int hashEndAttack = Animator.StringToHash("EndAttack");
        public static readonly int hashHit = Animator.StringToHash("Hit");
        public static readonly int hashReset = Animator.StringToHash("Reset");

        private void Awake()
        {
            if (instance != null)
            {
                Debug.Log("EnemyManager의 instance가 중복입니다.");

                Destroy(gameObject);
            }

            instance = this;
        }

        public static bool IsAttackPlayer(EnemyData data)
        {
            if (data.currentRunAwayTime <= 0)
            {
                data.isRunAway = false;
                data.currentRunAwayTime = 1f;

                return true;
            }

            if (data.isLongDistanceAttack)
            {
                float distance = Vector3.Distance(data.enemyObject.transform.position, Player.transform.position);

                if (data.isRunAway)
                {
                    return data.isRunAwayDistance <= distance && distance <= data.isMaxAttackPlayerDistance;
                }

                return data.isMinAttackPlayerDistance <= distance && distance <= data.isMaxAttackPlayerDistance;
            }
            else
            {
                return Vector3.Distance(data.enemyObject.transform.position, Player.transform.position) <= data.isAttackPlayerDistance;
            }
        }

        public static bool IsRunAway(EnemyData data)
        {
            if (data.isLongDistanceAttack)
            {
                float distance = Vector3.Distance(data.enemyObject.transform.position, Player.transform.position);

                if (data.isRunAway)
                {
                    if (data.isRunAwayDistance <= distance)
                    {
                        data.isRunAway = false;
                    }
                    else
                    {
                        data.currentRunAwayTime -= Time.deltaTime;
                    }

                    return data.isRunAway;
                }
                else if (data.isMinAttackPlayerDistance > distance)
                {
                    data.isRunAway = true;
                    data.currentRunAwayTime = Random.Range(data.minRunAwayTime, data.maxRunAwayTime);

                    return true;
                }
            }

            return false;
        }

        public static bool IsAttackDelay(EnemyData data, float currentAttackDelay = 0)
        {
            if (currentAttackDelay <= 0 || data.isCurrentAttackTime)
            {
                return data.isCurrentAttackTime;
            }

            data.isCurrentAttackTime = true;
            Util.DelayFunc(() => data.isCurrentAttackTime = false, currentAttackDelay);

            return data.isCurrentAttackTime;
        }

        public void PlayerDeadEvent() // 함수 const 변수 여기로 옮기고, Player 버그 해결
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
                StageManager.Instance.StageClear();
            }
        }
    }
}