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
        //objDataDic[key] = data;  //걍 이렇게 바로 해도 됨.
        
        if(!valueDataDic.ContainsKey(key))
            valueDataDic.Add(key, data);
        else
            valueDataDic[key] = data;
    }

    public static object GetValueData(string key)
    {
        if(valueDataDic.ContainsKey(key))
        {
            return valueDataDic[key];
        }
        Debug.Log("키가 없다 : " + key);
        return null;
    }

    public static T GetValueData<T>(string key)
    {
        if (valueDataDic.ContainsKey(key))
        {
            return (T)valueDataDic[key];
        }
        Debug.Log("키가 없다 : " + key);
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

    public static GameObject GetGameObjectData(string key)
    {
        if (gameObjectDataDic.ContainsKey(key))
        {
            return gameObjectDataDic[key];
        }
        Debug.Log("키가 없다 : " + key);
        return null;
    }

    public static T GetGameObjectData<T>(string key)
    {
        if (gameObjectDataDic.ContainsKey(key))
        {
            return gameObjectDataDic[key].GetComponent<T>();
        }
        Debug.Log("키가 없다 : " + key);
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

    public static bool TryGetGameObject(string key, out GameObject obj)
    {
        obj = gameObjectDataDic.ContainsKey(key) ? gameObjectDataDic[key] : null;
        return obj != null;
    }

    public static bool TryGetGameObject<T>(string key, out T obj)
    {
        obj = gameObjectDataDic.ContainsKey(key) ? gameObjectDataDic[key].GetComponent<T>() : default;
        return obj != null;
    }

    #endregion
}
