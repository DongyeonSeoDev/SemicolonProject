using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using Water;
using UnityEditor;
using FkTweening;

public partial class GameManager : MonoSingleton<GameManager>
{
    #region Save
    private string savedJson, filePath;
    private string cryptoText;
    private readonly string cryptoKey = "XHUooeUjJzMKdt";

    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }
    #endregion

    #region Game Data
    private Dictionary<string, ItemSO> itemDataDic = new Dictionary<string, ItemSO>();
    public Dictionary<string, ItemSO> ItemDataDic => itemDataDic;
    #endregion

    //private List<Triple<string, int, int>> limitedBattleCntItems = new List<Triple<string, int, int>>(); //n���� �Ŀ� ������� �����۵� ����Ʈ (���̵�, ���� ���� ��, �ִ� ���� ��(�� �Ǹ� �����))

    #region prefab and parent
    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public GameObject itemPrefab, itemCloneEffectPrefab;

    public PoolBaseData[] poolBaseDatas;
    #endregion

    #region Mini Game
    public PickupCheck pickupCheckGame;
    #endregion

    #region Dropped Items
    public List<Item> droppedItemList = new List<Item>();
    private Dictionary<string, int> droppedItemTempDict = new Dictionary<string, int>();
    #endregion

    #region Util
    public Transform slimeFollowObj { get; private set; }
    #endregion

    #region event and Action
    public event Action gameQuitEvent;
    public event Action UpdateEvent;
    private Dictionary<string, Action> updateActionDic = new Dictionary<string, Action>();
    #endregion

#if UNITY_EDITOR
    public Dictionary<KeyCode, Action> testKeyInputActionDict = new Dictionary<KeyCode, Action>();
#endif

    //�ؾ��� �� : ���� ���� �ڼ��� ����â ��ġ ���� ����, ������ ǥ��

    private void Awake()
    {
        filePath = Global.saveFileName_1.PersistentDataPath();
        saveData = new SaveData();   
        KeyCodeToString.Init();
        StateManager.Instance.Init();
        
        Load();
        Init();
    }

    #region Data

    public void SaveData()
    {
        MonsterCollection.Instance.Save();
        KeyActionManager.Instance.SaveKey();
        NGlobal.playerStatUI.Save();
        saveData.Save();
    }

    public void Save()
    {
        Debug.Log("Save Start");

        SaveData();

        savedJson = JsonUtility.ToJson(saveData);
        cryptoText = Crypto.Encrypt(savedJson, cryptoKey);
        //byte[] bytes = Encoding.UTF8.GetBytes(savedJson);
        //string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, cryptoText);
    }

    public void Load()
    {
        Debug.Log("Load Start");

        if (File.Exists(filePath))
        {
            string code = File.ReadAllText(filePath);
            savedJson = Crypto.Decrypt(code, cryptoKey);
            //byte[] bytes = Convert.FromBase64String(code);
            //savedJson = Encoding.UTF8.GetString(bytes);
            saveData = JsonUtility.FromJson<SaveData>(savedJson);
        }

        saveData.Load();
        //UtilEditor.PauseEditor();
        SetData();
    }

    private void SetData()
    {
        if (!saveData.tutorialInfo.isEnded)
        {
            saveData.ResetComplete();
        }
        
        {   //Ű���� ���� �ҷ���
            KeySetting.SetDefaultKeySetting();

            if (saveData.option.keyInputDict.keyValueDic.Keys.Count > 0)
            {
                foreach (KeyAction key in saveData.option.keyInputDict.keyList)
                {
                    KeySetting.keyDict[key] = saveData.option.keyInputDict[key];
                }
            }

            if (!saveData.tutorialInfo.isEnded)
            {
                saveData.userInfo.uiActiveDic = KeySetting.InitKeyActionActive;
            }
            else
            {
                saveData.ResetAfterTuto();
            }
        }

        //UtilEditor.PauseEditor();
    }

   
