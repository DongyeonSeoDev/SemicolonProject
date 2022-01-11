using System.Collections.Generic;
using UnityEngine;
using Water;

[CreateAssetMenu(fileName = "Food Data", menuName = "Scriptable Object/Food Data", order = int.MaxValue)]
public class Food : ScriptableObject
{
    private Sprite foodSprite;

    public int id;
    public string foodName;
    //public string foodSpritePath; //��������Ʈ ���

    public List<IngredientCount> needIngredients = new List<IngredientCount>();  //�� ������ ����� ���ؼ� � ������ �� ���� �ʿ������� ���� ����Ʈ

    public Sprite GetSprite() 
    {
        if(!foodSprite)
        {
            foodSprite = Resources.Load<Sprite>(Global.foodSpritePath+name);
        }
        return foodSprite;
    }
}


