using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Water;

public partial class UIManager : MonoSingleton<UIManager>
{
    #region ���� UI ���� ������
    public List<GameUI> gameUIList = new List<GameUI>();
    [SerializeField] private List<GameUI> activeUIList = new List<GameUI>();

    public Queue<bool> activeUIQueue = new Queue<bool>(); //� UI�� �����ų� ������ �ִϸ��̼�(Ʈ����) ���� �߿� �ٸ� UI (��)Ȱ��ȭ ���� ���� ����
    #endregion

    #region ���콺 ����ٴϴ� ���� UI���� ������
    [Space(20)]
    public Image cursorInfoImg;  //���콺 ����ٴϴ� ���� �ؽ�Ʈ�� �ִ� �̹���
    public Text cursorInfoText; // ���콺 ����ٴϴ� ���� �ؽ�Ʈ
    private RectTransform cursorImgRectTrm;
    private Vector3 cursorInfoImgOffset;

    private bool isOnCursorInfo = false;  //���콺 ����ٴϴ� ���� �ؽ�Ʈ Ȱ��ȭ �����ΰ�
    private float sw; //cursorImgRectTrm�� ó�� �ʺ�(�ּ� �ʺ�)
    public float widthOffset = 39;  //���콺 ����ٴϴ� ���� �ؽ�Ʈ���� �̹��� �ʺ� Ű�ﶧ ���ڴ� Ű�� ����

    private Pair<float, float> screenHalf = new Pair<float, float>();
    #endregion

    #region Inventory Item Detail View
    private int selectedItemId = -1;
    private ItemSlot selectedItemSlot;

    [Space(20)]
    public Image itemImg, itemTypeImg;
    public Text itemExplanation;
    public Text itemCntTxt, itemTypeTxt;
    public TextMeshProUGUI itemNameTmp;
    public Button itemUseBtn, itemJunkBtn;
    #endregion

    #region CombinationFood
    public Pair<Image, Text> combInfoUI;
    #endregion

    #region Item Remove
    public Pair<Image, Text> removeItemInfo;
    public InputField itemRemoveCntInput;
    #endregion

    #region Prefab and Parent
    [Space(20)]
    public GameObject systemMsgPrefab, acquisitionTxtPrefab;
    public Transform systemMsgParent, acquisitionTxtParent;

    public GameObject npcNameUIPrefab;
    public Transform npcUICvsTrm;
    #endregion

    public Triple<Image, TextMeshProUGUI, Image> playerHPInfo;
    private bool isStartDelayHPFillTimer;
    private float setDelayHPFillTime;

    //public Text statText;
    public Text[] statTexts;

    private GameManager gm;
    private SlimeGameManager sgm;

    private void Awake()
    {
        cursorImgRectTrm = cursorInfoImg.GetComponent<RectTransform>();
        sw = cursorImgRectTrm.rect.width;

        CreatePool();
        noticeMsgGrd = noticeUIPair.first.GetComponent<NoticeMsg>().msgTmp.colorGradient;
        ordinaryCvs.worldCamera = Camera.main;
        ordinaryCvs.planeDistance = 20;
    }

    private void CreatePool()
    {
        //UI���� Ǯ ����
        PoolManager.CreatePool(systemMsgPrefab, systemMsgParent, 5, "SystemMsg");
        PoolManager.CreatePool(npcNameUIPrefab, npcUICvsTrm, 2, "NPCNameUI");
        PoolManager.CreatePool(acquisitionTxtPrefab, acquisitionTxtParent, 5, "AcquisitionMsg");
        PoolManager.CreatePool(noticeUIPair.first, noticeUIPair.second, 2, "NoticeMsg");
        PoolManager.CreatePool(interactionMarkPair.first, interactionMarkPair.second, 2, "InteractionMark");
    }

    private void Start()
    {
        gm = GameManager.Instance;
        sgm = SlimeGameManager.Instance;

        DefineAction();

        screenHalf.first = Screen.width * 0.5f;
        screenHalf.second = Screen.height * 0.5f;
    }

