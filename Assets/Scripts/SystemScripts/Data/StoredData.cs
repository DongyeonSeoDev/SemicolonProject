using System.Collections.Generic;
using UnityEngine;

public static class StoredData
{
    private static Dictionary<string, object> objDataDic = new Dictionary<string, object>();
    private static Dictionary<string, GameObject> gameObjectDataDic = new Dictionary<string, GameObject>();

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

    public static void DeleteGameObjectKey(string key)
    {
        if (gameObjectDataDic.ContainsKey(key))
        {
            gameObjectDataDic.Remove(key);
        }
    }

    public static bool HasGameObjectKey(string key) => gameObjectDataDic.ContainsKey(key);
}
