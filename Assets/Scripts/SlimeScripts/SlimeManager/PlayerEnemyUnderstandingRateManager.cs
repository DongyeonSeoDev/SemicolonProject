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
    private Dictionary<string, int> playerEnemyUnderStandingRateDic = new Dictionary<string, int>();
    public Dictionary<string, int> PlayerEnemyUnderStandingRateDic
    {
        get { return playerEnemyUnderStandingRateDic; }
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

    private Dictionary<string, float> mountingPercentageDict = new Dictionary<string, float>();
    public Dictionary<string, float> MountingPercentageDict
    {
        get { return mountingPercentageDict; }
    }

    [SerializeField]
    private int canMountObjNum = 2;

    private List<string> mountedObjList = new List<string>(); // 장착한 오브젝트들의 아이디관련 List
    public List<string> MountedObjList
    {
        get { return mountedObjList; }
    }

    [Header("변신을 위한 최소의 이해도")]
    [SerializeField]
    private int minBodyChangeUnderstandingRate = 100;
    public int MinBodyChangeUnderstandingRate
    {
        get { return minBodyChangeUnderstandingRate; }
    }
    [Header("이해도를 어디까지 올릴 수 있는가")]
    [SerializeField]
    private int maxUnderstandingRate = 120;
    public int MaxUnderstandingRate
    {
        get { return maxUnderstandingRate; }
    }

    void Start()
    {
        // SetUnderstandingRate("Enemy", 0); // For Test

        changableBodyDict.Clear();

        changableBodyList.ForEach(x =>
        {
            // x.bodyScript = x.body.GetComponent<Enemy.Enemy>();
            changableBodyDict.Add(x.bodyId.ToString(), (x.body, x.additionalBodyStat));
            playerEnemyUnderStandingRateDic.Add(x.bodyId.ToString(), 100);

            // Debug.Log(enemyId);
        });

        EventManager.StartListening("PlayerDead", ResetUnderstandingRate);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", ResetUnderstandingRate);
    }
    public void SetMountingPercentageDict(string key, float value) 
    {
        if (mountingPercentageDict.ContainsKey(key))
        {
            mountingPercentageDict[key] = value;
        }
        else
        {
            mountingPercentageDict.Add(key, value);
        }
    }
    public float GetMountingPercentageDict(string key)
    {
        if (mountingPercentageDict.ContainsKey(key))
        {
            return mountingPercentageDict[key];
        }
        else
        {
            Debug.LogWarning("The key '" + key + "' is not Contain.");
            return 0f;
        }
    }
    public void SetUnderstandingRate(string key, int value)
    {
        if (playerEnemyUnderStandingRateDic.ContainsKey(key))
        {
            playerEnemyUnderStandingRateDic[key] = value;
        }
        else
        {
            playerEnemyUnderStandingRateDic.Add(key, value);
        }
    }
    public int GetUnderstandingRate(string key)
    {
        if (playerEnemyUnderStandingRateDic.ContainsKey(key))
        {
            return playerEnemyUnderStandingRateDic[key];
        }
        else
        {
            Debug.LogWarning("The key '" + key + "' is not Contain.");

            return 0;
        }
    }
    public void ResetUnderstandingRate()
    {
        for (int i = 0; i < playerEnemyUnderStandingRateDic.Count; i++)
        {
            playerEnemyUnderStandingRateDic[changableBodyList[i].bodyId.ToString()] = 0;
        }
    }
    public bool CheckCanMountObj()
    {
        return mountedObjList.Count < canMountObjNum;
    }
    public void SetMountObj(string objId)
    {
        if (CheckCanMountObj())
        {
            mountedObjList.Add(objId);
        }
    }
    public void UnSetMountObj(string objId)
    {
        mountedObjList.Remove(objId);
    }
    public bool CheckMountObjIdContain(string objId)
    {
        return mountedObjList.Contains(objId);
    }
    public bool CheckMountingEnemy(string objId)
    {
        if (!PlayerEnemyUnderstandingRateManager.Instance.MountingPercentageDict.ContainsKey(objId))
        {
            return false;
        }

        float value = UnityEngine.Random.Range(0f, 100f);

        if (value <= PlayerEnemyUnderstandingRateManager.Instance.GetMountingPercentageDict(objId)) // 확률 체크
        {
            if (!PlayerEnemyUnderstandingRateManager.Instance.CheckCanMountObj())
            {
                return false;
            }
            // 장착을 물어봄
            // 장착을 하면 true를, 장착을 하지 않으면 flase를 return함, 지금은 그냥 true를 return

            Debug.Log("장착 물어보는 창이 뜨네요");

            PlayerEnemyUnderstandingRateManager.Instance.SetMountingPercentageDict(objId, 0); // 장착을 했건 안했건 확률은 0이된다
            PlayerEnemyUnderstandingRateManager.Instance.SetMountObj(objId);

            return true;
        }
        else
        {
            return false;
        }
    }
    public void UpUnderStandingRate(string objId, int upValue) // 이해도(동화율)을 올려줌
    {
        PlayerEnemyUnderstandingRateManager.Instance.SetUnderstandingRate(objId,
            PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(objId) + upValue);

        if (PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(objId) > PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate) // 최대치 처리
        {
            PlayerEnemyUnderstandingRateManager.Instance.SetUnderstandingRate(objId, PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate);
        }
    }
}
