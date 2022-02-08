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
        public GameObject body;
        public Enemy.Enemy bodyScript;
    }

    private Dictionary<string, int> playerEnemyUnderStandingRateDic = new Dictionary<string, int>();

    [SerializeField]
    private List<GameObject> changableBodyList = new List<GameObject>();
    private Dictionary<string, GameObject> changableBodyDict = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> ChangalbeBodyDict
    {
        get { return changableBodyDict; }
    }

    void Start()
    {
        // SetUnderstandingRate("Enemy", 0); // For Test

        changableBodyDict.Clear();

        Debug.Log("bbb");

        changableBodyList.ForEach(x =>
        {
            Debug.Log(x.GetComponent<Enemy.Enemy>());
            string enemyId = x.GetComponent<Enemy.Enemy>().GetEnemyId();
            // x.bodyScript = x.body.GetComponent<Enemy.Enemy>();
            changableBodyDict.Add(enemyId, x);
            playerEnemyUnderStandingRateDic.Add(enemyId, 0);

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
