using UnityEngine;
using UnityEngine.UI;

public class KeyInfoUI : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Text keyActionNameTxt, keyCodeTxt;
    public Text KeyCodeTxt { get { return keyCodeTxt; } }

    public int ID { get; private set; }

    public void Set(KeyAction keyAction, KeyCode keyCode, System.Action action)
    {
        keyActionNameTxt.text = Global.ToKeyActionName(keyAction);
        keyCodeTxt.text = keyCode.ToString();
        btn.onClick.AddListener(()=>action());
        ID = (int)keyAction;
    }
    public void Set(KeyCode keyCode)
    {
        keyCodeTxt.text = keyCode.ToString();
    }

    public void SetFixedKey(KeyAction keyAction, string keyCode)
    {
        keyActionNameTxt.text = Global.ToKeyActionName(keyAction);
        keyCodeTxt.text = keyCode;
        btn.onClick.AddListener(() => UIManager.Instance.RequestSystemMsg("해당 키는 바꿀 수 없습니다."));
        btn.GetComponent<NameInfoFollowingCursor>().explanation = "고정키";
    }
}
