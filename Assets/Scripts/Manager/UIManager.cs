using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Water;
using System;
using UnityEngine.Audio;

public partial class UIManager : MonoSingleton<UIManager>
{
    #region 게임 UI 관리 변수들
    public List<GameUI> gameUIList = new List<GameUI>(); //UIType 열겨형 순서에 맞게 UI 옵젝을 인스펙터에 집어넣어야 함
    [SerializeField] private List<GameUI> activeUIList = new List<GameUI>();  //활성화 되어있는 UI들 확인용으로 [SerializeField]

    public Queue<bool> activeUIQueue = new Queue<bool>(); //어떤 UI가 켜지거나 꺼지는 애니메이션(트위닝) 진행 중에 다른 UI (비)활성화 막기 위한 변수
    public Dictionary<UIType, bool> uiTweeningDic = new Dictionary<UIType, bool>(); //해당 UI가 트위닝 중인지(켜지거나 꺼지는 중인지) 확인하기 위함

    public bool CanInteractUI { get; private set; }
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
    private string selectedItemId = String.Empty; //클릭한 아이템 슬롯의 아이템 아이디
    private ItemSlot selectedItemSlot; //클릭한 아이템 슬롯

    public Image itemImg, itemTypeImg;
    public Text itemExplanation;
    public Text itemCntTxt, itemTypeTxt;
    public Text itemAbilExplanation;
    public TextMeshProUGUI itemNameTmp;
    public Button itemUseBtn, itemJunkBtn;

    public RectTransform selectedImg;
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

    public GameObject mobSpeciesIconPref;
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
    public GameObject iconSelBtn;

    public Dictionary<string, bool> mobSaveWindowActiveDic = new Dictionary<string, bool>(); //해당 아이디의 몬스터를 장착할지 물어보는 창이 떴는지 확인
    public Dictionary<string, Triple<Sprite, string, string>> iconSelBtnDataDic = new Dictionary<string, Triple<Sprite, string, string>>(); //key : 버튼 스프라이트, 버튼에 뜰 텍스트, 마우스 오버시 뜰 설명

    [SerializeField] private List<Triple<Sprite, string, string>> iconSelBtnDataList = new List<Triple<Sprite, string, string>>();
    #endregion

    #region CanvasGroup
    //쉽게 구분하려고 배열로 안함
    [Space(10)]
    public CanvasGroup normalPanelCanvasg;
    public CanvasGroup priorNormalPanelCvsg;
    public CanvasGroup settingCvsg;
    public CanvasGroup worldUICvsg;
    public CanvasGroup ordinaryCvsg;
    public CanvasGroup msgCvsg;
    public CanvasGroup menuCvsg;
    [SerializeField] private CanvasGroup loadingCvsg;

    private List<CanvasGroup> cvsgList = new List<CanvasGroup>();
    #endregion

    #region Warning Window UI
    public WarningWindow wnWd;
    [HideInInspector] public string warningStr;
    #endregion

    #region Sound
    public Slider masterSoundSlider, BGMSlider, SFXSlider;
    public AudioMixer masterAudioMixer;
    #endregion

    #region State
    [SerializeField] private Triple<Image, TextMeshProUGUI, Text> stateInfoTriple;  //이미지, 이름, 설명
    public VertexGradient buffVG, imprecVG;
    #endregion

    #region Confirm
    [SerializeField] private Button confirmPanelYesBtn, confirmPanelNoBtn;
    [SerializeField] private Text confirmPanelText;
    #endregion


    [SerializeField] private ResolutionOption resolutionOption;

    //public Setting setting;

    public List<AcquisitionUI> acqUIList;

    #region 메뉴(인벤,스탯,도감,설정) 관련 변수들
    public List<MenuButton> menuBtns;
    private UIType selectedMenuType;

    //public Text statText;
    public Text[] statTexts;

    #endregion

    public GameUI CurrentReConfirmUI { get; set; }

    private GameManager gm;
    private SlimeGameManager sgm;

