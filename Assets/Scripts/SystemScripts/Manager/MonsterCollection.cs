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

    //�� ����� ���� Ȯ��â
    public Triple<Image, Text, Text> mobItemImgNameEx;
    #endregion

    private void Start()
    {
        urmg = PlayerEnemyUnderstandingRateManager.Instance;
        
        mobInfoUIPair.second.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Clamp(urmg.ChangableBodyList.Count / 3 + 1, 6, 10000);

        //��� �� ���� �����ͼ� UI�����ϰ� �� ����
        urmg.ChangableBodyList.ForEach(body =>
        {
            MonsterInfoSlot ui = Instantiate(mobInfoUIPair.first, mobInfoUIPair.second).GetComponent<MonsterInfoSlot>();
            ui.Init(body);
            mobIdToSlot.Add(body.bodyId.ToString(), ui);
        });

        Load();
        AllUpdateCollection();
        AllUpdateDrainProbability();
    }

    public void UpdateCollection(string id)  //�� ��ȭ�� ���� ����
    { 
        mobIdToSlot[id].UpdateAssimilationRate((float)urmg.PlayerEnemyUnderStandingRateDic[id]/urmg.MinBodyChangeUnderstandingRate);
    }

    public void AllUpdateCollection()   //��� �� ��ȭ�� ���� ����
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateCollection(key);
    }

    public void UpdateDrainProbability(string id) //�� ��� Ȯ�� ���� ����
    {
        mobIdToSlot[id].UpdateDrainProbability(urmg.GetMountingPercentageDict(id));
    }
    public void AllUpdateDrainProbability()
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateDrainProbability(key);
    }

    public void Detail(PlayerEnemyUnderstandingRateManager.ChangeBodyData data, string id) //�� ���� �ڼ��� ����
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

    public void CloseDetail() //�� ���� �ڼ��� ���� ����
    {
        selectedDetailMobId = string.Empty;
    }

    #region Detail Stat

    public void DetailStat()
    {
        EternalStat stat = mobIdToSlot[selectedDetailMobId].bodyData.additionalBodyStat;


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

    public void Save()
    {
        foreach(string key in urmg.PlayerEnemyUnderStandingRateDic.Keys)
        {
            GameManager.Instance.savedData.userInfo.monstersAssimilationRate[key] = urmg.PlayerEnemyUnderStandingRateDic[key];       
        }
    }

    public void Load()
    {
        foreach(string key in GameManager.Instance.savedData.userInfo.monstersAssimilationRate.keyList)
        {
            urmg.PlayerEnemyUnderStandingRateDic[key] = GameManager.Instance.savedData.userInfo.monstersAssimilationRate[key];
        }
    }
}
