using System.Collections.Generic;
using UnityEngine;
using System;

public static class TimeManager
{
    private static Queue<bool> timePauseQueue = new Queue<bool>();
    private static float currentTimeScale = 1f;
    public static float CurrentTimeScale
    {
        get => currentTimeScale;
        set => currentTimeScale = value;
    }

    public static bool IsTimePaused => timePauseQueue.Count > 0;

    public static Action timePauseAction = null;
    public static Action timeResumeAction = null;

    public static void TimePause(Action pauseAction = null)
    {
        if (!IsTimePaused)
        {
            Time.timeScale = 0f;
            pauseAction?.Invoke();

            timePauseAction?.Invoke();
        }

        timePauseQueue.Enqueue(true);
    }

    public static void TimeResume(Action resumeAction = null)
    {
        if (timePauseQueue.Count > 0)
            timePauseQueue.Dequeue();
        else
            Debug.Log("이미 일시정지 아닌 상태다");

        if(!IsTimePaused)
        {
            Time.timeScale = currentTimeScale;
            resumeAction?.Invoke();

            timeResumeAction?.Invoke();
        }
    }

    //duration초 동안 timeScale을 scale로 만들고 duration초 다 지나면 endAction실행
    public static void SetTimeScale(float scale, float duration, Action endAction = null, bool realTime = false)
    {
        currentTimeScale = scale;
        if (!IsTimePaused)
        {
            Time.timeScale = currentTimeScale;
        }
        Util.DelayFunc(() =>
        {
            currentTimeScale = 1f;
            if (!IsTimePaused)
            {
                Time.timeScale = currentTimeScale;
            }
            endAction?.Invoke();
        }, duration, null, realTime);
    }
}
