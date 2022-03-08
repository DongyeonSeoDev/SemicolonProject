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
    private float coolTimer;
    //private float elapsed = 0f;
    private float CoolTime => SlimeGameManager.Instance.BodyChangeTime;

    #endregion

    private void Awake()
    {
        slotNumber = transform.GetSiblingIndex() + 1;
        coolTimeUIPair.second.gameObject.SetActive(false);
    }

    public void InitSet()
    {
        if (slotKey == KeyAction.CHANGE_SLIME)
        {
            Register("origin");
        }
        else
        {
            Unregister();
        }
    }

    public void CheckUsedMob(string id)
    {
        if (Registered)
        {
            //outline같은걸로 표시 필요
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
        cvsg.alpha = 1;
    }

    public void Unregister()
    {
        if(isCoolTime)
        {
            isCoolTime = false;
            Util.DelayFunc(() => Unregister(), 0.1f);
            return;
        }

        MonsterCollection.Instance.MarkAcqBodyFalse(bodyID);
        coolTimeUIPair.first.fillAmount = 0;
        coolTimeUIPair.second.gameObject.SetActive(false);
        bodyImg.sprite = MonsterCollection.Instance.notExistBodySpr;
        bodyID = string.Empty;
        cvsg.alpha = 0.4f;
    }

    public void StartCoolTimeUI()
    {
        if (string.IsNullOrEmpty(bodyID)) return;

        //elapsed = 0f;
        coolTimer = CoolTime;
        isCoolTime = true;
        coolTimeUIPair.second.gameObject.SetActive(true);
    }

    private void Update()
    {
        if(isCoolTime)
        {
            //elapsed += Time.deltaTime;
            //coolTimeUIPair.first.fillAmount = (CoolTime - elapsed) / CoolTime;
            //coolTimeUIPair.second.text = elapsed.ToString("0.0");

            coolTimer -= Time.deltaTime;
            coolTimeUIPair.first.fillAmount = coolTimer / CoolTime;
            coolTimeUIPair.second.text = coolTimer.ToString("0.0");

            if (coolTimer <= 0f)
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
