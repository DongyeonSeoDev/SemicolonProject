using UnityEngine.UI;
using UnityEngine;

public class ChangeableBody : MonoBehaviour  //bottom left UI
{
    [SerializeField] private int slotNumber;
    [SerializeField] private KeyAction slotKey;
    [SerializeField] private string bodyID = ""; //�� ���Կ� ����Ǿ��ִ� ���� ���̵�   (monster id)

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
