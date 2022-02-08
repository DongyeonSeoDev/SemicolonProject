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
        public Enemy.EnemyType bodyId;
        public GameObject body;
    }

    private Dictionary<string, int> playerEnemyUnderStandingRateDic = new Dictionary<string, int>();

    [SerializeField]
    private List<ChangeBodyData> changableBodyList = new List<ChangeBodyData>();
    private Dictionary<string, GameObject> changableBodyDict = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> ChangalbeBodyDict
    {
        get { return changableBodyDict; }
    }

    void Start()
    {
        // SetUnderstandingRate("Enemy", 0); // For Test

        changableBodyDict.Clear();

        changableBodyList.ForEach(x =>
        {
            // x.bodyScript = x.body.GetComponent<Enemy.Enemy>();
            changableBodyDict.Add(x.bodyId.ToString(), x.body);
            playerEnemyUnderStandingRateDic.Add(x.bodyId.ToString(), 100);

            // Debug.Log(enemyId);
        });
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
}
