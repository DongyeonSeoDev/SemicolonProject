using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using System.Text;
using System.IO;
using Water;
using UnityEditor;

public partial class GameManager : MonoSingleton<GameManager>
{
    private string savedJson, filePath;
    private string cryptoText;
    private readonly string cryptoKey = "XHUooeUjJzMKdt";

    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }

    private Dictionary<string, ItemSO> itemDataDic = new Dictionary<string, ItemSO>();
    public Dictionary<string, ItemSO> ItemDataDic => itemDataDic;


    private List<Triple<string, int, int>> limitedBattleCntItems = new List<Triple<string, int, int>>(); //n교전 후에 사라지는 아이템들 리스트 (아이디, 현재 교전 수, 최대 교전 수(가 되면 사라짐))

    #region prefab and parent
    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public GameObject itemPrefab, itemCloneEffectPrefab;
    #endregion

    public PickupCheck pickupCheckGame;

    [HideInInspector] public List<Item> droppedItemList = new List<Item>();

    public int InventoryItemCount
    { get => savedData.userInfo.userItems.keyValueDic.Keys.Count; }

    public Transform slimeFollowObj { get; private set; }

    public event Action gameQuitEvent;

#if UNITY_EDITOR
    public Dictionary<KeyCode, Action> testKeyInputActionDict = new Dictionary<KeyCode, Action>();
#endif

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
        saveData.Save();
    }

    public void Save()
    {
        SaveData();

        savedJson = JsonUtility.ToJson(saveData);
        cryptoText = Crypto.Encrypt(savedJson, cryptoKey);
        //byte[] bytes = Encoding.UTF8.GetBytes(savedJson);
        //string code = Convert.ToBase64String(bytes);
        File.WriteAllText(filePath, cryptoText);
    }

    public void Load()
    {
        if (File.Exists(filePath))
        {
            string code = File.ReadAllText(filePath);
            savedJson = Crypto.Decrypt(code, cryptoKey);
            //byte[] bytes = Convert.FromBase64String(code);
            //savedJson = Encoding.UTF8.GetString(bytes);
            saveData = JsonUtility.FromJson<SaveData>(savedJson);
        }

        saveData.Load();
        SetData();
    }

    private void SetData()
    {
        if (!saveData.tutorialInfo.isEnded)
        {
            saveData = new SaveData();
        }
        
        {   //키세팅 정보 불러옴
            KeySetting.SetDefaultKeySetting();

            if (saveData.option.keyInputDict.keyList.Count > 0)
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
           
        }
        //슬라임에게 스탯 데이터 넣기
        //옵션 설정 내용 넣기 
        //아이템 정보 불러오기  --> Inventory 스크립트에서 처리
        //몬스터 동화율 정보 불러오기 --> MonsterCollection 스크립트에서 처리
    }

   /* private IEnumerator SetUIActiveDicFalseUI()
    {
        //따로 처리할 것들
        saveData.userInfo.uiActiveDic[UIType.QUIT] = false;

        while (UIManager.Instance == null) yield return null;

        for(int i=0; i<UIManager.Instance.acqUIList.Count; i++)
        {
            saveData.userInfo.uiActiveDic[UIManager.Instance.acqUIList[i].uiType] = false;
        }

        StoredData.SetObjectKey("SetUIAcqState", true);
    }*/

#endregion

    private void Init()
    {
        //saveData.userInfo = new UserInfo();  //UserInfo클래스의 저장 정보를 날림
        saveData.userInfo.userItems.ClearDic();
        saveData.userInfo.monsterInfoDic.ClearDic();

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
            string id = (string)_id;
            int limitedBattleCount = itemDataDic[id].existBattleCount;
            if (limitedBattleCount > 0)
            {
                limitedBattleCntItems.Add(new Triple<string, int, int>(id, 0, limitedBattleCount));
            }
        });

        //풀 생성
        PoolManager.CreatePool(itemPrefab, transform, 6, "Item");
        PoolManager.CreatePool(itemCloneEffectPrefab, transform, 6, "ItemFollowEffect");
        PoolManager.CreatePool(emptyPrefab, transform, 3, "EmptyObject");
        PoolManager.CreatePool(UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/SystemPrefabs/Etc/PlayerFollowEmptyObj.prefab"),transform, 2, "PlayerFollowEmptyObj");

        //이벤트 정의
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

        slimeFollowObj = PoolManager.GetItem("EmptyObject").transform;
        slimeFollowObj.gameObject.AddComponent(typeof(SlimeFollowObj));
        slimeFollowObj.name = typeof(SlimeFollowObj).Name;
        StoredData.SetGameObjectKey("Slime Follow Obj", slimeFollowObj.gameObject);

        //WDUtil.PrintStructSize(typeof(StageFork));
    }

    private void Update()
    {


#if UNITY_EDITOR
        foreach(KeyCode key in testKeyInputActionDict.Keys)
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
    }

#region Item

    public ItemSO GetItemData(string id) => itemDataDic[id];
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

    void PlayerRespawnEvent()
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

    public void UpdateItemBattleRestCount() //교전 수가 정해진 아이템들 현재 교전 수 1 증가하고 최대치인 것은 삭제
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
            UIManager.Instance.RequestLeftBottomMsg("[최대 교전 수 도달]");
            Inventory.Instance.RemoveItem(item.first, 1);
            //UIManager.Instance.RequestLeftBottomMsg(string.Format("최대 교전 수 도달로 아이템을 잃었습니다. ({0} -{1})", GetItemData(item.first).itemName, 1));
        });
    }

    public void RemoveLimitedBattleCntItemsElement(string id)  //limitedBattleCntItems의 요소를 하나 제거
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
