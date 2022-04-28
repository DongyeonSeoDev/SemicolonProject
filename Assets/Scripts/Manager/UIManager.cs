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
    #region ���� UI ���� ������
    public List<GameUI> gameUIList = new List<GameUI>(); //UIType ������ ������ �°� UI ������ �ν����Ϳ� ����־�� ��
    [SerializeField] private List<GameUI> activeUIList = new List<GameUI>();  //Ȱ��ȭ �Ǿ��ִ� UI�� Ȯ�ο����� [SerializeField]

    public Queue<bool> activeUIQueue = new Queue<bool>(); //� UI�� �����ų� ������ �ִϸ��̼�(Ʈ����) ���� �߿� �ٸ� UI (��)Ȱ��ȭ ���� ���� ����
    public Dictionary<UIType, bool> uiTweeningDic = new Dictionary<UIType, bool>(); //�ش� UI�� Ʈ���� ������(�����ų� ������ ������) Ȯ���ϱ� ����

    public bool CanInteractUI { get; private set; }
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
    private string selectedItemId = String.Empty; //Ŭ���� ������ ������ ������ ���̵�
    private ItemSlot selectedItemSlot; //Ŭ���� ������ ����

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

    #region HP UI ����
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

    public Dictionary<string, bool> mobSaveWindowActiveDic = new Dictionary<string, bool>(); //�ش� ���̵��� ���͸� �������� ����� â�� ������ Ȯ��
    public Dictionary<string, Triple<Sprite, string, string>> iconSelBtnDataDic = new Dictionary<string, Triple<Sprite, string, string>>(); //key : ��ư ��������Ʈ, ��ư�� �� �ؽ�Ʈ, ���콺 ������ �� ����

    [SerializeField] private List<Triple<Sprite, string, string>> iconSelBtnDataList = new List<Triple<Sprite, string, string>>();
    #endregion

    #region CanvasGroup
    //���� �����Ϸ��� �迭�� ����
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
    [SerializeField] private Triple<Image, TextMeshProUGUI, Text> stateInfoTriple;  //�̹���, �̸�, ����
    public VertexGradient buffVG, imprecVG;
    #endregion

    #region Confirm
    [SerializeField] private Button confirmPanelYesBtn, confirmPanelNoBtn;
    [SerializeField] private Text confirmPanelText;
    #endregion


    [SerializeField] private ResolutionOption resolutionOption;

    //public Setting setting;

    public List<AcquisitionUI> acqUIList;

    #region �޴�(�κ�,����,����,����) ���� ������
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
        //UI���� Ǯ ����   Pool Create (related UI)
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
        //������ ���� (���� ���� �߰�)   made food.( add item)
        Global.AddAction(Global.MakeFood, item =>
        {
            OnUIInteract(UIType.COMBINATION, true); //���� ���� ���� �Ϸ� �г� ���
            combEff.Play();

            ItemInfo info = ((ItemInfo)item);
            ItemSO data = gm.GetItemData(info.id);
            combInfoUI.first.sprite = data.GetSprite();
            combInfoUI.second.text = string.Format("{0} <color=blue>{1}</color>��", data.itemName, info.count);

            RequestLogMsg(string.Format("�������� ȹ���Ͽ����ϴ�. ({0} +{1})", data.itemName, info.count));

            OnUIInteractSetActive(UIType.ITEM_DETAIL, false, true); //���� ���� ����â�� ������� �� �� �ڼ��� ����â ���������� ����
        });

        //Pickup item
        Global.AddMonoAction(Global.AcquisitionItem, i =>
        {
            Item item = i as Item;
            RequestLogMsg(string.Format("�������� ȹ���Ͽ����ϴ�. ({0} +{1})", item.itemData.itemName, item.DroppedCnt));
            UpdateInventoryItemCount(item.itemData.id);
        });
        //Pickup plant (ä�� ����)
        Global.AddMonoAction(Global.PickupPlant, item =>
        {
            Pick p = (Pick)item;
            RequestLogMsg(string.Format("�������� ȹ���Ͽ����ϴ�. ({0} +{1})", p.itemData.itemName, p.DroppedCount));
            UpdateInventoryItemCount(p.itemData.id);
        });
        //������ ����  
        Global.AddAction(Global.JunkItem, JunkItem);
        EventManager.StartListening("ChangeResolution", OnChangedResolution);

        EventManager.StartListening("PlayerDead", () => OnUIInteractSetActive(UIType.DEATH, true, true));
        EventManager.StartListening("GameClear", () => OnUIInteract(UIType.CLEAR, true));
        EventManager.StartListening("StageClear", () =>InsertNoticeQueue("Clear", clearNoticeMsgVGrd, 90));
        EventManager.StartListening("ChangeBody", (str, dead) => { if(!dead) InsertNoticeQueue(MonsterCollection.Instance.GetMonsterInfo(str).bodyName + "(��)�� �����Ͽ����ϴ�"); });
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

    #region UI (��)Ȱ��ȭ ����
    public void OnUIInteractBtnClick(int type) { OnUIInteract((UIType)type); }

    public void OnUIInteract(UIType type, bool ignoreQueue = false) //UI���ų� ���� (���� ��Ƽ�� ������ �ݴ�� ����)
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

    public void OnUIInteractSetActive(UIType type, bool isActive ,bool ignoreQueue = false) //UI���ų� ���� (���ϴ� ��Ƽ�� ���·� ���ְ� �̹� �� ���¸� ĵ��)
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

    public void OnClickMenuBtn(UIType type) //�޴����� ��ư ������ ��
    {
        if (activeUIQueue.Count > 0) return;

        MenuBtnSelectedMark(type);

        activeUIQueue.Enqueue(false);

        ExceptionHandler(type);
        gameUIList[(int)type].ActiveTransition();
        selectedMenuType = type;
    }

    private void MenuBtnSelectedMark(UIType type)  //�޴� ��ư �ȴ��� �͵��� Ŭ�� ǥ�� ���ְ� ������ ǥ�� �����
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

    private bool ExceptionHandler(UIType type) //UI���ݴ� ��ȣ�ۿ뿡 ���� ����ó��. true�� �����ϸ� ��ȣ�ۿ� ���� 
    {
        bool bValue = false;

        if (gameUIList[(int)type].GetComponent<MenuPanel>() != null) //�޴� UI�� �ϳ��� ��ȣ�ۿ��� ��
        {
            if(!Util.IsActiveGameUI(UIType.MENU)) //�޴�â�� ����������
            {
                OnUIInteractSetActive(UIType.MENU, true, true);  //�޴�â�� ���
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
                if (KeyActionManager.Instance.IsChangingKeySetting)  //Ű���� ���� �߿��� esc�� Ű���� UI �Ȳ�����
                    return true;
                break;
            case UIType.SETTING:
                for(int i=0; i<gameMenuList.Count; i++)
                {
                    if (gameMenuList[i].gameObject.activeSelf)     //���� ���� �޴� UI�� �����ִ� �߿��� ����â �� ����
                        return true;
                }
                //normalPanelCanvasg.DOFade(!gameUIList[(int)UIType.SETTING].gameObject.activeSelf ? 0:1, 0.3f);
                //setting.SetChildImgs(true);
                break;
            case UIType.MONSTERINFO_DETAIL:  //�� ����� ����â�̳� �߰����� â UI���������� �� ���� �ڼ��� ���� UI ��ȣ�ۿ� ���ݱ� X
                if (gameUIList[(int)UIType.MONSTERINFO_DETAIL_ITEM].gameObject.activeSelf || gameUIList[(int)UIType.MONSTERINFO_DETAIL_STAT].gameObject.activeSelf)
                    return true;
                break; 
            case UIType.CHANGEABLEMOBLIST:      //�� ����â �߸� �Ͻ�����
                if (!gameUIList[(int)UIType.CHANGEABLEMOBLIST].gameObject.activeSelf)
                    TimeManager.TimePause();
                break;
            case UIType.STAT:  //���� UI ������Ʈ
                UpdateStatUI();
                EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, false);
                break;
            case UIType.MONSTERINFO_DETAIL_STAT: //�� �ڼ��� ���⿡�� �߰� ���� ���� UI ����
                MonsterCollection.Instance.DetailStat();
                break;
            case UIType.MONSTERINFO_DETAIL_ITEM:  //�� �ڼ��� ���⿡�� ����� ���� UI ����
                MonsterCollection.Instance.DetailItem();
                break;
            case UIType.CHEF_FOODS_PANEL:
                if (gameUIList[(int)type].gameObject.activeSelf)  //���� �丮 ���� â �������µ� ���� ���� �г� ���������� �װ� ���� �ݾ���
                {
                    if(gameUIList[(int)UIType.PRODUCTION_PANEL].gameObject.activeSelf)
                    {
                        OnUIInteract(UIType.PRODUCTION_PANEL);
                        return true;
                    }
                }
                break;
            case UIType.MINIGAME_PICKUP:
                if (GameManager.Instance.pickupCheckGame.IsGameStart) return true; //�̴ϰ��� �ϰ� ������ ��ȣ�ۿ� ����
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

    void FilterStackUI(GameUI ui, bool add) //activeUIList�� �ְų� ���� �ʴ� UI��
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

    public void ActiveSpecialProcess(UIType type) //UI ���� ���� Ư���� ó��
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

    public void InActiveSpecialProcess(UIType type)//UI ���� ���� Ư���� ó��
    {
        switch (type)
        {
            case UIType.PRODUCTION_PANEL:
                CookingManager.Instance.MakeFoodInfoUIReset();  //���� ���� ǥ�� ���ֱ�
                break;
            case UIType.FOOD_DETAIL:
                CookingManager.Instance.detailID = string.Empty;  
                break;
            case UIType.ITEM_DETAIL:  //������ ����ǥ�� ���ֱ�
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
            case UIType.KEYSETTING:  //Ű���� â ������ ��ȣ�ۿ� ǥ��, ��ų ����, �� ���� ���� �� Ű�ڵ尡 ������ UI���� �ٽ� ������Ʈ��
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
        RequestSystemMsg("�������� UI�̰ų� ���׷� ���ؼ� UI�� ����� �ȳ���");
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

    #region Inventory
    public void DetailItemSlot(ItemSlot slot)  //�κ��丮���� ������ ���� Ŭ��
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
        //itemCntTxt.text = string.Format("����: {0}��", gm.GetItemCount(itemID));
        itemTypeTxt.text = Global.GetItemTypeName(data.itemType);

        itemAbilExplanation.text = data.abilExplanation;

        selectedImg.gameObject.SetActive(true);
        selectedImg.transform.SetParent(slot.root.transform);
        selectedImg.transform.localPosition = Vector3.zero;
        selectedImg.transform.SetAsLastSibling();
        selectedImg.transform.localScale = Vector3.one; //������ �ű� ���� �������� �۾����� ������� �ǵ���

        //itemUseBtn.gameObject.SetActive(data.itemType != ItemType.ETC);
        //if (data.itemType == ItemType.ETC && ((Ingredient)data).isUseable) itemUseBtn.gameObject.SetActive(true);

        Inventory.Instance.invenUseActionImg.SetActive(data.itemType != ItemType.ETC);
        if (data.itemType == ItemType.ETC && ((Ingredient)data).isUseable) Inventory.Instance.invenUseActionImg.SetActive(true);
    }

    public void UpdateInventoryItemCount(string id)
    {
        if (selectedItemId == id)
        {
            //itemCntTxt.text = string.Format("����: {0}��", gm.GetItemCount(id));
        }
    }
    #endregion

    #region Message
    public void RequestSystemMsg(string msg, int fontSize = 35, float existTime = 1.5f) //ȭ�� �߾� ��ܿ� �ߴ� �ý��� �޽���
    {
        PoolManager.GetItem("SystemMsg").GetComponent<SystemMsg>().Set(msg, fontSize, existTime, Util.Change255To1Color(221, 0, 0, 255));
    }
    public void RequestSystemMsg(string msg,Color textColor ,int fontSize = 35, float existTime = 1.5f) //ȭ�� �߾� ��ܿ� �ߴ� �ý��� �޽���
    {
        PoolManager.GetItem("SystemMsg").GetComponent<SystemMsg>().Set(msg, fontSize, existTime, textColor);
    }

    public void RequestLogMsg(string msg, float duration = 2f)  //ȭ�� ���� �ϴܿ� ǥ�õǴ� �α� �ؽ�Ʈ
    {
        Text t = PoolManager.GetItem<Text>("AcquisitionMsg");
        t.text = msg;
        Util.DelayFunc(() => t.gameObject.SetActive(false), duration, this, true);
    }

    public void RequestSelectionWindow(string message, List<Action> actions, List<string> btnTexts, bool activeWarning = true, List<Func<bool>> conditions = null, bool useIcon = false) //����â�� ���
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

    public void RequestWarningWindow(Action confirm, string warning, string verify = "Ȯ��", string cancel = "���")
    {
        wnWd.Register(confirm, warning, verify, cancel);
    }

    public void DefaultSelectionAction() //�ý��� Ȯ��â���� �� ��ư���� ������ ���� �����Լ��� �� �Լ��� ������
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
    public void DoChangeBody(string id)  //���� �������� â ���
    {
        if (mobSaveWindowActiveDic[id]) return;

        mobSaveWindowActiveDic[id] = true;

        //���� ���������� List<Action>�� ����� �̰ɷ� ������ �ϸ� ���ο��� DefaultSelectionAction�� ���ϸ� �������� �߰��ǹǷ� �ι�°���ʹ� DefaultSelectionAction�� �� �� �̻� ȣ���ϰ� �Ǵ� ������ �����
        RequestSelectionWindow("<color=#7A98FF>" + MonsterCollection.Instance.mobIdToSlot[id].BodyData.bodyName + "</color>��(��) ���� ���Կ�\n �����Ͻðڽ��ϱ�?\n(�����ϸ� �ش� ������ ��� Ȯ����\n 0%�� ���ư��ϴ�.)",
            new List<Action>() {() => CancelMonsterSaveChance(id) , () => SaveMonsterBody(id) }, new List<string>() {"����", "����"});
    }
    
    public void ShowSavedBodyList(bool isShow)
    {
        OnUIInteract(UIType.CHANGEABLEMOBLIST, true);

        EventManager.TriggerEvent(isShow ? "TimePause" : "TimeResume");
    }

    

    public void CancelMonsterSaveChance(string id)  //���� ������ ���� ���� ����
    {
        mobSaveWindowActiveDic[id] = false;
        EventManager.TriggerEvent("PlayerBodySet", id, false);
    }

    public void SaveMonsterBody(string id) //���� ������ ���� �����ϱ�
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

    #region �κ� ��ư (inventory button)
    public void OnClickRemoveItemBtn()  //������ ������ ��ư
    {
        //removeItemInfo.first.sprite = selectedItemSlot.itemImg.sprite;
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

        OnUIInteract(UIType.REMOVE_ITEM);  //������ �����⸦ �ߴٴ� �� �� �г��� �����־��ٴ� ���̴� (�׷��Ƿ� �ٽ� ���ش�)

        if (selectedItemSlot.itemInfo == null)
            OnUIInteract(UIType.ITEM_DETAIL, true); //���� ������ ���Կ� �� �̻� ������ ������ �κ� ������ �ڼ��� ����â�� ����
        OnUIInteractSetActive(UIType.PRODUCTION_PANEL, false, true);  //�丮 ����â�� ���������� ����
    }

    public void OnClickItemUseBtn()  //���� ���� ������ ���� ������ ����
    {
        ItemSO data = gm.GetItemData(selectedItemId);
        if ( (data.itemType == ItemType.ETC && !((Ingredient)data).isUseable)) return;

        data.Use();
        Inventory.Instance.RemoveItem(selectedItemId, 1, "�������� �Ҹ��߽��ϴ�.");

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
        statTexts[3].text = Mathf.RoundToInt(Mathf.Abs(stat.Speed)).ToString(); //���ǵ尡 ������ �Ҽ��� ������ ����� �� ���Ƽ� �ϴ��� ������ ������ ��.
        statTexts[4].text = string.Concat(stat.CriticalRate, '%');
        statTexts[5].text = stat.CriticalDamage.ToString() + "%";
        statTexts[6].text = stat.Intellect.ToString();
        statTexts[7].text = stat.AttackSpeed.ToString();

        //statText.text = $"HP\t\t{currentHP}/{stat.hp}\n\n���ݷ�\t\t{stat.damage}\n\n����\t\t{stat.defense}\n\n�̵��ӵ�\t\t{stat.speed}";
    }

    public void UpdatePlayerHPUI(bool decrease = false)
    {
        isStartDelayHPFillTimer = false;

        Player p = sgm.Player;
        float hp = Mathf.Clamp(p.CurrentHp, 0f, p.PlayerStat.MaxHp);

        float rate = (float)hp / p.PlayerStat.MaxHp;
        playerHPInfo.first.DOFillAmount(rate, 0.3f).OnComplete(()=> { if (!decrease) playerHPInfo.third.fillAmount = playerHPInfo.first.fillAmount; });
        playerHPInfo.second.text = string.Concat(Mathf.Ceil(hp), '/', Mathf.Ceil(p.PlayerStat.MaxHp));  //ǥ�ø� �ø��ؼ�

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
