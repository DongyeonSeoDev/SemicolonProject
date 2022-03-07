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
    public GameObject trfAbleTxtPref;

    #region Detail View
    private string selectedDetailMobId;

    public Triple<Image, Text, Text> monsterImgNameEx;

    public Image mobDropItemImg;

    //몹 드랍템 정보 확인창
    public Triple<Image, Text, Text> mobItemImgNameEx;

    public Text[] statText; //몹으로 변신시 상승 능력치 확인 텍스트
    public Text statIncrRatePerAssim; //동화율 n 오를 때마다 처음 스탯의 m퍼센트만큼 증가함을 나타내는 텍스트

    #endregion

    [SerializeField] private ChangeBodyData defaultSlimeBodyData;
    [SerializeField] private string defaultSlimeID;

    //몹 저장 슬롯 꽉 찼을 때 제거할 슬롯 선택창의 슬롯들
    [Space(15)]
    [SerializeField] private List<ChangeBodySlot> changeBodySlots;

    //Bottom Left Save Body UI List
    [Space(15)]
    [SerializeField] private List<ChangeableBody> savedBodys;
    [SerializeField] private int maxSavedBodyCount = 3;

    public Sprite notExistBodySpr; //빈 슬롯일 때의 스프라이트 (몸)

    private void Start()
    {
        urmg = PlayerEnemyUnderstandingRateManager.Instance;
        Water.PoolManager.CreatePool(trfAbleTxtPref, mobInfoUIPair.second, 2, "CanTrfMark");
        
        mobInfoUIPair.second.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Clamp(urmg.ChangableBodyList.Count / 3 + 1, 6, 10000);
        statIncrRatePerAssim.text = "[동화율 10%당 " + (SlimeGameManager.Instance.UpStatPercentage * 100f).ToString() + "%씩 스탯 상승]";
        changeBodySlots.ForEach(x => x.SetSlotNumber());

        {  //정보 읽기용으로 기본 슬라임(주인공) 정보도 딕셔너리에 넣어줌
            MonsterInfoSlot ui = Instantiate(mobInfoUIPair.first, mobInfoUIPair.second).GetComponent<MonsterInfoSlot>();
            ui.Init(defaultSlimeBodyData);
            mobIdToSlot.Add(defaultSlimeID, ui);
            ui.gameObject.SetActive(false);
        }
        //모든 몹 정보 가져와서 UI생성하고 값 넣음
        urmg.ChangableBodyList.ForEach(body =>
        {
            MonsterInfoSlot ui = Instantiate(mobInfoUIPair.first, mobInfoUIPair.second).GetComponent<MonsterInfoSlot>();
            ui.Init(body);
            mobIdToSlot.Add(body.bodyId.ToString(), ui);
        });

        savedBodys.ForEach(x => x.InitSet());

        Load();
        AllUpdateUnderstanding();
        AllUpdateDrainProbability();

        EventManager.StartListening("PlayerDead", () =>
        {
            AllUpdateUnderstanding();
            AllUpdateDrainProbability();

            foreach (MonsterInfoSlot slot in mobIdToSlot.Values)
                slot.MarkAcqBody(false);
            for(int i=2; i<=maxSavedBodyCount; i++)
            {
                RemoveSavedBody(i);
            }
            changeBodySlots.ForEach(x => x.Unregister());
        });
        EventManager.StartListening("ChangeBody", str => 
        {
            string.IsNullOrEmpty(str); //매개변수 string인걸 나타내기위한 것
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

    public ChangeBodyData GetMonsterInfo(string id) => mobIdToSlot[id].BodyData;

    #region Detail Stat

    public void DetailStat()
    {
        EternalStat stat = mobIdToSlot[selectedDetailMobId].BodyData.additionalBodyStat;

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
        ItemSO item = mobIdToSlot[selectedDetailMobId].BodyData.dropItem;

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
        if (slotNumber == -1) 
        {
            for (int i = 0; i < savedBodys.Count; i++)
            {
                if(!savedBodys[i].Registered)
                {
                    savedBodys[i].Register(id);
                    break;
                }
            }
        }
        else  //원하는 (이미 저장된)슬롯 하나 제거하고 넣음
        {
            if(slotNumber == 1)
            {
                Debug.Log("기본 캐릭터에 덮어씌우기 못함");
                return;
            }
            savedBodys[slotNumber-1].Register(id);  
        }

        EffectManager.Instance.OnTopRightBtnEffect(UIType.MONSTER_COLLECTION, true);
        mobIdToSlot[id].MarkAcqBody(true);
    }

    public void RemoveSavedBody(int slotNumber)
    {
        if (slotNumber == 1)
        {
            Debug.Log("기본 캐릭터는 삭제 못함");
            return;
        }

        savedBodys[slotNumber - 1].Unregister();          //2번 슬롯을 삭제하려면 인덱스 1로 접근해야함

        /*for (int i = 0; i < savedBodys.Count; i++)
        {
            if (savedBodys[i].SlotNumber == slotNumber)
            {
                savedBodys[i].Unregister();
            }
        }*/
    }

    #endregion

    #region RemoveableBodySlotList

    //함수명 변수명 뭐라고 해야할지 애매하네
    public void AddBody(string id, int slotNumber = -1)
    {
        if (slotNumber == -1)
        {
            for (int i = 0; i < changeBodySlots.Count; i++)
            {
                if (!changeBodySlots[i].Registered)
                {
                    changeBodySlots[i].Register(id);
                }
            }
        }
        else
        {
            //changeBodySlots.Find(x => x.SlotNumber == slotNumber).Register(id, "몬스터 이름");
            changeBodySlots[slotNumber - 1].Register(id);
        }
    }

    public void RemoveBody(int slotNumber)
    {
        changeBodySlots[slotNumber - 1].Unregister();
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
