using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;

[Serializable]
public struct ChangeBodyData
{
    public string bodyName;
    public Enemy.EnemyType bodyId;
    public GameObject body;
    public EternalStat additionalBodyStat; // 변신 후의 플레이어의 Additional스탯, (이해도 100% 기준)
    public Sprite bodyImg;
    public ItemSO dropItem;
    public string bodyExplanation;
}
public class PlayerEnemyUnderstandingRateManager : MonoSingleton<PlayerEnemyUnderstandingRateManager>
{
    private Dictionary<string, int> playerEnemyUnderStandingRateDict  = new Dictionary<string, int>();
    /// <summary>
    /// (공허의 유산) -> "자~ 잠시 개입하겠어요. 이거 쓰지 말고, GetUnderstandingRate나 SetUnderstandingRate를! 사용하라 맨이야."
    /// </summary>
    public Dictionary<string, int> PlayerEnemyUnderStandingRateDic
    {
        get { return playerEnemyUnderStandingRateDict ; }
    }

    [SerializeField]
    private List<ChangeBodyData> changableBodyList = new List<ChangeBodyData>();
    public List<ChangeBodyData> ChangableBodyList
    {
        get { return changableBodyList; }
    }

    private Dictionary<string, (GameObject, EternalStat)> changableBodyDict = new Dictionary<string, (GameObject, EternalStat)>();
    public Dictionary<string, (GameObject, EternalStat)> ChangalbeBodyDict
    {
        get { return changableBodyDict; }
    }

    private Dictionary<string, float> drainProbabilityDict = new Dictionary<string, float>();
    public Dictionary<string, float> DrainProbabilityDict
    {
        get { return drainProbabilityDict; }
    }

    [SerializeField]
    private int canMountObjNum = 2;

    private List<string> mountedObjList = new List<string>(); // 장착한 오브젝트들의 아이디관련 List
    public List<string> MountedObjList
    {
        get { return mountedObjList; }
    }

    private Dictionary<string, Queue<int>> willUpUnderstandingRateDict = new Dictionary<string, Queue<int>>(); // 후에 올라갈 이해도의 몹 아이디값과 올라갈 수치값

    [Header("이해도를 어디까지 올릴 수 있는가")]
    [SerializeField]
    private int maxUnderstandingRate = 120;
    public int MaxUnderstandingRate
    {
        get { return maxUnderstandingRate; }
    }

