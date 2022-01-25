using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;

public class PlayerEnemyUnderstandingRateManager : MonoSingleton<PlayerEnemyUnderstandingRateManager>
{
    public struct ChangeBody
    {
        public string bodyName;
        public GameObject body;
    }

    private Dictionary<string, int> playerEnemyUnderStandingRateDic = new Dictionary<string, int>();

    [SerializeField]
    private List<ChangeBody> changableBodyList = new List<ChangeBody>();
    public List<ChangeBody> ChangableBodyList
    {
        get { return changableBodyList; }
    }

    void Start()
    {
        SetUnderstandingRate("Enemy", 0); // For Test
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
