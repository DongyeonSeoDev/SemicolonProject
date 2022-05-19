using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatUI : MonoBehaviour
{
    private Stat playerStat;

    public Pair<Image,Text> statExpPair; // 1: ��������Ʈ ����ġ ��������, 2: ������ �ִ� ��������Ʈ �ؽ�Ʈ
    public Pair<GameObject, Transform> statInfoUIPair;

    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: ����Ʈ, 2: �߰�
    //���ý������� ���߿�

    private int needStatPoint; //���� �ø��� ��ư�� ���콺 �� �� �ø��� ���ؼ� �ʿ��� ���� ����Ʈ ����


    private void Start()
    {
        InitSet();
    }

    private void InitSet()
    {
        if (GameManager.Instance.savedData.tutorialInfo.isEnded)
        {
            SlimeGameManager.Instance.Player.PlayerStat = GameManager.Instance.savedData.userInfo.playerStat;
        }

        playerStat = SlimeGameManager.Instance.Player.PlayerStat;

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

    public float GetCurrentPlayerStat(ushort id)  //�ش� ������ ���� ��ġ�� ��ȯ  
    {
        if(eternalStatDic.ContainsKey(id))
        {
            return eternalStatDic[id].first.statValue + eternalStatDic[id].second.statValue;
        }

        Debug.LogWarning("Not Exist ID : " + id);
        return 0f;
    }

    public void UpdateAllStatUI()  //��� ������ ���� ���Ȱ� ��������Ʈ ��� Ƚ���� ������
    {
        foreach(StatInfoElement item in statInfoUIDic.Values)
        {
            item.UpdateUI();
        }
    }

    public void UpdateStatUI(ushort id)  //������ ������ ���� ���Ȱ� ��������Ʈ ��� Ƚ���� ������
    {
        statInfoUIDic[id].UpdateUI();
    }

    public void UpdateStatExp(bool tweening)  //��������Ʈ ����ġ�ٸ� ������
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

    public void UpdateCurStatPoint(bool statUpMark) //���� �������ִ� ��������Ʈ ����
    {
        statExpPair.second.text = statUpMark ? string.Concat(playerStat.currentStatPoint, "<color=red>-", needStatPoint,"</color>") : playerStat.currentStatPoint.ToString();
    }

    public void OnMouseEnterStatUpBtn(int needStatPoint)  //� ������ ���� �ø��� ��ư�� ���콺��ų� �� ��
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
}
