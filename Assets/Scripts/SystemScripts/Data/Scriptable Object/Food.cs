using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Food Data", menuName = "Scriptable Object/Food Data", order = int.MaxValue)]
public class Food : ItemSO
{
 
    //public string foodSpritePath; //스프라이트 경로

    public List<IngredientCount> needIngredients = new List<IngredientCount>();  //이 음식을 만들기 위해서 어떤 재료들이 몇 개씩 필요한지에 대한 리스트

    public override Sprite GetSprite() 
    {
        if(!itemSprite)
        {
            itemSprite = Resources.Load<Sprite>(Global.foodSpritePath+name+"Spr");
        }
        return itemSprite;
    }

    public override void Use()
    {
        base.Use();
        Debug.Log("사용함 " + name);
    }
}


