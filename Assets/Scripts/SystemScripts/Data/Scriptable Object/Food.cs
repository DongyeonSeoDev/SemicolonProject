using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Food Data", menuName = "Scriptable Object/Food Data", order = int.MaxValue)]
public class Food : ScriptableObject
{
    public int id;
    public string foodName;
    public string foodSpritePath;

    public List<IngredientCount> needIngredients = new List<IngredientCount>();
}


