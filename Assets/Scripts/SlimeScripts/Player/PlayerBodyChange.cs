using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyChange : MonoBehaviour
{
    private PlayerEnemyUnderstandingRateManager playerEnemyUnderstandingRateManager = null;

    private void Start()
    {
        playerEnemyUnderstandingRateManager = PlayerEnemyUnderstandingRateManager.Instance;
    }
    public void BodyChange(string bodyName)
    {
        if(playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyName) >= 100f)
        {
            ChangeBody(bodyName);
        }
    }
    private void ChangeBody(string bodyName)
    {
        foreach(PlayerEnemyUnderstandingRateManager.ChangeBody item in playerEnemyUnderstandingRateManager.ChangableBodyList)
        {
            if(item.bodyName == bodyName)
            {
                // body 변경 함수

                          
            }
        }
    }
}
