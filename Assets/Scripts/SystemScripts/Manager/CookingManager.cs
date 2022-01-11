using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using DG.Tweening;

public class CookingManager : MonoSingleton<CookingManager>
{
    #region �׽�Ʈ �ڵ� (Test Code)
    [SerializeField] private SaveData saveData;
    public SaveData savedData { get { return saveData; } }

    public ItemInfo testItemInfo;

    public Chef testChef;

    //Resources ���� �� ���
    [SerializeField] string foodDataPath = "System/FoodData/";
    [SerializeField] string ingredientDataPath = "System/IngredientData/";

    #endregion

    //private List<Food> allFoods;
    //private List<Ingredient> allIngredients;

    private Dictionary<int, Food> foodDic = new Dictionary<int, Food>();  
    private Dictionary<Food, FoodButton> foodBtnDic = new Dictionary<Food, FoodButton>();  
    private Dictionary<int, Ingredient> ingredientDic = new Dictionary<int, Ingredient>(); 

    private FoodButton selectedFoodBtn;  //���� ����� â���� �ڽ��� ������ ���� ��ư
    private List<IngredientImage> selectedFoodIngrImgs = new List<IngredientImage>(); //���� ���� �����ϰ� �������� �ʿ� ��� UI��

    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (���� ���� â����) ���� ��ư ����Ʈ
    [SerializeField] private List<IngredientImage> ingredientImages = new List<IngredientImage>(); //(���� ���� â����) ��� ���� UI��

    private int makeFoodCount; //�������(������) ���� ����
    public int MakeFoodCount { get { return makeFoodCount; } }

    public GameObject foodBtnPrefab, ingredientImgPrefab;
    public Transform foodBtnParent, ingredientImgParent;

    public Image foodImg;  //�������(������) ���� �̹���
    public Text makeFoodCountText; //�������(������) ���� ���� �ؽ�Ʈ
    public Text foodNameText; //�����̸�

    public CanvasGroup foodsPanel;
    public CanvasGroup makeFoodPanel; //������ ���� ���� �г�

    public Button countPlusBtn, countMinusBtn; //���� ���� ���� �ø���(���̱�) ��ư

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

    public int GetItemCount(int id) //�������� �ش� id�� ������ ���� ������ 
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

    public void ShowFoodList(Chef currentChef) //��ȭ�� �丮�簡 ���� �� �ִ� ���� ����Ʈ ǥ��
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

    public void CheckCannotMakeFoods()  //��� �������� ������� ������ ���ͷ�Ƽ�� false��
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

    public void SelectFoodBtn(FoodButton foodBtn)  //���� ���� â���� ���� ���� ����
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

    public void CheckAmount() //���� ������ ������ �ø��ų� ���� �� �ִ��� Ȯ���ؼ� + - ��ư�� ��ȣ�ۿ��� ������
    {
        countMinusBtn.interactable = !(makeFoodCount == 1);
        countPlusBtn.interactable = CanCountPlus();
    }

    private bool CanCountPlus()  //���� ���� ���� +1 ����?
    {
        foreach(IngredientImage ing in selectedFoodIngrImgs)
        {
            if (!ing.EnoughCount(GetItemCount(ing.IngredientInfo.ingredient.id), makeFoodCount + 1))
                return false;
        }

        return true;
    }

    public void ChangeMakeFoodCount(bool increase) //���� ������ ������ ���̰ų� �ø�
    {
        makeFoodCount = makeFoodCount + (increase ? 1 : -1);
        UpdateMakeFoodCount();
    }

    public void UpdateMakeFoodCount()  //���� ���� ���� ���� ������ �׿����� UI ����
    {
        makeFoodCountText.text = makeFoodCount.ToString();
        selectedFoodIngrImgs.ForEach(x => x.UpdateInfo());
        CheckAmount();
    }

    public void MakeFood()  //���� ����
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
                   Debug.Log($"{GetFood(item.id).itemName} : {item.count}��");
            }
        }
    }
}
