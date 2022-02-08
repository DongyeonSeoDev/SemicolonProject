using UnityEngine;
using System.Collections;
using System;

public static partial class Util
{
    public static void ExecuteFunc(Action func, float delay, float duration, MonoBehaviour mono, Action start=null, Action end = null, bool realTime = false)
    {
        mono.StartCoroutine(ExecuteFuncCo(func, delay, duration, start, end, realTime));
    }

    private static IEnumerator ExecuteFuncCo(Action func, float delay, float duration, Action start = null, Action end = null, bool realTime = false)
    {
        if (!realTime)
            yield return new WaitForSeconds(delay);
        else
            yield return new WaitForSecondsRealtime(delay);

        start?.Invoke();
        float elapsed = duration;

        while (elapsed > 0)
        {
            elapsed -= Time.unscaledDeltaTime;
            yield return null;
            func();
        }

        #region ÁÖ¼®
        /*if (!realTime)
        {
            while (elapsed > 0)
            {
                elapsed -= Time.deltaTime;
                yield return null;
                func();
            }
        }
        else
        {
            
            WaitForSecondsRealtime wsr = new WaitForSecondsRealtime(Time.unscaledDeltaTime);
            while(elapsed>0)
            {
                elapsed -= Time.unscaledDeltaTime;
                yield return wsr;
                func();
            }
        }*/
        #endregion

        end?.Invoke();
    }
}