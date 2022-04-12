using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class KeyActionManager : MonoSingleton<KeyActionManager>
{
    private Dictionary<int, KeyInfoUI> keyInfoDic = new Dictionary<int, KeyInfoUI>();
    private Pair<int, int> selectedAndAlreadyID = new Pair<int, int>();  //선택된 키, 이미 존재하는 키

    private int changingKey = -1;
    public bool IsChangingKeySetting
    {
        get { return changingKey != -1; }
    }

    [SerializeField] private GameObject clickPrevPanel;

    public Pair<GameObject, Transform> keyInfoPair;

    [SerializeField] private Text playerHeadTxt; //플레이어 머리 위에 뜨는 독백(?) 텍스트
    public Vector3 playerHeadTextOffset;
    private Vector3 playerHeadTextCurOffset;
    private float phtOffTime;
    private bool twComp = false;

    private void Awake()
    {
        KeySetting.SetFixedKeySetting();
    }

    private void Start()
    {
        foreach(KeyAction action in KeySetting.fixedKeyDict.Keys)
        {
            Instantiate(keyInfoPair.first, keyInfoPair.second).GetComponent<KeyInfoUI>()
            .SetFixedKey(action, KeyCodeToString.GetString( KeySetting.fixedKeyDict[action] ));
        }
        foreach(KeyAction action in KeySetting.keyDict.Keys)
        {
            KeyInfoUI keyUI = Instantiate(keyInfoPair.first, keyInfoPair.second).GetComponent<KeyInfoUI>();
            keyUI.Set(action, KeySetting.keyDict[action], ()=>ChangeUserCustomKey((int)action, keyUI.ID));
            keyInfoDic.Add(keyUI.ID, keyUI);
        }
        SkillUIManager.Instance.UpdateSkillKeyCode();
        MonsterCollection.Instance.UpdateSavedBodyChangeKeyCodeTxt();

        SetPlayerHeadText("TestMsg", 5f, 40);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelKeySetting();
        }
        
    }

    private void LateUpdate()
    {
        FollowPlayerHeadText();
    }

    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey && IsChangingKeySetting)
        {
            if (keyEvent.keyCode == KeyCode.Escape) return;  //esc는 메시지 남기면 안되므로 따로 처리함

            if(!CanChangeKey(keyEvent.keyCode))
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
        switch(keyCode) 
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

    private (KeyAction,int) CheckExistSameKey(KeyCode keyCode)  //중복되는 키가 있는지 체크
    {
        foreach(KeyAction key in KeySetting.keyDict.Keys)
        {
            if (KeySetting.keyDict[key] == keyCode)
            {
                selectedAndAlreadyID.second = (int)key;
                return (key,(int)key);
            }
        }

        return (KeyAction.NULL,-1);
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
        foreach(KeyAction key in KeySetting.keyDict.Keys)
        {
            GameManager.Instance.savedData.option.keyInputDict[key] = KeySetting.keyDict[key];
        }
    }

    private void FollowPlayerHeadText()
    {
        if(playerHeadTxt.gameObject.activeSelf)
        {
            Transform target = SlimeGameManager.Instance.CurrentPlayerBody.transform;  //변신 시 플레이어가 잠깐 사라져서 이렇게 받아서 함
            if (target)
            {
                playerHeadTxt.transform.position = Util.ScreenToWorldPosForScreenSpace(target.position + playerHeadTextCurOffset, Util.WorldCvs);
            }

            if(!twComp && Time.time > phtOffTime)
            {
                twComp = true;
                playerHeadTxt.DOColor(Color.clear, 0.4f).OnComplete(()=>playerHeadTxt.gameObject.SetActive(false));
            }

            /*if (playerHeadTextCurOffset.y < playerHeadTextOffset.y)
            {
                playerHeadTextCurOffset.y += Time.deltaTime * (!twComp ? 3f : -3f);
            }*/
            playerHeadTextCurOffset.y += Time.deltaTime * (!twComp ? 3f : -3f);
            playerHeadTextCurOffset.y = Mathf.Clamp(playerHeadTextCurOffset.y, 0, playerHeadTextOffset.y);
        }
    }

    public void SetPlayerHeadText(string msg, float duration = -1f, int fontSize = 28)
    {
        playerHeadTxt.DOKill();
        playerHeadTxt.color = Color.black;
        playerHeadTextCurOffset = Vector3.zero;
        twComp = false;

        playerHeadTxt.text = msg;
        playerHeadTxt.fontSize = fontSize;
        playerHeadTxt.gameObject.SetActive(true);

        playerHeadTxt.DOColor(Color.white, 0.4f);

        if(duration > 0f)
        {
            phtOffTime = Time.time + duration;
        }
        else
        {
            phtOffTime = Time.time + Mathf.Clamp( msg.Length * 0.3f, 1f, 30f);
        }
    }
}
