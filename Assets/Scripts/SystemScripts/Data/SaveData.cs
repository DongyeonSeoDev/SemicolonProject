using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Option option = new Option();
    public UserInfo userInfo = new UserInfo();

    public void Save()
    {
        userInfo.userItems.Save();
        userInfo.monstersAssimilationRate.Save();
        option.keyInputDict.Save();
    }
    public void Load()
    {
        userInfo.userItems.Load();
        userInfo.monstersAssimilationRate.Load();
        option.keyInputDict.Load();
    }
}

[Serializable]
public class UserInfo
{
    //스탯 정보
    public int currentHp;
    public Stat playerStat = new Stat();

    public SaveDic<int, ItemInfo> userItems = new SaveDic<int, ItemInfo>(); //인벤토리 목록 

    public SaveDic<string, int> monstersAssimilationRate = new SaveDic<string, int>(); //몬스터 동화율
}

[Serializable]
public class Option
{
    public SaveDic<KeyAction, KeyCode> keyInputDict = new SaveDic<KeyAction, KeyCode>();

    //해상도는 자동으로 저장됨
    //public Pair<float, float> screenWidthHeight = new Pair<float, float>();
    //public bool isFullScreen = true;

    public float masterSound = 0.5f;
    public float bgmSize = 0.6f;
    public float soundEffectSize = 0.7f;
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

    public void ClearDic()
    {
        keyValueDic.Clear();
    }
}