#endregion

    private void Init()
    {
        //saveData.userInfo = new UserInfo();  //UserInfoŬ������ ���� ������ ����
        //saveData.userInfo.userItems.ClearDic();
        //saveData.userInfo.monsterInfoDic.ClearDic();

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

        /*Global.AddAction("GetItem", _id =>
        {
            *//*string id = (string)_id;
            int limitedBattleCount = itemDataDic[id].existBattleCount;
            if (limitedBattleCount > 0)
            {
                limitedBattleCntItems.Add(new Triple<string, int, int>(id, 0, limitedBattleCount));
            }*//*
        });*/

        //Ǯ ����
        PoolManager.CreatePool(itemPrefab, transform, 6, "Item");
        PoolManager.CreatePool(itemCloneEffectPrefab, transform, 6, "ItemFollowEffect");
        PoolManager.CreatePool(emptyPrefab, transform, 3, "EmptyObject");
        PoolManager.CreatePool(Resources.Load<GameObject>("System/Etc/PlayerFollowEmptyObj"), transform, 2, "PlayerFollowEmptyObj");

        for(int i=0; i<poolBaseDatas.Length; i++)
            PoolManager.CreatePool(poolBaseDatas[i]);
        

        //�̺�Ʈ ����
        EventManager.StartListening("PlayerDead", PlayerDead);
        EventManager.StartListening("PlayerRespawn", PlayerRespawnEvent);
        //EventManager.StartListening("StageClear", UpdateItemBattleRestCount);

        EventManager.StartListening("StageClear", AbsorbCurMapAllItems);
        EventManager.StartListening("ExitCurrentMap", () => PoolManager.PoolObjSetActiveFalse("ItemFollowEffect"));
    }

    private void Start()
    {

#if UNITY_EDITOR
        Util.DelayFunc(() =>
        {
            checkGameStringKeys.poolKeyList = PoolManager.poolDic.Keys.ToList();
            Global.SetResordEventKey();
        }, 3f);

        testKeyInputActionDict.Add(KeyCode.F7, () =>
        {
            checkItrObjDic.Clear();
            foreach(string key in ObjectManager.Instance.itrObjDic.Keys)
            {
                checkItrObjDic.Add(new Pair<string, InteractionObj>(key, ObjectManager.Instance.itrObjDic[key]));
            }
        });

#endif

        slimeFollowObj = PoolManager.GetItem("EmptyObject").transform;
        slimeFollowObj.gameObject.AddComponent(typeof(SlimeFollowObj));
        slimeFollowObj.name = typeof(SlimeFollowObj).Name;
        StoredData.SetGameObjectKey("Slime Follow Obj", slimeFollowObj.gameObject);

        //WDUtil.PrintStructSize(typeof(StageFork));
    }

    public void AddUpdateAction(string key, Action action)
    {
        if (!updateActionDic.ContainsKey(key))
        {
            updateActionDic.Add(key, action);   
        }
        else
        {
            updateActionDic[key] += action;
        }
    }

    public void RemoveUpdateAction(string key, Action action)
    {
        if(updateActionDic.ContainsKey(key))
           updateActionDic[key] -= action;
    }

    public void RemoveUpdateAction(string key)
    {
        updateActionDic.Remove(key);
    }

    private void Update()
    {

        UpdateEvent?.Invoke();

        if(updateActionDic.Count > 0)
        {
            foreach(Action action in updateActionDic.Values)
            {
                action?.Invoke();
            }
        }

#if UNITY_EDITOR
        foreach (KeyCode key in testKeyInputActionDict.Keys)
        {
            if(Input.GetKeyDown(key))
            {
                testKeyInputActionDict[key].Invoke();
            }
        }
#endif


    }

    void PlayerDead()
    {
        PoolManager.PoolObjSetActiveFalse("ItemFollowEffect");
        //PoolManager.PoolObjSetActiveFalse("EmptyObject");
        StateManager.Instance.RemoveAllStateAbnormality(false);
        saveData.userInfo.playerStat.ResetAfterRegame();
    }

