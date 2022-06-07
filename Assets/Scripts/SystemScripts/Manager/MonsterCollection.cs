using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MonsterCollection : MonoSingleton<MonsterCollection>
{
    private PlayerEnemyUnderstandingRateManager urmg;

    public Dictionary<string, MonsterInfoSlot> mobIdToSlot = new Dictionary<string, MonsterInfoSlot>();

    private Dictionary<string, MonsterLearningInfo> mobLearningInfoDic = new Dictionary<string, MonsterLearningInfo>();
    private MonsterLearningInfo tempDummyMobLearningInfo;

    public Pair<GameObject, Transform> mobInfoUIPair;
    public GameObject trfAbleTxtPref;

    #region Detail View
    private string selectedDetailMobId;
    private MonsterInfoSlot selectedMobSlot;
    public GameObject slotSelectedImg;

    public Triple<Image, TextMeshProUGUI, Text> monsterImgNameEx;
    public Pair<Image, Text> mobTypeImgText;
    //public Pair<TextMeshProUGUI, TextMeshProUGUI> mobDrainProbAndAssimTmp;
    public Pair<Text, Text> mobDrainProbAndAssimTmp;

    public Image mobDropItemImg;

    //몹 드랍템 정보 확인창
    public Triple<Image, Text, Text> mobItemImgNameEx;

    public Text[] statText; //몹으로 변신시 상승 능력치 확인 텍스트
    public Text statIncrRatePerAssim; //동화율 n 오를 때마다 처음 스탯의 m퍼센트만큼 증가함을 나타내는 텍스트

    //몬스터 (주로 스탯의)특성 정보
    public Text featureTxt;

    #endregion

    [SerializeField] private ChangeBodyData defaultSlimeBodyData;

    public string IDToSave { get; set; }  //저장할 몬스터 몸 아이디 일시 저장

    //몹 저장 슬롯 꽉 찼을 때 제거할 슬롯 선택창의 슬롯들
    [Space(15)]
    [SerializeField] private List<ChangeBodySlot> changeBodySlots;

    //Top Left Save Body UI List
    [Space(15)]
    [SerializeField] private List<ChangeableBody> savedBodys;
    [SerializeField] private int maxSavedBodyCount = 3;

    private Vector3 currentBodySlotScale;
    private List<Vector3> savedBodyPosList = new List<Vector3>();

    public Sprite notExistBodySpr; //빈 슬롯일 때의 스프라이트 (몸)
    public Sprite questionSpr; // ? 스프라이트

    private void Awake()
    {
        for(int i=0; i<savedBodys.Count; i++)
        {
            savedBodyPosList.Add(savedBodys[i].RectTrm.anchoredPosition);
        }
        currentBodySlotScale = savedBodys[0].transform.localScale;

        EventManager.StartListening("UpdateKeyCodeUI", UpdateSavedBodyChangeKeyCodeTxt);
    }

    private void Start()
    {
        urmg = PlayerEnemyUnderstandingRateManager.Instance;
        //Water.PoolManager.CreatePool(trfAbleTxtPref, mobInfoUIPair.second, 2, "CanTrfMark");

        mobInfoUIPair.second.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Clamp(urmg.ChangableBodyList.Count / 3 + 1, 6, 10000);
        statIncrRatePerAssim.text = "[동화율 " + (PlayerEnemyUnderstandingRateManager.Instance.UnderstandingRatePercentageWhenUpStat).ToString()  + "%당 "
            + (PlayerEnemyUnderstandingRateManager.Instance.UpStatPercentage * 100f).ToString() + "%씩 스탯 상승]";
        changeBodySlots.ForEach(x => x.SetSlotNumber());

        //모든 몹 정보 가져와서 UI생성하고 값 넣음
        MonsterLearningInfo mlInfo = new MonsterLearningInfo();
        urmg.ChangableBodyList.ForEach(body =>
        {
            MonsterInfoSlot ui = Instantiate(mobInfoUIPair.first, mobInfoUIPair.second).GetComponent<MonsterInfoSlot>();
            ui.Init(body);

            string id = body.bodyId.ToString();
            mobIdToSlot.Add(id, ui);
            mobLearningInfoDic.Add(id, mlInfo);
        });

        savedBodys.ForEach(x => x.InitSet());

        Load();
        AllUpdateUnderstanding();
        AllUpdateDrainProbability();
        AllUpdateMonColImgs();

        foreach (MonsterInfoSlot slot in mobIdToSlot.Values)
            slot.MarkAcqBody(false);

        EventManager.StartListening("PlayerRespawn", () =>
        {
            
            AllUpdateUnderstanding();
            AllUpdateDrainProbability();

            foreach (MonsterInfoSlot slot in mobIdToSlot.Values)
                slot.MarkAcqBody(false);
            for (int i = 1; i <= maxSavedBodyCount; i++)
            {
                RemoveSavedBody(i);
            }
            changeBodySlots.ForEach(x => x.Unregister());

            for (int i = 0; i < savedBodys.Count; i++)
            {
                savedBodys[i].RectTrm.anchoredPosition = savedBodyPosList[i];
                savedBodys[i].transform.localScale = Vector3.one;
            }
            savedBodys[0].transform.localScale = currentBodySlotScale;
        });
        EventManager.StartListening("ChangeBody", (str, dead) =>
        {
            for (int i = 0; i < savedBodys.Count; i++)
            {
                savedBodys[i].CheckUsedMob(str);
                if (!dead)
                    savedBodys[i].StartCoolTimeUI();
            }

            int index = 1;
            for (int i = 0; i < savedBodys.Count; i++)
            {
                if(savedBodys[i].BodyID == str)
                {
                    savedBodys[i].RectTrm.DOAnchorPos(savedBodyPosList[0], 0.3f);
                    savedBodys[i].transform.DOScale(currentBodySlotScale, 0.3f);
                }
                else
                {
                    savedBodys[i].RectTrm.DOAnchorPos(savedBodyPosList[index++], 0.3f);
                    savedBodys[i].transform.DOScale(Vector3.one, 0.3f);
                }
            }
        });
        EventManager.StartListening("EnemyDead", (obj, id) =>
        {
            if (!mobLearningInfoDic[id].kill)
            {
                ChangeLearningStateKill(id, true);
            }
            //Util.DelayFunc(() => UpdateMonsterDetailPanel(id), 0.3f)
        });  ////몹 잡았는데 몹 자세히 보기 열려있으면 새로고침하는데 함수 호출 순서 이슈때문에 약간의 딜레이를 줌
    }

    public void UpdateUnderstanding(string id)  //몹 동화율 정보 업뎃
    {
        if (!mobIdToSlot.ContainsKey(id)) return;

        mobIdToSlot[id].UpdateAssimilationRate((float)urmg.GetUnderstandingRate(id) / 100);
    }

    public void AllUpdateUnderstanding()   //모든 몹 동화율 정보 업뎃
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateUnderstanding(key);
    }

    public void UpdateDrainProbability(string id) //몹 흡수 확률 정보 업뎃
    {
        if (!mobIdToSlot.ContainsKey(id)) return;

        mobIdToSlot[id].UpdateDrainProbability(urmg.GetDrainProbabilityDict(id));
    }
    public void AllUpdateDrainProbability()
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateDrainProbability(key);
    }

    public void AllUpdateMonColImgs() //몬스터 도감에서 모든 몬스터 이미지를 갱신한다. (저장 안된건 ?고 된건 사진 나오게)
    {
        foreach(string key in mobLearningInfoDic.Keys)
        {
            if (mobLearningInfoDic[key].meet)
                mobIdToSlot[key].SetMonsterImg(true);
        }
    }

    public void Detail(MonsterInfoSlot slot, string id) //몹 정보 자세히 보기
    {
        if (selectedDetailMobId == id) return;
        selectedDetailMobId = id;
        selectedMobSlot = slot;

        ChangeBodyData data = slot.BodyData;
        UIManager.Instance.OnUIInteractSetActive(UIType.MONSTERINFO_DETAIL, true);

        if (mobLearningInfoDic[id].meet)
        {
            monsterImgNameEx.first.sprite = data.bodyImg;
            monsterImgNameEx.second.text = data.bodyName;
            monsterImgNameEx.third.text = data.bodyExplanation;

            mobDrainProbAndAssimTmp.first.text = "흡수확률: " + urmg.GetDrainProbabilityDict(id).ToString() + "%";
            mobDrainProbAndAssimTmp.second.text = "동화율: " + urmg.GetUnderstandingRate(id).ToString() + "%";
        }
        else
        {
            monsterImgNameEx.first.sprite = questionSpr;
            monsterImgNameEx.second.text = "???";
            monsterImgNameEx.third.text = data.hint;

            mobDrainProbAndAssimTmp.first.text = "흡수확률: ??%";
            mobDrainProbAndAssimTmp.second.text = "동화율: ??%";
        }

        if (mobLearningInfoDic[id].kill)
        {
            ItemSO item = data.dropItem;
            mobDropItemImg.sprite = item.GetSprite();
            mobDropItemImg.GetComponent<NameInfoFollowingCursor>().explanation = item.itemName;
        }
        else
        {
            mobDropItemImg.sprite = questionSpr;
            mobDropItemImg.GetComponent<NameInfoFollowingCursor>().explanation = "???";
        }

        if (UIManager.Instance.gameUIList[(int)UIType.MONSTERINFO_DETAIL_ITEM].gameObject.activeSelf)
            DetailItem();
        if (UIManager.Instance.gameUIList[(int)UIType.MONSTERINFO_DETAIL_STAT].gameObject.activeSelf)
            DetailStat();
        if (UIManager.Instance.gameUIList[(int)UIType.MONSTERINFO_DETAIL_FEATURE].gameObject.activeSelf)
            DetailFeature();

        Util.SetSlotMark(slotSelectedImg.transform, slot.MobImgBg);
    }

    public void UpdateMonsterDetailPanel(string id) //흡수확률과 동화율 새로 고침한다  (방식을 바꿔서 굳이 할 필요 없음)
    {
        if (Util.IsActiveGameUI(UIType.MONSTERINFO_DETAIL))
        {
            mobDrainProbAndAssimTmp.first.text = ("흡수확률: " + urmg.GetDrainProbabilityDict(id).ToString() + "%");
            mobDrainProbAndAssimTmp.second.text = ("동화율: " + urmg.GetUnderstandingRate(id).ToString() + "%");
        }
    }

    public void CloseDetail() //몹 정보 자세히 보기 닫음
    {
        selectedDetailMobId = string.Empty;
        selectedMobSlot = null;
        slotSelectedImg.SetActive(false);
    }

    public ChangeBodyData GetMonsterInfo(string id)
    {
        if (mobIdToSlot.ContainsKey(id)) return mobIdToSlot[id].BodyData;
        else if (id == Global.OriginBodyID) return defaultSlimeBodyData;
        else return new ChangeBodyData();
    }

    public void MarkAcqBodyFalse(string id)  //도감에서 변신가능 표시 끔
    {
        if(mobIdToSlot.ContainsKey(id))
        {
            mobIdToSlot[id].MarkAcqBody(false);
        }
    }

    #region Detail Stat

    public void DetailStat()
    {
        if (mobLearningInfoDic[selectedDetailMobId].assimilation)
        {
            EternalStat stat = mobIdToSlot[selectedDetailMobId].BodyData.additionalBodyStat;
            EternalStat addiStat = PlayerEnemyUnderstandingRateManager.Instance.GetExtraUpStat(selectedDetailMobId);

            statText[0].text = MinusException(stat.maxHp.statValue) + AdditionalStat(addiStat.maxHp.statValue);
            statText[1].text = MinusException(stat.maxDamage.statValue) + AdditionalStat(addiStat.maxDamage.statValue);  //maxDamage만큼 min/max 데미지를 올려준다
            statText[2].text = MinusException(stat.defense.statValue) + AdditionalStat(addiStat.defense.statValue);
            statText[3].text = MinusException(Mathf.RoundToInt(stat.speed.statValue)) + AdditionalStat(Mathf.RoundToInt(addiStat.speed.statValue));
            statText[4].text = string.Concat(MinusException(stat.criticalRate.statValue), '%') + AdditionalStat(addiStat.criticalRate.statValue, true);
            statText[5].text = string.Concat(MinusException(stat.criticalDamage.statValue), '%') + AdditionalStat(addiStat.criticalDamage.statValue, true);
            statText[6].text = MinusException(stat.intellect.statValue) + AdditionalStat(addiStat.intellect.statValue);
            statText[7].text = MinusException(stat.attackSpeed.statValue) + AdditionalStat(addiStat.attackSpeed.statValue);
        }
        else
        {
            for(int i=0; i<statText.Length; i++)
            {
                statText[i].text = "??";
            }
        }
    }

    private string MinusException(int value)
    {
        if (value < 0) return value.ToString();
        else return "+" + value.ToString();
    }

    private string MinusException(float _value)
    {
        int value = Mathf.RoundToInt(_value);
        if (value < 0) return value.ToString();
        else return "+" + value.ToString();
    }

    private string AdditionalStat(int value, bool percent = false)
    {
        if (!percent) return string.Concat('(', value >= 0 ? "+" : "", value, ')');
        else return string.Concat("(", value >= 0 ? "+" : "", value, "%)");
    }
    private string AdditionalStat(float _value, bool percent = false)
    {
        int value = Mathf.RoundToInt(_value);
        if (!percent) return string.Concat("(", value >= 0 ? "+" : "", value, ')');
        else return string.Concat("(", value >= 0 ? "+" : "", value, "%)");
    }

    #endregion

    #region Detail Item

    public void DetailItem()
    {
        if (mobLearningInfoDic[selectedDetailMobId].kill)
        {
            ItemSO item = mobIdToSlot[selectedDetailMobId].BodyData.dropItem;

            mobItemImgNameEx.first.sprite = item.GetSprite();
            mobItemImgNameEx.second.text = item.itemName;
            mobItemImgNameEx.third.text = item.explanation;
        }
        else
        {
            mobItemImgNameEx.first.sprite = questionSpr;
            mobItemImgNameEx.second.text = "???";
            mobItemImgNameEx.third.text = "?????";
        }
    }

    #endregion

    #region Detail Feature

    public void DetailFeature()
    {
        featureTxt.text = mobLearningInfoDic[selectedDetailMobId].kill ? mobIdToSlot[selectedDetailMobId].BodyData.featureExplanation : "???";
    }

    #endregion

    #region Monster Learning

    public void ChangeLearningStateMeet(string id, bool value)
    {
        tempDummyMobLearningInfo = mobLearningInfoDic[id];
        tempDummyMobLearningInfo.meet = value;
        mobLearningInfoDic[id] = tempDummyMobLearningInfo;

        if (value)
        {
            mobIdToSlot[id].SetMonsterImg(true);
        }
    }
    public void ChangeLearningStateKill(string id, bool value)
    {
        tempDummyMobLearningInfo = mobLearningInfoDic[id];
        tempDummyMobLearningInfo.kill = value;
        mobLearningInfoDic[id] = tempDummyMobLearningInfo;
    }
    public void ChangeLearningStateAssimilation(string id, bool value)
    {
        tempDummyMobLearningInfo = mobLearningInfoDic[id];
        tempDummyMobLearningInfo.assimilation = value;
        mobLearningInfoDic[id] = tempDummyMobLearningInfo;
    }

    public void CheckRecordedMonsters(List<EnemySpawnData> spawnEnemyList)
    {
        List<Enemy.Type> list = new List<Enemy.Type>();
        int i;

        for(i=0; i < spawnEnemyList.Count; i++)
        {
            if(!list.Contains(spawnEnemyList[i].enemyId))
            {
                list.Add(spawnEnemyList[i].enemyId);
            }
        }

        string id;
        for(i=0; i<list.Count; i++)
        {
            id = list[i].ToString();
            if (!mobLearningInfoDic[id].meet)
            {
                ChangeLearningStateMeet(id, true);
            }
        }
    }

    #endregion

    #region ChangeableBodyList Top Left

    public void UpdateSavedBodyChangeKeyCodeTxt()
    {
        for(int i=0; i<savedBodys.Count; i++)
        {
            savedBodys[i].UpdateKeyCodeTxt();
        }
    }
    
    public void AddSavedBody(string id, int slotNumber = -1)
    {
        if (!GameManager.Instance.savedData.userInfo.isGainBodyChangeSlot)
        {
            GameManager.Instance.savedData.userInfo.isGainBodyChangeSlot = true;
            TutorialManager.Instance.GetBodyChangeSlot();
        }

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
        //UIManager.Instance.RequestLogMsg(GetMonsterInfo(id).bodyName + "(를)을 완전히 흡수하였습니다.");
        UIManager.Instance.InsertNoticeQueue(GetMonsterInfo(id).bodyName + " 몸체 획득", 53);
        mobIdToSlot[id].MarkAcqBody(true);
        ChangeLearningStateAssimilation(id, true);
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
                    break;
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

    public void UpdateAllChangeableBody()
    {
        for(int i=0; i< changeBodySlots.Count; i++)
        {
            changeBodySlots[i].UpdateUI();
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
        foreach(string key in mobLearningInfoDic.Keys)
        {
            uInfo.monsterLearningDic[key] = mobLearningInfoDic[key];
        }
    }

    public void Load()
    {
        UserInfo uInfo = GameManager.Instance.savedData.userInfo;
        foreach (string key in uInfo.monsterInfoDic.keyValueDic.Keys)
        {
            urmg.PlayerEnemyUnderStandingRateDic[key] = uInfo.monsterInfoDic[key].understandingRate;
            urmg.DrainProbabilityDict[key] = uInfo.monsterInfoDic[key].absorptionRate;
        }
        foreach(string key in uInfo.monsterLearningDic.keyValueDic.Keys)
        {
            mobLearningInfoDic[key] = uInfo.monsterLearningDic[key];
        }
    }
    #endregion
}
