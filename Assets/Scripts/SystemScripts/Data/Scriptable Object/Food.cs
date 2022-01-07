using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Data", menuName = "Scriptable Object/Food Data", order = int.MaxValue)]
public class Food : ScriptableObject
{
    private Sprite foodSprite;

    public int id;
    public string foodName;
    public string foodSpritePath; //스프라이트 경로

    public List<IngredientCount> needIngredients = new List<IngredientCount>();  //이 음식을 만들기 위해서 어떤 재료들이 몇 개씩 필요한지에 대한 리스트

    public Sprite GetSprite() 
    {
        if(!foodSprite)
        {
            foodSprite = Resources.Load<Sprite>(foodSpritePath);
        }
        return foodSprite;
    }
}


