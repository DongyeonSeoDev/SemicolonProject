using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Water
{
    public class UtilMenu
    {
        /*[MenuItem("Resource Manage/Data/Set Item ID")]  //원래는 아이템 아이디 타입이 int형이었으나 string으로 바꾸면서 필요 없어짐
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
        }*/

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

        [MenuItem("Resource Manage/Data/Set Stage Prefab")]
        static void SetStagePrefab()
        {
            StageDataSO[] datas = Resources.LoadAll<StageDataSO>("Stage/SO/Stage1/");
            foreach (StageDataSO data in datas)
                data.SetPrefab();
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