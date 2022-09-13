using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Water;
using System.Text;

public class StatStore : MonoSingleton<StatStore>   
{
    [SerializeField] private int maxStockAmount = 3;  //상점에서 판매중인 특성을 몇 개까지 보여주는지
    [SerializeField] private int maxRechargeCount = 2; //판매중인 특성 목록 새로 갱신 몇 번까지 가능한지
    [SerializeField] private int rechargeNeedPoint = 2; //리롤에 필요한 스탯포인트

    public Pair<GameObject, Transform> storePropertyPrefInfo;
    public Transform userPropPrefParent;

    private List<StoreProperty> storeProperties = new List<StoreProperty>();
    private List<ushort> allPropIDList = new List<ushort>();
    private List<ushort> purchasedPropIDList = new List<ushort>();
    private List<ushort> prevStockIDList = new List<ushort>();  //바로 이전에 팔던 특성들의 ID 리스트
    private int curRechargeCount;
    private int availableCount;   //아직 없거나 만렙이 아닌 특성들의 수

    private StoreProperty selectedProp;

    private Queue<bool> tweeningQueue = new Queue<bool>();

    public NameInfoFollowingCursor updateNifc;

    #region Panel

    public TextMeshProUGUI stockUpdateTMP;
    public Text[] userPointTexts;
    public RectTransform buyPanel, sellPanel;
    public Scrollbar sellScrollBar;
    private Vector2 panelOriginPos;
    private Vector2 rightPos, leftPos;
    private bool isBuyPanel;  //현재 구매 패널이 띄워져 있는지

    #endregion

    private void Awake()
    {
        for(ushort i = NGlobal.CStatStartID; i<= NGlobal.CStatEndID; i+= NGlobal.StatIDOffset)
        {
            allPropIDList.Add(i);
        }
        for(int i=0; i< maxStockAmount; i++)
        {
            storeProperties.Add(Instantiate(storePropertyPrefInfo.first, storePropertyPrefInfo.second).GetComponent<StoreProperty>());
        }
        panelOriginPos = buyPanel.anchoredPosition;
        rightPos = panelOriginPos + new Vector2(150, 0);
        leftPos = panelOriginPos - new Vector2(150, 0);

        updateNifc.explanation = rechargeNeedPoint.ToString() + "포인트 소모";
    }

