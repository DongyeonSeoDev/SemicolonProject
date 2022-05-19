using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatUI : MonoBehaviour
{
    private Stat playerStat;

    public Pair<Image,Text> statExpPair; // 1: ��������Ʈ ����ġ ��������, 2: ������ �ִ� ��������Ʈ �ؽ�Ʈ
    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();
   
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: ����Ʈ, 2: �߰�
    //���ý������� ���߿�

    private int needStatPoint; //���� �ø��� ��ư�� ���콺 �� �� �ø��� ���ؼ� �ʿ��� ���� ����Ʈ ����


    private void Start()
    {
        DicInit();
    }

    private void DicInit()
    {
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;
        //���⼭ ���� ������ Dic�� ��´�
    }

    public void UpdateAllStatUI()
    {
        foreach(StatInfoElement item in statInfoUIDic.Values)
        {
            item.UpdateUI();
        }
    }

    public void UpdateStatUI(ushort id)
    {
        statInfoUIDic[id].UpdateUI();
    }

    public void UpdateStatExp(bool tweening)
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

    public void UpdateCurStatPoint(bool statUpMark)
    {
        statExpPair.second.text = statUpMark ? string.Concat(playerStat.currentStatPoint, "<color=red>-", needStatPoint,"</color>") : playerStat.currentStatPoint.ToString();
    }

    public void OnMouseEnterStatUpBtn(int needStatPoint)
    {
        if(needStatPoint == -1)
        {
            UpdateCurStatPoint(false);
            return;
        }  
        this.needStatPoint = needStatPoint;
    }
}
