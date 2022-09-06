using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class EternalOptionData  //게임의 세이브 파일이 어떻든간에 해상도처럼 어디서든 값이 공용으로 쓰이는 세이브 데이터
{
    //볼륨
    public float masterSound = -5f;
    public float bgmSize = 0.8f;
    public float soundEffectSize = 0.8f;
}

[Serializable]
public class SaveData
{
    public Option option = new Option();
    public UserInfo userInfo = new UserInfo();
    public TutorialInfo tutorialInfo = new TutorialInfo();
    public StageInfo stageInfo = new StageInfo();

    public void ResetComplete()  //튜토리얼 못 끝냈을 때의 초기화
    {
        option.keyInputDict.ClearDic();
        userInfo = new UserInfo();
        tutorialInfo = new TutorialInfo();
        stageInfo = new StageInfo();
    }

    public void ResetAfterTuto()  //튜토리얼 끝냈을 때의 데이터 초기화
    {
        UserInfo uInfo = new UserInfo();
        uInfo.monsterLearningDic = userInfo.monsterLearningDic;
        uInfo.uiActiveDic = userInfo.uiActiveDic;
        //uInfo.isGainBodyChangeSlot = userInfo.isGainBodyChangeSlot;
        userInfo.playerStat.ResetAfterRegame();
        uInfo.playerStat = userInfo.playerStat;

        userInfo = uInfo;
    }

    public void Save()
    {
        userInfo.userItems.Save();
        //userInfo.monsterInfoDic.Save();
        userInfo.uiActiveDic.Save();
        userInfo.monsterLearningDic.Save();
        option.keyInputDict.Save();
    }
    public void Load()
    {
        userInfo.userItems.Load();
        //userInfo.monsterInfoDic.Load();
        userInfo.uiActiveDic.Load();
        userInfo.monsterLearningDic.Load();
        option.keyInputDict.Load();
    }
}

[Serializable]
public class TutorialInfo
{
    public bool isEnded = false;
    public bool isEndBodyChangeTuto = false;
    //public string tutorialId;
}

[Serializable]
public class StageInfo
{
    public string currentStageID = "Stage0-01"; //어느 스테이지까지 갔는지
    public DoorDirType passDoorDir;
}

[Serializable]
public class UserInfo
{
    //스탯 정보
    public Stat playerStat = new Stat();  //현재 스탯

    public string currentBodyID = Global.OriginBodyID; //현재 장착중인 몸 아이디

    //public string quikSlotItemID; //퀵슬롯에 등록된 아이템 아이디

    //public List<Triple<int, int, int>> limitedBattleCntItems = new List<Triple<int, int, int>>(); //n교전 후에 사라지는 아이템들 리스트 (아이디, 현재 교전 수, 최대 교전 수(가 되면 사라짐))
    public SaveDic<string, ItemInfo> userItems = new SaveDic<string, ItemInfo>(); //인벤토리 목록 
    //public SaveDic<string, MonsterInfo> monsterInfoDic = new SaveDic<string, MonsterInfo>(); //몬스터 이해도(동화율), 흡수 확률 등의 정보
    public SaveDic<string, MonsterLearningInfo> monsterLearningDic = new SaveDic<string, MonsterLearningInfo>();

    public SaveDic<KeyAction, bool> uiActiveDic = new SaveDic<KeyAction, bool>();
    //public bool isGainBodyChangeSlot = false;
}

[Serializable]
public class Option
{
    public SaveDic<KeyAction, KeyCode> keyInputDict = new SaveDic<KeyAction, KeyCode>();  //키세팅

    //효과
    public bool IsAtkShakeCamera = true;  //슬라임 기본 공격했을 때 화면 흔들림 여부
    public bool IsHitShakeCam = true; //플레이어나 적이 쳐맞았을 때 카메라 흔들림 여부
    public bool IsHitTimeFreeze = true; //플레이어나 적이 쳐맞았을 때 타임 프리즈 여부

    //자동퀵슬롯
    //public bool isAutoQuikSlot = true;
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
        keyList.Clear();
        valueList.Clear();
    }
}
#endregion