using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;

namespace Water
{
    public class GameManager : MonoSingleton<GameManager>  //저장/로드는 지금은 안함
    {
        private string savedJson, filePath;
        private readonly string saveFileName_1 = "SaveFile1";

        [SerializeField] private SaveData saveData;
        public SaveData savedData { get { return saveData; } }

        private Dictionary<int, ItemSO> itemDataDic = new Dictionary<int, ItemSO>();

        public GameObject foodBtnPrefab, ingredientImgPrefab;
        public Transform foodBtnParent, ingredientImgParent;

        public GameObject itemPrefab, itemCloneEffectPrefab;

        [HideInInspector] public List<Item> droppedItemList = new List<Item>();

        
        public int InventoryItemCount 
        { get { return savedData.userInfo.userItems.keyValueDic.Keys.Count; } }

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Confined;
            filePath = Util.GetFilePath(saveFileName_1);
            saveData = new SaveData();
            //Load();
            Init();
        }

        #region Data

        public void SaveData()
        {
            saveData.Save();
        }

        public void Save()
        {
            SaveData();

            savedJson = JsonUtility.ToJson(saveData);
            byte[] bytes = Encoding.UTF8.GetBytes(savedJson);
            string code = Convert.ToBase64String(bytes);
            File.WriteAllText(filePath, code);
        }

        public void Load()
        {
            if (File.Exists(filePath))
            {
                string code = File.ReadAllText(filePath);
                byte[] bytes = Convert.FromBase64String(code);
                savedJson = Encoding.UTF8.GetString(bytes);
                saveData = JsonUtility.FromJson<SaveData>(savedJson);
            }

            saveData.Load();
            SetData();
        }

        private void SetData()
        {
            //슬라임에게 스탯 데이터 넣기
            //옵션 설정 내용 넣기
            //아이템 정보 불러오기
        }

        #endregion

        private void Init()
        {
            List<Food> allFoods = new List<Food>(Resources.LoadAll<Food>(Global.foodDataPath));
            List<FoodButton> fbList = new List<FoodButton>();
            List<IngredientImage> igdImgList = new List<IngredientImage>();

            for (int i = 0; i < allFoods.Count; ++i)
            {
                itemDataDic.Add(allFoods[i].id, allFoods[i]);

                FoodButton fb = Instantiate(foodBtnPrefab, foodBtnParent).GetComponent<FoodButton>();
                fb.FoodData = allFoods[i];
                fbList.Add(fb);
            }
            Global.AddMonoAction("SetFoodBtnList", x => x.GetComponent<CookingManager>().FoodBtnList = fbList);
            
            foreach (Ingredient ing in Resources.LoadAll<Ingredient>(Global.ingredientDataPath))
            {
                itemDataDic.Add(ing.id, ing);
                igdImgList.Add(Instantiate(ingredientImgPrefab, ingredientImgParent).GetComponent<IngredientImage>());
            }
            Global.AddMonoAction("SetIngredientImgList", x => x.GetComponent<CookingManager>().IngredientImages = igdImgList);

            KeySetting.SetDefaultKeySetting();

            PoolManager.CreatePool(itemPrefab, transform, 6, "Item");
            PoolManager.CreatePool(itemCloneEffectPrefab, transform, 6, "ItemFollowEffect");

            SlimeEventManager.StartListening("PlayerRespawn", ResetDroppedItems);
        }

        #region Item

        public ItemSO GetItemData(int id) => itemDataDic[id];
        public bool ExistItem(int id) => itemDataDic.ContainsKey(id);

        public int GetItemCount(int id) //보유중인 해당 id의 아이템 개수 가져옴 
        {
            if (savedData.userInfo.userItems.keyValueDic.ContainsKey(id))
                return savedData.userInfo.userItems[id].count;
            return 0;
        }

        public void AddItem(ItemInfo itemInfo)
        {
            if (saveData.userInfo.userItems.keyValueDic.ContainsKey(itemInfo.id))
                saveData.userInfo.userItems[itemInfo.id].count += itemInfo.count;
            else
                saveData.userInfo.userItems[itemInfo.id] = itemInfo;
        }

        public void RemoveItem(int id, int count = 1)
        {
            if (saveData.userInfo.userItems.keyValueDic.ContainsKey(id) && saveData.userInfo.userItems[id].count >= count)
            {
                saveData.userInfo.userItems[id].count -= count;
                if (saveData.userInfo.userItems[id].count <= 0)
                {
                    saveData.userInfo.userItems.keyValueDic.Remove(id);
                }
            }
            else
            {
                Debug.Log("버리려는 아이템 개수가 보유 중인 아이템 개수보다 많음");
            }
        }

        void ResetDroppedItems(Vector2 unusedValue)
        {
            for(int i=0; i<droppedItemList.Count; i++)
            {
                droppedItemList[i].gameObject.SetActive(false);
            }
            droppedItemList.Clear();
        }

        #endregion

        public void RespawnPlayer()
        {
            SlimeEventManager.TriggerEvent("PlayerRespawn", new Vector2(0.3f, 4.55f)); //일단 임시로 일케함
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        

        #region OnApplication
        private void OnApplicationQuit()
        {
            //Save();
        }
        private void OnApplicationFocus(bool focus)
        {
            if(!focus)
            {
                //Save();
            }
        }
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                //Save();
            }
        }
        #endregion
    }
}