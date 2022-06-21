using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using FkTweening;

public class KeyActionManager : MonoSingleton<KeyActionManager>
{
    #region Input User key
    private Dictionary<int, KeyInfoUI> keyInfoDic = new Dictionary<int, KeyInfoUI>();
    private Pair<int, int> selectedAndAlreadyID = new Pair<int, int>();  //선택된 키, 이미 존재하는 키

    private int changingKey = -1;
    public bool IsChangingKeySetting
    {
        get { return changingKey != -1; }
    }

    [SerializeField] private GameObject clickPrevPanel;

    public Pair<GameObject, Transform> keyInfoPair;
    #endregion

    #region Head Text
    [SerializeField] private Text playerHeadTxt; //플레이어 머리 위에 뜨는 독백(?) 텍스트
    public Vector3 playerHeadTextOffset;
    private HeadUIData headTextInfo;
    #endregion

    #region Head KeyInput Image
    public Image headImg, headFillImg;
    public Sprite question, exclamation, empty;

    [SerializeField] private List<KeyActionData> keyActionDataList = new List<KeyActionData>();
    private Dictionary<KeyAction, KeyActionData> keyActionDataDic = new Dictionary<KeyAction, KeyActionData>();

    public Vector3 headImgOffset;
    private HeadUIData headImgUIInfo;

    private float headImgFullTime = -1f;
    private float keyInputFillElapsed;
    private KeyAction tutoInputKeyAction;

    public bool IsNoticingGetMove => headImgFullTime > 0f;
    #endregion

    #region Quik Slot

    public CanvasGroup quikCvsg;
    public Image quikItemImg;
    public Text quikItemCountTxt;
    public Text quikKeyCodeTxt;
    private CustomContentsSizeFilter quikSlotCcsf;
    private string quikItemId;
    public string QuikItemId => quikItemId;

    private bool isUseableQuik = true;

    private bool isAutoQuik = true;

    #endregion

    private void Awake()
    {
        KeySetting.SetFixedKeySetting();

        headTextInfo = new HeadUIData(playerHeadTxt.GetComponent<RectTransform>(), playerHeadTextOffset);
        headImgUIInfo = new HeadUIData(headImg.GetComponent<RectTransform>(), headImgOffset, 1.7f);
        quikSlotCcsf = quikKeyCodeTxt.transform.parent.GetComponent<CustomContentsSizeFilter>();    

        for (int i = 0; i < keyActionDataList.Count; i++)
        {
            keyActionDataDic.Add(keyActionDataList[i].keyAction, keyActionDataList[i]);
        }

        EventManager.StartListening("UpdateKeyCodeUI", UpdateQuikKeyCode);
        EventManager.StartListening("GotoNextStage_LoadingStart", () =>
        {
            isUseableQuik = false;
        });

        //Global.AddAction("ItemUse", UpdateQuikSlotUI);
        Global.AddAction("RemoveItem", UpdateQuikSlotUI);
        Global.AddAction("GetItem", CheckGetHealItem);
    }

    private void OnEnable()
    {
        StageManager.Instance.NextStagePreEvent += () =>
        {
            isUseableQuik = true;
        };
    }

    private void Start()
    {
        if (TutorialManager.Instance.IsTutorialStage)
        {
            playerHeadTxt.GetComponent<CanvasGroup>().alpha = 0;
            StoredData.SetGameObjectKey("PlayerHeadTxtObj", playerHeadTxt.gameObject);

            CanvasGroup cvsg = headImg.transform.parent.GetComponent<CanvasGroup>();
            cvsg.alpha = 0;
            StoredData.SetGameObjectKey("PlayerHeadImgParent", cvsg.gameObject);
        }

        foreach (KeyAction action in KeySetting.fixedKeyDict.Keys)
        {
            Instantiate(keyInfoPair.first, keyInfoPair.second).GetComponent<KeyInfoUI>()
            .SetFixedKey(action, KeyCodeToString.GetString(KeySetting.fixedKeyDict[action]));
        }
        foreach (KeyAction action in KeySetting.keyDict.Keys)
        {
            KeyInfoUI keyUI = Instantiate(keyInfoPair.first, keyInfoPair.second).GetComponent<KeyInfoUI>();
            keyUI.Set(action, KeySetting.keyDict[action], () => ChangeUserCustomKey((int)action, keyUI.ID));
            keyInfoDic.Add(keyUI.ID, keyUI);
        }

        {  // 저장된 퀵 슬롯 불러옴
            string qid = GameManager.Instance.savedData.userInfo.quikSlotItemID;
            if (string.IsNullOrEmpty(qid))
            {
                UnregisterQuikSlot();
            }
            else
            {
                RegisterQuikSlot(qid);
            }
        }

        EventManager.TriggerEvent("UpdateKeyCodeUI");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelKeySetting();
        }

