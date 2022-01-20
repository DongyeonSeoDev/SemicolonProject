using UnityEngine;
using Water;

public abstract class ItemSO : ScriptableObject
{
    [SerializeField] protected Sprite itemSprite;

    public ItemType itemType;

    //public bool isUseable = true;

    public int id;
    public int maxCount; //�ش� �������� �ִ� �� ������ �κ��丮�� ��ġ�� �ؼ� ���� �� �ִ���

    public string itemName;

    [TextArea]
    public string explanation;

    public virtual Sprite GetSprite()
    {
        return itemSprite;
    }

    public abstract void Use();
}