    private void Awake()
    {
        changableBodyDict.Clear();

        changableBodyList.ForEach(x =>
        {
            // x.bodyScript = x.body.GetComponent<Enemy.Enemy>();
            changableBodyDict.Add(x.bodyId.ToString(), (x.body, x.additionalBodyStat));
            //playerEnemyUnderStandingRateDict.Add(x.bodyId.ToString(), 120);

            // Debug.Log(enemyId);
        });
    }
    void Start()
    {
        // SetUnderstandingRate("Enemy", 0); // For Test
        EventManager.StartListening("PlayerDead", ResetDicts);
        EventManager.StartListening("PlayerBodySet", MountBody);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", ResetDicts);
        EventManager.StopListening("PlayerBodySet", MountBody);
    }
    public void SetDrainProbabilityDict(string key, float value) 
    {
        if (drainProbabilityDict.ContainsKey(key))
        {
            drainProbabilityDict[key] = value;
        }
        else
        {
            drainProbabilityDict.Add(key, value);
        }

        MonsterCollection.Instance.UpdateDrainProbability(key);
    }
    public float GetDrainProbabilityDict(string key)
    {
        if (drainProbabilityDict.ContainsKey(key))
        {
            return drainProbabilityDict[key];
        }
        else
        {
            ////Debug.LogWarning("The key '" + key + "' is not Contain.");                                              
            return 0f;
        }
    }
    public void UpDrainProbabilityDict(string key, float upValue)
    {
        SetDrainProbabilityDict(key, GetDrainProbabilityDict(key) + upValue);
    }
    public void SetUnderstandingRate(string key, int value)
    {
        if (playerEnemyUnderStandingRateDict.ContainsKey(key))
        {
            playerEnemyUnderStandingRateDict [key] = value;
        }
        else
        {
            playerEnemyUnderStandingRateDict.Add(key, value);
        }

        MonsterCollection.Instance.UpdateUnderstanding(key);
    }
    public int GetUnderstandingRate(string key)
    {
        if (playerEnemyUnderStandingRateDict.ContainsKey(key))
        {
            return playerEnemyUnderStandingRateDict [key];
        }
        else
        {
            Debug.LogWarning("The key '" + key + "' is not Contain.");

            return 0;
        }
    }
    public void ResetDicts()
    {
        for (int i = 0; i < playerEnemyUnderStandingRateDict .Count; i++)
        {
            playerEnemyUnderStandingRateDict [changableBodyList[i].bodyId.ToString()] = 0;
        }

        mountedObjList.Clear();
    }
    public bool CheckCanMountObj()
    {
        return mountedObjList.Count < canMountObjNum;
    }
    public void SetMountObj(string objId, int idx = -1)
    {
        if(mountedObjList.Contains(objId))
        {
            Debug.Log(objId + "는 이미 장착한 오브젝트입니다.");

            return;
        }

        if (idx <= -1)
        {
            if (CheckCanMountObj())
            {
                mountedObjList.Add(objId);
            }
            else
            {
                //EventManager.TriggerEvent("TimePause");
                UIManager.Instance.OnUIInteract(UIType.CHANGEABLEMOBLIST, true);
            }
            MonsterCollection.Instance.AddSavedBody(objId);
            MonsterCollection.Instance.AddBody(objId);
        }
        else
        {
            mountedObjList[idx] = objId;
            MonsterCollection.Instance.AddSavedBody(objId, idx + 2);
            MonsterCollection.Instance.AddBody(objId, idx + 1);
        }

        SetDrainProbabilityDict(objId, 0);
    }
    public void UnSetMountObj(string objId)
    {
        mountedObjList.Remove(objId);
    }
    public bool CheckMountObjIdContain(string objId)
    {
        return mountedObjList.Contains(objId);
    }
    public void CheckMountingEnemy(string objId, int upValue)
    {
        if (!DrainProbabilityDict.ContainsKey(objId))
        {
            return;
        }

        if(mountedObjList.Contains(objId))
        {
            UpUnderstandingRateWithQueue(objId, upValue);

            return;
        }

        float value = UnityEngine.Random.Range(0f, 100f);

        if (value <= GetDrainProbabilityDict(objId)) // 확률 체크, 예외처리
        {

            // 장착을 물어봄
            //UIManager.Instance.DoChangeBody(objId);
            //Debug.Log("장착 물어보는 창이 뜨네요");

            UpUnderstandingRateWithQueue(objId, upValue);
            SetDrainProbabilityDict(objId, 0); // 장착을 했건 안했건 확률은 0이된다
            UIManager.Instance.SaveMonsterBody(objId);
        }
    }

    private void UpUnderstandingRateWithQueue(string objId, int upValue)
    {
        if (willUpUnderstandingRateDict.ContainsKey(objId))
        {
            willUpUnderstandingRateDict[objId].Enqueue(upValue);
        }
        else
        {
            Queue<int> queue = new Queue<int>();
            queue.Enqueue(upValue);

            willUpUnderstandingRateDict.Add(objId, queue);
        }
    }

    public void MountBody(string objId, bool mountIt)
    {
        int upValue = willUpUnderstandingRateDict[objId].Dequeue();

        if(mountIt)
        {
            SetMountObj(objId);
            UpUnderstandingRate(objId, upValue);
        }
    }
    public void UpUnderstandingRate(string objId, int upValue) // 이해도(동화율)을 올려줌
    {
        int u = GetUnderstandingRate(objId) + upValue;

        if (u > MaxUnderstandingRate) // 최대치 처리
        {
            SetUnderstandingRate(objId, MaxUnderstandingRate);
        }
        else
        {
            SetUnderstandingRate(objId, u);
        }
    }
}
