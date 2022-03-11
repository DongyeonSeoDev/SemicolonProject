using System.Collections.Generic;
using UnityEngine;
using System;

public static class TimeManager
{
    private static Queue<bool> timePauseQueue = new Queue<bool>();
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
            Time.timeScale = 1f;
            resumeAction?.Invoke();

            timeResumeAction?.Invoke();
        }
    }
}
