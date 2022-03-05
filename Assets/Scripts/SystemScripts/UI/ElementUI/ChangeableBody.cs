using UnityEngine.UI;
using UnityEngine;

public class ChangeableBody : MonoBehaviour  //bottom left UI
{
    [SerializeField] private int slotNumber;
    public int SlotNumber => slotNumber;

    [SerializeField] private KeyAction slotKey;

    [SerializeField] private string bodyID = ""; //이 슬롯에 저장되어있는 몬스터 아이디   (monster id)
    public bool Registered => !string.IsNullOrEmpty(bodyID);

    public Image bodyImg;
    public Pair<Image, Text> coolTimeUIPair;
    public Text keyCodeTxt;

    public CustomContentsSizeFilter customContentsSizeFilter;

    #region timer

    [SerializeField] CanvasGroup cvsg;
    private bool isCoolTime = false;
    private float elapsed = 0f;
    private float coolTime = 10f; //테스트용 임시 변수

    #endregion

    private void Awake()
    {
        slotNumber = transform.GetSiblingIndex() + 1;
        coolTimeUIPair.second.gameObject.SetActive(false);
    }

    private void Start()
    {
        if(slotKey == KeyAction.CHANGE_SLIME)
        {
            Register("origin");
        }
        else
        {
            Unregister();
        }
    }

    public void UpdateKeyCodeTxt()
    {
        keyCodeTxt.text = KeyCodeToString.GetString(KeySetting.keyDict[slotKey]);
        Util.DelayFunc(() => customContentsSizeFilter.UpdateSize(), 0.2f);
    }

    public void Register(string id)
    {
        bodyID = id;
        bodyImg.sprite = Global.GetMonsterBodySprite(id);
    }

    public void Unregister()
    {
        if(isCoolTime)
        {
            isCoolTime = false;
            Util.DelayFunc(() => Unregister(), 0.1f);
            return;
        }

        coolTimeUIPair.first.fillAmount = 0;
        coolTimeUIPair.second.gameObject.SetActive(false);
        bodyImg.sprite = MonsterCollection.Instance.notExistBodySpr;
        bodyID = string.Empty;
        cvsg.alpha = 0.4f;
    }

    public void StartCoolTimeUI()
    {
        if (string.IsNullOrEmpty(bodyID)) return;

        //elapsed = coolTime;
        elapsed = 0f;
        isCoolTime = true;
        coolTimeUIPair.second.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(isCoolTime)
        {
            //elapsed -= Time.deltaTime;
            //coolTimeUIPair.first.fillAmount = elapsed / coolTime;

            elapsed += Time.deltaTime;
            coolTimeUIPair.first.fillAmount = (coolTime - elapsed) / coolTime;
            coolTimeUIPair.second.text = elapsed.ToString("0.0");

            if(elapsed > coolTime)
            {
                isCoolTime = false;
                coolTimeUIPair.second.gameObject.SetActive(false);
                coolTimeUIPair.first.fillAmount = 0;
            }
        }

        if(Input.GetKeyDown(KeySetting.keyDict[slotKey]))
        {
            SlimeGameManager.Instance.PlayerBodyChange(bodyID);
        }
    }
}
