using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockBack : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.StartListening("PlayerKnockBack", OnKnockBack);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerKnockBack", OnKnockBack);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnKnockBack(float moveDistance, float speed, float moveTime)
    {

    }
}
