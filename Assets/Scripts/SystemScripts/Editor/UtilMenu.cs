using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class UtilMenu 
{
    [MenuItem("Resource Manage/Data/Set Item ID")]
    static void SetResourceID()
    {
        int index = 0;
        int offset = 5;

        foreach (Food food in Resources.LoadAll<Food>("System/FoodData/"))
        {
            food.id = index;
            index += offset;
        }

        foreach (Ingredient ingredient in Resources.LoadAll<Ingredient>("System/IngredientData/"))
        {
            ingredient.id = index;
            index += offset;
        }
    }

    
}
