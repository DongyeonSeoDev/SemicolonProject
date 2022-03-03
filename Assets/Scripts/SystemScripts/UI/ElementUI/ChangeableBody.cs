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

    #region timer

    [SerializeField] CanvasGroup cvsg;
    private bool isCoolTime = false;
    private float elapsed = 0f;
    private float coolTime = 10f; //�׽�Ʈ�� �ӽ� ����

    #endregion

    private void Awake()
    {
        slotNumber = transform.GetSiblingIndex() + 1;
    }

    public void UpdateKeyCodeTxt()
    {
        keyCodeTxt.text = KeyCodeToString.GetString(KeySetting.keyDict[slotKey]);
    }

    public void Register(string id, Sprite bodySpr)
    {
        bodyID = id;
        bodyImg.sprite = bodySpr;
    }

    public void Unregister()
    {
        coolTimeUIPair.first.fillAmount = 0;
        coolTimeUIPair.second.gameObject.SetActive(false);
        //bodyImg.sprite = �ƹ��͵� ���� �� ��Ÿ���� ��������Ʈ;
        bodyID = string.Empty;
        cvsg.alpha = 0.4f;
    }

    public void StartCoolTimeUI()
    {
        elapsed = 0f;
        isCoolTime = true;
        coolTimeUIPair.second.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(isCoolTime)
        {
            elapsed += Time.deltaTime;
            coolTimeUIPair.first.fillAmount = elapsed / coolTime;
            coolTimeUIPair.second.text = elapsed.ToString("0.0");
            //coolTimeUIPair.second.text = elapsed.ToString("F2");
        }
    }
}
