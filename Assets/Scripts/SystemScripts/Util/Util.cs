using UnityEngine;
using System.Collections;
using System;

public static partial class Util
{
    public static void ExecuteFunc(Action func, float delay, float duration, MonoBehaviour mono, Action start=null, Action end = null)
    {
        mono.StartCoroutine(ExecuteFuncCo(func, delay, duration, start, end));
    }

    private static IEnumerator ExecuteFuncCo(Action func, float delay, float duration, Action start = null, Action end = null)
    {
        yield return new WaitForSeconds(delay);
        start?.Invoke();
        float elapsed = duration;
        
        while(elapsed > 0)
        {
            elapsed -= Time.deltaTime;
            yield return null;
            func();
        }
        end?.Invoke();
    }
}