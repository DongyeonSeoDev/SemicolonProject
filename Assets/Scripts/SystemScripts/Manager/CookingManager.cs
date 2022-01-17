using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using DG.Tweening;

public class CookingManager : MonoSingleton<CookingManager>
{
    #region �׽�Ʈ �ڵ� (Test Code)
    [Header("TEST")]

    public ItemInfo testItemInfo;

    public Chef testChef;


    #endregion

    //private List<Food> allFoods;
    //private List<Ingredient> allIngredients;

    //private Dictionary<int, Food> foodDic = new Dictionary<int, Food>(); 
    //private Dictionary<int, Ingredient> ingredientDic = new Dictionary<int, Ingredient>(); 

    private GameManager gm;

    private Dictionary<int, FoodButton> foodBtnDic = new Dictionary<int, FoodButton>();  

    private FoodButton selectedFoodBtn = null;  //���� ����� â���� �ڽ��� ������ ���� ��ư
    private List<IngredientImage> selectedFoodIngrImgs = new List<IngredientImage>(); //���� ���� �����ϰ� �������� �ʿ� ��� UI��

    [Space(15)]
    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (���� ���� â����) ���� ��ư ����Ʈ
    [SerializeField] private List<IngredientImage> ingredientImages = new List<IngredientImage>(); //(���� ���� â����) ��� ���� UI��

    public List<FoodButton> FoodBtnList { get { return foodBtnList; } set { foodBtnList = value; } }
    public List<IngredientImage> IngredientImages { get { return ingredientImages; } set { ingredientImages = value; } }


    private int makeFoodCount; //�������(������) ���� ����
    public int MakeFoodCount { get { return makeFoodCount; } }

    public Image foodImg;  //�������(������) ���� �̹���
    public Text makeFoodCountText; //�������(������) ���� ���� �ؽ�Ʈ
    public Text foodNameText; //�����̸�

    //public CanvasGroup foodsPanel;
    //public CanvasGroup makeFoodPanel; //������ ���� ���� �г�

    public Button countPlusBtn, countMinusBtn; //���� ���� ���� �ø���(���̱�) ��ư

    private void Start()
    {
        gm = GameManager.Instance;
        SetData();
    }

    private void SetData()
    {
        Global.MonoActionTrigger("SetFoodBtnList", this);
        Global.MonoActionTrigger("SetIngredientImgList", this);
        Global.RemoveKey("SetFoodBtnList");
        Global.RemoveKey("SetIngredientImgList");

        foodBtnList.ForEach(x =>
        {
            foodBtnDic.Add(x.FoodData.id, x);
        });

        countPlusBtn.onClick.AddListener(() => ChangeMakeFoodCount(true));
        countMinusBtn.onClick.AddListener(() => ChangeMakeFoodCount(false));
        Global.AddMonoAction(Global.TalkWithChef, x => ShowFoodList((Chef)x));
        Global.AddAction(Global.MakeFood, item =>
        {
            gm.AddItem(item as ItemInfo);
            selectedFoodIngrImgs.ForEach(x => gm.RemoveItem(x.IngredientInfo.ingredient.id, x.IngredientInfo.needCount * makeFoodCount));
            MakeFoodInfoUIReset();
            CheckCannotMakeFoods();
            SortMakeFoods();
            UIManager.Instance.OnUIInteract(UIType.PRODUCTION_PANEL);
        });
    }

    public void ShowFoodList(Chef currentChef) //��ȭ�� �丮�簡 ���� �� �ִ� ���� ����Ʈ ǥ��
    {
        foodBtnList.ForEach(x => x.gameObject.SetActive(false));
        currentChef.CanFoodList.ForEach(x =>
        {
            foodBtnDic[x.id].gameObject.SetActive(true);
        });

        UIManager.Instance.OnUIInteract(UIType.CHEF_FOODS_PANEL);
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

        bool first = !selectedFoodBtn;
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

        if(first)
           UIManager.Instance.OnUIInteract(UIType.PRODUCTION_PANEL);
    }

    public void CheckAmount() //���� ������ ������ �ø��ų� ���� �� �ִ��� Ȯ���ؼ� + - ��ư�� ��ȣ�ۿ��� ������
    {
        bool bMinus = !(makeFoodCount == 1);
        bool bPlus = CanCountPlus();
        countMinusBtn.interactable = bMinus;
        countPlusBtn.interactable = bPlus;
        countMinusBtn.GetComponent<UIScale>().transitionEnable = bMinus;
        countPlusBtn.GetComponent<UIScale>().transitionEnable = bPlus;
    }

    private bool CanCountPlus()  //���� ���� ���� +1 ����?
    {
        foreach(IngredientImage ing in selectedFoodIngrImgs)
        {
            if (!ing.EnoughCount(gm.GetItemCount(ing.IngredientInfo.ingredient.id), makeFoodCount + 1))
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
        Global.ActionTrigger(Global.MakeFood, new ItemInfo(selectedFoodBtn.FoodData.id, makeFoodCount));
    }

    public void MakeFoodInfoUIReset()
    {
        
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
            gm.AddItem(testItemInfo);
        }
        else if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Global.MonoActionTrigger(Global.TalkWithChef, testChef);
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            gm.AddItem(new ItemInfo(10, 10));
            gm.AddItem(new ItemInfo(15, 10));
            gm.AddItem(new ItemInfo(20, 10));
            gm.AddItem(new ItemInfo(25, 10));
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            foreach (ItemInfo item in gm.savedData.userInfo.userItems.keyValueDic.Values)
            {
                Debug.Log($"{gm.GetItemData(item.id).itemName} : {item.count}��");
            }
        }
    }
}
