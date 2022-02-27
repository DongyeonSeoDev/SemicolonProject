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

    //¸÷ µå¶øÅÛ Á¤º¸ È®ÀÎÃ¢
    public Triple<Image, Text, Text> mobItemImgNameEx;

    //¸÷ ÀúÀå ½½·Ô ²Ë Ã¡À» ¶§ Á¦°ÅÇÒ ½½·Ô ¼±ÅÃÃ¢ÀÇ ½½·Ôµé
    [Space(15)]
    [SerializeField] private List<ChangeBodySlot> changeBodySlots;

    //Bottom Left Save Body UI List
    [Space(15)]
    [SerializeField] private List<ChangeableBody> savedBodys;
    #endregion

    private void Start()
    {
        urmg = PlayerEnemyUnderstandingRateManager.Instance;
        
        mobInfoUIPair.second.GetComponent<GridLayoutGroup>().constraintCount = Mathf.Clamp(urmg.ChangableBodyList.Count / 3 + 1, 6, 10000);

        //¸ðµç ¸÷ Á¤º¸ °¡Á®¿Í¼­ UI»ý¼ºÇÏ°í °ª ³ÖÀ½
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

    public void UpdateCollection(string id)  //¸÷ µ¿È­À² Á¤º¸ ¾÷µ«
    { 
        mobIdToSlot[id].UpdateAssimilationRate((float)urmg.PlayerEnemyUnderStandingRateDic[id]/urmg.MinBodyChangeUnderstandingRate);
    }

    public void AllUpdateCollection()   //¸ðµç ¸÷ µ¿È­À² Á¤º¸ ¾÷µ«
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateCollection(key);
    }

    public void UpdateDrainProbability(string id) //¸÷ Èí¼ö È®·ü Á¤º¸ ¾÷µ«
    {
        mobIdToSlot[id].UpdateDrainProbability(urmg.GetMountingPercentageDict(id));
    }
    public void AllUpdateDrainProbability()
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateDrainProbability(key);
    }

    public void Detail(ChangeBodyData data, string id) //¸÷ Á¤º¸ ÀÚ¼¼È÷ º¸±â
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

    public void CloseDetail() //¸÷ Á¤º¸ ÀÚ¼¼È÷ º¸±â ´ÝÀ½
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

    #region ChangeableBodyList

    public void UpdateSavedBodyChangeKeyCodeTxt()
    {
        for(int i=0; i<savedBodys.Count; i++)
        {
            savedBodys[i].UpdateKeyCodeTxt();
        }
    }

    #endregion

    #region save and load
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
    #endregion
}
