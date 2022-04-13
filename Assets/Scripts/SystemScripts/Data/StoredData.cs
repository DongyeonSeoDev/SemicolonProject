using System.Collections.Generic;
using UnityEngine;

public static class StoredData
{
    private static Dictionary<string, object> objDataDic = new Dictionary<string, object>();
    private static Dictionary<string, MonoBehaviour> monoDataDic = new Dictionary<string, MonoBehaviour>();

    public static void SetObjectKey(string key, object data)
    {
        //objDataDic[key] = data;  //�� �̷��� �ٷ� �ص� ��.

        if(!objDataDic.ContainsKey(key))
            objDataDic.Add(key, data);
        else
            objDataDic[key] = data;
    }

    public static object GetObjectKey(string key)
    {
        if(objDataDic.ContainsKey(key))
        {
            return objDataDic[key];
        }
        Debug.Log("Ű�� ���� : " + key);
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

    public static void SetMonoKey(string key, MonoBehaviour data)
    {
        
        if (!monoDataDic.ContainsKey(key))
            monoDataDic.Add(key, data);
        else
            monoDataDic[key] = data;
    }

    public static object GetMonoKey(string key)
    {
        if (monoDataDic.ContainsKey(key))
        {
            return monoDataDic[key];
        }
        Debug.Log("Ű�� ���� : " + key);
        return null;
    }

    public static void DeleteMonoKey(string key)
    {
        if (monoDataDic.ContainsKey(key))
        {
            monoDataDic.Remove(key);
        }
    }

    public static bool HasMonoKey(string key) => monoDataDic.ContainsKey(key);
}
