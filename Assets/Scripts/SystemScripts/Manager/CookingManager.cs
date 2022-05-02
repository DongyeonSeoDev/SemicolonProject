using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using TMPro;

public class CookingManager : MonoSingleton<CookingManager>
{
    private GameManager gm;

    private Dictionary<string, FoodButton> foodBtnDic = new Dictionary<string, FoodButton>();  

    private FoodButton selectedFoodBtn = null;  //���� ����� â���� �ڽ��� ������ ���� ��ư
    private List<IngredientImage> selectedFoodIngrImgs = new List<IngredientImage>(); //���� ���� �����ϰ� �������� �ʿ� ��� UI��

    [Space(15)]
    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (���� ���� â����) ���� ��ư ����Ʈ
    [SerializeField] private List<IngredientImage> ingredientImages = new List<IngredientImage>(); //(���� ���� â����) ��� ���� UI��

    public List<FoodButton> FoodBtnList { get => foodBtnList;  set => foodBtnList = value; }
    public List<IngredientImage> IngredientImages { get => ingredientImages; set => ingredientImages = value;  }


    private int makeFoodCount; //�������(������) ���� ����
    public int MakeFoodCount => makeFoodCount;

    public TextMeshProUGUI NpcNameTxt; 
    public Image foodImg;  //�������(������) ���� �̹���
    public Text makeFoodCountText; //�������(������) ���� ���� �ؽ�Ʈ
    public TextMeshProUGUI foodNameText; //�����̸�

    public Button countPlusBtn, countMinusBtn, makeBtn; //���� ���� ���� �ø���(���̱�) ��ư, ���� ��ư

    public Color disableBtnColor, disableTextColor; //�� ����� ���� ��ư ��/�ؽ�Ʈ ��

    #region �����̳� ��� �ڼ��� ����â
    public Image detailFoodImg;
    public Text detailNameTxt, explanationTxt, abilExplanationTxt;
    [HideInInspector] public string detailID = string.Empty; //���� �ڼ��� �������� ���� Ȥ�� ����� ���̵�
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

        Global.AddMonoAction(Global.TalkWithChef, x => ShowFoodList((Chef)x));

        Global.AddAction(Global.MakeFood, item =>
        {
            selectedFoodIngrImgs.ForEach(x => 
            {
                Inventory.Instance.RemoveItem(x.IngredientInfo.ingredient.id, x.IngredientInfo.needCount * makeFoodCount, "�������� �Ҹ��߽��ϴ�.");
            });
            MakeFoodInfoUIReset();
            CheckCannotMakeFoods();
            SortMakeFoods();
            UIManager.Instance.OnUIInteractSetActive(UIType.PRODUCTION_PANEL, false, true);  //���� ���� â ����
        });

        Global.AddAction(Global.JunkItem, x =>
        {
            if (Util.IsActiveGameUI(UIType.CHEF_FOODS_PANEL)) //�丮�� ��ȭ �г�(���� ���� ����Ʈ �г�)�� ���������� �����͸�&�������Ѵ�
            {
                Util.DelayFunc(() =>
                {
                    MakeFoodInfoUIReset();
                    CheckCannotMakeFoods();
                    SortMakeFoods();
                }, 0.5f, this, true);
            }
        });

        Global.AddAction("ItemUse", id =>
        {
            if (Util.IsActiveGameUI(UIType.CHEF_FOODS_PANEL))
            {
                MakeFoodInfoUIReset();
                CheckCannotMakeFoods();
                SortMakeFoods();
            }
            UIManager.Instance.OnUIInteractSetActive(UIType.PRODUCTION_PANEL, false, true);
        });
    }

    public void ShowFoodList(Chef currentChef) //��ȭ�� �丮�簡 ���� �� �ִ� ���� ����Ʈ ǥ��
    {
        TimeManager.TimePause();
        NpcNameTxt.SetText(currentChef.ObjName);
        foodBtnList.ForEach(x => x.gameObject.SetActive(false));
        currentChef.CanFoodList.ForEach(x =>
        {
            foodBtnDic[x.id].gameObject.SetActive(true);
        });

        UIManager.Instance.OnUIInteract(UIType.CHEF_FOODS_PANEL);
        CheckCannotMakeFoods();
        SortMakeFoods();
    }

    public void CheckCannotMakeFoods()  
    {
        //foodBtnList.FindAll(x=>x.gameObject.activeSelf).ForEach(x => x.button.interactable = x.CanMake());  //��� �������� ������� ������ ���ͷ�Ƽ�� false��
        foodBtnList.FindAll(x => x.gameObject.activeSelf).ForEach(x => 
        {
            bool canMake = x.CanMake();
            x.button.image.color = canMake ? Color.white : disableBtnColor;
            x.foodNameTmp.color = canMake ? Color.white : disableTextColor;
        });
    }

    private void SortMakeFoods()
    {
        int index = 0;
        for(int i=0; i<foodBtnList.Count; i++)
        {
            if(foodBtnList[i].gameObject.activeSelf && foodBtnList[i].IsEnoughLoot)
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

        makeBtn.interactable = selectedFoodBtn.IsEnoughLoot;
        makeBtn.GetComponent<UIScale>().transitionEnable = selectedFoodBtn.IsEnoughLoot;

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

    public void MakeFoodInfoUIReset()  //���� ���� ���� ���� ���� ���·� ��������
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
        if (string.IsNullOrEmpty(detailID))
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
        abilExplanationTxt.text = data.abilExplanation;
    }

   
}
