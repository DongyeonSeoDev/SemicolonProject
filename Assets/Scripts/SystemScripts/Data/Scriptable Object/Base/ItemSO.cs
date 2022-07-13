using UnityEngine;
using System;

public abstract class ItemSO : ScriptableObject
{
    [SerializeField] protected Sprite itemSprite;
    [SerializeField] protected Sprite secondItemSprite;  //�ι�° ������ ��������Ʈ (ex : ä���� ��ҷμ� �ʿ� ��ġ�� ���� ��������Ʈ�� �ٸ��� �� �� ��)

    public ItemType itemType;

    public bool isHealItem;

    //public bool isUseable = true;

    public string id => name;  
    public int maxCount; //�ش� �������� �ִ� �� ������ �κ��丮�� ��ġ�� �ؼ� ���� �� �ִ���

    //public int existBattleCount = -1; //n���� ���� �Ŀ��� �ش� �������� �����. -1�̸� ����. �ش� �������� �� ��� �� ���� �� ī��Ʈ�� �� �� ��.

    public string itemName;

    [TextArea]
    public string explanation;

    [TextArea]
    public string abilExplanation;

    public virtual Sprite GetSprite()
    {
        return itemSprite;
    }

    public virtual Sprite GetSecondSprite()
    {
        return secondItemSprite;
    }

    public virtual void Use()
    {
        //������ ��� Ŭ���� ��� ������ ��ũ���ͺ� ������Ʈ�� �̸��� �����ؾ� ��
        //Type type = Type.GetType(name);
        //ItemAbil abil = Activator.CreateInstance(type) as ItemAbil;
        //abil.Use();

        (Activator.CreateInstance(Type.GetType(name)) as ItemAbil).Use();
    }
}
