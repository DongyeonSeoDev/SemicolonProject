using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;

public class PlayerEnemyUnderstandingRateManager : MonoSingleton<PlayerEnemyUnderstandingRateManager>
{
    [Serializable]
    public struct ChangeBodyData
    {
        public string bodyName;
        public Enemy.EnemyType bodyId;
        public GameObject body;
        public EternalStat additionalBodyStat; // 변신 후의 플레이어의 Additional스탯, (이해도 100% 기준)
        public Sprite bodyImg;
        public string bodyExplanation;
    }

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
}
