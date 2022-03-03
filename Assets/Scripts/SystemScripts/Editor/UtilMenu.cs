using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

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

        [MenuItem("File/Delete/SaveFile")]
        static void DeleteSaveFile()
        {
            if(File.Exists(Global.saveFileName_1.PersistentDataPath()))
            {
                File.Delete(Global.saveFileName_1.PersistentDataPath());
            }
        }

       /* [MenuItem("Hierarchy Manage/UI/ActiveUIMoveObj")]
        static void ActiveUIMoveObj()
        {
            foreach (UIMove um in GameObject.FindObjectsOfType<UIMove>()) um.gameObject.SetActive(true);
        }

        [MenuItem("Hierarchy Manage/UI/InActiveUIMoveObj")]
        static void InActiveUIMoveObj()
        {
            foreach (UIMove um in GameObject.FindObjectsOfType<UIMove>()) um.gameObject.SetActive(false);
        }*/
    }
}