using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData 
{
    public SaveDic<int, ItemInfo> userItems = new SaveDic<int, ItemInfo>();

    public void Save()
    {
        userItems.Save();
    }
    public void Load()
    {
        userItems.Load();
    }
}

[Serializable]
public class ItemInfo
{
    public int id;
    public int count;
    public ItemType itemType;
}

[Serializable]
public class SaveDic<K,V>
{
    public List<K> keyList = new List<K>();
    public List<V> valueList = new List<V>();

    public Dictionary<K, V> keyValueDic = new Dictionary<K, V>();

    public V this[K key]
    {
        get
        {
            return keyValueDic[key];
        }
        set
        {
            keyValueDic[key] = value;
        }
    }

    public void Load()
    {
        keyValueDic.Clear();
        for(int i=0; i<keyList.Count; i++)
        {
            keyValueDic.Add(keyList[i], valueList[i]);
        }
    }

    public void Save()
    {
        keyList.Clear();
        valueList.Clear();
        foreach(K k in keyValueDic.Keys)
        {
            keyList.Add(k);
            valueList.Add(keyValueDic[k]);
        }
    }
}
