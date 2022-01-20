using System.Collections.Generic;
using UnityEngine;
using Water;

[CreateAssetMenu(fileName = "Food Data", menuName = "Scriptable Object/Food Data", order = int.MaxValue)]
public class Food : ItemSO
{
 
    //public string foodSpritePath; //��������Ʈ ���

    public List<IngredientCount> needIngredients = new List<IngredientCount>();  //�� ������ ����� ���ؼ� � ������ �� ���� �ʿ������� ���� ����Ʈ

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
        
    }
}


