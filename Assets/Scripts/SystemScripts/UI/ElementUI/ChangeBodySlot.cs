using UnityEngine;
using UnityEngine.UI;

public class ChangeBodySlot : MonoBehaviour
{
    [SerializeField] private int slotId = -1; //KeyAction���� ���� �ٲٱ��� ���� ����  (int value of KeyAction Enum)
    [SerializeField] private string bodyID = ""; //�� ���Կ� ����Ǿ��ִ� ���� ���̵�   (monster id)

    public Pair<Image, Text> monsterImgName;
    public Button transformBtn, deleteBtn;

    //private Button mobDetailBtn;

    /*private void Awake()
    {
        mobDetailBtn = monsterImgName.first.GetComponent<Button>();
    }*/

    public void Set(int slotID)
    {
        slotId = slotID;
    }
    public void Register(string bodyID)
    {
        this.bodyID = bodyID;
    }

    public void Unregister()
    {
        bodyID = "";
    }
}
