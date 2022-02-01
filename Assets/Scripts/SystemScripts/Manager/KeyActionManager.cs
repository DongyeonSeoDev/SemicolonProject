using UnityEngine;

public class KeyActionManager : MonoSingleton<KeyActionManager>
{
    private int changingKey = -1;
    public bool IsChangingKeySetting
    {
        get { return changingKey != -1; }
    }

    [SerializeField] private GameObject clickPrevPanel; 


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelKeySetting();
        }
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
                return;
            }

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
            }

            CancelKeySetting();
            Debug.Log(((KeyAction)changingKey).ToString() + " : " + keyEvent.keyCode.ToString());
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
            case KeyCode.UpArrow:
                return false;
            case KeyCode.LeftArrow:
                return false;
            case KeyCode.RightArrow:
                return false;
            case KeyCode.DownArrow:
                return false;
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
                return (key,(int)key);
            }
        }

        return (KeyAction.NULL,-1);
    }

    public void ChangeUserCustomKey(int key) //키셋 변경 버튼 클릭
    {
        changingKey = key;
        clickPrevPanel.SetActive(true);
    }

    public void CancelKeySetting()  //키셋 변경 취소
    {
        if (IsChangingKeySetting)
        {
            changingKey = -1;
            clickPrevPanel.SetActive(false);
        }
    }

    public void ResetKeySetting()  //키세팅 초기값으로 
    {
        KeySetting.SetDefaultKeySetting();
    }

    public void SaveKey()
    {
        foreach(KeyAction key in KeySetting.keyDict.Keys)
        {
            GameManager.Instance.savedData.option.keyInputDict[key] = KeySetting.keyDict[key];
        }
    }
}
