using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using DG.Tweening;

public class CookingManager : MonoSingleton<CookingManager>
{
    #region 테스트 코드 (Test Code)
    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }

    public ItemInfo testItemInfo;

    public Chef testChef;

    //Resources 폴더 속 경로
    [SerializeField] string foodDataPath = "System/FoodData/";
    [SerializeField] string ingredientDataPath = "System/IngredientData/";

    #endregion

    //private List<Food> allFoods;
    //private List<Ingredient> allIngredients;

    private Dictionary<int, Food> foodDic = new Dictionary<int, Food>();  
    private Dictionary<Food, FoodButton> foodBtnDic = new Dictionary<Food, FoodButton>();  
    private Dictionary<int, Ingredient> ingredientDic = new Dictionary<int, Ingredient>(); 

    private FoodButton selectedFoodBtn;  //음식 만들기 창에서 자신이 선택한 음식 버튼
    private List<IngredientImage> selectedFoodIngrImgs = new List<IngredientImage>(); //만들 음식 선택하고 보여지는 필요 재료 UI들

    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (음식 제작 창에서) 음식 버튼 리스트
    [SerializeField] private List<IngredientImage> ingredientImages = new List<IngredientImage>(); //(음식 제작 창에서) 재료 정보 UI들

    private int makeFoodCount; //만드려는(선택한) 음식 개수
    public int MakeFoodCount { get { return makeFoodCount; } }

    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public Image foodImg;  //만드려는(선택한) 음식 이미지
    public Text makeFoodCountText; //만드려는(선택한) 음식 개수 텍스트
    public Text foodNameText; //음식이름

    public CanvasGroup foodsPanel;
    public CanvasGroup makeFoodPanel; //선택한 음식 정보 패널

    public Button countPlusBtn, countMinusBtn; //음식 제작 개수 늘리기(줄이기) 버튼

    private void Awake()
    {
        SetData();
    }

    private void SetData()
    {
        List<Food> allFoods = new List<Food>(Resources.LoadAll<Food>(foodDataPath));
        
        for(int i=0; i< allFoods.Count; ++i)
        {
            foodDic.Add(allFoods[i].id, allFoods[i]);

            FoodButton fb = Instantiate(foodBtnPrefab, foodBtnParent).GetComponent<FoodButton>();
            fb.FoodData = allFoods[i];
            foodBtnList.Add(fb);
        }

        foodBtnList.ForEach(x =>
        {
            foodBtnDic.Add(x.FoodData, x);
        });

        foreach (Ingredient ing in Resources.LoadAll<Ingredient>(ingredientDataPath))
        {
            ingredientDic.Add(ing.id, ing);

            ingredientImages.Add(Instantiate(ingredientImgPrefab, ingredientImgParent).GetComponent<IngredientImage>());
        }

        countPlusBtn.onClick.AddListener(() => ChangeMakeFoodCount(true));
        countMinusBtn.onClick.AddListener(() => ChangeMakeFoodCount(false));
    }

    public Food GetFood(int id) => foodDic[id];

    public int GetItemCount(int id) //보유중인 해당 id의 아이템 개수 가져옴 
    {
        if (savedData.userItems.keyValueDic.ContainsKey(id))
            return savedData.userItems[id].count;
        return 0;
    }

    public void AddItem(ItemInfo itemInfo)
    {
        if (saveData.userItems.keyValueDic.ContainsKey(itemInfo.id))
            saveData.userItems[itemInfo.id].count += itemInfo.count;
        else
            saveData.userItems[itemInfo.id] = itemInfo;
    }

    public void RemoveItem(int id, int count)
    {
        if(saveData.userItems.keyValueDic.ContainsKey(id) && saveData.userItems[id].count >= count)
        {
            saveData.userItems[id].count -= count;
            if(saveData.userItems[id].count <= 0)
            {
                saveData.userItems.keyValueDic.Remove(id);
            }
        }
    }

    public void ShowFoodList(Chef currentChef) //대화한 요리사가 만들 수 있는 음식 리스트 표시
    {
        foodBtnList.ForEach(x => x.gameObject.SetActive(false));
        currentChef.CanFoodList.ForEach(x =>
        {
            foodBtnDic[x].gameObject.SetActive(true);
        });

        foodsPanel.gameObject.SetActive(true);
        CheckCannotMakeFoods();
        SortMakeFoods();
    }

    public void CheckCannotMakeFoods()  //재료 부족으로 못만드는 음식은 인터렉티브 false로
    {
        foodBtnList.FindAll(x=>x.gameObject.activeSelf).ForEach(x => x.button.interactable = x.CanMake());
    }

    private void SortMakeFoods()
    {
        int index = 0;
        for(int i=0; i<foodBtnList.Count; i++)
        {
            if(foodBtnList[i].gameObject.activeSelf && foodBtnList[i].button.interactable)
            {
                foodBtnList[i].transform.SetSiblingIndex(index);
                index++;
            }
        }
    }

    public void SelectFoodBtn(FoodButton foodBtn)  //음식 제작 창에서 만들 음식 선택
    {
        if (selectedFoodBtn == foodBtn) return;

        selectedFoodBtn = foodBtn;
        Food selectedFood = selectedFoodBtn.FoodData;

        makeFoodCount = 1;
        foodImg.sprite = selectedFood.GetSprite();
        makeFoodCountText.text = "1";
        foodNameText.text = selectedFood.itemName;

        ingredientImages.ForEach(x => x.gameObject.SetActive(false));
        selectedFoodIngrImgs.Clear();

        for (int i=0; i<foodBtn.FoodData.needIngredients.Count; ++i)
        {
            ingredientImages[i].gameObject.SetActive(true);
            ingredientImages[i].SetInfo(foodBtn.FoodData.needIngredients[i]);
            selectedFoodIngrImgs.Add(ingredientImages[i]);
        }

        CheckAmount();
        makeFoodPanel.gameObject.SetActive(true);
    }

    public void CheckAmount() //만들 음식의 개수를 늘리거나 줄일 수 있는지 확인해서 + - 버튼의 상호작용을 설정함
    {
        countMinusBtn.interactable = !(makeFoodCount == 1);
        countPlusBtn.interactable = CanCountPlus();
    }

    private bool CanCountPlus()  //만들 음식 개수 +1 가능?
    {
        foreach(IngredientImage ing in selectedFoodIngrImgs)
        {
            if (!ing.EnoughCount(GetItemCount(ing.IngredientInfo.ingredient.id), makeFoodCount + 1))
                return false;
        }

        return true;
    }

    public void ChangeMakeFoodCount(bool increase) //만들 음식의 개수를 줄이거나 늘림
    {
        makeFoodCount = makeFoodCount + (increase ? 1 : -1);
        UpdateMakeFoodCount();
    }

    public void UpdateMakeFoodCount()  //만들 음식 개수 변경 했으니 그에따른 UI 변경
    {
        makeFoodCountText.text = makeFoodCount.ToString();
        selectedFoodIngrImgs.ForEach(x => x.UpdateInfo());
        CheckAmount();
    }

    public void MakeFood()  //음식 제작
    {
        Food food = selectedFoodBtn.FoodData;
        AddItem(new ItemInfo(food.id, makeFoodCount, ItemType.CONSUME));
        selectedFoodIngrImgs.ForEach(x => RemoveItem(x.IngredientInfo.ingredient.id, x.IngredientInfo.needCount * makeFoodCount));
        MakeFoodInfoUIReset();
        CheckCannotMakeFoods();
        SortMakeFoods();
    }

    public void MakeFoodInfoUIReset()
    {
        makeFoodPanel.gameObject.SetActive(false);
        selectedFoodBtn = null;
    }

    private void Update()
    {
        Test(); //Test Code
    }

    void Test()  //Test Code
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            AddItem(testItemInfo);
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            ShowFoodList(testChef);
        }
        else if(Input.GetKeyDown(KeyCode.Z))
        {
            MakeFoodInfoUIReset();
            foodsPanel.gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            AddItem(new ItemInfo(10, 10, ItemType.ETC));
            AddItem(new ItemInfo(15, 10, ItemType.ETC));
            AddItem(new ItemInfo(20, 10, ItemType.ETC));
            AddItem(new ItemInfo(25, 10, ItemType.ETC));
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            foreach (ItemInfo item in saveData.userItems.keyValueDic.Values)
            {
                if(item.itemType == ItemType.CONSUME)
                   Debug.Log($"{GetFood(item.id).itemName} : {item.count}개");
            }
        }
    }
}
