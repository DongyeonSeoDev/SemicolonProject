using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient Data", menuName = "Scriptable Object/Ingredient Data", order = int.MaxValue)]
public class Ingredient : ItemSO
{

    //public string spritePath;  //스프라이트 경로

    public bool isUseable;

    public override Sprite GetSprite()
    {
        if (!itemSprite)
        {
            itemSprite = Resources.Load<Sprite>(Global.ingredientSpritePath + name + "Spr");
        }
        return itemSprite;
    }

    public override Sprite GetSecondSprite()
    {
        if (!secondItemSprite)
        {
            secondItemSprite = Resources.Load<Sprite>(Global.ingredientSpritePath + name + "Spr2");
        }

        return secondItemSprite;
    }

    public override void Use()
    {
        if(isUseable)
        {
            base.Use();
        }
    }
}