    #region Init
    private void Awake()
    {
        InitData();
        CreatePool();
    }

    private void InitData()
    {
        int i;

        CanInteractUI = true;

        cursorImgRectTrm = cursorInfoImg.GetComponent<RectTransform>();
        sw = cursorImgRectTrm.rect.width;

        noticeMsgGrd = noticeUIPair.first.GetComponent<NoticeMsg>().msgTmp.colorGradient;
        allCanvasScalers = canvasParent.GetComponentsInChildren<CanvasScaler>();
        defaultTopCenterMsgVG = topCenterMsgTMP.colorGradient;
        //topCenterMsgTMPCvsg = topCenterMsgTMP.GetComponent<CanvasGroup>();

       
        for(i=0; i<gameCanvases.Length; i++)
        {
            gameCanvases[i].worldCamera = Util.MainCam;
            gameCanvases[i].planeDistance = Global.cameraPlaneDistance;
        }
        for (i = 0; i < Global.EnumCount<UIType>(); i++)
        {
            uiTweeningDic.Add((UIType)i, false);
        }
        for(i=0; i<iconSelBtnDataList.Count; ++i)
        {
            iconSelBtnDataDic.Add(iconSelBtnDataList[i].first.name, new Triple<Sprite, string, string>(iconSelBtnDataList[i].first, iconSelBtnDataList[i].second, iconSelBtnDataList[i].third));
        }

        cvsgList.Add(msgCvsg);
        cvsgList.Add(menuCvsg);
        cvsgList.Add(worldUICvsg);
        cvsgList.Add(ordinaryCvsg);
        cvsgList.Add(priorNormalPanelCvsg);
        cvsgList.Add(normalPanelCanvasg);

        //setting.InitSet();
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
        PoolManager.CreatePool(mobSpeciesIconPref, interactionMarkPair.second, 2, "MobSpeciesIcon");
        PoolManager.CreatePool(iconSelBtn, selectionBtnPair.second, 3, "IconSelBtn");
    }

    private void OnEnable()
    {
        gm = GameManager.Instance;
        sgm = SlimeGameManager.Instance;
    }

    private void Start()
    {
        DefineAction();

        PlayerEnemyUnderstandingRateManager.Instance.ChangableBodyList.ForEach(x => mobSaveWindowActiveDic.Add(x.bodyId.ToString(), false));
        Load();
    }

    public void Load()
    {
        Option option = gm.savedData.option;
        masterSoundSlider.value = option.masterSound;
        BGMSlider.value = option.bgmSize;
        SFXSlider.value = option.soundEffectSize;
    }

