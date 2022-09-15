using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreProperty : MonoBehaviour
{
    public Image statImg;
    public TextMeshProUGUI nameTMP;
    public Button btn;
    public UIScale UIScaleCtrl;

    public CanvasGroup cvsg;
    public CanvasGroup childCvsg;  //������ �� ������ ��ҵ� Ʈ�� ������ ���� 

    public Text maxLv, abil, growth, point;  //�ִ뷾, �� ����, ���� ���, ����
    public Text curLv; //�Ǹ��ǿ����� ���̴� 

    public ushort ID { get; private set; }
    public bool IsSellItem { get; private set; }   
    public int Point { get; private set; }

    private void Awake()
    {
        btn.onClick.AddListener(()=>StatStore.Instance.OnClickStoreProp(this));
    }

    public void Renewal(ushort id, bool purchase)
    {
        ID = id;
        IsSellItem = !purchase;
        ChoiceStatSO stat = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);
        nameTMP.text = stat.statName;
        statImg.sprite = stat.statSpr;

        btn.interactable = true;
        UIScaleCtrl.transitionEnable = true;
        cvsg.alpha = 1;

        maxLv.text = "�ִ뷹�� : <color=#4444EC>" + stat.maxStatLv.ToString() + "</color>";
        abil.text = string.Format(stat.detailAbilExplanation, Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().ChoiceDataDict[ID].upTargetStatPerChoiceStat);
        growth.text = stat.growthWay;

        curLv.gameObject.SetActive(!purchase);
        if (!purchase)
        {
            curLv.text = "���緹�� : <color=#7C7CFB>" + NGlobal.playerStatUI.choiceStatDic[ID].statLv.ToString() + "</color>";
        }

        if (!IsSellItem) Point = stat.purchase2;
        else Point = stat.sell2 + Mathf.Clamp(NGlobal.playerStatUI.choiceStatDic[ID].statLv - 1, 0, 30) * stat.upCost2;

        point.text = $"<color=yellow>{Point}</color> POINT";
    }

    public void Buy()
    {
        cvsg.alpha = 0.5f;
        btn.interactable = false;
        UIScaleCtrl.transitionEnable = false;
    }

    public void Sell()
    {
        gameObject.SetActive(false);
    }
}