    public CharType GetCharType(ushort id) => NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id).charType;

    private void SetIncounter(ref List<ushort> list, int cnt)
    {
        CharType type;
        ushort tmp;
        if (cnt > 0)
        {
            //1번칸은 비밀이나 몬스터
            type = GetCharType(list[0]);
            if (!(type == CharType.SECRET || type == CharType.MONSTER))
            {
                int idx = list.FindIndex(x => (GetCharType(x) == CharType.MONSTER || GetCharType(x) == CharType.SECRET) && !prevStockIDList.Contains(x));  //아무런 값도 못가져왔을 때의 예외처리 필요
                tmp = list[0];
                list[0] = list[idx];
                list[idx] = tmp;
            }

            //2,3번칸은 상점
            /*if (cnt > 1)
            {
                ushort tmp2 = 0;
                for (i = 1; i < cnt; i++)
                {
                    type = GetCharType(list[i]);
                    if (type != CharType.STORE)
                    {
                        int idx = list.FindIndex(x => GetCharType(x) == CharType.STORE && x != tmp2);  //아무런 값도 못가져왔을 때의 예외처리 필요
                        tmp = list[i];
                        list[i] = list[idx];
                        list[idx] = tmp;
                        tmp2 = list[i];

                        Debug.Log(list[i]);
                        Debug.Log(list[idx]);
                    }
                }
            }*/
        }
    }

    public void EnteredStatArea()  //상점 구역 입장했을 때 호출됨
    {
        curRechargeCount = 0;
        purchasedPropIDList.Clear();
        prevStockIDList.Clear();
        stockUpdateTMP.SetText(string.Concat("갱신(", maxRechargeCount, ')'));

        List<ushort> list = allPropIDList.FindAllRandom(id =>
        {
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            return stat.statLv < stat.maxStatLv;
        }, 20);

        availableCount = list.Count;
        int i, cnt = maxStockAmount;

        if (list.Count < maxStockAmount) cnt = list.Count;

        //특성 카드 인카운터
        SetIncounter(ref list, cnt);

        //카드 세팅
        for (i = 0; i < cnt; i++)
        {
            storeProperties[i].Renewal(list[i], true);
            storeProperties[i].gameObject.SetActive(true);
            prevStockIDList.Add(list[i]);
        }
        for (i = cnt; i < maxStockAmount; i++)
        {
            storeProperties[i].gameObject.SetActive(false);
        }
    }

    public void Renewal()  //리롤하기 위한 포인트가 있어야 함. 최대 리롤 가능 횟수만큼 리롤하면 더 이상 리롤 불가
    {
        if (tweeningQueue.Count > 0) return;

        if (curRechargeCount < maxRechargeCount)
        {
            if (NGlobal.playerStatUI.PlayerStat.currentStatPoint >= rechargeNeedPoint)
            {
                if(availableCount <= maxStockAmount)
                {
                    UIManager.Instance.RequestSystemMsg("구매할 수 있는 다른 특성이 없습니다");
                    return; 
                }

                tweeningQueue.Enqueue(false);
                int i, cnt = maxStockAmount;

                for(i=0; i<storeProperties.Count; i++)
                {
                    int si = i;
                    storeProperties[si].childCvsg.DOFade(0, 0.4f).SetUpdate(true);
                }

                NGlobal.playerStatUI.PlayerStat.UseStatPoint(rechargeNeedPoint);
                UpdateUserPoint();
                NGlobal.playerStatUI.UpdateScrStatUI();
                curRechargeCount++;
                purchasedPropIDList.Clear();
                stockUpdateTMP.SetText(string.Concat("갱신(", maxRechargeCount-curRechargeCount, ')'));

                List<ushort> list = allPropIDList.FindAllRandom(id =>
                {
                    StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
                    return stat.statLv < NGlobal.playerStatUI.GetStatSOData(id).maxStatLv && !prevStockIDList.Contains(id);
                }, 15);

                if (list.Count < maxStockAmount) cnt = list.Count;

                SetIncounter(ref list, cnt);

                prevStockIDList.Clear();

                Util.DelayFunc(() =>
                {
                    for (i = 0; i < cnt; i++)
                    {
                        storeProperties[i].Renewal(list[i], true);
                        storeProperties[i].gameObject.SetActive(true);
                        prevStockIDList.Add(list[i]);
                    }
                    for (i = cnt; i < maxStockAmount; i++)
                    {
                        storeProperties[i].gameObject.SetActive(false);
                    }

                    for (i = 0; i < storeProperties.Count; i++)
                    {
                        int si = i;
                        storeProperties[si].childCvsg.DOFade(1, 0.35f).SetUpdate(true);
                    }

                    availableCount = allPropIDList.FindAll(id => NGlobal.playerStatUI.choiceStatDic[id].statLv < NGlobal.playerStatUI.GetStatSOData(id).maxStatLv).Count;

                    Util.DelayFunc(() => tweeningQueue.Dequeue(), 0.42f, this, true);

                }, 0.5f, this, true);
            }
            else
            {
                UIManager.Instance.RequestSystemMsg("포인트가 부족합니다");
            }
        }
        else
        {
            UIManager.Instance.RequestSystemMsg("새로 갱신은 최대 2번까지 가능합니다");
        }
    }

    public void Purchase(ushort id) //purchasedPropIDList에 없는 id만 구매 가능, 구매하기 위한 포인트가 있어야 함
    {
        if (tweeningQueue.Count > 0) return;

        if (!purchasedPropIDList.Contains(id))
        {
            selectedProp.Buy();
            NGlobal.playerStatUI.PlayerStat.UseStatPoint(GetDataSO(id).purchase);
            NGlobal.playerStatUI.UpdateScrStatUI();
            purchasedPropIDList.Add(id);
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            if (!stat.isUnlock)
            {
                NGlobal.playerStatUI.StatUnlock(stat);
            }
            else
            {
                if (stat.statLv < stat.maxStatLv)
                {
                    stat.statLv++;
                    NGlobal.playerStatUI.InsertPropertyInfo(id);
                }
            }
            Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(id);
        }
    }

    public void Sell(ushort id) //어떤 스탯을 가지고 있어야 팔 수 있음. 레벨에 따라서 팔아서 받는 포인트 증가
    {
        if (tweeningQueue.Count > 0) return;

        StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
        if (stat.isUnlock)
        {
            NGlobal.playerStatUI.PlayerStat.GetStatPoint(selectedProp.Point);
            NGlobal.playerStatUI.SellStat(stat);
            selectedProp.Sell();
            NGlobal.playerStatUI.UpdateScrStatUI();
            Global.CurrentPlayer.GetComponent<PlayerChoiceStatControl>().WhenTradeStat(id);
        }
    }

    public void ShowStockList()  //NPC와 말을 걸고 상점 UI 보여줌
    {
        TimeManager.TimePause();
        buyPanel.anchoredPosition = panelOriginPos;
        buyPanel.GetComponent<CanvasGroup>().alpha = 1;
        buyPanel.gameObject.SetActive(true);
        sellPanel.gameObject.SetActive(false);
        isBuyPanel = true;
        UIManager.Instance.OnUIInteract(UIType.STORE);
        UpdateUserPoint();
    }

    public void OnClickStoreProp(StoreProperty prop)  //구매하거나 팔 특성을 클릭함
    {
        selectedProp = prop;
        if (!prop.IsSellItem && prop.Point > NGlobal.playerStatUI.PlayerStat.currentStatPoint)
        {
            UIManager.Instance.RequestSystemMsg("포인트가 부족합니다");
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<color=#495FD9><size=120%>");
            sb.Append(NGlobal.playerStatUI.GetStatSOData(prop.ID).statName);
            sb.Append("</size></color> 특성을 ");
            sb.Append(prop.Point);
            sb.Append("포인트에 ");
            sb.Append(prop.IsSellItem ? "<color=#495FD9>판매</color>하시겠습니까?" : "<color=#495FD9>구매</color>하시겠습니까?");

            System.Action conf = null;

            if (prop.IsSellItem) conf = () => Sell(selectedProp.ID);
            else conf = () => Purchase(selectedProp.ID);
            conf += UpdateUserPoint;

            UIManager.Instance.RequestWarningWindow(conf, sb.ToString());
        }
    }

    public void OnChangePanel()  //구매/판매 패널 전환
    {
        if (tweeningQueue.Count > 0) return;

        tweeningQueue.Enqueue(false);
        isBuyPanel = !isBuyPanel;

        if(!isBuyPanel)
        {
            PoolManager.PoolObjSetActiveFalse("StoreProp");
            for(int i=0; i<allPropIDList.Count; i++)
            {
                if(NGlobal.playerStatUI.choiceStatDic[allPropIDList[i]].isUnlock)
                {
                    StoreProperty sp = PoolManager.GetItem<StoreProperty>("StoreProp");
                    sp.Renewal(allPropIDList[i], false);
                }
            }

            Util.DelayFunc(() => sellScrollBar.value = 0, 0.1f, this, true);
        }

        RectTransform hiddenRt = isBuyPanel ? sellPanel : buyPanel;
        RectTransform appearRt = isBuyPanel ? buyPanel : sellPanel;

        appearRt.anchoredPosition = rightPos;
        appearRt.GetComponent<CanvasGroup>().alpha = 0;
        appearRt.gameObject.SetActive(true);

        hiddenRt.gameObject.SetActive(false);
        appearRt.DOAnchorPos(panelOriginPos, 0.4f).SetUpdate(true);
        appearRt.GetComponent<CanvasGroup>().DOFade(1, 0.4f).SetUpdate(true).OnComplete(()=>tweeningQueue.Dequeue());
    }

    public void UpdateUserPoint()
    {
        string pointTxt = "<color=yellow>" + NGlobal.playerStatUI.PlayerStat.currentStatPoint.ToString() + "</color> POINT";
        for(int i=0; i<userPointTexts.Length; i++)
            userPointTexts[i].text = pointTxt;
    }

    public ChoiceStatSO GetDataSO(ushort id) => NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id);   
}
