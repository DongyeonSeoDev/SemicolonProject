using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnemyUnderstandingRateManager : MonoBehaviour
{
    private static PlayerEnemyUnderstandingRateManager instance = null;
    public static PlayerEnemyUnderstandingRateManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<PlayerEnemyUnderstandingRateManager>();

                if(instance == null)
                {
                    GameObject temp = new GameObject("PlayerEnemyUnderstandingRateManager");
                    instance = temp.AddComponent<PlayerEnemyUnderstandingRateManager>();
                }
            }

            return instance;
        }
    }

    private Dictionary<string, int> playerEnemyUnderStandingRateDic = new Dictionary<string, int>();
    void Start()
    {
        SetUnderstandingRate("Enemy", 0); // For Test
    }
    public void SetUnderstandingRate(string key, int value)
    {
        if(playerEnemyUnderStandingRateDic.ContainsKey(key))
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
        if(playerEnemyUnderStandingRateDic.ContainsKey(key))
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
