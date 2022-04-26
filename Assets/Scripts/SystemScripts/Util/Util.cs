using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
using UnityEditor;

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

    public static void ExecuteFunc(Action func, float delay, float duration, MonoBehaviour mono = null, Action start=null, Action end = null, bool realTime = false)
    {
        if (!mono) mono = GameManager.Instance;

        mono.StartCoroutine(ExecuteFuncCo(func, delay, duration, start, end, realTime));
    }

    private static IEnumerator ExecuteFuncCo(Action func, float delay, float duration, Action start = null, Action end = null, bool realTime = false)
    {
        if (!realTime)
            yield return new WaitForSeconds(delay * TimeManager.CurrentTimeScale);
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
            return itemList[UnityEngine.Random.Range(0,itemList.Count)];
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

        for(int i=0; i< randomScale; i++)
        {
            int ran1 = UnityEngine.Random.Range(0, itemList.Count);
            int ran2 = UnityEngine.Random.Range(0, itemList.Count);
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

        for (int i = 0; i < randomScale; i++)
        {
            int ran1 = UnityEngine.Random.Range(0, itemList.Count);
            int ran2 = UnityEngine.Random.Range(0, itemList.Count);
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

    public static T EnumParse<T>(string str) => (T)Enum.Parse(typeof(T), str);

    public static GameObject LoadAssetPrefab(string path) => AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/" + path + ".prefab");
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