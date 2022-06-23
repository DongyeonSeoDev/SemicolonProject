using System.Collections.Generic;
using UnityEngine;

public static class StoredData
{
    #region Valiable
    private static Dictionary<string, object> valueDataDic = new Dictionary<string, object>();
    private static Dictionary<string, GameObject> gameObjectDataDic = new Dictionary<string, GameObject>();
    #endregion

    #region object
    public static void SetValueKey(string key, object data)
    {
        if(!valueDataDic.ContainsKey(key))
            valueDataDic.Add(key, data);
        else
            valueDataDic[key] = data;
    }

    public static object GetValueData(string key, bool afterDeleteKey = false)
    {
        if(valueDataDic.ContainsKey(key))
        {
            if (!afterDeleteKey)
            {
                return valueDataDic[key];
            }
            else
            {
                object data = valueDataDic[key];
                DeleteValueKey(key);
                return data;
            }
        }
        Debug.Log("Not Exist Key : " + key);
        return null;
    }

    public static T GetValueData<T>(string key, bool afterDeleteKey = false)
    {
        if (valueDataDic.ContainsKey(key))
        {
            if (!afterDeleteKey)
            {
                return (T)valueDataDic[key];
            }
            else
            {
                T t = (T)valueDataDic[key];
                DeleteValueKey(key);
                return t;
            }
        }
        Debug.Log("Not Exist Key : " + key);
        return default;
    }

    public static void DeleteValueKey(string key)
    {
        if (valueDataDic.ContainsKey(key))
        {
            valueDataDic.Remove(key);
        }
    }

    public static bool HasValueKey(string key) => valueDataDic.ContainsKey(key);

    #endregion

    #region GameObject
    public static void SetGameObjectKey(string key, GameObject data)
    {
        
        if (!gameObjectDataDic.ContainsKey(key))
            gameObjectDataDic.Add(key, data);
        else
            gameObjectDataDic[key] = data;
    }

    public static GameObject GetGameObjectData(string key, bool afterDeleteKey = false)
    {
        if (gameObjectDataDic.ContainsKey(key))
        {
            if(!afterDeleteKey)
                return gameObjectDataDic[key];

            GameObject data = gameObjectDataDic[key];
            DeleteGameObjectKey(key);
            return data;
        }
        Debug.Log("Not Exist Key : " + key);
        return null;
    }

    public static T GetGameObjectData<T>(string key, bool afterDeleteKey = false)
    {
        if (gameObjectDataDic.ContainsKey(key))
        {
            if(!afterDeleteKey)
                return gameObjectDataDic[key].GetComponent<T>();

            T data = gameObjectDataDic[key].GetComponent<T>();
            DeleteGameObjectKey(key);
            return data;
        }
        Debug.Log("Not Exist Key : " + key);
        return default;
    }

    public static void DeleteGameObjectKey(string key)
    {
        if (gameObjectDataDic.ContainsKey(key))
        {
            gameObjectDataDic.Remove(key);
        }
    }

    public static bool HasGameObjectKey(string key) => gameObjectDataDic.ContainsKey(key);

    public static void Reset()
    {
        valueDataDic.Clear();
        gameObjectDataDic.Clear();
    }

    #endregion
}
