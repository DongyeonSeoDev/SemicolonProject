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
        Slime_03,
        SkeletonArcher_04,
        Boss_SkeletonKing_01,
    }

    public enum TriggerType
    {
        SetTrigger,
        ResetTrigger,
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

        public int enemyCount;

        public LayerMask whatIsEnemy;
        public LayerMask whatIsPlayer;

        public Dictionary<string, List<Enemy>> enemyDictionary = new Dictionary<string, List<Enemy>>();

        private static StageData currentStageData = null;

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

        public static void SetEnemyAnimationDictionary(Dictionary<EnemyAnimationType, int> enemyAnimationDictionary, List<EnemyAnimation> animationList)
        {
            for (int i = 0; i < animationList.Count; i++)
            {
                enemyAnimationDictionary.Add(animationList[i].type, Animator.StringToHash(animationList[i].animationName));
            }
        }

        public static void AnimatorSet(Dictionary<EnemyAnimationType, int> enemyAnimationDictionary, EnemyAnimationType animationType, Animator animator, TriggerType setAnimationtype)
        {
            if (enemyAnimationDictionary.ContainsKey(animationType))
            {
                switch (setAnimationtype)
                {
                    case TriggerType.SetTrigger:
                        animator.SetTrigger(enemyAnimationDictionary[animationType]);
                        break;
                    case TriggerType.ResetTrigger:
                        animator.ResetTrigger(enemyAnimationDictionary[animationType]);
                        break;
                }
            }
        }

        public static void AnimatorSet(Dictionary<EnemyAnimationType, int> enemyAnimationDictionary, EnemyAnimationType animationType, Animator animator, bool value)
        {
            if (enemyAnimationDictionary.ContainsKey(animationType))
            {
                animator.SetBool(enemyAnimationDictionary[animationType], value);
            }
        }

        public static void SpriteFlipCheck(EnemyData enemyData)
        {
            enemyData.enemyRigidbody2D.velocity = Vector2.zero;
            enemyData.enemyRigidbody2D.angularVelocity = 0f;

            if (enemyData.eEnemyController == EnemyController.AI)
            {
                enemyData.moveVector = (EnemyManager.Player.transform.position - enemyData.enemyObject.transform.position).normalized;
            }
            else if (enemyData.eEnemyController == EnemyController.PLAYER)
            {
                enemyData.moveVector.x = Util.MainCam.ScreenToWorldPoint(Input.mousePosition).x - EnemyManager.Player.transform.position.x;
            }

            if (enemyData.enemySpriteRotateCommand != null)
            {
                enemyData.enemySpriteRotateCommand.Execute();
            }
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
                StageManager.Instance.NextEnemy();
            }
        }

        public static Stack<Vector2Int> NextPosition(Vector2 startPosition, Vector2 endPosition)
        {
            Vector2Int start = new Vector2Int(Mathf.RoundToInt(startPosition.x), Mathf.RoundToInt(startPosition.y));
            Vector2Int end = new Vector2Int(Mathf.RoundToInt(endPosition.x), Mathf.RoundToInt(endPosition.y));

            return FindPath.NextPosition(currentStageData, start, end);
        }

        public static void SetCurrentStageData(StageData stageData) => currentStageData = stageData;
        public static void SetEnemyData(Vector2Int targetPosition, bool value) => FindPath.SetEnemyPosition(currentStageData, targetPosition, value);
    }
}