using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SingletonClass<ObjectManager> 
{
    private Dictionary<string, InteractionObj> itrObjDic = new Dictionary<string, InteractionObj>();
    public Dictionary<string, InteractionObj> ItrObjDic => itrObjDic;

    public void Reset()
    {
        itrObjDic.Clear();
    }

    public T GetObj<T>(string id) where T : InteractionObj
    {
        if (itrObjDic.ContainsKey(id))
        {
            try
            {
                T obj = itrObjDic[id] as T;
                return obj;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
                Debug.LogWarning("해당 타입으로 변환 안됨 " + typeof(T).ToString());
                return default;
            }
        }
        
        Debug.LogWarning("해당 아이디의 오브젝트가 존재하지 않음  ID : " + id);
        return default(T);
    }
}
