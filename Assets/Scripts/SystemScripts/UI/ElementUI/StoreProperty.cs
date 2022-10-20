using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

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

    public GameObject[] lines;

    public TextMeshProUGUI boxExText; //(�� �ý���) ���� ������ �� �ϳ��� ��ҵ鸶�� ����ִ� ���� �ؽ�Ʈ

    public ushort ID { get; private set; }
    public bool IsSellItem { get; private set; }   
    public int Point { get; private set; }

    public StatStore.StatBox box { get; private set; }

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

        maxLv.gameObject.SetActive(true);
        abil.gameObject.SetActive(true);
        growth.gameObject.SetActive(true);

        for (int i = 0; i < lines.Length; i++) lines[i].gameObject.SetActive(true);
        boxExText.gameObject.SetActive(false);   

        maxLv.text = "�ִ뷹�� : <color=#4444EC>" + stat.maxStatLv.ToString() + "</color>";
        abil.text = string.Format(stat.detailAbilExplanation, Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().ChoiceDataDict[ID].upTargetStatPerChoiceStat);
        growth.text = stat.growthWay;

        curLv.gameObject.SetActive(!purchase);
        boxExText.gameObject.SetActive(purchase);
        statImg.gameObject.SetActive(!purchase);
        if (!purchase)
        {
            curLv.text = "���緹�� : <color=#7C7CFB>" + NGlobal.playerStatUI.choiceStatDic[ID].statLv.ToString() + "</color>";
        }

        if (!IsSellItem) Point = stat.purchase;
        else Point = stat.sell + Mathf.Clamp(NGlobal.playerStatUI.choiceStatDic[ID].statLv - 1, 0, 30) * stat.upCost;

        point.text = $"<color=yellow>{Point}</color> POINT";
    }

    public void Buy(ushort id = 0)
    {
        cvsg.alpha = 0.5f;
        btn.interactable = false;
        UIScaleCtrl.transitionEnable = false;

        childCvsg.DOFade(0f, 0.3f).SetUpdate(true).OnComplete(() =>
        {
            maxLv.gameObject.SetActive(true);
            abil.gameObject.SetActive(true);
            growth.gameObject.SetActive(true);
            curLv.gameObject.SetActive(true);
            statImg.gameObject.SetActive(true);

            boxExText.gameObject.SetActive(false);

            for (int i = 0; i < lines.Length; i++) lines[i].gameObject.SetActive(true);

            ChoiceStatSO data = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);

            maxLv.text = "�ִ뷹�� : <color=#4444EC>" + data.maxStatLv.ToString() + "</color>";
            abil.text = string.Format(data.detailAbilExplanation, Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().ChoiceDataDict[id].upTargetStatPerChoiceStat);
            growth.text = data.growthWay;
            curLv.text = "���緹�� : <color=#7C7CFB>" + NGlobal.playerStatUI.choiceStatDic[id].statLv.ToString() + "</color>";
            nameTMP.text = data.statName;
            statImg.sprite = data.statSpr;

            Util.DelayFunc(() => childCvsg.DOFade(1f, 0.35f).SetUpdate(true), 0.2f, this, true);
        });
    }

    public void Sell()
    {
        gameObject.SetActive(false);
    }

    public void SetRandomCard(StatStore.StatBox info)   //(���� �ٲ� �ý���) Ư�� ������ �� ī�� ����
    {
        cvsg.alpha = 1f;
        btn.interactable = true;
        UIScaleCtrl.transitionEnable = true;
        transform.rotation = Quaternion.identity;

        maxLv.gameObject.SetActive(false);
        abil.gameObject.SetActive(false);
        growth.gameObject.SetActive(false);
        curLv.gameObject.SetActive(false);
        statImg.gameObject.SetActive(false);
        gameObject.SetActive(true);
        boxExText.gameObject.SetActive(true);

        for(int i=0; i<lines.Length; i++) lines[i].gameObject.SetActive(false);

        nameTMP.text = info.boxName;
        Point = info.needPoint;
        point.text = $"<color=yellow>{Point}</color> POINT";

        ID = 0;
        IsSellItem = false;
        box = info;

        boxExText.text = info.explanation;
    }
}
