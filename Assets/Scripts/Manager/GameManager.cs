using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using Water;

public partial class GameManager : MonoSingleton<GameManager>
{
    private string savedJson, filePath;

    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }

    private Dictionary<int, ItemSO> itemDataDic = new Dictionary<int, ItemSO>();

    private List<Triple<int, int, int>> limitedBattleCntItems = new List<Triple<int, int, int>>(); //n���� �Ŀ� ������� �����۵� ����Ʈ (���̵�, ���� ���� ��, �ִ� ���� ��(�� �Ǹ� �����))

    #region prefab and parent
    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public GameObject itemPrefab, itemCloneEffectPrefab;
    #endregion

    [HideInInspector] public List<Item> droppedItemList = new List<Item>();

    public int InventoryItemCount
    { get => savedData.userInfo.userItems.keyValueDic.Keys.Count; }

    private void Awake()
    {
#if !UNITY_EDITOR
        Cursor.lockState = CursorLockMode.Confined;
#endif
        filePath = Global.saveFileName_1.PersistentDataPath();
        saveData = new SaveData();
        KeyCodeToString.Init();
        
        Load();
        Init();
    }

#region Data

    public void SaveData()
    {
        MonsterCollection.Instance.Save();
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
            KeySetting.SetDefaultKeySetting();

            if (saveData.option.keyInputDict.keyList.Count > 0)
            {
                foreach (KeyAction key in saveData.option.keyInputDict.keyList)
                {
                    KeySetting.keyDict[key] = saveData.option.keyInputDict[key];
                }
            }
        }
        //�����ӿ��� ���� ������ �ֱ�
        //�ɼ� ���� ���� �ֱ� 
        //������ ���� �ҷ�����  --> Inventory ��ũ��Ʈ���� ó��
        //���� ��ȭ�� ���� �ҷ����� --> MonsterCollection ��ũ��Ʈ���� ó��
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
            Global.ActionTrigger("GetItem", ((Item)item).itemData.id);
        });

        Global.AddAction("GetItem", _id =>
        {
            int id = (int)_id;
            int limitedBattleCount = itemDataDic[id].existBattleCount;
            if (limitedBattleCount > 0)
            {
                limitedBattleCntItems.Add(new Triple<int, int, int>(id, 0, limitedBattleCount));
            }
        });

        //Ǯ ����
        PoolManager.CreatePool(itemPrefab, transform, 6, "Item");
        PoolManager.CreatePool(itemCloneEffectPrefab, transform, 6, "ItemFollowEffect");
        PoolManager.CreatePool(emptyPrefab, transform, 3, "EmptyObject");

        //�̺�Ʈ ����
        EventManager.StartListening("PlayerDead", PlayerDead);
        EventManager.StartListening("PlayerRespawn", PlayerRespawnEvent);
        EventManager.StartListening("StageClear", UpdateItemBattleRestCount);
    }

    private void Start()
    {
        Util.DelayFunc(() =>
        {
            checkGameStringKeys.poolKeyList = PoolManager.poolDic.Keys.ToList();
            Global.SetResordEventKey();
        }, 3f);
    }

    void PlayerDead()
    {
        PoolManager.PoolObjSetActiveFalse("ItemFollowEffect");
        PoolManager.PoolObjSetActiveFalse("EmptyObject");
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

    void PlayerRespawnEvent(Vector2 unusedValue)
    {
        ResetDroppedItems();

        /*for(i=0; i<pickList.Count; i++)
        {
            if (!pickList[i].gameObject.activeSelf) pickList[i].gameObject.SetActive(true);
        }*/

        limitedBattleCntItems.Clear();
    }

    public void ResetDroppedItems()
    {
        for (int i = 0; i < droppedItemList.Count; i++)
        {
            droppedItemList[i].gameObject.SetActive(false);
        }
        droppedItemList.Clear();
    }

    public void UpdateItemBattleRestCount() //���� ���� ������ �����۵� ���� ���� �� 1 �����ϰ� �ִ�ġ�� ���� ����
    {
        List<Triple<int, int, int>> sList = new List<Triple<int, int, int>>();

        limitedBattleCntItems.ForEach(item =>
        {
            item.second++;
            if (item.second == item.third)
                sList.Add(item);
        });

        sList.ForEach(item =>
        {
            //limitedBattleCntItems.Remove(item);  //Inventory��ũ��Ʈ���� ó��
            UIManager.Instance.RequestLeftBottomMsg("[�ִ� ���� �� ����]");
            Inventory.Instance.RemoveItem(item.first, 1);
            //UIManager.Instance.RequestLeftBottomMsg(string.Format("�ִ� ���� �� ���޷� �������� �Ҿ����ϴ�. ({0} -{1})", GetItemData(item.first).itemName, 1));
        });
    }

    public void RemoveLimitedBattleCntItemsElement(int id)  //limitedBattleCntItems�� ��Ҹ� �ϳ� ����
    {
        for(int i=0; i< limitedBattleCntItems.Count; ++i)
        {
            if(limitedBattleCntItems[i].first == id)
            {
                limitedBattleCntItems.RemoveAt(i);
                return;
            }
        }
    }

#endregion

    public void RespawnPlayer()
    {
        //EventManager.TriggerEvent("PlayerRespawn", StageManager.Instance.respawnPos);   //���߿� �������� �Ǹ� �̰ɷ�
        EventManager.TriggerEvent("PlayerRespawn", Vector2.zero); //�ӽÿ�
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
