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
    public void BodyChange(string bodyId)
    {
        if(playerEnemyUnderstandingRateManager.GetUnderstandingRate(bodyId) >= 100f)
        {
            ChangeBody(bodyId);
        }
    }
    private void ChangeBody(string bodyId)
    {
        foreach(PlayerEnemyUnderstandingRateManager.ChangeBodyData item in playerEnemyUnderstandingRateManager.ChangableBodyList)
        {
            if(item.bodyId == bodyId)
            {
                // body 변경 함수

                          
            }
        }
    }
}