    private void DefineAction()
    {
        Global.AddAction(Global.MakeFood, item =>
        {
            OnUIInteract(UIType.COMBINATION, true); //���� ���� ���� �Ϸ� �г� ���
            ItemInfo info = ((ItemInfo)item);
            ItemSO data = gm.GetItemData(info.id);
            combInfoUI.first.sprite = data.GetSprite();
            combInfoUI.second.text = string.Format("{0} <color=blue>{1}</color>��", data.itemName, info.count);

            RequestLeftBottomMsg(string.Format("�������� ȹ���Ͽ����ϴ�. ({0} +{1})", data.itemName, info.count));
        });

        Global.AddMonoAction(Global.AcquisitionItem, i =>
        {
            Item item = i as Item;
            RequestLeftBottomMsg(string.Format("�������� ȹ���Ͽ����ϴ�. ({0} +{1})", item.itemData.itemName, item.DroppedCnt));

        });

        Global.AddMonoAction(Global.PickupPlant, item =>
        {
            RequestLeftBottomMsg(string.Format("�������� ȹ���Ͽ����ϴ�. ({0} +{1})", ((Pick)item).itemData.itemName, 1));
        });

        Global.AddAction(Global.JunkItem, JunkItem);

        EventManager.StartListening("PlayerDead", () => OnUIInteract(UIType.DEATH, true));
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("GameClear", () => OnUIInteract(UIType.CLEAR, true));
        EventManager.StartListening("TimePause", () => Time.timeScale = 0 );
        EventManager.StartListening("TimeResume", () => Time.timeScale = 1);
    }

    private void Respawn(Vector2 unusedValue) => OnUIInteract(UIType.DEATH, true);

    private void Update()
    {
        UserInput();
        CursorInfo();
        Notice();
        DelayHPFill();
    }

