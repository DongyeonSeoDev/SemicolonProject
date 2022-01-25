using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using TMPro;

public class CookingManager : MonoSingleton<CookingManager>
{
    #region �׽�Ʈ �ڵ� (Test Code)
    [Header("TEST")]

    public ItemInfo testItemInfo;

    public Chef testChef;

    [SerializeField] private bool isTestMode = true;

    #endregion

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

    public TextMeshProUGUI NpcNameTxt; 
    public Image foodImg;  //�������(������) ���� �̹���
    public Text makeFoodCountText; //�������(������) ���� ���� �ؽ�Ʈ
    public TextMeshProUGUI foodNameText; //�����̸�

    public Button countPlusBtn, countMinusBtn; //���� ���� ���� �ø���(���̱�) ��ư

    #region �����̳� ��� �ڼ��� ����â
    public Image detailFoodImg;
    public Text detailNameTxt, explanationTxt;
    [HideInInspector] public int detailID = -1; //���� �ڼ��� �������� ���� Ȥ�� ����� ���̵�
    #endregion

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
        Global.AddMonoAction(Global.TalkWithChef, x => ((NPC)x).Interaction());
        Global.AddAction(Global.MakeFood, item =>
        {
            selectedFoodIngrImgs.ForEach(x => 
            {
                Inventory.Instance.RemoveItem(x.IngredientInfo.ingredient.id, x.IngredientInfo.needCount * makeFoodCount);
            });
            MakeFoodInfoUIReset();
            CheckCannotMakeFoods();
            SortMakeFoods();
            UIManager.Instance.OnUIInteract(UIType.PRODUCTION_PANEL);
        });
    }

    public void ShowFoodList(Chef currentChef) //��ȭ�� �丮�簡 ���� �� �ִ� ���� ����Ʈ ǥ��
    {
        NpcNameTxt.SetText(currentChef.NPCName);
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
        if (!first) selectedFoodBtn.selecteLine.enabled = false;
        
        selectedFoodBtn = foodBtn;
        Food selectedFood = selectedFoodBtn.FoodData;

        makeFoodCount = 1;
        foodImg.sprite = selectedFood.GetSprite();
        makeFoodCountText.text = "����: 1��";
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
        makeFoodCountText.text = string.Format("����: {0}��",makeFoodCount);
        selectedFoodIngrImgs.ForEach(x => x.UpdateInfo());
        CheckAmount();
    }

    public void MakeFood()  //���� ����
    {
        if (Inventory.Instance.CanCombine(selectedFoodBtn.FoodData.id, makeFoodCount))
            Global.ActionTrigger(Global.MakeFood, new ItemInfo(selectedFoodBtn.FoodData.id, makeFoodCount));
        else UIManager.Instance.RequestSystemMsg("�κ��丮�� ���� ���� �� �����ϴ�.");
    }

    public void MakeFoodInfoUIReset()
    {
        if (selectedFoodBtn)
        {
            selectedFoodBtn.selecteLine.enabled = false;
            selectedFoodBtn = null;
        }
    }

    public void OnPointerFoodImage(bool on)  //���� ���� --> ���� ������ �̹����� ���콺 ��ų� ����
    {
        if (on) UIManager.Instance.SetCursorInfoUI(selectedFoodBtn.FoodData.itemName);
        else UIManager.Instance.OffCursorInfoUI();
    }

    public void OnClickFoodImg()
    {
        Detail(selectedFoodBtn.FoodData);
    }

    public void OnClickIngrImg(Ingredient data)
    {
        Detail(data);
    }

    void Detail(ItemSO data)
    {
        if (detailID == -1)
        {
            UIManager.Instance.OnUIInteract(UIType.FOOD_DETAIL);
        }
        else if(data.id == detailID)
        {
            UIManager.Instance.OnUIInteract(UIType.FOOD_DETAIL);
            return;
        }

        detailID = data.id;

        detailFoodImg.sprite = data.GetSprite();
        detailNameTxt.text = data.itemName;
        explanationTxt.text = data.explanation;
    }

   /*private void Update()
    {
        Test(); //Test Code
    }*/

    void Test()  //Test Code
    {
        if (!isTestMode) return;

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
            for(int i=0; i<=40; i+=5)
            {
                Inventory.Instance.GetItem(new ItemInfo(i, 6));
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            UIManager.Instance.AllUIPositionReset();
        }
    }
}
