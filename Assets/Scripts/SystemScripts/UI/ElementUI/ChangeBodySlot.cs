using UnityEngine;
using UnityEngine.UI;

public class ChangeBodySlot : MonoBehaviour
{
    [SerializeField] private int slotNumber;
    [SerializeField] private string bodyID = ""; //이 슬롯에 저장되어있는 몬스터 아이디   (monster id)

    public Pair<Image, Text> monsterImgName;
    public Button changeBtn;

    private void Awake()
    {
        slotNumber = transform.GetSiblingIndex() + 1;

        changeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.RequestWarningWindow(() =>
            {
                //다른 몹으로 덮어쓰기
                UIManager.Instance.OnUIInteract(UIType.CHANGEABLEMOBLIST, true);
            }, "선택된 흡수한 몸을 제거하시겠습니까?");
        });
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(bodyID))
        {
            //monsterImgName.first.sprite = empty slot sprite
            monsterImgName.second.text = string.Empty;
            changeBtn.interactable = false;
            changeBtn.GetComponent<UIScale>().transitionEnable = false;
        }
    }

    public void Register(string bodyID, Sprite mobSpr, string mobName)
    {
        this.bodyID = bodyID;

        monsterImgName.first.sprite = mobSpr;
        monsterImgName.second.text = mobName;

        changeBtn.interactable = true;
        changeBtn.GetComponent<UIScale>().transitionEnable = true;
    }

    public void Unregister()
    {
        bodyID = string.Empty;
    }

    
}
