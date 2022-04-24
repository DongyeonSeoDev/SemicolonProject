using UnityEngine;
using System.Collections.Generic;

public static class TypeToTextAsset
{
    private static Dictionary<string, string[]> allDictionary = new Dictionary<string, string[]>(); 

    public static void Register(string key, string textAssetPath, char criteria = '\n')
    {
        allDictionary[key] = Resources.Load<TextAsset>(textAssetPath).text.Split(criteria);
    }

    public static string GetText(string key, int type) => allDictionary[key][type];

    public static bool HasKey(string key) => allDictionary.ContainsKey(key);
}