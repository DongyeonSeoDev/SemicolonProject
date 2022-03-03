using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Water;
using System;

public partial class UIManager : MonoSingleton<UIManager>
{
    #region 게임 UI 관리 변수들
    public List<GameUI> gameUIList = new List<GameUI>(); //UIType 열겨형 순서에 맞게 UI 옵젝을 인스펙터에 집어넣어야 함
    [SerializeField] private List<GameUI> activeUIList = new List<GameUI>();  //활성화 되어있는 UI들 확인용으로 [SerializeField]

    public Queue<bool> activeUIQueue = new Queue<bool>(); //어떤 UI가 켜지거나 꺼지는 애니메이션(트위닝) 진행 중에 다른 UI (비)활성화 막기 위한 변수
    public Dictionary<UIType, bool> uiTweeningDic = new Dictionary<UIType, bool>(); //해당 UI가 트위닝 중인지(켜지거나 꺼지는 중인지) 확인하기 위함
    #endregion

    #region 마우스 따라다니는 정보 UI관련 변수들
    [Space(20)]
    public Image cursorInfoImg;  //마우스 따라다니는 정보 텍스트가 있는 이미지
    public Text cursorInfoText; // 마우스 따라다니는 정보 텍스트
    private RectTransform cursorImgRectTrm;
    private Vector3 cursorInfoImgOffset;

    private bool isOnCursorInfo = false;  //마우스 따라다니는 정보 텍스트 활성화 상태인가
    private float sw; //cursorImgRectTrm의 처음 너비(최소 너비)
    public float widthOffset = 39;  //마우스 따라다니는 정보 텍스트에서 이미지 너비 키울때 글자당 키울 길이

    private Pair<float, float> screenHalf = new Pair<float, float>();
    #endregion

    #region Inventory Item Detail View
    private int selectedItemId = -1; //클릭한 아이템 슬롯의 아이템 아이디
    private ItemSlot selectedItemSlot; //클릭한 아이템 슬롯

    [Space(20)]
    public Image itemImg, itemTypeImg;
    public Text itemExplanation;
    public Text itemCntTxt, itemTypeTxt;
    public TextMeshProUGUI itemNameTmp;
    public Button itemUseBtn, itemJunkBtn;
    #endregion

    #region CombinationFood
    [Space(10)]
    public Pair<Image, Text> combInfoUI;
    public ParticleSystem combEff;
    #endregion

    #region Item Remove
    [Space(10)]
    public Pair<Image, Text> removeItemInfo;
    public InputField itemRemoveCntInput;
    #endregion

    #region Prefab and Parent
    [Space(20)]
    public GameObject systemMsgPrefab, acquisitionTxtPrefab;
    public Transform systemMsgParent, acquisitionTxtParent;

    public GameObject npcNameUIPrefab; //NPC Name Text UI
    public Transform npcUICvsTrm;  //
    #endregion

    #region HP UI 관련
    [Space(10)]
    public Triple<Image, TextMeshProUGUI, Image> playerHPInfo;  //HPBar Image, HP Text (TMP), Green HPBar (Delay)
    private bool isStartDelayHPFillTimer; // green HP bar decrease soon
    private float setDelayHPFillTime; //time to reduce green hp bar
    #endregion

    #region SelectionWindow
    private Stack<SelectionWindow> selWdStack = new Stack<SelectionWindow>();
    public bool IsSelecting => selWdStack.Count != 0;

    [Space(10)]
    public Pair<GameObject, Transform> selWindowPair;
    public Pair<GameObject, Transform> selectionBtnPair;
    #endregion

    #region CanvasGroup
    [Space(10)]
    public CanvasGroup normalPanelCanvasg;
    public CanvasGroup priorNormalPanelCvsg;
    public CanvasGroup settingCvsg;
    public CanvasGroup worldUICvsg;
    public CanvasGroup ordinaryCvsg;
    public CanvasGroup msgCvsg;
    #endregion

    #region Warning Window UI
    public WarningWindow wnWd;
    #endregion

