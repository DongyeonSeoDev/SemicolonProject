using UnityEngine.UI;
using UnityEngine;

public class ChangeableBody : MonoBehaviour  //bottom left UI
{
    [SerializeField] private int slotNumber;
    [SerializeField] private KeyAction slotKey;
    [SerializeField] private string bodyID = ""; //이 슬롯에 저장되어있는 몬스터 아이디   (monster id)

    public Image bodyImg;

    public Pair<Image, Text> coolTimeUIPair;

    public Text keyCodeTxt;

    private void Awake()
    {
        slotNumber = transform.GetSiblingIndex() + 1;
    }

    public void UpdateKeyCodeTxt()
    {
        keyCodeTxt.text = KeyCodeToString.GetString(KeySetting.keyDict[slotKey]);
    }
}
