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
    #endregion

    private void Start()
    {
        urmg = PlayerEnemyUnderstandingRateManager.Instance;

        mobInfoUIPair.second.GetComponent<GridLayoutGroup>().constraintCount = urmg.ChangableBodyList.Count / 3 + 1;

        urmg.ChangableBodyList.ForEach(body =>
        {
            MonsterInfoSlot ui = Instantiate(mobInfoUIPair.first, mobInfoUIPair.second).GetComponent<MonsterInfoSlot>();
            ui.Init(body);
            mobIdToSlot.Add(body.bodyId.ToString(), ui);
        });

        Load();
        AllUpdateCollection();
    }

    public void UpdateCollection(string id)
    { 
        mobIdToSlot[id].UpdateRate((float)urmg.PlayerEnemyUnderStandingRateDic[id]/urmg.MinBodyChangeUnderstandingRate);
    }

    public void AllUpdateCollection()
    {
        foreach(string key in mobIdToSlot.Keys)
            mobIdToSlot[key].UpdateRate((float)urmg.PlayerEnemyUnderStandingRateDic[key] / urmg.MinBodyChangeUnderstandingRate);
    }

    public void Detail(PlayerEnemyUnderstandingRateManager.ChangeBodyData data, string id)
    {
        if (selectedDetailMobId == id) return;

        UIManager.Instance.OnUIInteractSetActive(UIType.MONSTERINFO_DETAIL, true);

        monsterImgNameEx.first.sprite = data.bodyImg;
        monsterImgNameEx.second.text = data.bodyName;
        monsterImgNameEx.third.text = data.bodyExplanation;


    }

    public void CloseDetail()
    {
        selectedDetailMobId = "";
    }
    

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
