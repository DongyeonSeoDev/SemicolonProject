using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using Water;

public partial class GameManager : MonoSingleton<GameManager>
{
    private string savedJson, filePath;
    private readonly string saveFileName_1 = "SaveFile1";

    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }

    private Dictionary<int, ItemSO> itemDataDic = new Dictionary<int, ItemSO>();

    #region prefab and parent
    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public GameObject itemPrefab, itemCloneEffectPrefab;
    #endregion

    [HideInInspector] public List<Item> droppedItemList = new List<Item>();


    public int InventoryItemCount
    { get { return savedData.userInfo.userItems.keyValueDic.Keys.Count; } }

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        filePath = Util.GetFilePath(saveFileName_1);
        saveData = new SaveData();
        Load();
        Init();
    }

    #region Data

    public void SaveData()
    {
        KeyActionManager.Instance.SaveKey();
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
        {   //Ű���� ���� �ҷ���
            if (saveData.option.keyInputDict.keyList.Count == 0)
                KeySetting.SetDefaultKeySetting();
            else
            {
                foreach (KeyAction key in saveData.option.keyInputDict.keyList)
                {
                    KeySetting.keyDict[key] = saveData.option.keyInputDict[key];
                }
            }
        }
        //�����ӿ��� ���� ������ �ֱ�
        //�ɼ� ���� ���� �ֱ�
        //������ ���� �ҷ�����
    }

    #endregion

    private void Init()
    {
        saveData.userInfo = new UserInfo();

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

        Global.AddMonoAction(Global.AcquisitionItem, item =>
        {
            item.gameObject.SetActive(false);
            droppedItemList.Remove((Item)item);
        });

        //Ǯ ����
        PoolManager.CreatePool(itemPrefab, transform, 6, "Item");
        PoolManager.CreatePool(itemCloneEffectPrefab, transform, 6, "ItemFollowEffect");

        //�̺�Ʈ ����
        EventManager.StartListening("PlayerRespawn", ResetDroppedItems);
    }

    #region Item

    public ItemSO GetItemData(int id) => itemDataDic[id];
    public bool ExistItem(int id) => itemDataDic.ContainsKey(id);

    public int GetItemCount(int id) //�������� �ش� id�� ������ ���� ������ 
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
            Debug.Log("�������� ������ ������ ���� ���� ������ �������� ����");
        }
    }

    void ResetDroppedItems(Vector2 unusedValue)
    {
        for (int i = 0; i < droppedItemList.Count; i++)
        {
            droppedItemList[i].gameObject.SetActive(false);
        }
        droppedItemList.Clear();
    }

    #endregion

    public void RespawnPlayer()
    {
        EventManager.TriggerEvent("PlayerRespawn", new Vector2(0.3f, 4.55f)); //�ϴ� �ӽ÷� ������
    }

    public void QuitGame()
    {
        Application.Quit();
    }



    #region OnApplication
    private void OnApplicationQuit()
    {
        Save();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            Save();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            Save();
        }
    }
    #endregion
}
