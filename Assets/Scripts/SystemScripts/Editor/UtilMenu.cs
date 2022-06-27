using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Water
{
    public class UtilMenu
    {

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

        [MenuItem("File/Delete/" + Global.GAME_SAVE_FILE + "(Test Save File)")]
        static void DeleteTestSaveFile()
        {
            if(File.Exists(Global.GAME_SAVE_FILE.PersistentDataPath()))
            {
                File.Delete(Global.GAME_SAVE_FILE.PersistentDataPath());
            }
        }

        [MenuItem("File/Delete/" + Global.SAVE_FILE_1)]
        static void DeleteSaveFile1()
        {
            if (File.Exists(Global.SAVE_FILE_1.PersistentDataPath()))
            {
                File.Delete(Global.SAVE_FILE_1.PersistentDataPath());
            }
        }

        [MenuItem("File/Delete/" + Global.SAVE_FILE_2)]
        static void DeleteSaveFile2()
        {
            if (File.Exists(Global.SAVE_FILE_2.PersistentDataPath()))
            {
                File.Delete(Global.SAVE_FILE_2.PersistentDataPath());
            }
        }

        [MenuItem("File/Delete/" + Global.SAVE_FILE_3)]
        static void DeleteSaveFile3()
        {
            if (File.Exists(Global.SAVE_FILE_3.PersistentDataPath()))
            {
                File.Delete(Global.SAVE_FILE_3.PersistentDataPath());
            }
        }

        [MenuItem("File/Delete/" + SaveFileStream.EternalOptionSaveFileName)]
        static void DeleteSaveOptionFile()
        {
            if (File.Exists(SaveFileStream.EternalOptionSaveFileName.PersistentDataPath()))
            {
                File.Delete(SaveFileStream.EternalOptionSaveFileName.PersistentDataPath());
            }
        }

        [MenuItem("File/Delete/All")]
        static void DeleteAllSaveFile()
        {
            DeleteTestSaveFile();
            DeleteSaveFile1();
            DeleteSaveFile2();
            DeleteSaveFile3();
            DeleteSaveOptionFile();
        }
    }
}