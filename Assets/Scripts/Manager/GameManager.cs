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

    //private List<Triple<string, int, int>> limitedBattleCntItems = new List<Triple<string, int, int>>(); //n교전 후에 사라지는 아이템들 리스트 (아이디, 현재 교전 수, 최대 교전 수(가 되면 사라짐))

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

    //해야할 것 : 선택 스탯 자세히 보기창 위치 버그 수정, 퀵슬롯 표시

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
        
        {   //키세팅 정보 불러옴
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
        //saveData.userInfo = new UserInfo();  //UserInfo클래스의 저장 정보를 날림
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

        //풀 생성
        PoolManager.CreatePool(itemPrefab, transform, 6, "Item");
        PoolManager.CreatePool(itemCloneEffectPrefab, transform, 6, "ItemFollowEffect");
        PoolManager.CreatePool(emptyPrefab, transform, 3, "EmptyObject");
        PoolManager.CreatePool(Resources.Load<GameObject>("System/Etc/PlayerFollowEmptyObj"), transform, 2, "PlayerFollowEmptyObj");

        for(int i=0; i<poolBaseDatas.Length; i++)
            PoolManager.CreatePool(poolBaseDatas[i]);
        

        //이벤트 정의
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

    public int GetItemCount(string id) //보유중인 해당 id의 아이템 개수 가져옴 
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
            Debug.Log("버리려는 아이템 개수가 보유 중인 아이템 개수보다 많음");
        }
    }

    public bool UseItem(string id)
    {
        ItemSO data = GetItemData(id);
        if (data.itemType == ItemType.ETC && !((Ingredient)data).isUseable) return false;
        if (GetItemCount(id) <= 0)
        {
            Debug.Log("아이템 수가 0 이하인데 사용하려 함!! - ID : " + id);
            return false;
        }

        data.Use();
        Inventory.Instance.RemoveItem(id, 1, "아이템을 소모했습니다.");
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
        //드랍템들 흡수하고 list를 비우고 드랍템들 꺼줘야함. 이 때 만약 맵 이동하면 바로 list 비우고 드랍템들 꺼줌

        /*for(int i=0; i<droppedItemList.Count; i++)
        {
            Inventory.Instance.GetItem(droppedItemList[i]);
        }*/ //이렇게하면 전부 제대로 실행이 안된다.(계산하는 양이 많아서인지) 다른 방법을 써보자.
    }

    #region 주석
    /*public void UpdateItemBattleRestCount() //교전 수가 정해진 아이템들 현재 교전 수 1 증가하고 최대치인 것은 삭제
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
            //limitedBattleCntItems.Remove(item);  //Inventory스크립트에서 처리
            UIManager.Instance.RequestLogMsg("[최대 교전 수 도달]");
            Inventory.Instance.RemoveItem(item.first, 1);
            //UIManager.Instance.RequestLeftBottomMsg(string.Format("최대 교전 수 도달로 아이템을 잃었습니다. ({0} -{1})", GetItemData(item.first).itemName, 1));
        });
    }*/

    /*public void RemoveLimitedBattleCntItemsElement(string id)  //limitedBattleCntItems의 요소를 하나 제거
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
