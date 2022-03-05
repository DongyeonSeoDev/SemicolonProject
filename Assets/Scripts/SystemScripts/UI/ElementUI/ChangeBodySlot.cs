using UnityEngine;
using UnityEngine.UI;

public class ChangeBodySlot : MonoBehaviour  //Ŭ������ �������� �ָ��ϳ�
{
    [SerializeField] private int slotNumber;
    public int SlotNumber => slotNumber;
    [SerializeField] private string bodyID = ""; //�� ���Կ� ����Ǿ��ִ� ���� ���̵�   (monster id)
    public bool Registered => !string.IsNullOrEmpty(bodyID);

    public Pair<Image, Text> monsterImgName;
    public Button changeBtn;


    public void SetSlotNumber() => slotNumber = transform.GetSiblingIndex() + 1;

    private void Awake()
    {
        changeBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.RequestWarningWindow(() =>
            {
                //�ٸ� ������ �����
                UIManager.Instance.OnUIInteract(UIType.CHANGEABLEMOBLIST, true);
            }, "���õ� ����� ���� �����Ͻðڽ��ϱ�?");
        });
    }

    public void Register(string bodyID, string mobName)
    {
        this.bodyID = bodyID;

        monsterImgName.first.sprite = Global.GetMonsterBodySprite(bodyID);
        monsterImgName.second.text = mobName;

        changeBtn.interactable = true;
        changeBtn.GetComponent<UIScale>().transitionEnable = true;
    }

    public void Unregister()
    {
        bodyID = string.Empty;

        monsterImgName.first.sprite = MonsterCollection.Instance.notExistBodySpr;
        monsterImgName.second.text = string.Empty;
        changeBtn.interactable = false;
        changeBtn.GetComponent<UIScale>().transitionEnable = false;
    }

    
}
