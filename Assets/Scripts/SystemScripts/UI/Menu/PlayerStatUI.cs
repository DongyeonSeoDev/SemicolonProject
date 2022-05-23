using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatUI : MonoBehaviour
{
    private Stat playerStat;
    public Stat PlayerStat => playerStat;

    public Pair<Image,Text> statExpPair; // 1: 스탯포인트 경험치 게이지바, 2: 가지고 있는 스탯포인트 텍스트
    public Pair<GameObject, Transform> statInfoUIPair;

    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: 디폴트, 2: 추가
    //선택스탯쪽은 나중에

    private int needStatPoint; //스탯 올리기 버튼에 마우스 댈 때 올리기 위해서 필요한 스탯 포인트 저장

    private Dictionary<ushort, StatSO> statDataDic;

    public StatSO GetStatSOData(ushort id)
    {
        if(statDataDic.ContainsKey(id))
        {
            return statDataDic[id];
        }
        Debug.Log("존재하지 않는 Stat ID : " + id);
        return null;
    }
    public T GetStatSOData<T>(ushort id) where T : StatSO
    {
        if (statDataDic.ContainsKey(id))
        {
            return statDataDic[id] as T;
        }
        Debug.Log("존재하지 않는 Stat ID : " + id);
        return null;
    }

    private void Awake()
    {
        statDataDic = new Dictionary<ushort, StatSO>(); 
        foreach(StatSO so in Resources.LoadAll<StatSO>("System/StatData/"))
        {
            statDataDic.Add(so.statId, so);
        }
    }

    private void Start()
    {
        InitSet();
    }

    private void InitSet()
    {
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

        if (GameManager.Instance.savedData.tutorialInfo.isEnded)
        {
            playerStat = GameManager.Instance.savedData.userInfo.playerStat;
            Debug.Log("Player Stat is Loaded");
            SlimeGameManager.Instance.Player.PlayerStat = playerStat;
        }
        else
        {
            GameManager.Instance.savedData.userInfo.playerStat = playerStat;
        }

        eternalStatDic.Add(NGlobal.MaxHpID, new Pair<StatElement, StatElement>(playerStat.eternalStat.maxHp, playerStat.additionalEternalStat.maxHp));
        eternalStatDic.Add(NGlobal.MinDamageID, new Pair<StatElement, StatElement>(playerStat.eternalStat.minDamage, playerStat.additionalEternalStat.minDamage));
        eternalStatDic.Add(NGlobal.MaxDamageID, new Pair<StatElement, StatElement>(playerStat.eternalStat.maxDamage, playerStat.additionalEternalStat.maxDamage));
        eternalStatDic.Add(NGlobal.DefenseID, new Pair<StatElement, StatElement>(playerStat.eternalStat.defense, playerStat.additionalEternalStat.defense));
        eternalStatDic.Add(NGlobal.IntellectID, new Pair<StatElement, StatElement>(playerStat.eternalStat.intellect, playerStat.additionalEternalStat.intellect));
        eternalStatDic.Add(NGlobal.SpeedID, new Pair<StatElement, StatElement>(playerStat.eternalStat.speed, playerStat.additionalEternalStat.speed));
        eternalStatDic.Add(NGlobal.AttackSpeedID, new Pair<StatElement, StatElement>(playerStat.eternalStat.attackSpeed, playerStat.additionalEternalStat.attackSpeed));
        eternalStatDic.Add(NGlobal.CriticalRate, new Pair<StatElement, StatElement>(playerStat.eternalStat.criticalRate, playerStat.additionalEternalStat.criticalRate));
        eternalStatDic.Add(NGlobal.CriticalDamage, new Pair<StatElement, StatElement>(playerStat.eternalStat.criticalDamage, playerStat.additionalEternalStat.criticalDamage));

        foreach(ushort key in eternalStatDic.Keys)
        {
            StatInfoElement el = Instantiate(statInfoUIPair.first, statInfoUIPair.second).GetComponent<StatInfoElement>();
            el.InitSet(eternalStatDic[key].first);
            statInfoUIDic.Add(key, el);
        }
    }

    public float GetCurrentPlayerStat(ushort id)  //해당 스탯의 최종 수치를 반환  
    {
        if(eternalStatDic.ContainsKey(id))
        {
            return eternalStatDic[id].first.statValue + eternalStatDic[id].second.statValue;
        }

        Debug.LogWarning("Not Exist ID : " + id);
        return 0f;
    }

    public void UpdateAllStatUI()  //모든 스탯의 현재 스탯과 스탯포인트 사용 횟수를 업뎃함
    {
        foreach(StatInfoElement item in statInfoUIDic.Values)
        {
            item.UpdateUI();
        }
    }

    public void UpdateStatUI(ushort id)  //선택한 스탯의 현재 스탯과 스탯포인트 사용 횟수를 업뎃함
    {
        statInfoUIDic[id].UpdateUI();
    }

    public void UpdateStatExp(bool tweening)  //스탯포인트 경험치바를 업뎃함
    {
        float rate = playerStat.currentExp / playerStat.maxExp;
        if(tweening)
        {
            statExpPair.first.DOFillAmount(rate, 0.3f).SetUpdate(true);
        }
        else
        {
            statExpPair.first.fillAmount = rate;
        }
    }

    public void UpdateCurStatPoint(bool statUpMark) //현재 가지고있는 스탯포인트 업뎃
    {
        statExpPair.second.text = statUpMark ? string.Concat(playerStat.currentStatPoint, "<color=red>-", needStatPoint,"</color>") : playerStat.currentStatPoint.ToString();
    }

    public void OnMouseEnterStatUpBtn(int needStatPoint)  //어떤 스탯의 스탯 올리기 버튼에 마우스대거나 뗄 때
    {
        this.needStatPoint = needStatPoint;
        UpdateCurStatPoint(needStatPoint != -1);
    }

    public bool CanStatUp(ushort id) => Mathf.Pow(2, eternalStatDic[id].first.statLv) <= playerStat.currentStatPoint;

    public void StatUp(ushort id)
    {
        UpdateCurStatPoint(false);
        StatElement stat = eternalStatDic[id].first;
        int value = (int)Mathf.Pow(2, stat.statLv);
        playerStat.currentStatPoint -= value;
        playerStat.accumulateStatPoint += value;
        stat.statLv++;
        stat.statValue += stat.upStatValue;
        //eternalStatDic[id].second.statValue += stat.upStatValue;

    }

    public void AddPlayerStatPointExp(float value)
    {
        playerStat.currentExp += value;
        if(playerStat.currentExp >= playerStat.maxExp)
        {
            int point = Mathf.FloorToInt(playerStat.currentExp / playerStat.maxExp);
            playerStat.currentExp -= point * playerStat.maxExp;
            playerStat.currentStatPoint += point;
        }
    }

    public void StatUnlock(StatElement se)
    {
        se.isUnlock = true;
        statInfoUIDic[se.id].gameObject.SetActive(true);
    }

    public void Save()
    {
        //튜토리얼이 끝났다면 플레이어의 스탯정보를 저장
        /*if (GameManager.Instance.savedData.tutorialInfo.isEnded)
        {
            GameManager.Instance.savedData.userInfo.playerStat = playerStat;
        }*/
    }
}
