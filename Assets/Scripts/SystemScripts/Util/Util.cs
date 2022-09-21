using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using FkTweening;

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
    public static Vector3 ScreenToWorldPosForScrSpace(Vector3 scrPos)
    {
        Vector3 v = ScreenToWorldPos(scrPos);
        v.z = Global.cameraPlaneDistance;
        return v;
    }

    //이것을 쓸거면 Anchor가 Left Bottom으로 설정되어있어야 함
    public static Vector3 WorldToScreenPosForScreenSpace(Vector3 point) => RectTransformUtility.WorldToScreenPoint(MainCam, point);  //camera일때

    //anchor 중앙   이걸 쓰는게 좋음
    public static Vector3 WorldToScreenPosForScreenSpace(Vector3 point, Canvas cvs)
    {
        Vector3 scrPos = WorldToScreenPoint(point);
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

    public static void SetSlotMark(Transform mark, Transform parent)
    {
        mark.gameObject.SetActive(true);
        mark.SetParent(parent);
        mark.localPosition = Vector3.zero;
        mark.SetAsLastSibling();
        mark.localScale = Vector3.one;
    }

    public static void PriDelayFunc(string key, Action a, float delay, MonoBehaviour mono, bool realTime, bool applyCurTimeScale = true)
    {
        if (!mono) mono = GameManager.Instance;

        DOUtil.ExecuteTweening(key, DelayFuncCo(a, delay, realTime, applyCurTimeScale), mono);
    }

    public static void StopCo(string key, MonoBehaviour mono)
    {
        if (!mono) mono = GameManager.Instance;

        DOUtil.StopCo(key, mono);
    }

    public static void ExecuteFunc(Action func, float delay, float duration, MonoBehaviour mono = null, Action start=null, Action end = null, bool realTime = false, bool applyTimeScale = true)
    {
        if (!mono) mono = GameManager.Instance;

        mono.StartCoroutine(ExecuteFuncCo(func, delay, duration, start, end, realTime, applyTimeScale));
    }

    private static IEnumerator ExecuteFuncCo(Action func, float delay, float duration, Action start = null, Action end = null, bool realTime = false, bool applyTimeScale = false)
    {
        if (!realTime)
            yield return new WaitForSeconds(applyTimeScale ? delay : delay * TimeManager.CurrentTimeScale);
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

    public static T EnumParse<T>(string str) => (T)Enum.Parse(typeof(T), str);

    //public static GameObject LoadAssetPrefab(string path) => AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + path + ".prefab");

    public static int GetRandomIndex(List<float> weightList)  //가중치 방식 기반으로 랜덤한 인덱스 반환
    {
        float total = 0, weight = 0;
        int i;
        for (i = 0; i < weightList.Count; i++)
            total += weightList[i];

        float sel = total * UnityEngine.Random.Range(0f, 1f);

        for (i = 0; i < weightList.Count; i++)
        {
            weight += weightList[i];
            if (sel < weight)
            {
                return i;
            }
        }

        Debug.Log("예외상황 : 확인 필요");
        Debug.Log($"total : {total}, sel : {sel}, weight : {weight}");
        for (i = 0; i < weightList.Count; i++) Debug.Log(weightList[i].ToString());

        return GetRandomIndex(weightList);
    }
}

public static partial class Util
{
    public static T Find<T>(this IEnumerable<T> list, Func<T, bool> action)
    {
        foreach (T item in list)
        {
            if (action(item))
            {
                return item;
            }
        }
        return default(T);
    }

    public static List<T> FindAll<T>(this IEnumerable<T> list, Func<T, bool> action)
    {
        List<T> itemList = new List<T>();
        foreach (T item in list)
        {
            if (action(item))
            {
                itemList.Add(item);
            }
        }
        return itemList;
    }

    public static T FindRandom<T>(this IEnumerable<T> list, Func<T, bool> action)  //조건에 만족하는 요소중에 랜덤으로 가져옴
    {
        List<T> itemList = new List<T>();
        foreach (T item in list)
        {
            if (action(item))
            {
                itemList.Add(item);
            }
        }

        if (itemList.Count > 1)
        {
            return itemList[UnityEngine.Random.Range(0, itemList.Count)];
        }
        else if (itemList.Count == 1)
        {
            return itemList[0];
        }
        else
        {
            Debug.Log("아무 요소도 없음");
            return default(T);
        }
    }

    public static List<T> FindAllRandom<T>(this IEnumerable<T> list, Func<T, bool> action, int randomScale = 10)  //조건에 만족하는 요소들을 랜덤으로 가져옴
    {
        List<T> itemList = new List<T>();
        foreach (T item in list)
        {
            if (action(item))
            {
                itemList.Add(item);
            }
        }

        int ran1, ran2;
        for (int i = 0; i < randomScale; i++)
        {
            ran1 = UnityEngine.Random.Range(0, itemList.Count);
            ran2 = UnityEngine.Random.Range(0, itemList.Count);
            T temp = itemList[ran1];
            itemList[ran1] = itemList[ran2];
            itemList[ran2] = temp;
        }

        return itemList;
    }

    public static List<T> ToRandomList<T>(this IEnumerable<T> list, int randomScale = 10)  //리스트 섞음
    {
        List<T> itemList = list.ToList();
        if (itemList.Count < 2) return itemList;

        int ran1, ran2;
        for (int i = 0; i < randomScale; i++)
        {
            ran1 = UnityEngine.Random.Range(0, itemList.Count);
            ran2 = UnityEngine.Random.Range(0, itemList.Count);
            T temp = itemList[ran1];
            itemList[ran1] = itemList[ran2];
            itemList[ran2] = temp;
        }
        return itemList;
    }

    public static T ToRandomElement<T>(this IEnumerable<T> list)
    {
        List<T> itemList = list.ToList();
        if (itemList.Count < 2) return itemList[0];
        return itemList[UnityEngine.Random.Range(0, itemList.Count)];
    }

    public static void SetAlpha(this Image img, float a)
    {
        Color c = img.color;
        c.a = a;
        img.color = c;
    }
}

public static class TalkUtil
{
    public static void ShowSubtitle(string dialogDataKey)
    {
        TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle(dialogDataKey));
    }
}

namespace Water
{
    public static class WDUtil
    {
        public static T StringToClass<T>(string str) where T : class => Activator.CreateInstance(Type.GetType(str)) as T;

        public static void PrintStructSize(Type type)
        {
            Debug.Log(type.ToString() + " Size : " + Marshal.SizeOf(type));
        }

        public static FieldInfo GetFieldInfo<T>(this T t, string fieldName)
        => t.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        
    }
}