#region Item

    public ItemSO GetItemData(string id) => itemDataDic[id];
    public T GetItemData<T>(string id) where T : ItemSO => itemDataDic[id] as T;
    public bool ExistItem(string id) => itemDataDic.ContainsKey(id);

    public int GetItemCount(string id) //�������� �ش� id�� ������ ���� ������ 
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

    public void RemoveItem(string id, int count = 1)
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

    public bool UseItem(string id)
    {
        ItemSO data = GetItemData(id);
        if (data.itemType == ItemType.ETC && !((Ingredient)data).isUseable) return false;
        if (GetItemCount(id) <= 0)
        {
            Debug.Log("������ ���� 0 �����ε� ����Ϸ� ��!! - ID : " + id);
            return false;
        }

        data.Use();
        Inventory.Instance.RemoveItem(id, 1, "�������� �Ҹ��߽��ϴ�.");
        SoundManager.Instance.PlaySoundBox("UseItemSFX");
        Global.ActionTrigger("ItemUse", id);

        return true;
    }

    void PlayerRespawnEvent()
    {
        ResetDroppedItems();

        /*for(i=0; i<pickList.Count; i++)
        {
            if (!pickList[i].gameObject.activeSelf) pickList[i].gameObject.SetActive(true);
        }*/

        //limitedBattleCntItems.Clear();
    }

    public void ResetDroppedItems()
    {
        for (int i = 0; i < droppedItemList.Count; i++)
        {
            droppedItemList[i].gameObject.SetActive(false);
        }
        droppedItemList.Clear();
    }

    public void AbsorbCurMapAllItems()
    {
        
        for (int i = 0; i < droppedItemList.Count; i++)
        {
            if (droppedItemTempDict.ContainsKey(droppedItemList[i].itemData.id))
            {
                droppedItemTempDict[droppedItemList[i].itemData.id] += droppedItemList[i].DroppedCnt;
            }
            else
            {
                droppedItemTempDict.Add(droppedItemList[i].itemData.id, droppedItemList[i].DroppedCnt);
            }
            droppedItemList[i].FollowEffect();
            droppedItemList[i].gameObject.SetActive(false);
        }
        foreach (string key in droppedItemTempDict.Keys)
        {
            Inventory.Instance.GetItem(new ItemInfo(key, droppedItemTempDict[key]));
        }

        droppedItemTempDict.Clear();
        droppedItemList.Clear();
        //����۵� ����ϰ� list�� ���� ����۵� �������. �� �� ���� �� �̵��ϸ� �ٷ� list ���� ����۵� ����

        /*for(int i=0; i<droppedItemList.Count; i++)
        {
            Inventory.Instance.GetItem(droppedItemList[i]);
        }*/ //�̷����ϸ� ���� ����� ������ �ȵȴ�.(����ϴ� ���� ���Ƽ�����) �ٸ� ����� �Ẹ��.
    }

    #region �ּ�
    /*public void UpdateItemBattleRestCount() //���� ���� ������ �����۵� ���� ���� �� 1 �����ϰ� �ִ�ġ�� ���� ����
    {
        List<Triple<string, int, int>> sList = new List<Triple<string, int, int>>();

        limitedBattleCntItems.ForEach(item =>
        {
            item.second++;
            if (item.second == item.third)
                sList.Add(item);
        });

        sList.ForEach(item =>
        {
            //limitedBattleCntItems.Remove(item);  //Inventory��ũ��Ʈ���� ó��
            UIManager.Instance.RequestLogMsg("[�ִ� ���� �� ����]");
            Inventory.Instance.RemoveItem(item.first, 1);
            //UIManager.Instance.RequestLeftBottomMsg(string.Format("�ִ� ���� �� ���޷� �������� �Ҿ����ϴ�. ({0} -{1})", GetItemData(item.first).itemName, 1));
        });
    }*/

    /*public void RemoveLimitedBattleCntItemsElement(string id)  //limitedBattleCntItems�� ��Ҹ� �ϳ� ����
    {
        for(int i=0; i< limitedBattleCntItems.Count; ++i)
        {
            if(limitedBattleCntItems[i].first == id)
            {
                limitedBattleCntItems.RemoveAt(i);
                return;
            }
        }
    }*/
    #endregion

    #endregion

    public void RespawnPlayer()
    {
        UIManager.Instance.OnUIInteract(UIType.DEATH, true);
        UIManager.Instance.StartLoading(() => EventManager.TriggerEvent("PlayerRespawn"), () => EventManager.TriggerEvent("StartNextStage"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    #region OnApplication
    private void OnApplicationQuit()
    {
        Save();
        gameQuitEvent?.Invoke();
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
