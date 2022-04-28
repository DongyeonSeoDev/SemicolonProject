using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class SaveData
{
    public Option option = new Option();
    public UserInfo userInfo = new UserInfo();
    public TutorialInfo tutorialInfo = new TutorialInfo();

    public void Save()
    {
        userInfo.userItems.Save();
        userInfo.monsterInfoDic.Save();
        userInfo.uiActiveDic.Save();
        option.keyInputDict.Save();
    }
    public void Load()
    {
        userInfo.userItems.Load();
        userInfo.monsterInfoDic.Load();
        userInfo.uiActiveDic.Load();
        option.keyInputDict.Load();
    }
}

[Serializable]
public class TutorialInfo
{
    public bool isEnded = false;

    public string tutorialId;
}

[Serializable]
public class UserInfo
{
    //stage
    public string currentStageID = "Stage0-01"; //어느 스테이지까지 갔는지

    //스탯 정보
    public int currentHp; //현재 HP
    public Stat playerStat = new Stat();  //현재 스탯

    public string currentBodyID; //현재 장착중인 몸 아이디

    public List<Triple<int, int, int>> limitedBattleCntItems = new List<Triple<int, int, int>>(); //n교전 후에 사라지는 아이템들 리스트 (아이디, 현재 교전 수, 최대 교전 수(가 되면 사라짐))
    public SaveDic<string, ItemInfo> userItems = new SaveDic<string, ItemInfo>(); //인벤토리 목록 
    public SaveDic<string, MonsterInfo> monsterInfoDic = new SaveDic<string, MonsterInfo>(); //몬스터 이해도(동화율), 흡수 확률 등의 정보

    public SaveDic<KeyAction, bool> uiActiveDic = new SaveDic<KeyAction, bool>();
}

[Serializable]
public class Option
{
    public SaveDic<KeyAction, KeyCode> keyInputDict = new SaveDic<KeyAction, KeyCode>();  //키세팅

    //해상도는 자동으로 저장됨
    //public Pair<float, float> screenWidthHeight = new Pair<float, float>();
    //public bool isFullScreen = true;
    
    //볼륨
    public float masterSound = -5f;
    public float bgmSize = 0.8f;
    public float soundEffectSize = 0.8f;
}



#region Dic
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
#endregion