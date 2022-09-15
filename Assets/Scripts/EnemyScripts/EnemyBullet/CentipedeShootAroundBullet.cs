using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeShootAroundBullet : MonoBehaviour
{
    [SerializeField]
    private float gradient = 1f;
    [SerializeField]
    private float startTime = 0f;
    [SerializeField]
    private float endTime = 5f;

    [SerializeField]
    private float timer = 0f;

    public CentipedeShootAroundBullet(float gradient, float startTime, float endTime)
    {
        this.gradient = gradient;
        this.startTime = startTime;
        this.endTime = endTime;
        timer = 0f;
    }

    void Update()
    {
        timer += Time.deltaTime;

        transform.localPosition = GetPositionPerTime(timer);
    }

    private Vector2 GetPositionPerTime(float time)
    {
        Vector2 result = Vector2.zero;

        float resultX = time;
        float resultY = gradient * (time - startTime) * (time - endTime);

        result = new Vector2(resultX, resultY);

        return result;
    }
}

