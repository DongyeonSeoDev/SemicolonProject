using UnityEngine.UI;
using UnityEngine;

public class ChangeableBody : MonoBehaviour  //bottom left UI
{
    [SerializeField] private int slotNumber;
    public int SlotNumber => slotNumber;

    [SerializeField] private KeyAction slotKey;
    public KeyAction SlotKey => slotKey;    

    [SerializeField] private string bodyID = ""; //이 슬롯에 저장되어있는 몬스터 아이디   (monster id)
    public string BodyID => bodyID;
    public bool Registered => !string.IsNullOrEmpty(bodyID);

    private RectTransform rectTrm;
    public RectTransform RectTrm
    {
        get
        {
            if(rectTrm==null)
            {
                rectTrm = GetComponent<RectTransform>();
            }
            return rectTrm;
        }
    }

    public Image bodyImg;
    public Pair<Image, Text> coolTimeUIPair;
    public Text keyCodeTxt;

    public CustomContentsSizeFilter customContentsSizeFilter;

    public GameObject outlineObj;

    #region timer

    [SerializeField] CanvasGroup cvsg;
    private bool isCoolTime = false;
    private float CoolTimer => SlimeGameManager.Instance.BodyChangeTimer;
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
            outlineObj.SetActive(true);
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
            outlineObj.SetActive(id == bodyID);
        }
        else
        {
            if (outlineObj.activeSelf)
                outlineObj.SetActive(false);
        }
    }

    public void UpdateKeyCodeTxt()
    {
        keyCodeTxt.text = KeyCodeToString.GetString(KeySetting.keyDict[slotKey]);
        Util.DelayFunc(() => customContentsSizeFilter.UpdateSize(), 0.1f);
    }

    public void Register(string id)
    {
        if(!string.IsNullOrEmpty(bodyID))
        {
            MonsterCollection.Instance.MarkAcqBodyFalse(bodyID);
            MonsterCollection.Instance.UpdateUnderstanding(bodyID);
            MonsterCollection.Instance.UpdateDrainProbability(bodyID);
        }

        bodyID = id;
        bodyImg.sprite = MonsterCollection.Instance.GetPlayerMonsterSpr(id);
        //bodyImg.sprite = Global.GetMonsterBodySprite(id);
        cvsg.alpha = 1;

        if(SlimeGameManager.Instance.BodyChangeTimer > 0)
        {
            StartCoolTimeUI();
        }
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

            coolTimeUIPair.first.fillAmount = CoolTimer / CoolTime;
            coolTimeUIPair.second.text = CoolTimer.ToString("0.0");

            if (CoolTimer <= 0f)
            {
                isCoolTime = false;
                coolTimeUIPair.second.gameObject.SetActive(false);
                coolTimeUIPair.first.fillAmount = 0;
            }
        }

        if(Input.GetKeyDown(KeySetting.keyDict[slotKey]) && !TimeManager.IsTimePaused && InteractionHandler.canTransformEnemy)
        {
            SlimeGameManager.Instance.PlayerBodyChange(bodyID);
        }
    }
}
