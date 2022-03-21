using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public static class DOUtil 
{
    private static Dictionary<string, IEnumerator> tweeningDict = new Dictionary<string, IEnumerator>();

    public static CTween DOIntensity(this Light2D light, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
    {
        string key = "DOT_Light2D_Intensity_" + light.name;
        Action act = () => ExecuteTweening(key, DOIntensityCo(light, endValue, duration, setUpdate, onComplete), light);
        act();
        CTween t = new CTween(key,act);
        return t;
    }

    public static CTween DOVignetteColor(this Vignette vig, Color endValue, float duration, bool setUpdate = false, Action onComplete = null)
    {
        string key = "DOT_Vignette_Color_" + vig.name;
        Action act = () => ExecuteTweening(key, DOVignetteColorCo(vig, endValue, duration, setUpdate, onComplete), Environment.Instance);
        act();
        CTween t = new CTween(key,act);
        return t;
    }

    public static CTween DOVignetteIntensity(this Vignette vig, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
    {
        string key = "DOT_Vignette_Intensity_" + vig.name;
        Action act = () => ExecuteTweening(key, DOVignetteIntensityCo(vig, endValue, duration, setUpdate, onComplete), Environment.Instance);
        act();
        CTween t = new CTween(key, act);
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

    static IEnumerator DOVignetteColorCo(Vignette vig, Color endValue, float duration, bool setUpdate, Action onComplete)
    {
        float[] ca = Util.SubColorF(vig.color.GetValue<Color>(), endValue);
        for (int i = 0; i < ca.Length; i++) ca[i] /= duration;
        float t = 0f;
        Color c = vig.color.GetValue<Color>();

        if (setUpdate)
        {
            for (int i = 0; i < ca.Length; i++) ca[i] *= Time.unscaledDeltaTime;
            while (t < duration)
            {
                yield return null;
                t += Time.unscaledDeltaTime;
                
                c = new Color(c.a + ca[0], c.g + ca[1], c.b + ca[2], c.a + ca[3]);
                vig.color.Override(c);
            }
        }
        else
        {
            for (int i = 0; i < ca.Length; i++) ca[i] *= Time.deltaTime;
            while (t < duration)
            {
                yield return null;
                t += Time.deltaTime;

                c = new Color(c.a + ca[0], c.g + ca[1], c.b + ca[2], c.a + ca[3]);
                vig.color.Override(c);
            }
        }
        vig.color.Override(endValue);
        onComplete?.Invoke();
    }

    static IEnumerator DOVignetteIntensityCo(Vignette vig, float endValue, float duration, bool setUpdate, Action onComplete)
    {
        float vps = (endValue - vig.intensity.value) / duration;
        float t = 0f;
        if (setUpdate)
        {
            while (t < duration)
            {
                yield return null;
                t += Time.unscaledDeltaTime;
                vig.intensity.value += vps * Time.unscaledDeltaTime;
            }
        }
        else
        {
            while (t < duration)
            {
                yield return null;
                t += Time.deltaTime;
                vig.intensity.value += vps * Time.deltaTime;
            }
        }
        vig.intensity.value = endValue;
        onComplete?.Invoke();
    }

    public static void ExecuteTweening(string key, IEnumerator tc, MonoBehaviour mono)
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
    public string key;
    public Action tween;
    
    public void Invoke()
    {
        tween?.Invoke();
    }

    public CTween() { }
    public CTween(string key, Action tween) 
    {
        this.tween = tween;
        this.key = key;
    }
}
