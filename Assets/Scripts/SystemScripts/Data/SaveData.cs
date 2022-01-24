using System.Collections.Generic;
using System;
using UnityEngine;

namespace Water
{
    [Serializable]
    public class SaveData
    {
        public Option option = new Option();
        public Stat playerStat = new Stat();
        public UserInfo userInfo = new UserInfo();

        public void Save()
        {
            userInfo.userItems.Save();
        }
        public void Load()
        {
            userInfo.userItems.Load();
        }
    }

    [Serializable]
    public class UserInfo
    {
        public SaveDic<int, ItemInfo> userItems = new SaveDic<int, ItemInfo>();
    }

    [Serializable]
    public class ItemInfo
    {
        public int id;
        public int count;

        public ItemInfo() { }
        public ItemInfo(int id, int count)
        {
            this.id = id;
            this.count = count;
        }
    }

    [Serializable]
    public class Option
    {
        public SaveDic<KeyAction, KeyCode> keyDict = new SaveDic<KeyAction, KeyCode>(); 
    }

    [Serializable]
    public class SaveDic<K, V>
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
            for (int i = 0; i < keyList.Count; i++)
            {
                keyValueDic.Add(keyList[i], valueList[i]);
            }
        }

        public void Save()
        {
            keyList.Clear();
            valueList.Clear();
            foreach (K k in keyValueDic.Keys)
            {
                keyList.Add(k);
                valueList.Add(keyValueDic[k]);
            }
        }
    }

}
