using UnityEngine;
using UnityEngine.UI;

public class ChangeBodySlot : MonoBehaviour
{
    [SerializeField] private int slotId = -1; //KeyAction에서 몸통 바꾸기의 값과 동일  (int value of KeyAction Enum)
    [SerializeField] private string bodyID = ""; //이 슬롯에 저장되어있는 몬스터 아이디   (monster id)

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
