using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;

public class KeyActionManager : MonoSingleton<KeyActionManager>
{
    private Dictionary<int, KeyInfoUI> keyInfoDic = new Dictionary<int, KeyInfoUI>();
    private Pair<int, int> selectedAndAlreadyID = new Pair<int, int>();  //���õ� Ű, �̹� �����ϴ� Ű

    private int changingKey = -1;
    public bool IsChangingKeySetting
    {
        get { return changingKey != -1; }
    }

    [SerializeField] private GameObject clickPrevPanel;

    public Pair<GameObject, Transform> keyInfoPair;

    #region Head Text
    [SerializeField] private Text playerHeadTxt; //�÷��̾� �Ӹ� ���� �ߴ� ����(?) �ؽ�Ʈ
    private RectTransform phtRectTr;
    public Vector3 playerHeadTextOffset;
    private Vector3 playerHeadTextCurOffset;
    private float phtOffTime;
    private bool twComp;  //������� tween �������ΰ�
    #endregion

    private void Awake()
    {
        KeySetting.SetFixedKeySetting();
        phtRectTr = playerHeadTxt.GetComponent<RectTransform>();
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

        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelKeySetting();
        }
        
        
    }

    private void FixedUpdate()
    {
        FollowPlayerHeadText();
    }

    private void OnGUI()
    {
        Event keyEvent = Event.current;
        if (keyEvent.isKey && IsChangingKeySetting)
        {
            if (keyEvent.keyCode == KeyCode.Escape) return;  //esc�� �޽��� ����� �ȵǹǷ� ���� ó����

            if(!CanChangeKey(keyEvent.keyCode))
            {
                UIManager.Instance.RequestSystemMsg("�ش� Ű�δ� ������ �� �����ϴ�.");
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

    private bool CanChangeKey(KeyCode keyCode) //�ش� Ű�� ������ �� �ִ��� (����Ű�� esc ���ܽ�Ŵ)
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

    private (KeyAction,int) CheckExistSameKey(KeyCode keyCode)  //�ߺ��Ǵ� Ű�� �ִ��� üũ
    {
        foreach(KeyAction key in KeySetting.keyDict.Keys)
        {
            if (KeySetting.keyDict[key] == keyCode)
            {
                selectedAndAlreadyID.second = (int)key;
                return (key,(int)key);
            }
        }

        return (KeyAction.NONE,-1);
    }

    public void ChangeUserCustomKey(int key, int id) //Ű�� ���� ��ư Ŭ��
    {
        changingKey = key;
        clickPrevPanel.SetActive(true);
        selectedAndAlreadyID.first = id;
    }

    public void CancelKeySetting()  //Ű�� ���� ���
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

    public void ResetKeySetting()  //Ű���� �ʱⰪ���� 
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
            Transform target = SlimeGameManager.Instance.CurrentPlayerBody.transform;  //���� �� �÷��̾ ��� ������� �̷��� �޾Ƽ� ��
            if (target)
            {
                phtRectTr.anchoredPosition = Util.ScreenToWorldPosForScreenSpace(target.position + playerHeadTextCurOffset, Util.WorldCvs);
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
            playerHeadTextCurOffset.y += Time.deltaTime * (!twComp ? 1.7f : -1.7f);
            playerHeadTextCurOffset.y = Mathf.Clamp(playerHeadTextCurOffset.y, 1, playerHeadTextOffset.y);
        }
    }

    public void SetPlayerHeadText(string msg, float duration = -1f, int fontSize = 22)
    {
        playerHeadTxt.DOKill();
        playerHeadTxt.color = Color.clear;
        playerHeadTextCurOffset = new Vector2(0, 1);
        twComp = false;

        playerHeadTxt.text = msg;
        playerHeadTxt.fontSize = fontSize;
        playerHeadTxt.gameObject.SetActive(true);

        playerHeadTxt.DOColor(Color.black, 0.4f);

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