    private void UserInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activeUIList.Count > 0)
            {
                OnUIInteract(activeUIList[activeUIList.Count - 1]._UItype);
            }
            else
            {
                OnUIInteract(UIType.QUIT);
            }
        }
        else if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.INVENTORY]))
        {
            OnUIInteract(UIType.INVENTORY);
        }
        else if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.STAT]))
        {
            OnUIInteract(UIType.STAT);
        }
        else if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.SETTING]))
        {
            OnUIInteract(UIType.SETTING);
        }
    }

    #region UI (��)Ȱ��ȭ ����
    public void OnUIInteractBtnClick(int type) { OnUIInteract((UIType)type); }

    public void OnUIInteract(UIType type, bool ignoreQueue = false) //UI���ų� ����
    {
        if (activeUIQueue.Count > 0 && !ignoreQueue) return;
        if (ExceptionHandler(type)) return;

        activeUIQueue.Enqueue(false);
        GameUI ui = gameUIList[(int)type];

        if (!ui.gameObject.activeSelf)
        {
            ui.ActiveTransition();
        }
        else
        {
            ui.InActiveTransition();
        }
    }

    private bool ExceptionHandler(UIType type) //��ȣ�ۿ뿡 ���� ����ó��
    {
        switch (type)
        {
            case UIType.KEYSETTING:
                if (KeyActionManager.Instance.IsChangingKeySetting)
                    return true;
                break;
            case UIType.SETTING:
                for(int i=0; i<gameMenuList.Count; i++)
                {
                    if (gameMenuList[i].gameObject.activeSelf)
                        return true;
                }
                break;
        }
        return false;
    }

    public void UpdateUIStack(GameUI ui, bool add = true) //�����ִ� UI����Ʈ ����
    {
        FilterStackUI(ui, add);
        if (add)
        {
            ActiveSpecialProcess(ui._UItype);
        }
        else
        {
            ui.gameObject.SetActive(false);
            InActiveSpecialProcess(ui._UItype);
        }
        activeUIQueue.Dequeue();

        if(activeUIQueue.Count == 0 && isOnCursorInfo)
        {
            OffCursorInfoUI();
        }
    }

    public void PreventItrUI(float time)
    {
        activeUIQueue.Enqueue(false);
        Util.DelayFunc(() => activeUIQueue.Dequeue(), time, this, true);
    }

    void FilterStackUI(GameUI ui, bool add) //activeUIList�� �ְų� ���� �ʴ� UI��
    {
        switch (ui._UItype)
        {
            case UIType.CLEAR:
                return;
            case UIType.DEATH:
                return;
        }
        if (add) activeUIList.Add(ui);
        else activeUIList.Remove(ui);
    }

    private void ActiveSpecialProcess(UIType type) //UI ���� ���� Ư���� ó��
    {
        switch (type)
        {
            case UIType.STAT:
                UpdateStatUI();
                EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, false);
                break;
            case UIType.INVENTORY:
                EffectManager.Instance.OnTopRightBtnEffect(UIType.INVENTORY, false);
                break;
            case UIType.SETTING:
                EventManager.TriggerEvent("TimePause");
                break;
        }
    }

    private void InActiveSpecialProcess(UIType type)//UI ���� ���� Ư���� ó��
    {
        switch (type)
        {
            case UIType.PRODUCTION_PANEL:
                CookingManager.Instance.MakeFoodInfoUIReset();
                break;
            case UIType.FOOD_DETAIL:
                CookingManager.Instance.detailID = -1;
                break;
            case UIType.ITEM_DETAIL:
                selectedItemId = -1;
                if (selectedItemSlot)
                {
                    selectedItemSlot.outline.DOKill();
                    selectedItemSlot.outline.enabled = false;
                    selectedItemSlot = null;
                }
                break;
            case UIType.SETTING:
                EventManager.TriggerEvent("TimeResume");
                itrNoticeList.ForEach(x => x.Set());
                break;
            case UIType.CHEF_FOODS_PANEL:
                EventManager.TriggerEvent("TimeResume");
                break;
        }
    }
    #endregion

    #region UI Position
    public void UIPositionReset(UIType type)
    {
        gameUIList[(int)type].ResetPos();
    }

    public void AllUIPositionReset()
    {
        gameUIList.ForEach(ui => ui.ResetPos());
    }
    #endregion

    #region Cursor ����ٴϴ� �����ؽ�Ʈ
    private void CursorInfo()
    {
        if (isOnCursorInfo)
        {
            Vector3 mPos = Input.mousePosition;
            cursorInfoImgOffset.x = mPos.x < screenHalf.first ? Mathf.Abs(cursorInfoImgOffset.x) : -Mathf.Abs(cursorInfoImgOffset.x);
            cursorInfoImgOffset.y = mPos.y < screenHalf.second ? Mathf.Abs(cursorInfoImgOffset.y) : -Mathf.Abs(cursorInfoImgOffset.y);
            cursorImgRectTrm.position = mPos + cursorInfoImgOffset;
        }
    }

    public void SetCursorInfoUI(string msg, int fontSize = 39)
    {
        isOnCursorInfo = true;

        cursorInfoText.text = msg;
        cursorInfoText.fontSize = fontSize;

        cursorImgRectTrm.sizeDelta = new Vector2(Mathf.Clamp(msg.Length * widthOffset, sw, 1000f), cursorImgRectTrm.rect.height);
        cursorInfoImgOffset = new Vector3(cursorImgRectTrm.rect.width, cursorImgRectTrm.rect.height) * 0.5f;

        cursorInfoImg.gameObject.SetActive(true);
    }

    public void OffCursorInfoUI()
    {
        cursorInfoImg.gameObject.SetActive(false);
        isOnCursorInfo = false;
    }
    #endregion


    public void DetailItemSlot(ItemSlot slot)  //�κ��丮���� ������ ���� Ŭ��
    {
        int itemID = slot.itemInfo.id;

        if (selectedItemSlot)
        {
            selectedItemSlot.outline.DOKill();
            selectedItemSlot.outline.enabled = false;
        }
        selectedItemSlot = slot;

        if (selectedItemId == itemID) return;
        else if (selectedItemId == -1) OnUIInteract(UIType.ITEM_DETAIL);
        selectedItemId = itemID;

        ItemSO data = gm.GetItemData(itemID);

        itemImg.sprite = data.GetSprite();
        itemTypeImg.sprite = Global.GetItemTypeSpr(data.itemType);
        itemNameTmp.text = data.itemName;
        itemExplanation.text = data.explanation;
        itemCntTxt.text = string.Format("����: {0}��", gm.GetItemCount(itemID));
        itemTypeTxt.text = Global.GetItemTypeName(data.itemType);

        itemUseBtn.gameObject.SetActive(data.itemType != ItemType.ETC);
        if (data.itemType == ItemType.ETC && ((Ingredient)data).isUseable) itemUseBtn.gameObject.SetActive(true);
    }

    public void RequestSystemMsg(string msg, int fontSize = 35, float existTime = 1.5f) //ȭ�� �߾� ��ܿ� �ߴ� �ý��� �޽���
    {
        PoolManager.GetItem("SystemMsg").GetComponent<SystemMsg>().Set(msg, fontSize, existTime, Util.Change255To1Color(221, 0, 0, 255));
    }
    public void RequestSystemMsg(string msg,Color textColor ,int fontSize = 35, float existTime = 1.5f) //ȭ�� �߾� ��ܿ� �ߴ� �ý��� �޽���
    {
        PoolManager.GetItem("SystemMsg").GetComponent<SystemMsg>().Set(msg, fontSize, existTime, textColor);
    }

    public void RequestLeftBottomMsg(string msg)  //ȭ�� ���� �ϴܿ� ǥ�õǴ� �α� �ؽ�Ʈ
    {
        Text t = PoolManager.GetItem("AcquisitionMsg").GetComponent<Text>();
        t.text = msg;
        Util.DelayFunc(() => t.gameObject.SetActive(false), 2f, this);
    }

    #region �κ� ��ư
    public void OnClickRemoveItemBtn()  //������ ������ ��ư
    {
        removeItemInfo.first.sprite = selectedItemSlot.itemImg.sprite;
        removeItemInfo.second.text = itemNameTmp.text;
        itemRemoveCntInput.text = "1";
        OnUIInteract(UIType.REMOVE_ITEM);
    }

    public void OnClickItemJunkBtn() //������ ������ Ȯ�� ��ư Ŭ��
    {
        int rmCount = int.Parse(itemRemoveCntInput.text);

        if (rmCount <= 0)
        {
            RequestSystemMsg("1�� �̻���� ���� �� �ֽ��ϴ�.");
            return;
        }
        else if (rmCount > gm.GetItemCount(selectedItemId))
        {
            RequestSystemMsg("���� �������� ���� ���� �� �����ϴ�.");
            return;
        }


        Global.ActionTrigger(Global.JunkItem, rmCount);
    }

    void JunkItem(object rmCount)  //�� ��¥ ����
    {
        Inventory.Instance.RemoveItem(selectedItemId, (int)rmCount);

        OnUIInteract(UIType.REMOVE_ITEM);

        if (selectedItemSlot.itemInfo == null)
            OnUIInteract(UIType.ITEM_DETAIL, true);
        if (gameUIList[(int)UIType.PRODUCTION_PANEL].gameObject.activeSelf)
            OnUIInteract(UIType.PRODUCTION_PANEL, true);
    }

    public void OnClickItemUseBtn()
    {
        gm.GetItemData(selectedItemId).Use();
        Inventory.Instance.RemoveItem(selectedItemId, 1);

        if (selectedItemSlot.itemInfo == null)
            OnUIInteract(UIType.ITEM_DETAIL, true);
    }
    #endregion

    #region Stat
    public void UpdateStatUI()
    {
        Stat stat = sgm.Player.PlayerStat;

        statTexts[0].text = string.Concat(Mathf.Clamp(sgm.Player.CurrentHp, 0, stat.MaxHp), '/', stat.MaxHp);
        statTexts[1].text = stat.Damage.ToString();
        statTexts[2].text = stat.Defense.ToString();
        statTexts[3].text = Mathf.RoundToInt(Mathf.Abs(stat.Speed)).ToString();


        //statText.text = $"HP\t\t{currentHP}/{stat.hp}\n\n���ݷ�\t\t{stat.damage}\n\n����\t\t{stat.defense}\n\n�̵��ӵ�\t\t{stat.speed}";
    }

    public void UpdatePlayerHPUI(bool decrease = false)
    {
        Player p = sgm.Player;
        int hp = Mathf.Clamp(p.CurrentHp, 0, p.PlayerStat.MaxHp);

        float rate = (float)hp / p.PlayerStat.MaxHp;
        playerHPInfo.first.DOFillAmount(rate, 0.3f).OnComplete(()=> { if (!decrease) playerHPInfo.third.fillAmount = playerHPInfo.first.fillAmount; });
        playerHPInfo.second.text = string.Concat(hp, '/', p.PlayerStat.MaxHp);

        if (decrease)
        {
            EffectManager.Instance.OnDamagedUIEffect(rate);
            isStartDelayHPFillTimer = true;
            setDelayHPFillTime = Time.time + 0.5f;
        }
        EffectManager.Instance.SetHPFillEffectScale(rate);
    }

    private void DelayHPFill()
    {
        if(isStartDelayHPFillTimer)
        {
            if(setDelayHPFillTime < Time.time)
            {
                playerHPInfo.third.DOFillAmount(playerHPInfo.first.fillAmount, 0.3f).OnComplete(() => isStartDelayHPFillTimer = false);
            }
        }
    }
    #endregion
}
