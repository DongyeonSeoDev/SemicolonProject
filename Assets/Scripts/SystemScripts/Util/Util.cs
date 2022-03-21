using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public static partial class Util
{
    public static Color SubColor(Color a, Color b)  //a - b   Color
    {
        float[] rgba = new float[4] {Mathf.Abs(a.r - b.r), Mathf.Abs(a.g - b.g), Mathf.Abs( a.b - b.b), Mathf.Abs( a.a - b.a)};
        return new Color(rgba[0], rgba[1], rgba[2], rgba[3]);
    }

    public static float[] SubColorF(Color a, Color b) => new float[] { a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a };   //a - b   float[]

    public static Canvas WorldCvs => UIManager.Instance.gameCanvases[2];

    public static bool IsActiveGameUI(UIType type) => UIManager.Instance.gameUIList[(int)type].gameObject.activeSelf;

    public static Vector3 ScreenToWorldPos(Vector3 screenPos) => MainCam.ScreenToWorldPoint(screenPos);

    //이것을 쓸거면 Anchor가 Left Bottom으로 설정되어있어야 함
    public static Vector3 ScreenToWorldPosForScreenSpace(Vector3 point) => RectTransformUtility.WorldToScreenPoint(MainCam, point);  //camera일때

    //anchor 중앙
    public static Vector3 ScreenToWorldPosForScreenSpace(Vector3 point, Canvas cvs)
    {
        Vector3 scrPos = Util.WorldToScreenPoint(point);
        Vector2 localPos = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(cvs.GetComponent<RectTransform>(), scrPos, MainCam, out localPos);
        return localPos;
    } 

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

    public static T Find<T>(this IEnumerable<T> list, Func<T,bool> action)
    {
        foreach (T item in list)
        {
            if(action(item))
            {
                return item;
            }
        }
        return default(T);
    }

    public static T EnumParse<T>(string str) => (T)Enum.Parse(typeof(T), str);

    public static void PrintStructSize(Type type)
    {
        Debug.Log(type.ToString() + " Size : " + Marshal.SizeOf(type));
    }
}