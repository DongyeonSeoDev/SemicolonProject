using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScriptHelper
{
    public static void ForEach<T>(this IEnumerable<T> list, Action<T> action)
    {
        foreach (var item in list)
        {
            action(item);
        }
    }
    public static List<T> ToList<T>(this IEnumerable<T> a)
    {
        List<T> results = new List<T>();

        foreach (T item in a)
        {
            results.Add(item);
        }

        return results;
    }
    public static Vector2 RandomVector(Vector2 min, Vector2 max)
    {
        float x = UnityEngine.Random.Range(min.x, max.x);
        float y = UnityEngine.Random.Range(min.y, max.y);

        return new Vector2(x, y);
    }
    public static Vector2 Sum(this Vector2 vec1, Vector2 vec2)
    {
        return new Vector2(vec1.x + vec2.x, vec1.y + vec2.y);
    }
    public static Vector3 Sum(this Vector3 vec1, Vector3 vec2)
    {
        return new Vector3(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z);
    }
    public static Vector4 Sum(this Vector4 vec1, Vector4 vec2)
    {
        return new Vector4(vec1.x + vec2.x, vec1.y + vec2.y, vec1.z + vec2.z, vec1.w + vec2.w);
    }
    public static Color SetColorAlpha(this Color color, float a)
    {
        Color newColor = color;
        newColor.a = a;

        return newColor;
    }
    // ---Limit매소드에 대한 설명---
    // value = 0, min = 1, max = 3일 땐 3을 리턴한다.
    // value = -2, min = 1, max = 3일 땐 1을 리턴한다.
    // value = -5, min = 1, max = 3일 땐 1을 리턴한다.
    // value = 4, min = 1, max = 3일 땐 1을 리턴한다.
    // value = 5, min = 1, max = 3일 땐 2를 리턴한다.
    // value = 8, min = 1, max = 3일 땐 2를 리턴한다.
    // value값이 min값과 max값 사이를 왕복한다고 생각하면 된다.
    // value값이 min값보다 적어지면 (min - value) 만큼을 max값에서 빼서 나온 값을 value값에 대입한다. 이 과정을 value값이 min값 이상이 될 때 까지 반복히면, Limit함수의 리턴값과 같은 값이 나온다.
    // value값이 max값보다 많아지면 (value - max) 만큼을 min값에서 더해서 나온 값을 value값에 대입한다. 이 과정을 value값이 max값 이하가 될 때 까지 반복하면, Limit함수의 리턴값과 같은 값이 나온다.
    public static int Limit(this int value, int min, int max)
    {
        int plusNumOffset = max - min;
        int minusNumOffset = Mathf.Abs(value) / max;
        int result = value;

        value = value >= 0 ? value % plusNumOffset + plusNumOffset : (value + max * minusNumOffset);

        if (value < min)
        {
            result = max - (Mathf.Abs(min - value) - 1);
        }
        else if (value > max)
        {
            result = min + (Mathf.Abs(value - max) - 1);
        }

        return result;
    }
}
public class SlimeGameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
