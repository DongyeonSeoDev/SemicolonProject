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
    [TextArea] public string bodyExplanation;
    [TextArea] public string featureExplanation;
    [TextArea] public string hint;
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
    private Dictionary<string, EternalStat> currentExtraStatDict = new Dictionary<string, EternalStat>();// 

    [Header("이해도를 어디까지 올릴 수 있는가")]
    [SerializeField]
    private int maxUnderstandingRate = 120;
    public int MaxUnderstandingRate
    {
        get { return maxUnderstandingRate; }
    }

    [Header("동화율 몇퍼당 변신시 능력치가 오를지를 정하는 변수")]
    [SerializeField]
    private int understandingRatePercentageWhenUpStat = 10;
    public float UnderstandingRatePercentageWhenUpStat
    {
        get { return understandingRatePercentageWhenUpStat; }
    }

    [Header("동화율 'understadingRatePercentageWhenUpStat'퍼당 변신시 오르게되는 능력치가 오르게 되는 수치(배율)")]
    [SerializeField]
    private float upStatPercentage = 0.05f;
    public float UpStatPercentage
    {
        get { return upStatPercentage; }
    }

    private Dictionary<string, int> currentUpNewBodyStat = new Dictionary<string, int>();

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

        #region 몸체의 동화율에의한 슬라임 스탯 증가 처리

        int pasteUpNewBodyStat = 0;
        if (currentExtraStatDict.ContainsKey(key))
        {
            pasteUpNewBodyStat = currentUpNewBodyStat[key];
        }

        EternalStat extraStat = GetExtraUpStat(key);

        if (currentExtraStatDict.ContainsKey(key))
        {
            if (currentExtraStatDict[key] != extraStat)
            {
                SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.Sub(currentExtraStatDict[key]);

                currentExtraStatDict[key] = extraStat;

                SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.Sum(currentExtraStatDict[key]);
                
                if (pasteUpNewBodyStat != currentUpNewBodyStat[key])
                {
                    UIManager.Instance.UpdatePlayerHPUI();
                    UIManager.Instance.RequestLogMsg("'" + MonsterCollection.Instance.GetMonsterInfo(key).bodyName + "'의 동화율 값이 변함에 따라, 스탯이 변경되었습니다.");
                }
            }
        }
        else
        {
            currentExtraStatDict.Add(key, extraStat);

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.Sum(currentExtraStatDict[key]);

            if (currentUpNewBodyStat[key] > 0)
            {
                UIManager.Instance.UpdatePlayerHPUI();
                UIManager.Instance.RequestLogMsg("'" + MonsterCollection.Instance.GetMonsterInfo(key).bodyName + "'의 동화율이 상승함에 따라, 스탯이 변경되었습니다.");
            }
        }

        #endregion

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
            playerEnemyUnderStandingRateDict.Add(key, 0);

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
        currentExtraStatDict.Clear();
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
                MonsterCollection.Instance.AddBody(objId);
                return;
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
    public (bool, float) CheckMountingEnemy(string objId, int upValue)
    {
        float drainPercentage = GetDrainProbabilityDict(objId);

        if (!DrainProbabilityDict.ContainsKey(objId))
        {
            return (false, drainPercentage);
        }

        if(mountedObjList.Contains(objId))
        {
            UpUnderstandingRateWithQueue(objId, upValue);

            return (false, drainPercentage);
        }

        float value = UnityEngine.Random.Range(0f, 100f);

        if (value <= drainPercentage) // 확률 체크, 예외처리
        {
            UpUnderstandingRateWithQueue(objId, upValue);
            SetDrainProbabilityDict(objId, 0);
            UIManager.Instance.SaveMonsterBody(objId);

            return (true, drainPercentage);
        }

        return (false, drainPercentage);
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
    public EternalStat GetExtraUpStat(string objId)
    {
        EternalStat result = new EternalStat();
        EternalStat upStat = ChangalbeBodyDict[objId].Item2;

        if (ChangalbeBodyDict.ContainsKey(objId))
        {
            int upNewBodyStat = (GetUnderstandingRate(objId) / understandingRatePercentageWhenUpStat);

            if (upNewBodyStat >= 1) // this code is "imsi" code that inserted "imsi" values.
            {

                result = (upStat * upNewBodyStat * upStatPercentage);// 10% 마다 upStatPercentage배씩 상승
            }

            if (currentUpNewBodyStat.ContainsKey(objId))
            {
                currentUpNewBodyStat[objId] = upNewBodyStat;
            }
            else
            {
                currentUpNewBodyStat.Add(objId, upNewBodyStat);
            }
        }
        else
        {
            Debug.LogWarning(objId + " is not on the changableBodyList!");
        }

        return result;
    }
}