    public void OnChangedResolution()
    {
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

            RequestLogMsg(string.Format("아이템을 획득하였습니다. ({0} +{1})", data.itemName, info.count));

            OnUIInteractSetActive(UIType.ITEM_DETAIL, false, true); //음식 제작 성공창이 띄워졌을 때 템 자세히 보기창 열려있으면 꺼줌
        });

        //Pickup item
        Global.AddMonoAction(Global.AcquisitionItem, i =>
        {
            Item item = i as Item;
            RequestLogMsg(string.Format("아이템을 획득하였습니다. ({0} +{1})", item.itemData.itemName, item.DroppedCnt));
            UpdateInventoryItemCount(item.itemData.id);
        });
        //Pickup plant (채집 성공)
        Global.AddMonoAction(Global.PickupPlant, item =>
        {
            Pick p = (Pick)item;
            RequestLogMsg(string.Format("아이템을 획득하였습니다. ({0} +{1})", p.itemData.itemName, p.DroppedCount));
            UpdateInventoryItemCount(p.itemData.id);
        });
        //아이템 버림  
        Global.AddAction(Global.JunkItem, JunkItem);
        EventManager.StartListening("ChangeResolution", OnChangedResolution);

        EventManager.StartListening("PlayerDead", () => OnUIInteractSetActive(UIType.DEATH, true, true));
        EventManager.StartListening("GameClear", () => OnUIInteract(UIType.CLEAR, true));
        EventManager.StartListening("StageClear", () =>InsertNoticeQueue("Clear", clearNoticeMsgVGrd, 90));
        EventManager.StartListening("ChangeBody", (str, dead) => { if(!dead) InsertNoticeQueue(MonsterCollection.Instance.GetMonsterInfo(str).bodyName + "(으)로 변신하였습니다"); });
        EventManager.StartListening("PickupMiniGame", (Action<bool>)(start =>
        {
            normalPanelCanvasg.DOFade(start ? 0 : 1, 0.25f);
        }));
        EventManager.StartListening("StartCutScene", () =>
        {
            SetUIAlpha(0f);
            CanInteractUI = false;
            Cursor.visible = false;
        });
        EventManager.StartListening("EndCutScene", () =>
        {
            SetUIAlpha(1f);
            CanInteractUI = true;
            Cursor.visible = true;
        });
    }

    #endregion

    #region update
    private void Update()
    {
        UserInput();
        CursorInfo();
        Notice();
        DelayHPFill();
    }

    private bool CheckInputAndActive(KeyAction key)
    {

        if(Input.GetKeyDown(KeySetting.keyDict[key]))
        {
            if(gm.savedData.userInfo.uiActiveDic[key])
            {
                return true;
            }

            KeyActionManager.Instance.SetPlayerHeadText("?", 0.5f);
            return false;
        }

        return false;
    }

    private void SetUIAlpha(float a)
    {
        a = Mathf.Clamp(a, 0f, 1f);
        bool alphaZero = a == 0f;
        for(int i = 0; i<cvsgList.Count; i++)
        {
            cvsgList[i].alpha = a;
            cvsgList[i].interactable = !alphaZero;
            cvsgList[i].blocksRaycasts = !alphaZero;
        }
    }

    public Sprite GetInterfaceSprite(UIType type)
    {
        MenuButton mb = menuBtns.Find(x => x.uiType == type);
        if(mb)
        {
            return mb.transform.GetChild(0).GetComponent<Image>().sprite;
        }
        return null;
    }

    private void UserInput()
    {
        if (CanInteractUI)
        {
            if (Input.GetKeyDown(KeySetting.fixedKeyDict[KeyAction.SETTING]))
            {
                if (activeUIList.Count > 0)
                {
                    OnUIInteract(activeUIList[activeUIList.Count - 1]._UItype);
                }
                else
                {
                    if (gm.savedData.userInfo.uiActiveDic[KeyAction.SETTING])
                    {
                        OnUIInteract(UIType.SETTING);
                    }
                    else
                    {
                        KeyActionManager.Instance.SetPlayerHeadText("?", 0.5f);
                    }
                }
            }
            else if (CheckInputAndActive(KeyAction.INVENTORY))
            {
                OnUIInteract(UIType.INVENTORY);
            }
            else if (CheckInputAndActive(KeyAction.STAT))
            {
                OnUIInteract(UIType.STAT);
            }
            else if (CheckInputAndActive(KeyAction.MONSTER_COLLECTION))
            {
                OnUIInteract(UIType.MONSTER_COLLECTION);
            }
            /*else if (CheckInputAndActive(KeyAction.QUIT))
            {
                OnUIInteract(UIType.QUIT);
            }*/
            else if (Input.GetKeyDown(KeyCode.E) && Util.IsActiveGameUI(UIType.ITEM_DETAIL))
            {
                if (selectedItemSlot)
                {
                    OnClickItemUseBtn();
                }
            }
        }

#if UNITY_EDITOR
        if(Input.GetKeyDown(ScreenShot.captureKeyCode))
        {
            ScreenShot.StartScreenShot();
        }
#endif
    }
    #endregion

    #region UI (비)활성화 관련
    public void OnUIInteractBtnClick(int type) { OnUIInteract((UIType)type); }

    public void OnUIInteract(UIType type, bool ignoreQueue = false) //UI열거나 닫음 (현재 액티브 상태의 반대로 해줌)
    {
        if (!CheckExistUI((int)type)) return;

        //if (!gm.savedData.userInfo.uiActiveDic[type]) return;

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
        //if (!gm.savedData.userInfo.uiActiveDic[type]) return;

        GameUI ui = gameUIList[(int)type];
        if (ui.gameObject.activeSelf == isActive) return;
        if (uiTweeningDic[type]) return;

        if (activeUIQueue.Count > 0 && !ignoreQueue) return;
        if (ExceptionHandler(type)) return;

        activeUIQueue.Enqueue(false);

        if (isActive) ui.ActiveTransition();
        else ui.InActiveTransition();
    }

    public void OnClickMenuBtn(UIType type) //메뉴에서 버튼 눌렀을 때
    {
        if (activeUIQueue.Count > 0) return;

        MenuBtnSelectedMark(type);

        activeUIQueue.Enqueue(false);

        ExceptionHandler(type);
        gameUIList[(int)type].ActiveTransition();
        selectedMenuType = type;
    }

    private void MenuBtnSelectedMark(UIType type)  //메뉴 버튼 안눌린 것들은 클릭 표시 없애고 눌린건 표시 생기게
    { 
        for(int i = 0; i<menuBtns.Count; i++)
        {
            if(menuBtns[i].Selected)
            {
                menuBtns[i].OnSelected(false);
            }   
        }

        menuBtns.Find(x => x.uiType == type).OnSelected(true);
       
    }

    private bool ExceptionHandler(UIType type) //UI여닫는 상호작용에 대한 예외처리. true를 리턴하면 상호작용 안함 
    {
        bool bValue = false;

        if (gameUIList[(int)type].GetComponent<MenuPanel>() != null) //메뉴 UI중 하나와 상호작용할 때
        {
            if(!Util.IsActiveGameUI(UIType.MENU)) //메뉴창이 꺼져있으면
            {
                OnUIInteractSetActive(UIType.MENU, true, true);  //메뉴창을 띄움
                MenuBtnSelectedMark(type);
                selectedMenuType = type;
            }
            else
            {
                if(selectedMenuType == type)
                {
                    OnUIInteract(UIType.MENU);
                    return true;
                }
                else
                {
                    bValue = true;
                }
            }
        }

        switch (type)
        {
            case UIType.INVENTORY:
                if(!Util.IsActiveGameUI(type))
                {
                    Inventory.Instance.invenUseActionImg.SetActive(false);
                }
                break;
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
                //normalPanelCanvasg.DOFade(!gameUIList[(int)UIType.SETTING].gameObject.activeSelf ? 0:1, 0.3f);
                //setting.SetChildImgs(true);
                break;
            case UIType.MONSTERINFO_DETAIL:  //몹 드랍템 정보창이나 추가스탯 창 UI켜져있으면 몹 정보 자세히 보기 UI 상호작용 여닫기 X
                if (gameUIList[(int)UIType.MONSTERINFO_DETAIL_ITEM].gameObject.activeSelf || gameUIList[(int)UIType.MONSTERINFO_DETAIL_STAT].gameObject.activeSelf)
                    return true;
                break; 
            case UIType.CHANGEABLEMOBLIST:      //몸 제거창 뜨면 일시정지
                if (!gameUIList[(int)UIType.CHANGEABLEMOBLIST].gameObject.activeSelf)
                    TimeManager.TimePause();
                break;
            case UIType.STAT:  //스탯 UI 업데이트
                UpdateStatUI();
                EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, false);
                break;
            case UIType.MONSTERINFO_DETAIL_STAT: //몹 자세히 보기에서 추가 스탯 정보 UI 업뎃
                MonsterCollection.Instance.DetailStat();
                break;
            case UIType.MONSTERINFO_DETAIL_ITEM:  //몹 자세히 보기에서 드랍템 정보 UI 업뎃
                MonsterCollection.Instance.DetailItem();
                break;
            case UIType.CHEF_FOODS_PANEL:
                if (gameUIList[(int)type].gameObject.activeSelf)  //만들 요리 고르는 창 닫으려는데 음식 제작 패널 켜져있으면 그걸 먼저 닫아줌
                {
                    if(gameUIList[(int)UIType.PRODUCTION_PANEL].gameObject.activeSelf)
                    {
                        OnUIInteract(UIType.PRODUCTION_PANEL);
                        return true;
                    }
                }
                break;
            case UIType.MINIGAME_PICKUP:
                if (GameManager.Instance.pickupCheckGame.IsGameStart) return true; //미니게임 하고 있으면 상호작용 안함
                break;
            case UIType.UIOFFCONFIRM:
                if (Util.IsActiveGameUI(UIType.UIOFFCONFIRM)) CurrentReConfirmUI.IsCloseable = false;
                break;
            case UIType.MENU:
                if(Util.IsActiveGameUI(UIType.MONSTERINFO_DETAIL_ITEM))
                {
                    OnUIInteract(UIType.MONSTERINFO_DETAIL_ITEM);
                    return true;
                }
                else if(Util.IsActiveGameUI(UIType.MONSTERINFO_DETAIL_STAT))
                {
                    OnUIInteract(UIType.MONSTERINFO_DETAIL_STAT);
                    return true;
                }

                if(!Util.IsActiveGameUI(UIType.MENU))
                {
                    TimeManager.TimePause();
                }
                break;
        }
        return bValue;
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
        if (ui.GetComponent<MenuPanel>() != null) return;
        switch (ui._UItype)
        {
            case UIType.CLEAR:
                return;
            case UIType.DEATH:
                return;
            case UIType.CHANGEABLEMOBLIST:
                return;
            case UIType.ITEM_DETAIL:
                return;
            case UIType.MONSTERINFO_DETAIL:
                return;
            case UIType.MONSTERINFO_DETAIL_ITEM:
                return;
            case UIType.MONSTERINFO_DETAIL_STAT:
                return;
        }
        if (add) activeUIList.Add(ui);
        else activeUIList.Remove(ui);
    }

    public void ActiveSpecialProcess(UIType type) //UI 열릴 때의 특별한 처리
    {
        switch (type)
        {
            case UIType.INVENTORY:
                EffectManager.Instance.OnTopRightBtnEffect(UIType.INVENTORY, false);
                break;
            case UIType.MONSTER_COLLECTION:
                EffectManager.Instance.OnTopRightBtnEffect(UIType.MONSTER_COLLECTION, false);
                break;
            /*case UIType.SETTING:
                TimeManager.TimePause();
                setting.SetChildImgs(false);
                break;*/
           
        }
    }

    public void InActiveSpecialProcess(UIType type)//UI 닫힐 때의 특별한 처리
    {
        switch (type)
        {
            case UIType.PRODUCTION_PANEL:
                CookingManager.Instance.MakeFoodInfoUIReset();  //음식 선택 표시 없애기
                break;
            case UIType.FOOD_DETAIL:
                CookingManager.Instance.detailID = string.Empty;  
                break;
            case UIType.ITEM_DETAIL:  //아이템 선택표시 없애기
                selectedItemId = string.Empty;
                if (selectedItemSlot)
                {
                    //selectedItemSlot.outline.DOKill();
                    //selectedItemSlot.outline.enabled = false;
                    selectedItemSlot = null;
                    selectedImg.gameObject.SetActive(false);
                }
                break;
            /*case UIType.SETTING:
                TimeManager.TimeResume();
                break;*/
            case UIType.KEYSETTING:  //키세팅 창 닫히면 상호작용 표시, 스킬 슬롯, 몸 저장 슬롯 등 키코드가 나오는 UI들을 다시 업데이트함
                itrNoticeList.ForEach(x => x.Set());
                SkillUIManager.Instance.UpdateSkillKeyCode();
                MonsterCollection.Instance.UpdateSavedBodyChangeKeyCodeTxt();
                break;
            case UIType.CHEF_FOODS_PANEL:
                TimeManager.TimeResume();
                break;
            case UIType.MONSTERINFO_DETAIL:
                MonsterCollection.Instance.CloseDetail();
                break;
            case UIType.RESOLUTION:
                resolutionOption.ExitResolutionPanel();
                break;
            case UIType.MENU:
                TimeManager.TimeResume();
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

    public void SetReconfirmUI(string msg, Action confirm, Action cancel)
    {
        confirmPanelText.text = msg;
        confirm += InactiveConfirmUI;
        cancel += InactiveConfirmUI;

        confirmPanelYesBtn.onClick.RemoveAllListeners();
        confirmPanelNoBtn.onClick.RemoveAllListeners();

        confirmPanelYesBtn.onClick.AddListener(()=>confirm());
        confirmPanelNoBtn.onClick.AddListener(() => cancel());

        OnUIInteractSetActive(UIType.UIOFFCONFIRM, true, true);
    }

    void InactiveConfirmUI()
    {
        OnUIInteractSetActive(UIType.UIOFFCONFIRM, false, true);
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
        string itemID = slot.itemInfo.id;

        /*if (selectedItemSlot)
        {
            selectedItemSlot.outline.DOKill();
            selectedItemSlot.outline.enabled = false;
        }*/
        selectedItemSlot = slot;

        if (selectedItemId == itemID) return;
        else if (string.IsNullOrEmpty(selectedItemId)) OnUIInteract(UIType.ITEM_DETAIL);
        selectedItemId = itemID;

        ItemSO data = gm.GetItemData(itemID);

        //itemImg.sprite = data.GetSprite();
        itemTypeImg.sprite = Global.GetItemTypeSpr(data.itemType);
        itemNameTmp.text = data.itemName;
        itemExplanation.text = data.explanation;
        //itemCntTxt.text = string.Format("수량: {0}개", gm.GetItemCount(itemID));
        itemTypeTxt.text = Global.GetItemTypeName(data.itemType);

        itemAbilExplanation.text = data.abilExplanation;

        selectedImg.gameObject.SetActive(true);
        selectedImg.transform.SetParent(slot.root.transform);
        selectedImg.transform.localPosition = Vector3.zero;
        selectedImg.transform.SetAsLastSibling();
        selectedImg.transform.localScale = Vector3.one; //왜인지 옮길 수록 스케일이 작아져서 원래대로 되돌림

        //itemUseBtn.gameObject.SetActive(data.itemType != ItemType.ETC);
        //if (data.itemType == ItemType.ETC && ((Ingredient)data).isUseable) itemUseBtn.gameObject.SetActive(true);

        Inventory.Instance.invenUseActionImg.SetActive(data.itemType != ItemType.ETC);
        if (data.itemType == ItemType.ETC && ((Ingredient)data).isUseable) Inventory.Instance.invenUseActionImg.SetActive(true);
    }

    public void UpdateInventoryItemCount(string id)
    {
        if (selectedItemId == id)
        {
            //itemCntTxt.text = string.Format("수량: {0}개", gm.GetItemCount(id));
        }
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

    public void RequestLogMsg(string msg, float duration = 2f)  //화면 왼쪽 하단에 표시되는 로그 텍스트
    {
        Text t = PoolManager.GetItem<Text>("AcquisitionMsg");
        t.text = msg;
        Util.DelayFunc(() => t.gameObject.SetActive(false), duration, this, true);
    }

    public void RequestSelectionWindow(string message, List<Action> actions, List<string> btnTexts, bool activeWarning = true, List<Func<bool>> conditions = null, bool useIcon = false) //선택창을 띄움
    {
        TimeManager.TimePause();

        selWdStack.ForEach(x => x.Hide(true));

        for (int i = 0; i < actions.Count; i++)
            actions[i] += DefaultSelectionAction;
        
        SelectionWindow selWd = PoolManager.GetItem<SelectionWindow>("SelWindow");
        selWd.transform.SetAsLastSibling();
        selWd.Set(message, actions, btnTexts, activeWarning, conditions, useIcon);
        selWdStack.Push(selWd);
    }

    public void RequestWarningWindow(Action confirm, string warning, string verify = "확인", string cancel = "취소")
    {
        wnWd.Register(confirm, warning, verify, cancel);
    }

    public void DefaultSelectionAction() //시스템 확인창에서 각 버튼마다 눌렸을 때의 실행함수에 이 함수를 더해줌
    {
        TimeManager.TimeResume();
        selWdStack.Pop().Inactive();
        if (IsSelecting)
        {
            selWdStack.Peek().Hide(false);
        }
    }
    #endregion

    #region ETC
    public void DoChangeBody(string id)  //몸통 저장할지 창 띄움
    {
        if (mobSaveWindowActiveDic[id]) return;

        mobSaveWindowActiveDic[id] = true;

        //만약 전역변수로 List<Action>을 만들고 이걸로 전달을 하면 내부에서 DefaultSelectionAction을 더하면 전역에도 추가되므로 두번째부터는 DefaultSelectionAction을 두 번 이상 호출하게 되는 문제가 생긴다
        RequestSelectionWindow("<color=#7A98FF>" + MonsterCollection.Instance.mobIdToSlot[id].BodyData.bodyName + "</color>를(을) 변신 슬롯에\n 저장하시겠습니까?\n(거절하면 해당 몬스터의 흡수 확률은\n 0%로 돌아갑니다.)",
            new List<Action>() {() => CancelMonsterSaveChance(id) , () => SaveMonsterBody(id) }, new List<string>() {"거절", "저장"});
    }
    
    public void ShowSavedBodyList(bool isShow)
    {
        OnUIInteract(UIType.CHANGEABLEMOBLIST, true);

        EventManager.TriggerEvent(isShow ? "TimePause" : "TimeResume");
    }

    

    public void CancelMonsterSaveChance(string id)  //변신 가능한 몬스터 저장 거절
    {
        mobSaveWindowActiveDic[id] = false;
        EventManager.TriggerEvent("PlayerBodySet", id, false);
    }

    public void SaveMonsterBody(string id) //변신 가능한 몬스터 저장하기
    {
        mobSaveWindowActiveDic[id] = false;
        EventManager.TriggerEvent("PlayerBodySet", id, true);
        MonsterCollection.Instance.IDToSave = id;
    }
    #endregion

    #region loading

    public void StartLoading(Action loadingAction = null, Action loadingEndAction = null, float start = 0.6f, float middle = 0.4f, float end = 0.5f)
    {
        loadingCvsg.alpha = 0;
        loadingCvsg.gameObject.SetActive(true);
        loadingEndAction += () => loadingCvsg.gameObject.SetActive(false);

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(loadingCvsg.DOFade(1, start).SetEase(Ease.OutQuad).OnComplete(() => loadingAction?.Invoke()));
        seq.AppendInterval(middle);
        seq.Append(loadingCvsg.DOFade(0, end).SetEase(Ease.OutQuad));
        seq.OnComplete(() => loadingEndAction());
        seq.Play();
    }

    public void StartLoadingIn()
    {
        loadingCvsg.alpha = 1;
        loadingCvsg.gameObject.SetActive(true);

        loadingCvsg.DOFade(0, 1).SetEase(Ease.OutQuad).SetUpdate(true).OnComplete(() =>
        {
            loadingCvsg.gameObject.SetActive(false);
            EventManager.TriggerEvent("StartNextStage");
        });

    }
    public void StartLoadingOut()
    {
        loadingCvsg.alpha = 0;
        loadingCvsg.gameObject.SetActive(true);

        loadingCvsg.DOFade(1, 1).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void FadeInOrdinaryCvs()
    {
        ordinaryCvsg.DOFade(1, 0.3f);
    }
    #endregion

    #region 인벤 버튼 (inventory button)
    public void OnClickRemoveItemBtn()  //아이템 버리기 버튼
    {
        //removeItemInfo.first.sprite = selectedItemSlot.itemImg.sprite;
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

    public void OnClickItemUseBtn()  //아직 마석 장착에 대한 로직은 없음
    {
        ItemSO data = gm.GetItemData(selectedItemId);
        if ( (data.itemType == ItemType.ETC && !((Ingredient)data).isUseable)) return;

        data.Use();
        Inventory.Instance.RemoveItem(selectedItemId, 1, "아이템을 소모했습니다.");

        if (selectedItemSlot.itemInfo == null)
        {
            OnUIInteract(UIType.ITEM_DETAIL, true);
            selectedImg.gameObject.SetActive(false);
        }

        Global.ActionTrigger("ItemUse", selectedItemId);
    }
    #endregion

    #region Stat
    public void UpdateStatUI()
    {
        Stat stat = sgm.Player.PlayerStat;

        statTexts[0].text = string.Concat(Mathf.Ceil( Mathf.Clamp(sgm.Player.CurrentHp, 0, stat.MaxHp)), '/',Mathf.Ceil( stat.MaxHp));
        statTexts[1].text = string.Concat( stat.MinDamage, '~', stat.MaxDamage);
        statTexts[2].text = stat.Defense.ToString();
        statTexts[3].text = Mathf.RoundToInt(Mathf.Abs(stat.Speed)).ToString(); //스피드가 몇인지 소수로 나오면 어색할 것 같아서 일단은 정수로 나오게 함.
        statTexts[4].text = string.Concat(stat.CriticalRate, '%');
        statTexts[5].text = stat.CriticalDamage.ToString() + "%";
        statTexts[6].text = stat.Intellect.ToString();
        statTexts[7].text = stat.AttackSpeed.ToString();

        //statText.text = $"HP\t\t{currentHP}/{stat.hp}\n\n공격력\t\t{stat.damage}\n\n방어력\t\t{stat.defense}\n\n이동속도\t\t{stat.speed}";
    }

    public void UpdatePlayerHPUI(bool decrease = false)
    {
        isStartDelayHPFillTimer = false;

        Player p = sgm.Player;
        float hp = Mathf.Clamp(p.CurrentHp, 0f, p.PlayerStat.MaxHp);

        float rate = (float)hp / p.PlayerStat.MaxHp;
        playerHPInfo.first.DOFillAmount(rate, 0.3f).OnComplete(()=> { if (!decrease) playerHPInfo.third.fillAmount = playerHPInfo.first.fillAmount; });
        playerHPInfo.second.text = string.Concat(Mathf.Ceil(hp), '/', Mathf.Ceil(p.PlayerStat.MaxHp));  //표시만 올림해서

        if (decrease)
        {
            EffectManager.Instance.OnDamagedUIEffect(rate);
            Environment.Instance.OnDamaged();
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

    #region State
    
    public void StateInfoDetail(BuffStateDataSO data)
    {
        OnUIInteractSetActive(UIType.STATEINFO, true);

        stateInfoTriple.first.sprite = data.sprite;
        stateInfoTriple.second.SetText(data.stateName);
        stateInfoTriple.third.text = data.explanation;

        stateInfoTriple.second.colorGradient = !data.IsBuff ? imprecVG : buffVG;
    }

    #endregion

    #region Sound

    public void OnChangedMasterVolume()  // min : -40 , max : 0
    {
        float volume = masterSoundSlider.value;
        gm.savedData.option.masterSound = volume;
        masterAudioMixer.SetFloat("Master", volume != -40f ? volume : -80f);
    }

    public void OnChangedBGMVolume()  // min : -40 , max : 0
    {
        float volume = BGMSlider.value;
        gm.savedData.option.bgmSize = volume;
        SoundManager.Instance.SetBGMVolume(volume);
    }

    public void OnChangedSFXVolume()  // min : -40 , max : 0
    {
        float volume = SFXSlider.value;
        gm.savedData.option.soundEffectSize = volume;
        SoundManager.Instance.SetEffectSoundVolume(volume);
    }

    #endregion
}
