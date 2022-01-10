using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using DG.Tweening;
using Water;

public class CookingManager : MonoSingleton<CookingManager>
{
    #region �׽�Ʈ �ڵ� (Test Code)
    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }

    public ItemInfo testItemInfo;

    #endregion

    //private List<Food> allFoodList;  //�� ������ ��� ���� �����͵�
    private Dictionary<int, Food> foodDic = new Dictionary<int, Food>();  //Ű: ���� ���̵�, ��: ����
    private Dictionary<Food, FoodButton> foodBtnDic = new Dictionary<Food, FoodButton>();
    private Dictionary<int, Ingredient> ingredientDic = new Dictionary<int, Ingredient>();

    private FoodButton selectedFoodBtn;  //���� ����� â���� �ڽ��� ������ ���� ��ư
    private List<IngredientImage> selectedFoodIngrImgs = new List<IngredientImage>();
    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (���� ���� â����) ���� ��ư ����Ʈ
    [SerializeField] private List<IngredientImage> ingredientImages = new List<IngredientImage>();

    private int makeFoodCount; //�������(������) ���� ����
    public int MakeFoodCount { get { return makeFoodCount; } }

    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public Image foodImg;  //�������(������) ���� �̹���
    public Text makeFoodCountText; //�������(������) ���� ���� �ؽ�Ʈ

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

    public void ShowFoodList(Chef currentChef) //��ȭ�� �丮�簡 ���� �� �ִ� ���� ����Ʈ ǥ��
    {
        foodBtnList.ForEach(x => x.gameObject.SetActive(false));
        currentChef.CanFoodList.ForEach(x =>
        {
            foodBtnDic[x].gameObject.SetActive(true);
        });

        CheckCannotMakeFoods();
    }

    public void CheckCannotMakeFoods()  //��� �������� ������� ������ ���ͷ�Ƽ�� false��
    {
        foodBtnList.FindAll(x=>x.gameObject.activeSelf).ForEach(x => x.button.interactable = x.CanMake());
    }

    public void SelectFoodBtn(FoodButton foodBtn)  //���� ���� â���� ���� ���� ����
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