    [SerializeField] private CanvasGroup loadingCvsg;

    //public Text statText;
    public Text[] statTexts;

    private GameManager gm;
    private SlimeGameManager sgm;

    private void Awake()
    {
        StartLoading();
        InitData();
        CreatePool();
    }

    private void InitData()
    {
        cursorImgRectTrm = cursorInfoImg.GetComponent<RectTransform>();
        sw = cursorImgRectTrm.rect.width;

        noticeMsgGrd = noticeUIPair.first.GetComponent<NoticeMsg>().msgTmp.colorGradient;

        int i;
        for(i=0; i<gameCanvases.Length; i++)
        {
            gameCanvases[i].worldCamera = Util.MainCam;
            gameCanvases[i].planeDistance = Global.cameraPlaneDistance;
        }
        for (i = 0; i < Global.EnumCount<UIType>(); i++)
        {
            uiTweeningDic.Add((UIType)i, false);
        }
    }

    private void CreatePool()
    {
        //UI관련 풀 생성   Pool Create (related UI)
        PoolManager.CreatePool(systemMsgPrefab, systemMsgParent, 5, "SystemMsg");
        PoolManager.CreatePool(npcNameUIPrefab, npcUICvsTrm, 2, "NPCNameUI");
        PoolManager.CreatePool(acquisitionTxtPrefab, acquisitionTxtParent, 5, "AcquisitionMsg");
        PoolManager.CreatePool(noticeUIPair.first, noticeUIPair.second, 2, "NoticeMsg");
        PoolManager.CreatePool(interactionMarkPair.first, interactionMarkPair.second, 2, "InteractionMark");
        PoolManager.CreatePool(selWindowPair.first, selWindowPair.second, 1, "SelWindow");
        PoolManager.CreatePool(selectionBtnPair.first, selectionBtnPair.second, 2, "SelBtn");
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
        //음식을 만듦 (만든 음식 추가)   made food.( add item)
        Global.AddAction(Global.MakeFood, item =>
        {
            OnUIInteract(UIType.COMBINATION, true); //음식 제작 성공 완료 패널 띄움
            combEff.Play();

            ItemInfo info = ((ItemInfo)item);
            ItemSO data = gm.GetItemData(info.id);
            combInfoUI.first.sprite = data.GetSprite();
            combInfoUI.second.text = string.Format("{0} <color=blue>{1}</color>개", data.itemName, info.count);

            RequestLeftBottomMsg(string.Format("아이템을 획득하였습니다. ({0} +{1})", data.itemName, info.count));

            OnUIInteractSetActive(UIType.ITEM_DETAIL, false, true); //음식 제작 성공창이 띄워졌을 때 템 자세히 보기창 열려있으면 꺼줌
        });

        //Pickup item
        Global.AddMonoAction(Global.AcquisitionItem, i =>
        {
            Item item = i as Item;
            RequestLeftBottomMsg(string.Format("아이템을 획득하였습니다. ({0} +{1})", item.itemData.itemName, item.DroppedCnt));

        });
        //Pickup plant (채집 성공)
        Global.AddMonoAction(Global.PickupPlant, item =>
        {
            RequestLeftBottomMsg(string.Format("아이템을 획득하였습니다. ({0} +{1})", ((Pick)item).itemData.itemName, 1));
        });
        //아이템 버림  
        Global.AddAction(Global.JunkItem, JunkItem);

        EventManager.StartListening("PlayerDead", () => OnUIInteractSetActive(UIType.DEATH, true, true));
        EventManager.StartListening("PlayerRespawn", Respawn);
        EventManager.StartListening("GameClear", () => OnUIInteract(UIType.CLEAR, true));
        EventManager.StartListening("TimePause", () => Time.timeScale = 0 );
        EventManager.StartListening("TimeResume", () => Time.timeScale = 1);
        EventManager.StartListening("StageClear", () => { changeNoticeMsgGrd = clearNoticeMsgVGrd; InsertNoticeQueue("Stage Clear", 90, true); });
        EventManager.StartListening("ChangeBody", () => { });  //뭘로 변신했는지 메시지 띄워야함
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
        else if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.SETTING]))
        {
            OnUIInteract(UIType.SETTING);
        }
        else if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.MONSTER_COLLECTION]))
        {
            OnUIInteract(UIType.MONSTER_COLLECTION);
        }
        /*else if (Input.GetKeyDown(KeySetting.keyDict[KeyAction.CHANGEABLEBODYS]))
        {
            OnUIInteract(UIType.CHANGEABLEMOBLIST);
        }*/
    }

    #region UI (비)활성화 관련
    public void OnUIInteractBtnClick(int type) { OnUIInteract((UIType)type); }

    public void OnUIInteract(UIType type, bool ignoreQueue = false) //UI열거나 닫음 (현재 액티브 상태의 반대로 해줌)
    {
        if (!CheckExistUI((int)type)) return;

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

    public void OnUIInteractSetActive(UIType type, bool isActive ,bool ignoreQueue = false) //UI열거나 닫음 (원하는 액티브 상태로 해주고 이미 그 상태면 캔슬)
    {
        GameUI ui = gameUIList[(int)type];
        if (ui.gameObject.activeSelf == isActive) return;
        if (uiTweeningDic[type]) return;

        if (activeUIQueue.Count > 0 && !ignoreQueue) return;
        if (ExceptionHandler(type)) return;

        activeUIQueue.Enqueue(false);

        if (isActive) ui.ActiveTransition();
        else ui.InActiveTransition();
    }

    private bool ExceptionHandler(UIType type) //상호작용에 대한 예외처리
    {
        switch (type)
        {
            case UIType.KEYSETTING:
                if (KeyActionManager.Instance.IsChangingKeySetting)  //키세팅 변경 중에는 esc로 키세팅 UI 안꺼지게
                    return true;
                break;
            case UIType.SETTING:
                for(int i=0; i<gameMenuList.Count; i++)
                {
                    if (gameMenuList[i].gameObject.activeSelf)     //설정 속의 메뉴 UI가 켜져있는 중에는 설정창 못 끄게
                        return true;
                }
                normalPanelCanvasg.DOFade(!gameUIList[(int)UIType.SETTING].gameObject.activeSelf ? 0:1, 0.3f);
                break;
            case UIType.MONSTERINFO_DETAIL:
                if (gameUIList[(int)UIType.MONSTERINFO_DETAIL_ITEM].gameObject.activeSelf || gameUIList[(int)UIType.MONSTERINFO_DETAIL_STAT].gameObject.activeSelf)
                    return true;
                break;
        }
        return false;
    }

    public void UpdateUIStack(GameUI ui, bool add = true) //열려있는 UI리스트 관리
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
        uiTweeningDic[ui._UItype] = false;

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

    void FilterStackUI(GameUI ui, bool add) //activeUIList에 넣거나 빼지 않는 UI들
    {
        switch (ui._UItype)
        {
            case UIType.CLEAR:
                return;
            case UIType.DEATH:
                return;
            case UIType.CHANGEABLEMOBLIST:
                return;
        }
        if (add) activeUIList.Add(ui);
        else activeUIList.Remove(ui);
    }

    private void ActiveSpecialProcess(UIType type) //UI 열릴 때의 특별한 처리
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
            case UIType.MONSTERINFO_DETAIL_STAT:
                MonsterCollection.Instance.DetailStat();
                break;
            case UIType.MONSTERINFO_DETAIL_ITEM:
                MonsterCollection.Instance.DetailItem();
                break;
        }
    }

    private void InActiveSpecialProcess(UIType type)//UI 닫힐 때의 특별한 처리
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
                break;
            case UIType.KEYSETTING:
                itrNoticeList.ForEach(x => x.Set());
                SkillUIManager.Instance.UpdateSkillKeyCode();
                MonsterCollection.Instance.UpdateSavedBodyChangeKeyCodeTxt();
                break;
            case UIType.CHEF_FOODS_PANEL:
                EventManager.TriggerEvent("TimeResume");
                break;
            case UIType.MONSTERINFO_DETAIL:
                MonsterCollection.Instance.CloseDetail();
                break;
        }
    }

    private bool CheckExistUI(int num)
    {
        if (gameUIList.Count > num)
            return true;
        RequestSystemMsg("개발중인 UI이거나 버그로 인해서 UI가 제대로 안나옴");
        return false;
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

    #region Cursor 따라다니는 정보텍스트
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

    #region Inventory
    public void DetailItemSlot(ItemSlot slot)  //인벤토리에서 아이템 슬롯 클릭
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
        itemCntTxt.text = string.Format("수량: {0}개", gm.GetItemCount(itemID));
        itemTypeTxt.text = Global.GetItemTypeName(data.itemType);

        itemUseBtn.gameObject.SetActive(data.itemType != ItemType.ETC);
        if (data.itemType == ItemType.ETC && ((Ingredient)data).isUseable) itemUseBtn.gameObject.SetActive(true);
    }
    #endregion

    #region Message
    public void RequestSystemMsg(string msg, int fontSize = 35, float existTime = 1.5f) //화면 중앙 상단에 뜨는 시스템 메시지
    {
        PoolManager.GetItem("SystemMsg").GetComponent<SystemMsg>().Set(msg, fontSize, existTime, Util.Change255To1Color(221, 0, 0, 255));
    }
    public void RequestSystemMsg(string msg,Color textColor ,int fontSize = 35, float existTime = 1.5f) //화면 중앙 상단에 뜨는 시스템 메시지
    {
        PoolManager.GetItem("SystemMsg").GetComponent<SystemMsg>().Set(msg, fontSize, existTime, textColor);
    }

    public void RequestLeftBottomMsg(string msg)  //화면 왼쪽 하단에 표시되는 로그 텍스트
    {
        Text t = PoolManager.GetItem<Text>("AcquisitionMsg");
        t.text = msg;
        Util.DelayFunc(() => t.gameObject.SetActive(false), 2f, this, true);
    }

    public void RequestSelectionWindow(string message, List<Action> actions, List<string> btnTexts, bool timePause = true, bool activeWarning = true) //선택창을 띄움
    {
        if(timePause) EventManager.TriggerEvent("TimePause");

        selWdStack.ForEach(x => x.Hide(true));

        for (int i = 0; i < actions.Count; i++)
            actions[i] += DefaultSelectionAction;
        
        SelectionWindow selWd = PoolManager.GetItem<SelectionWindow>("SelWindow");
        selWd.transform.SetAsLastSibling();
        selWd.Set(message, actions, btnTexts, activeWarning);
        selWdStack.Push(selWd);
    }

    public void RequestWarningWindow(Action confirm, string warning, string verify = "확인", string cancel = "취소")
    {
        wnWd.Register(confirm, warning, verify, cancel);
    }
    #endregion

    public void DoChangeBody(string id)  //몸통 저장할지 창 띄움
    {
        RequestSelectionWindow(MonsterCollection.Instance.mobIdToSlot[id].bodyData.bodyName + "를(을) 변신 슬롯에 저장하시겠습니까?\n(거절하면 해당 몬스터의 흡수 확률은 0%로 돌아갑니다.)",
            new List<Action>() {() => CancelMonsterSaveChance(id) , () => SaveMonsterBody(id) }, new List<string>() {"거절", "저장"});
    }
    
    public void ShowSavedBodyList(bool isShow)
    {
        OnUIInteract(UIType.CHANGEABLEMOBLIST, true);

        EventManager.TriggerEvent(isShow ? "TimePause" : "TimeResume");
    }

    public void DefaultSelectionAction() //시스템 확인창에서 각 버튼마다 눌렸을 때의 실행함수에 이 함수를 더해줌
    {
        selWdStack.Pop().Inactive();
        if(IsSelecting)
        {
            selWdStack.Peek().Hide(false);
        }
        else
        {
            EventManager.TriggerEvent("TimeResume");
        }
    }

    public void CancelMonsterSaveChance(string id)  //변신 가능한 몬스터 저장 거절
    {
        EventManager.TriggerEvent("PlayerBodySet", id, false);
    }

    public void SaveMonsterBody(string id) //변신 가능한 몬스터 저장하기
    {
        EventManager.TriggerEvent("PlayerBodySet", id, true);
    }

    #region loading

    public void StartLoading(Action loadingAction = null, Action loadingEndAction = null, float start = 0.6f, float middle = 0.4f, float end = 0.5f)
    {
        loadingCvsg.alpha = 0;
        loadingCvsg.gameObject.SetActive(true);
        loadingEndAction += () => loadingCvsg.gameObject.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(loadingCvsg.DOFade(1, start).SetEase(Ease.OutQuad).OnComplete(()=>loadingAction?.Invoke()));
        seq.AppendInterval(middle);
        seq.Append(loadingCvsg.DOFade(0, end).SetEase(Ease.OutQuad));
        seq.OnComplete(() => loadingEndAction());
        seq.Play();
    }

    public void StartLoading()
    {
        loadingCvsg.alpha = 1;
        loadingCvsg.gameObject.SetActive(true);

        loadingCvsg.DOFade(0, 1).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() => loadingCvsg.gameObject.SetActive(false));
    }

    #endregion

    #region 인벤 버튼 (inventory button)
    public void OnClickRemoveItemBtn()  //아이템 버리기 버튼
    {
        removeItemInfo.first.sprite = selectedItemSlot.itemImg.sprite;
        removeItemInfo.second.text = itemNameTmp.text;
        itemRemoveCntInput.text = "1";
        OnUIInteract(UIType.REMOVE_ITEM);
    }

    public void OnClickItemJunkBtn() //아이템 버리기 확인 버튼 클릭
    {
        int rmCount = int.Parse(itemRemoveCntInput.text);

        if (rmCount <= 0)
        {
            RequestSystemMsg("1개 이상부터 버릴 수 있습니다.");
            return;
        }
        else if (rmCount > gm.GetItemCount(selectedItemId))
        {
            RequestSystemMsg("보유 개수보다 많이 버릴 수 없습니다.");
            return;
        }


        Global.ActionTrigger(Global.JunkItem, rmCount);
    }

    void JunkItem(object rmCount)  //템 진짜 버림
    {
        Inventory.Instance.RemoveItem(selectedItemId, (int)rmCount);

        OnUIInteract(UIType.REMOVE_ITEM);  //아이템 버리기를 했다는 건 이 패널이 켜져있었다는 것이다 (그러므로 다시 꺼준다)

        if (selectedItemSlot.itemInfo == null)
            OnUIInteract(UIType.ITEM_DETAIL, true); //버린 아이템 슬롯에 더 이상 아이템 없으면 인벤 아이템 자세히 보기창은 꺼줌
        OnUIInteractSetActive(UIType.PRODUCTION_PANEL, false, true);  //요리 제작창이 켜져있으면 꺼줌
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
        statTexts[1].text = string.Concat( stat.MinDamage, '~', stat.MaxDamage);
        statTexts[2].text = stat.Defense.ToString();
        statTexts[3].text = Mathf.RoundToInt(Mathf.Abs(stat.Speed)).ToString(); //스피드가 몇인지 소수로 나오면 어색할 것 같아서 일단은 정수로 나오게 함.
        statTexts[4].text = string.Concat(stat.CriticalRate, '%');
        statTexts[5].text = stat.CriticalDamage.ToString();
        statTexts[6].text = stat.Intellect.ToString();

        //statText.text = $"HP\t\t{currentHP}/{stat.hp}\n\n공격력\t\t{stat.damage}\n\n방어력\t\t{stat.defense}\n\n이동속도\t\t{stat.speed}";
    }

    public void UpdatePlayerHPUI(bool decrease = false)
    {
        isStartDelayHPFillTimer = false;

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
                playerHPInfo.third.DOFillAmount(playerHPInfo.first.fillAmount, 0.3f);
                isStartDelayHPFillTimer = false;
            }
        }
    }
    #endregion
}
