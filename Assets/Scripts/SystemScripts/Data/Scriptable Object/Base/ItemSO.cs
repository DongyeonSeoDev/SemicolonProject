using UnityEngine;
using System;

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

    public virtual void Use()
    {
        //������ ��� Ŭ���� ��� ������ ��ũ���ͺ� ������Ʈ�� �̸��� �����ؾ� ��
        Type type = Type.GetType(name);
        ItemAbil abil = Activator.CreateInstance(type) as ItemAbil;
        abil.Use();
    }
}
