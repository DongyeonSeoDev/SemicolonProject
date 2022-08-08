using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour
{
    private float originValue = 0f;
    private float targetValue = 0f;

    private float rotationTime = 0f;
    private float rotationTimer = 0f;
    public float RotationTimer
    {
        get { return rotationTimer; }
    }

    private void OnEnable()
    {
        EventManager.StartListening("PlayerRotation", StartRotationByLerp);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerRotation", StartRotationByLerp);
    }
    void Update()
    {
        CheckTimer();
    }
    private void StartRotationByLerp(float ra, float rt)
    {
        originValue = SlimeGameManager.Instance.CurrentPlayerBody.transform.rotation.z;
        targetValue = originValue + ra;

        rotationTime = rt;
        rotationTimer = 0f;
    }
    private void CheckTimer()
    {
        if(rotationTimer < rotationTime)
        {
            rotationTimer += Time.deltaTime;

            SlimeGameManager.Instance.CurrentPlayerBody.transform.rotation = Quaternion.Euler(
                Vector3.Lerp(new Vector3(0f,0f,originValue), new Vector3(0f, 0f, targetValue), rotationTimer / rotationTime));
        }
    }
}
