using UnityEngine;
using UnityEngine.UI;

public class ChangeBodySlot : MonoBehaviour
{
    [SerializeField] private int slotNumber;
    [SerializeField] private KeyAction slotKey; 
    [SerializeField] private string bodyID = ""; //�� ���Կ� ����Ǿ��ִ� ���� ���̵�   (monster id)

    public Pair<Image, Text> monsterImgName;
    public Button changeBtn;

    private void Awake()
    {
        slotNumber = transform.GetSiblingIndex() + 1;

        changeBtn.onClick.AddListener(() =>
        {
            //�ش� ���� �� �� ����
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
