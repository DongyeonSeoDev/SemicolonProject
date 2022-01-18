using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Water
{
    public class UtilMenu
    {
        [MenuItem("Resource Manage/Data/Set Item ID")]
        static void SetResourceID()
        {
            int index = 0;
            int offset = 5;

            foreach (Food food in Resources.LoadAll<Food>(Global.foodDataPath))
            {
                food.id = index;
                index += offset;
            }

            foreach (Ingredient ingredient in Resources.LoadAll<Ingredient>(Global.ingredientDataPath))
            {
                ingredient.id = index;
                index += offset;
            }
        }

        [MenuItem("Resource Manage/Data/Set Max Count of Item Possession")]
        static void SetItemPossessionMaxCount()
        {
            List<ItemSO> list = new List<ItemSO>();

            foreach (Food food in Resources.LoadAll<Food>(Global.foodDataPath)) 
                list.Add(food);
            
            foreach (Ingredient ingredient in Resources.LoadAll<Ingredient>(Global.ingredientDataPath))
                list.Add(ingredient);

            list.ForEach(item =>
            {
                switch(item.itemType)
                {
                    case ItemType.EQUIP:
                        item.maxCount = 1;
                        break;
                    case ItemType.CONSUME:
                        item.maxCount = 5;
                        break;
                    case ItemType.ETC:
                        item.maxCount = 20;
                        break;
                }
            });
        }
    }
}