using UnityEngine;
using System.Collections;
using System;

public static partial class Util
{
    public static Vector3 ScreenToWorldPos(Vector3 screenPos) => MainCam.ScreenToWorldPoint(screenPos);

    public static Vector3 ScreenToWorldPosForScreenSpace(Vector3 point) => RectTransformUtility.WorldToScreenPoint(MainCam, point);  //cameraÀÏ¶§

    public static Vector3 MousePositionForScreenSpace
    {
        get
        {
            Vector3 v = Input.mousePosition;
            v.z = Global.cameraPlaneDistance;
            return ScreenToWorldPos(v);
        }
    }

    public static bool CompareLayer(this Collider2D obj, int layer) => obj.gameObject.layer == layer;

    public static void ExecuteFunc(Action func, float delay, float duration, MonoBehaviour mono = null, Action start=null, Action end = null, bool realTime = false)
    {
        if (!mono) mono = GameManager.Instance;

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

        end?.Invoke();
    }
}