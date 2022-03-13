using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Experimental.Rendering.Universal;

public static class DOUtil 
{
    private static Dictionary<string, IEnumerator> tweeningDict = new Dictionary<string, IEnumerator>();

    public static CTween DOIntensity(this Light2D light, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
    {
        Action act = () => ExecuteTweening("DOT_Light2D_Intensity_" + light.name, DOIntensityCo(light, endValue, duration, setUpdate, onComplete), light);
        act();
        CTween t = new CTween(act);
        return t;
    }

    static IEnumerator DOIntensityCo(Light2D light, float endValue, float duration, bool setUpdate, Action onComplete)
    {
        float vps = (endValue-light.intensity) / duration;
        float t = 0f;
        if (setUpdate)
        {
            while (t < duration)
            {
                yield return null;
                t += Time.unscaledDeltaTime;
                light.intensity += vps * Time.unscaledDeltaTime;
            }
        }
        else
        {
            while (t < duration)
            {
                yield return null;
                t += Time.deltaTime;
                light.intensity += vps * Time.deltaTime;
            }
        }
        light.intensity = endValue;
        onComplete?.Invoke();
    }

    private static void ExecuteTweening(string key, IEnumerator tc, MonoBehaviour mono)
    {
        if (!tweeningDict.ContainsKey(key))
        {
            tweeningDict.Add(key, null);
        }

        if (tweeningDict[key] != null)
        {
            mono.StopCoroutine(tweeningDict[key]);
            tweeningDict[key] = null;
        }
        tweeningDict[key] = tc;
        mono.StartCoroutine(tweeningDict[key]);
    }

    public static void StartCo(string key, IEnumerator co, MonoBehaviour mono = null)
    {
        if (!mono) mono = Environment.Instance;
        ExecuteTweening(key,co, mono);
    }

    public static void StopCo(string key, MonoBehaviour mono = null)
    {
        if(mono == null) mono = Environment.Instance;

        if (tweeningDict.ContainsKey(key))
        {
            mono.StopCoroutine(tweeningDict[key]);
            tweeningDict[key] = null;
        }
    }
}

public class CTween
{
    public Action tween;
    
    public void Invoke()
    {
        tween?.Invoke();
    }

    public CTween() { }
    public CTween(Action tween) 
    {
        this.tween = tween; 
    }
}
