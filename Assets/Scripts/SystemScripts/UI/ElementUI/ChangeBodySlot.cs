using UnityEngine;
using UnityEngine.UI;

public class ChangeBodySlot : MonoBehaviour
{
    [SerializeField] private int slotId = -1; //KeyAction���� ���� �ٲٱ��� ���� ����
    [SerializeField] private string bodyID = ""; //�� ���Կ� ����Ǿ��ִ� ���� ���̵�

    public Pair<Image, Text> monsterImgName;

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