        if(Input.GetKeyDown(KeySetting.keyDict[KeyAction.ITEM_QUIKSLOT]))
        {
            UseQuikSlotItem();
        }
    }

    #region Quik Slot
    private void UseQuikSlotItem()
    {
        if(!string.IsNullOrEmpty(quikItemId) && isUseableQuik && !TimeManager.IsTimePaused && !Util.IsActiveGameUI(UIType.MENU) && InteractionHandler.canUseQuikSlot)
        {
            GameManager.Instance.UseItem(quikItemId);
        }
    }

    public void RegisterQuikSlot(string id)
    {
        if (string.IsNullOrEmpty(id) || !GameManager.Instance.GetItemData(id).isHealItem) return;

        if (quikItemId == id)
        {
            UnregisterQuikSlot();
            Inventory.Instance.SetActiveUseableMark(true, id);
            return;
        }

        quikItemId = id;
        quikItemCountTxt.gameObject.SetActive(true);
        quikItemImg.gameObject.SetActive(true);
        quikItemImg.sprite = GameManager.Instance.GetItemData(id).GetSprite();
        quikItemCountTxt.text = GameManager.Instance.GetItemCount(id).ToString();

        quikCvsg.alpha = 1;

        Inventory.Instance.SetActiveUseableMark(true, id);
    }

    public void UnregisterQuikSlot()
    {
        quikItemCountTxt.gameObject.SetActive(false);
        quikItemImg.gameObject.SetActive(false);
        quikItemId = string.Empty;

        quikCvsg.alpha = 0.5f;
    }

    private void UpdateQuikSlot()
    {
        int count = GameManager.Instance.GetItemCount(quikItemId);
        
        if(count == 0)
        {
            if (isAutoQuik)
            {
                string id = Inventory.Instance.FirstHealItem;
                if(!string.IsNullOrEmpty(id))
                {
                    SetAutoQuikSlotItem(id);
                }
                else
                    UnregisterQuikSlot();
            }
            else
                UnregisterQuikSlot();
            
        }
        else
        {
            quikItemCountTxt.text = count.ToString();
        }
    }

    private void UpdateQuikSlotUI(object data)
    {
        ItemInfo info = (ItemInfo)data;
        if (info.id == quikItemId)
        {
            UpdateQuikSlot();
        }
    }

    private void CheckGetHealItem(object id)
    {
        string sid = (string)id;
        UpdateQuikSlotUI(new ItemInfo(sid,0));

        if(GameManager.Instance.GetItemData(sid).isHealItem && isAutoQuik && string.IsNullOrEmpty(quikItemId))
        {
            RegisterQuikSlot(sid);
        }
    }

    public void UpdateQuikKeyCode()
    {
        //Debug.Log(KeySetting.keyDict.ContainsKey(KeyAction.ITEM_QUIKSLOT));  OnEnable에서 이거 찍으면 false뜸. keyDict세팅하는건 Awake..
        quikKeyCodeTxt.text = KeyCodeToString.GetString(KeySetting.keyDict[KeyAction.ITEM_QUIKSLOT]);
        quikSlotCcsf.UpdateSizeDelay();
    }

    public void SetAutoQuikSlot()
    {
        isAutoQuik = !isAutoQuik;
    }

    public void SetAutoQuikSlotItem(string aid = "")
    {
        if (string.IsNullOrEmpty(aid) && isAutoQuik && string.IsNullOrEmpty(quikItemId))
        {
            string id = Inventory.Instance.FirstHealItem;
            RegisterQuikSlot(id);
        }
        else if(!string.IsNullOrEmpty(aid))
        {
            RegisterQuikSlot(aid);
        }
    }

    #endregion

    private void FixedUpdate()
    {
        //플레이어가 FixedUpdate에서 움직이므로 여기서 돌려야 UI가 안떨림
        headTextInfo.Update();
        headImgUIInfo.Update();
        KeyInputUIFillUpdate();
    }

    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey && IsChangingKeySetting)
        {
            if (keyEvent.keyCode == KeyCode.Escape) return;  //esc는 메시지 남기면 안되므로 따로 처리함

            if (!CanChangeKey(keyEvent.keyCode))
            {
                UIManager.Instance.RequestSystemMsg("해당 키로는 변경할 수 없습니다.");
                CancelKeySetting();
                return;
            }

            UIManager.Instance.PreventItrUI(0.5f);
            KeyAction sameKeyAction;
            int sameKey;
            (sameKeyAction, sameKey) = CheckExistSameKey(keyEvent.keyCode);
            if (sameKey == -1)
            {
                KeySetting.keyDict[(KeyAction)changingKey] = keyEvent.keyCode;
            }
            else
            {
                KeyCode temp = KeySetting.keyDict[(KeyAction)changingKey];
                KeySetting.keyDict[(KeyAction)changingKey] = keyEvent.keyCode;
                KeySetting.keyDict[sameKeyAction] = temp;
                keyInfoDic[selectedAndAlreadyID.second].Set(KeySetting.keyDict[sameKeyAction]);
            }

            keyInfoDic[selectedAndAlreadyID.first].Set(keyEvent.keyCode);
            CancelKeySetting();
        }
    }

    private bool CanChangeKey(KeyCode keyCode) //해당 키로 변경할 수 있는지 (방향키와 esc 제외시킴)
    {
        switch (keyCode)
        {
            case KeyCode.W:
                return false;
            case KeyCode.A:
                return false;
            case KeyCode.S:
                return false;
            case KeyCode.D:
                return false;
            case KeyCode.LeftCommand:
                return false;
            case KeyCode.RightCommand:
                return false;
            case KeyCode.Menu:
                return false;


                /*case KeyCode.UpArrow:
                     return false;
                 case KeyCode.LeftArrow:
                     return false;
                 case KeyCode.RightArrow:
                     return false;
                 case KeyCode.DownArrow:
                     return false;*/
                /*case KeyCode.Escape:
                    return false;*/
        }
        return true;
    }

    private (KeyAction, int) CheckExistSameKey(KeyCode keyCode)  //중복되는 키가 있는지 체크
    {
        foreach (KeyAction key in KeySetting.keyDict.Keys)
        {
            if (KeySetting.keyDict[key] == keyCode)
            {
                selectedAndAlreadyID.second = (int)key;
                return (key, (int)key);
            }
        }

        return (KeyAction.NONE, -1);
    }

    public void ChangeUserCustomKey(int key, int id) //키셋 변경 버튼 클릭
    {
        changingKey = key;
        clickPrevPanel.SetActive(true);
        selectedAndAlreadyID.first = id;
    }

    public void CancelKeySetting()  //키셋 변경 취소
    {
        if (IsChangingKeySetting)
        {
            UIManager.Instance.PreventItrUI(0.5f);
            changingKey = -1;
            clickPrevPanel.SetActive(false);
            selectedAndAlreadyID.first = -1;
            selectedAndAlreadyID.second = -1;
        }
    }

    public void ResetKeySetting()  //키세팅 초기값으로 
    {
        KeySetting.SetDefaultKeySetting();
        foreach (KeyAction action in KeySetting.keyDict.Keys)
            keyInfoDic[(int)action].Set(KeySetting.keyDict[action]);
    }

    public void SaveKey()
    {
        foreach (KeyAction key in KeySetting.keyDict.Keys)
        {
            GameManager.Instance.savedData.option.keyInputDict[key] = KeySetting.keyDict[key];
        }
    }

    private void ClearHeadText()
    {
        playerHeadTxt.DOColor(Color.clear, 0.3f).OnComplete(() => playerHeadTxt.gameObject.SetActive(false));
    }

    public void SetPlayerHeadText(string msg, float duration = -1f, int fontSize = 22)
    {
        playerHeadTxt.DOKill();
        playerHeadTxt.color = Color.clear;

        playerHeadTxt.text = msg;
        playerHeadTxt.fontSize = fontSize;
        
        playerHeadTxt.DOColor(Color.black, 0.4f);

        if(duration <= 0f)
        {
            duration = Mathf.Clamp(msg.Length * 0.3f, 1f, 25f);
        }

        headTextInfo.Set(duration, new Vector2(0, 1), ClearHeadText);
    }

    private void ResetHeadImg()
    {
        headFillImg.gameObject.SetActive(false);
        headImg.DOKill();

        headImg.color = Color.clear;

        headImg.DOColor(Color.white, 0.3f);
    }

    private void ClearHeadImg()
    {
        headImg.DOColor(Color.clear, 0.3f).OnComplete(() =>
        {
            headImg.gameObject.SetActive(false);
            headFillImg.gameObject.SetActive(false);
        });
    }

    public void ShowQuestionMark() // 플레이어 머리 위에 ? 표시 띄워줌
    {
        if (IsNoticingGetMove) return;

        ResetHeadImg();
        headImg.sprite = question;
        headImgUIInfo.Set(0.75f, new Vector2(0, 1), ClearHeadImg);
    }
    public void ShowExclamationMark()  // 플레이어 머리 위에 ! 표시 띄워줌
    {
        if (IsNoticingGetMove) return;

        ResetHeadImg();
        headImg.sprite = exclamation;
        headImgUIInfo.Set(0.75f, new Vector2(0, 1), ClearHeadImg);
    }

    public void ExclamationCharging(float fullTime, KeyAction keyAction)  // 플레이어 머리 위에 ! 표시 띄워주고 게이지 차는 효과 시작
    {
        if (IsNoticingGetMove) return;

        headImgFullTime = fullTime;
        tutoInputKeyAction = keyAction;
        keyInputFillElapsed = 0f;

        headImg.DOKill();
        headImg.color = Color.white;
        headImg.SetAlpha(0.2f);
        headImg.sprite = exclamation;
        headImg.gameObject.SetActive(true);

        headFillImg.fillAmount = 0f;
        headFillImg.DOKill();
        headFillImg.transform.localScale = Vector3.one;
        headFillImg.color = Color.white;
        headFillImg.sprite = exclamation;
        headFillImg.gameObject.SetActive(true);
    }

    public void EndExclamationCharging(bool isSuccess)  // 플레이어 머리 위의 ! 표시 게이지 차는 효과 끝냄 (매개변수 - 게이지를 끝까지 채웠는가)
    {
        if (headImgFullTime < 0f) return;

        if(!isSuccess)
        {
            headImg.gameObject.SetActive(false);
            headFillImg.gameObject.SetActive(false);
            headImgFullTime = -1f;
        }
        else
        {
            headImg.color = Color.clear;
            headFillImg.sprite = keyActionDataDic[tutoInputKeyAction].keySprite;

            DOUtil.StartCo("Move Key Get UI", Util.DelayFuncCo(() =>
            {
                Sequence seq = DOTween.Sequence();
                seq.Append(headFillImg.transform.DOScale(Vector3.one * 1.5f, 0.4f).SetEase(Ease.Linear));
                seq.Append(headFillImg.transform.DOScale(SVector3.onePointThree, 0.26f).SetLoops(3, LoopType.Yoyo));
                seq.Append(headFillImg.DOColor(Color.clear, 0.33f).SetEase(Ease.Linear));
                seq.AppendCallback(() =>
                {
                    headImg.gameObject.SetActive(false);
                    headFillImg.gameObject.SetActive(false);
                    headImgFullTime = -1f;
                }).Play();
            }, 0.5f, false, false), this);
            
        }
    }

    private void KeyInputUIFillUpdate()  
    {
        if (headImgFullTime > 0f)
        {
            Transform target = Global.GetSlimePos;
            if (target)
            {
                headImg.GetComponent<RectTransform>().anchoredPosition = Util.WorldToScreenPosForScreenSpace(target.position + new Vector3(0,1.25f), Util.WorldCvs);
            }
            keyInputFillElapsed += Time.fixedDeltaTime;
            headFillImg.fillAmount = keyInputFillElapsed / headImgFullTime;
        }
    }
}
