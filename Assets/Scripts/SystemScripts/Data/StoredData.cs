using System.Collections.Generic;
using UnityEngine;

public static class StoredData
{
    private static Dictionary<string, object> objDataDic = new Dictionary<string, object>();
    private static Dictionary<string, GameObject> gameObjectDataDic = new Dictionary<string, GameObject>();

    #region object
    public static void SetObjectKey(string key, object data)
    {
        //objDataDic[key] = data;  //걍 이렇게 바로 해도 됨.
        
        if(!objDataDic.ContainsKey(key))
            objDataDic.Add(key, data);
        else
            objDataDic[key] = data;
    }

    public static object GetObjectData(string key)
    {
        if(objDataDic.ContainsKey(key))
        {
            return objDataDic[key];
        }
        Debug.Log("키가 없다 : " + key);
        return null;
    }

    public static void DeleteObjectKey(string key)
    {
        if (objDataDic.ContainsKey(key))
        {
            objDataDic.Remove(key);
        }
    }

    public static bool HasObjectKey(string key) => objDataDic.ContainsKey(key);

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
