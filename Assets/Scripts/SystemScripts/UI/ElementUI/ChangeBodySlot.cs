using UnityEngine;
using UnityEngine.UI;

public class ChangeBodySlot : MonoBehaviour  //클래스명 뭐라할지 애매하네
{
    [SerializeField] private int slotNumber;
    public int SlotNumber => slotNumber;
    [SerializeField] private string bodyID = ""; //이 슬롯에 저장되어있는 몬스터 아이디   (monster id)
    public bool Registered => !string.IsNullOrEmpty(bodyID);

    public Pair<Image, Text> monsterImgName;
    public Text assimilationRateTxt;
    public Button changeBtn;


    public void SetSlotNumber() => slotNumber = transform.GetSiblingIndex() + 1;

    private void Awake()
    {
        changeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.RequestWarningWindow(() =>
            {
                if (MonsterCollection.Instance.IDToSave != bodyID)
                {
                    PlayerEnemyUnderstandingRateManager.Instance.SetMountObj(MonsterCollection.Instance.IDToSave, SlotNumber - 1);
                }
                else
                {
                    PlayerEnemyUnderstandingRateManager.Instance.SetDrainProbabilityDict(MonsterCollection.Instance.IDToSave, 0);
                }
         
                UIManager.Instance.OnUIInteract(UIType.CHANGEABLEMOBLIST, true);
                TimeManager.TimeResume();

            }, "선택된 흡수한 몸을 제거하시겠습니까?");
        });
    }

    public void Register(string bodyID)
    {
        this.bodyID = bodyID;

        monsterImgName.first.sprite = MonsterCollection.Instance.GetPlayerMonsterSpr(bodyID);
        //monsterImgName.first.sprite = Global.GetMonsterBodySprite(bodyID);
        monsterImgName.second.text = Global.GetMonsterName(bodyID);
       
        changeBtn.interactable = true;
        changeBtn.GetComponent<UIScale>().transitionEnable = true;
    }

    public void Unregister()
    {
        bodyID = string.Empty;

        monsterImgName.first.sprite = MonsterCollection.Instance.notExistBodySpr;
        monsterImgName.second.text = string.Empty;
        assimilationRateTxt.text = string.Empty;
        changeBtn.interactable = false;
        changeBtn.GetComponent<UIScale>().transitionEnable = false;
    }

    public void UpdateUI()
    {
        assimilationRateTxt.text = string.Concat("동화율 : ", PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(bodyID).ToString().ToColorStr("#375B89"), '%');
    }


}
