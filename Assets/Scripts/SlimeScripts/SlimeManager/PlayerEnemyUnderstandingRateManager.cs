using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;

public class PlayerEnemyUnderstandingRateManager : MonoSingleton<PlayerEnemyUnderstandingRateManager>
{
    public struct ChangeBodyData
    {
        public string bodyId;
        public GameObject body;
        public Enemy.Enemy bodyScript;
    }

    private Dictionary<string, int> playerEnemyUnderStandingRateDic = new Dictionary<string, int>();

    [Header("body부분만 채워주면, 나머진 자동으로 들어감")]
    [SerializeField]
    private List<ChangeBodyData> changableBodyList = new List<ChangeBodyData>();
    public List<ChangeBodyData> ChangableBodyList
    {
        get { return changableBodyList; }
    }

    void Start()
    {
        // SetUnderstandingRate("Enemy", 0); // For Test

        changableBodyList.ForEach(x => {
           x.bodyScript = x.body.GetComponent<Enemy.Enemy>();
           x.bodyId = x.bodyScript.GetEnemyId();
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
