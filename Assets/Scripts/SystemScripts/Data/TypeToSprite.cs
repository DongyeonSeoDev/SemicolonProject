using UnityEngine;
using System.Collections.Generic;
using System;

public static class TypeToSprite
{
    private static Dictionary<string, Pair<string,Sprite[]>> allDictionary = new Dictionary<string,Pair<string,Sprite[]>>();

    public static void Register<T>(string key, string path, int count)
    {
        allDictionary.Add(key, new Pair<string, Sprite[]>(path, new Sprite[count]));
    }

    public static bool HasKeyAndCreate(string key) => allDictionary.ContainsKey(key);

    public static Sprite GetSprite<T>(T t, string key, int i)
    {
        if(allDictionary[key].second[i] == null)
        {
            allDictionary[key].second[i] = Resources.Load<Sprite>(allDictionary[key].first + t.ToString());
        }
        return allDictionary[key].second[i];
    }
}
