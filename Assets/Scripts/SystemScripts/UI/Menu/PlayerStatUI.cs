using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PlayerStatUI : MonoBehaviour
{
    private Stat playerStat;

    public Pair<Image,Text> statExpPair; // 1: 스탯포인트 경험치 게이지바, 2: 가지고 있는 스탯포인트 텍스트
    private Dictionary<ushort, StatInfoElement> statInfoUIDic = new Dictionary<ushort, StatInfoElement>();
   
    public Dictionary<ushort, Pair<StatElement,StatElement>> eternalStatDic = new Dictionary<ushort, Pair<StatElement, StatElement>>(); // 1: 디폴트, 2: 추가
    //선택스탯쪽은 나중에

    private int needStatPoint; //스탯 올리기 버튼에 마우스 댈 때 올리기 위해서 필요한 스탯 포인트 저장


    private void Start()
    {
        DicInit();
    }

    private void DicInit()
    {
        playerStat = SlimeGameManager.Instance.Player.PlayerStat;
        //여기서 스탯 정보를 Dic에 담는다
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
