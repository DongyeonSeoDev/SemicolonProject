using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MonsterCollection : MonoSingleton<MonsterCollection>
{
    private PlayerEnemyUnderstandingRateManager urmg;

    public Dictionary<string, MonsterInfoSlot> mobIdToSlot = new Dictionary<string, MonsterInfoSlot>();

    public Pair<GameObject, Transform> mobInfoUIPair;

    #region Detail View
    private string selectedDetailMobId;

    public Triple<Image, Text, Text> monsterImgNameEx;

    public Image mobDropItemImg;

    //몹 드랍템 정보 확인창
    public Triple<Image, Text, Text> mobItemImgNameEx;

    public Text[] statText; //몹으로 변신시 상승 능력치 확인 텍스트
    public Text statIncrRatePerAssim; //동화율 n 오를 때마다 처음 스탯의 m퍼센트만큼 증가함을 나타내는 텍스트

    #endregion

    //몹 저장 슬롯 꽉 찼을 때 제거할 슬롯 선택창의 슬롯들
    [Space(15)]
    [SerializeField] private List<ChangeBodySlot> changeBodySlots;

    //Bottom Left Save Body UI List
    [Space(15)]
    [SerializeField] private List<ChangeableBody> savedBodys;

    public Sprite notExistBodySpr; //빈 슬롯일 때의 스프라이트 (몸)

    private void Start()
    {
        urmg = PlayerEnemyUnderstandingRateManager.Instance;
        
        mobInfoUIPair.second.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Clamp(urmg.ChangableBodyList.Count / 3 + 1, 6, 10000);
        statIncrRatePerAssim.text = "[동화율 10%당 " + (SlimeGameManager.Instance.UpStatPercentage * 100f).ToString() + "%씩 스탯 상승]";
        changeBodySlots.ForEach(x => x.SetSlotNumber());

        //모든 몹 정보 가져와서 UI생성하고 값 넣음
        urmg.ChangableBodyList.ForEach(body =>
        {
            MonsterInfoSlot ui = Instantiate(mobInfoUIPair.first, mobInfoUIPair.second).GetComponent<MonsterInfoSlot>();
            ui.Init(body);
            mobIdToSlot.Add(body.bodyId.ToString(), ui);
        });

        Load();
        AllUpdateUnderstanding();
        AllUpdateDrainProbability();

        EventManager.StartListening("PlayerDead", () =>
        {
            AllUpdateUnderstanding();
            AllUpdateDrainProbability();
        });
        EventManager.StartListening("ChangeBody", () => 
        {
            for(int i=0; i< savedBodys.Count; i++)
            {
                savedBodys[i].StartCoolTimeUI();
            }
        });
    }

    public void UpdateUnderstanding(string id)  //몹 동화율 정보 업뎃
    { 
        mobIdToSlot[id].UpdateAssimilationRate((float)urmg.PlayerEnemyUnderStandingRateDic[id]/urmg.MinBodyChangeUnderstandingRate);
    }

    public void AllUpdateUnderstanding()   //모든 몹 동화율 정보 업뎃
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateUnderstanding(key);
    }

    public void UpdateDrainProbability(string id) //몹 흡수 확률 정보 업뎃
    {
        mobIdToSlot[id].UpdateDrainProbability(urmg.GetDrainProbabilityDict(id));
    }
    public void AllUpdateDrainProbability()
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateDrainProbability(key);
    }

    public void Detail(ChangeBodyData data, string id) //몹 정보 자세히 보기
    {
        if (selectedDetailMobId == id) return;
        selectedDetailMobId = id;

        UIManager.Instance.OnUIInteractSetActive(UIType.MONSTERINFO_DETAIL, true);

        monsterImgNameEx.first.sprite = data.bodyImg;
        monsterImgNameEx.second.text = data.bodyName;
        monsterImgNameEx.third.text = data.bodyExplanation;

        ItemSO item = data.dropItem;
        mobDropItemImg.sprite = item.GetSprite();
        mobDropItemImg.GetComponent<NameInfoFollowingCursor>().explanation = item.itemName;
    }

    public void CloseDetail() //몹 정보 자세히 보기 닫음
    {
        selectedDetailMobId = string.Empty;
    }

    #region Detail Stat

    public void DetailStat()
    {
        EternalStat stat = mobIdToSlot[selectedDetailMobId].bodyData.additionalBodyStat;

        statText[0].text = "+" + stat.maxHp.ToString();
        statText[1].text = "+" + stat.maxDamage.ToString();
        statText[2].text = "+" + stat.defense.ToString();
        statText[3].text = "+" + Mathf.RoundToInt(Mathf.Abs(stat.speed)).ToString(); 
        statText[4].text = "+" + string.Concat(stat.criticalRate, '%');
        statText[5].text = string.Concat( '+', stat.criticalDamage);
        statText[6].text = string.Concat('+', stat.intellect);
    }

    #endregion

    #region Detail Item

    public void DetailItem()
    {
        ItemSO item = mobIdToSlot[selectedDetailMobId].bodyData.dropItem;

        mobItemImgNameEx.first.sprite = item.GetSprite();
        mobItemImgNameEx.second.text = item.itemName;
        mobItemImgNameEx.third.text = item.explanation;
    }

    #endregion

    #region ChangeableBodyList Bottom Left

    public void UpdateSavedBodyChangeKeyCodeTxt()
    {
        for(int i=0; i<savedBodys.Count; i++)
        {
            savedBodys[i].UpdateKeyCodeTxt();
        }
    }
    
    public void AddSavedBody(string id, int slotNumber = -1)
    {
        int i;
        if (slotNumber == -1) 
        {
            for (i = 0; i < savedBodys.Count; i++)
            {
                if(!savedBodys[i].Registered)
                {
                    savedBodys[i].Register(id);
                }
            }
        }
        else
        {
            savedBodys.Find(x => x.SlotNumber == slotNumber).Register(id);
        } 
    }

    public void RemoveSavedBody(int slotNumber)
    {
        for (int i = 0; i < savedBodys.Count; i++)
        {
            if (savedBodys[i].SlotNumber == slotNumber)
            {
                savedBodys[i].Unregister();
            }
        }
    }

    #endregion

    #region RemoveableBodySlotList

    //함수명 변수명 뭐라고 해야할지 애매하네
    public void AddBody(string id, int slotNumber = -1)
    {
        int i;
        if (slotNumber == -1)
        {
            for (i = 0; i < changeBodySlots.Count; i++)
            {
                if (!changeBodySlots[i].Registered)
                {
                    changeBodySlots[i].Register(id, "몬스터 이름");
                }
            }
        }
        else
        {
            changeBodySlots.Find(x => x.SlotNumber == slotNumber).Register(id, "몬스터 이름");
        }
    }

    public void RemoveBody(int slotNumber)
    {
        for (int i = 0; i < changeBodySlots.Count; i++)
        {
            if (changeBodySlots[i].SlotNumber == slotNumber)
            {
                changeBodySlots[i].Unregister();
            }
        }
    }

    #endregion

    #region save and load
    public void Save()
    {
        UserInfo uInfo = GameManager.Instance.savedData.userInfo;
        foreach(string key in urmg.PlayerEnemyUnderStandingRateDic.Keys)
        {
            uInfo.monsterInfoDic[key] = new MonsterInfo(key, urmg.PlayerEnemyUnderStandingRateDic[key], urmg.GetDrainProbabilityDict(key));
        }
    }

    public void Load()
    {
        UserInfo uInfo = GameManager.Instance.savedData.userInfo;
        foreach (string key in uInfo.monsterInfoDic.keyList)
        {
            urmg.PlayerEnemyUnderStandingRateDic[key] = uInfo.monsterInfoDic[key].understandingRate;
            urmg.DrainProbabilityDict[key] = uInfo.monsterInfoDic[key].absorptionRate;
        }
    }
    #endregion
}
