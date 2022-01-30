using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDrain : MonoBehaviour
{
    private PlayerInput playerInput = null;

    [SerializeField]
    private GameObject drainCollider = null; // drain 체크에 사용될 Collider
    [SerializeField]
    private float reDrainTime = 10f;
    private float reDrainTimer = 0f; 

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        EventManager.StartListening("OnDrain", OnDrain);

        drainCollider.SetActive(false);
    }
    void Update()
    {
        if(reDrainTimer > 0f)
        {
            reDrainTimer -= Time.deltaTime;

            if(reDrainTimer <= 0f)
            {
                reDrainTimer = 0f;
            }
        }

        if(playerInput.IsDrain)
        {
            playerInput.IsDrain = false;

            if(reDrainTimer <= 0f)
            {
                reDrainTimer = reDrainTime;

                drainCollider.SetActive(true);
            }
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnDrain", OnDrain);
    }
    private void OnDrain(string objName, int upValue) // upValue는 이해도(동화율)이 얼마나 오를 것인가.
    {
        PlayerEnemyUnderstandingRateManager.Instance.SetUnderstandingRate(objName,
         PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(objName) + upValue);
    }
}
