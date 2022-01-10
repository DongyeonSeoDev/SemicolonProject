using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using DG.Tweening;
using Water;

public class CookingManager : MonoSingleton<CookingManager>
{
    #region 테스트 코드 (Test Code)
    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }

    public ItemInfo testItemInfo;

    #endregion

    //private List<Food> allFoodList;  //이 게임의 모든 음식 데이터들
    private Dictionary<int, Food> foodDic = new Dictionary<int, Food>();  //키: 음식 아이디, 값: 음식
    private Dictionary<Food, FoodButton> foodBtnDic = new Dictionary<Food, FoodButton>();
    private Dictionary<int, Ingredient> ingredientDic = new Dictionary<int, Ingredient>();

    private FoodButton selectedFoodBtn;  //음식 만들기 창에서 자신이 선택한 음식 버튼
    private List<IngredientImage> selectedFoodIngrImgs = new List<IngredientImage>();
    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (음식 제작 창에서) 음식 버튼 리스트
    [SerializeField] private List<IngredientImage> ingredientImages = new List<IngredientImage>();

    private int makeFoodCount; //만드려는(선택한) 음식 개수
    public int MakeFoodCount { get { return makeFoodCount; } }

    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public Image foodImg;  //만드려는(선택한) 음식 이미지
    public Text makeFoodCountText; //만드려는(선택한) 음식 개수 텍스트

    public Button countPlusBtn, countMinusBtn;

    private void Awake()
    {
        SetData();
    }

    private void SetData()
    {
        List<Food> allFoodList = new List<Food>(Resources.LoadAll<Food>("System/FoodData/"));
        
        for(int i=0; i< allFoodList.Count; ++i)
        {
            foodDic.Add(allFoodList[i].id, allFoodList[i]);
            Instantiate(foodBtnPrefab, foodBtnParent).GetComponent<FoodButton>().FoodData = allFoodList[i];
        }

        foodBtnList.ForEach(x =>
        {
            foodBtnDic.Add(x.FoodData, x);
        });

        foreach(Ingredient ing in Resources.LoadAll<Ingredient>("System/IngredientData/"))
            ingredientDic.Add(ing.id, ing);

        Instantiate(ingredientImgPrefab, ingredientImgParent);
    }

    public Food GetFood(int id) => foodDic[id];

    public int GetItemCount(int id)
    {
        if (savedData.userItems.keyValueDic.ContainsKey(id))
            return savedData.userItems[id].count;
        return 0;
    }

    public void ShowFoodList(Chef currentChef) //대화한 요리사가 만들 수 있는 음식 리스트 표시
    {
        foodBtnList.ForEach(x => x.gameObject.SetActive(false));
        currentChef.CanFoodList.ForEach(x =>
        {
            foodBtnDic[x].gameObject.SetActive(true);
        });

        CheckCannotMakeFoods();
    }

    public void CheckCannotMakeFoods()  //재료 부족으로 못만드는 음식은 인터렉티브 false로
    {
        foodBtnList.FindAll(x=>x.gameObject.activeSelf).ForEach(x => x.button.interactable = x.CanMake());
    }

    public void SelectFoodBtn(FoodButton foodBtn)  //음식 제작 창에서 만들 음식 선택
    {
        if (selectedFoodBtn == foodBtn) return;

        selectedFoodBtn = foodBtn;
        Food selectedFood = selectedFoodBtn.FoodData;

        makeFoodCount = 1;
        foodImg.sprite = selectedFood.GetSprite();
        makeFoodCountText.text = "1";

        ingredientImages.ForEach(x => x.gameObject.SetActive(false));
        selectedFoodIngrImgs.Clear();

        for (int i=0; i<foodBtn.FoodData.needIngredients.Count; ++i)
        {
            ingredientImages[i].gameObject.SetActive(true);
            ingredientImages[i].SetInfo(foodBtn.FoodData.needIngredients[i]);
            selectedFoodIngrImgs.Add(ingredientImages[i]);
        }

        CheckAmount();
    }

    public void CheckAmount()
    {
        countMinusBtn.interactable = !(makeFoodCount == 1);
        countPlusBtn.interactable = CanCountPlus();
    }

    private bool CanCountPlus()
    {
        foreach(IngredientImage ing in selectedFoodIngrImgs)
        {
            if (!ing.EnoughCount(GetItemCount(ing.IngredientInfo.ingredient.id), makeFoodCount + 1))
                return false;
        }

        return true;
    }

    public void ChangeMakeFoodCount(bool increase)
    {
        makeFoodCount = makeFoodCount + (increase ? 1 : -1);
        UpdateMakeFoodCount();
    }

    public void UpdateMakeFoodCount()
    {
        makeFoodCountText.text = makeFoodCount.ToString();
        selectedFoodIngrImgs.ForEach(x => x.UpdateInfo());
    }

    private void Update()
    {
        Test(); //Test Code
    }

    void Test()  //Test Code
    {
        if(Input.GetKeyDown(KeyCode.LeftAlt))
        {
            saveData.userItems[testItemInfo.id] = testItemInfo;
        }
    }
}
