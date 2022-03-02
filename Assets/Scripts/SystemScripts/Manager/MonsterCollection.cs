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

    public Text[] statText; //������ ���Ž� ��� �ɷ�ġ Ȯ�� �ؽ�Ʈ

    #endregion

    //�� ���� ���� �� á�� �� ������ ���� ����â�� ���Ե�
    [Space(15)]
    [SerializeField] private List<ChangeBodySlot> changeBodySlots;

    //Bottom Left Save Body UI List
    [Space(15)]
    [SerializeField] private List<ChangeableBody> savedBodys;

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

        EventManager.StartListening("PlayerDead", () =>
        {
            AllUpdateCollection();
            AllUpdateDrainProbability();
        });
    }

    public void UpdateCollection(string id)  //�� ��ȭ�� ���� ����
    { 
        mobIdToSlot[id].UpdateAssimilationRate((float)urmg.PlayerEnemyCollectionDic[id]/urmg.MinBodyChangeUnderstandingRate);
    }

    public void AllUpdateCollection()   //��� �� ��ȭ�� ���� ����
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateCollection(key);
    }

    public void UpdateDrainProbability(string id) //�� ���� Ȯ�� ���� ����
    {
        mobIdToSlot[id].UpdateDrainProbability(urmg.GetDrainProbabilityDict(id));
    }
    public void AllUpdateDrainProbability()
    {
        foreach (string key in mobIdToSlot.Keys)
            UpdateDrainProbability(key);
    }

    public void Detail(ChangeBodyData data, string id) //�� ���� �ڼ��� ����
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

        statText[0].text = "+" + stat.maxHp.ToString();
        statText[1].text = "+" + stat.damage.ToString();
        statText[2].text = "+" + stat.defense.ToString();
        statText[3].text = "+" + Mathf.RoundToInt(Mathf.Abs(stat.speed)).ToString(); //���ǵ尡 ������ �Ҽ��� ������ ����� �� ���Ƽ� �ϴ��� ������ ������ ��.
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
        UserInfo uInfo = GameManager.Instance.savedData.userInfo;
        foreach(string key in urmg.PlayerEnemyCollectionDic.Keys)
        {
            uInfo.monsterInfoDic[key] = new MonsterInfo(key, urmg.PlayerEnemyCollectionDic[key], urmg.GetDrainProbabilityDict(key));
        }
    }

    public void Load()
    {
        UserInfo uInfo = GameManager.Instance.savedData.userInfo;
        foreach (string key in uInfo.monsterInfoDic.keyList)
        {
            urmg.PlayerEnemyCollectionDic[key] = uInfo.monsterInfoDic[key].understandingRate;
            urmg.DrainProbabilityDict[key] = uInfo.monsterInfoDic[key].absorptionRate;
        }
    }
    #endregion
}
