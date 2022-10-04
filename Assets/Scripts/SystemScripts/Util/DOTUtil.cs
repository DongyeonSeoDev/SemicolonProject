using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace FkTweening
{
    public static class DOUtil
    {
        private static Dictionary<string, IEnumerator> tweeningDict = new Dictionary<string, IEnumerator>();

        #region DO
        public static void DOIntensity(this Light2D light, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
        {
            //string key = "DOT_Light2D_Intensity_" + light.name;
            ExecuteTweening("DOT_Light2D_Intensity_" + light.name, DOIntensityCo(light, endValue, duration, setUpdate, onComplete), light);
           
        }

        public static void DOVignetteColor(this Vignette vig, Color endValue, float duration, bool setUpdate = false, Action onComplete = null)
        {
            string key = "DOT_Vignette_Color_" + vig.name;
            ExecuteTweening(key, DOVignetteColorCo(vig, endValue, duration, setUpdate, onComplete), Environment.Instance);
            
        }

        public static void DOVignetteIntensity(this Vignette vig, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
        {
            ExecuteTweening("DOT_Vignette_Intensity_" + vig.name, DOVignetteIntensityCo(vig, endValue, duration, setUpdate, onComplete), Environment.Instance);
        }

        public static void DOChromIntensity(this ChromaticAberration CA, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
        {
            ExecuteTweening("DOT_ChromaticAberration_Intensity_" + CA.name, DOChromIntensityCo(CA, endValue, duration, setUpdate, onComplete), Environment.Instance);
        }

        public static void DOInnerRadius(this Light2D light, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
        {
            ExecuteTweening("DOT_Light2D_InnerRadius_" + light.name, DOInnerRadiusCo(light, endValue, duration, setUpdate, onComplete), light);
        }

        public static void DOOuterRadius(this Light2D light, float endValue, float duration, bool setUpdate = false, Action onComplete = null)
        {
            //string key = "DOT_Light2D_OuterRadius_" + light.name;
            ExecuteTweening("DOT_Light2D_OuterRadius_" + light.name, DOOuterRadiusCo(light, endValue, duration, setUpdate, onComplete), light);
            //Action act = () => ExecuteTweening(key, DOOuterRadiusCo(light, endValue, duration, setUpdate, onComplete), light);
            //act();
            //return new CTween(key, act);
        }

        public static void DOFillText(this Text text, int init, int target, int offset, float secondPerOffset, bool unscaled = false, Action onComplete = null)
        {
            ExecuteTweening("DOT_Text_Fill" + text.GetInstanceID().ToString(), DOFillTextCo(text, init, target, offset, secondPerOffset, unscaled, onComplete), text);
        }


        #endregion

        #region DOCO
        static IEnumerator DOIntensityCo(Light2D light, float endValue, float duration, bool setUpdate, Action onComplete)
        {
            float vps = (endValue - light.intensity) / duration;
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

        static IEnumerator DOChromIntensityCo(ChromaticAberration CA, float endValue, float duration, bool setUpdate, Action onComplete)
        {
            float vps = (endValue - CA.intensity.value) / duration;
            float t = 0f;
            if (setUpdate)
            {
                while (t < duration)
                {
                    yield return null;
                    t += Time.unscaledDeltaTime;
                    CA.intensity.value += vps * Time.unscaledDeltaTime;
                }
            }
            else
            {
                while (t < duration)
                {
                    yield return null;
                    t += Time.deltaTime;
                    CA.intensity.value += vps * Time.deltaTime;
                }
            }
            CA.intensity.value = endValue;
            onComplete?.Invoke();
        }

        static IEnumerator DOInnerRadiusCo(Light2D light, float endValue, float duration, bool setUpdate, Action onComplete)
        {
            float vps = (endValue - light.pointLightInnerRadius) / duration;
            float t = 0f;
            if (setUpdate)
            {
                while (t < duration)
                {
                    yield return null;
                    t += Time.unscaledDeltaTime;
                    light.pointLightInnerRadius += vps * Time.unscaledDeltaTime;
                }
            }
            else
            {
                while (t < duration)
                {
                    yield return null;
                    t += Time.deltaTime;
                    light.pointLightInnerRadius += vps * Time.deltaTime;
                }
            }
            light.pointLightInnerRadius = endValue;
            onComplete?.Invoke();
        }

        static IEnumerator DOOuterRadiusCo(Light2D light, float endValue, float duration, bool setUpdate, Action onComplete)
        {
            float vps = (endValue - light.pointLightOuterRadius) / duration;
            float t = 0f;
            if (setUpdate)
            {
                while (t < duration)
                {
                    yield return null;
                    t += Time.unscaledDeltaTime;
                    light.pointLightOuterRadius += vps * Time.unscaledDeltaTime;
                }
            }
            else
            {
                while (t < duration)
                {
                    yield return null;
                    t += Time.deltaTime;
                    light.pointLightOuterRadius += vps * Time.deltaTime;
                }
            }
            light.pointLightOuterRadius = endValue;
            onComplete?.Invoke();
        }

        static IEnumerator DOFillTextCo(Text tx, int init, int target, int offset, float secondPerOffset, bool unscaled, Action onComplete)
        {
            tx.text = init.ToString();
            if(init == target)
            {
                onComplete?.Invoke();   
                yield break;
            }

            bool plus = init < target;
            int cur = init;

            /*if(autoOffsetSpeed > 0)
            {
                offset = Mathf.Clamp(Mathf.Abs(target - init)/ autoOffsetSpeed, 1, 10000);
            }*/

            while(cur != target)
            {
                if (unscaled) yield return new WaitForSecondsRealtime(secondPerOffset);
                else yield return new WaitForSeconds(secondPerOffset);

                if (plus)
                {
                    cur += offset;
                    if (cur > target)
                    {
                        break;
                    }
                }
                else
                {
                    cur -= offset;
                    if (cur < target)
                    {
                        break;
                    }
                }

                tx.text = cur.ToString();
            }

            cur = target;
            tx.text = cur.ToString();
            onComplete?.Invoke();
        }

        #endregion

        #region Execute
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
            ExecuteTweening(key, co, mono);
        }

        public static void StopCo(string key, MonoBehaviour mono = null)
        {
            if (mono == null) mono = Environment.Instance;

            if (tweeningDict.ContainsKey(key))
            {
                mono.StopCoroutine(tweeningDict[key]);
                //IEnumerator temp = tweeningDict[key];
                //tweeningDict[key] = null;
                //tweeningDict[key] = temp;
            }
        }
        #endregion

        public static void Reset()
        {
            tweeningDict.Clear();
        }
    }
}