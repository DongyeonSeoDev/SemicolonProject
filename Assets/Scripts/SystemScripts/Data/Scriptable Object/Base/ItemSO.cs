using UnityEngine;
using System;

public abstract class ItemSO : ScriptableObject
{
    [SerializeField] protected Sprite itemSprite;
    [SerializeField] protected Sprite secondItemSprite;  //두번째 아이템 스프라이트 (ex : 채집물 요소로서 맵에 배치될 때의 스프라이트는 다르게 할 때 씀)

    public ItemType itemType;

    public bool isHealItem;

    //public bool isUseable = true;

    public string id => name;  
    public int maxCount; //해당 아이템을 최대 몇 개까지 인벤토리에 겹치게 해서 가질 수 있는지

    //public int existBattleCount = -1; //n번의 전투 후에는 해당 아이템이 사라짐. -1이면 무한. 해당 스테이지 내 모든 몹 잡을 시 카운트를 한 번 셈.

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
        //아이템 사용 클래스 명과 아이템 스크립터블 오브젝트의 이름이 동일해야 함
        //Type type = Type.GetType(name);
        //ItemAbil abil = Activator.CreateInstance(type) as ItemAbil;
        //abil.Use();

        (Activator.CreateInstance(Type.GetType(name)) as ItemAbil).Use();
    }
}
