using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using TMPro;

public class CookingManager : MonoSingleton<CookingManager>
{
    #region 테스트 코드 (Test Code)
    [Header("TEST")]

    public ItemInfo testItemInfo;

    public Chef testChef;

    [SerializeField] private bool isTestMode = true;

    #endregion

    private GameManager gm;

    private Dictionary<int, FoodButton> foodBtnDic = new Dictionary<int, FoodButton>();  

    private FoodButton selectedFoodBtn = null;  //음식 만들기 창에서 자신이 선택한 음식 버튼
    private List<IngredientImage> selectedFoodIngrImgs = new List<IngredientImage>(); //만들 음식 선택하고 보여지는 필요 재료 UI들

    [Space(15)]
    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (음식 제작 창에서) 음식 버튼 리스트
    [SerializeField] private List<IngredientImage> ingredientImages = new List<IngredientImage>(); //(음식 제작 창에서) 재료 정보 UI들

    public List<FoodButton> FoodBtnList { get { return foodBtnList; } set { foodBtnList = value; } }
    public List<IngredientImage> IngredientImages { get { return ingredientImages; } set { ingredientImages = value; } }


    private int makeFoodCount; //만드려는(선택한) 음식 개수
    public int MakeFoodCount { get { return makeFoodCount; } }

    public TextMeshProUGUI NpcNameTxt; 
    public Image foodImg;  //만드려는(선택한) 음식 이미지
    public Text makeFoodCountText; //만드려는(선택한) 음식 개수 텍스트
    public TextMeshProUGUI foodNameText; //음식이름

    public Button countPlusBtn, countMinusBtn; //음식 제작 개수 늘리기(줄이기) 버튼

    #region 음식이나 재료 자세히 보기창
    public Image detailFoodImg;
    public Text detailNameTxt, explanationTxt;
    [HideInInspector] public int detailID = -1; //현재 자세히 보기중인 음식 혹은 재료의 아이디
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

    public void ShowFoodList(Chef currentChef) //대화한 요리사가 만들 수 있는 음식 리스트 표시
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

        bool first = !selectedFoodBtn;
        if (!first) selectedFoodBtn.selecteLine.enabled = false;
        
        selectedFoodBtn = foodBtn;
        Food selectedFood = selectedFoodBtn.FoodData;

        makeFoodCount = 1;
        foodImg.sprite = selectedFood.GetSprite();
        makeFoodCountText.text = "수량: 1개";
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

    public void CheckAmount() //만들 음식의 개수를 늘리거나 줄일 수 있는지 확인해서 + - 버튼의 상호작용을 설정함
    {
        bool bMinus = !(makeFoodCount == 1);
        bool bPlus = CanCountPlus();
        countMinusBtn.interactable = bMinus;
        countPlusBtn.interactable = bPlus;
        countMinusBtn.GetComponent<UIScale>().transitionEnable = bMinus;
        countPlusBtn.GetComponent<UIScale>().transitionEnable = bPlus;
    }

    private bool CanCountPlus()  //만들 음식 개수 +1 가능?
    {
        foreach(IngredientImage ing in selectedFoodIngrImgs)
        {
            if (!ing.EnoughCount(gm.GetItemCount(ing.IngredientInfo.ingredient.id), makeFoodCount + 1))
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
        makeFoodCountText.text = string.Format("수량: {0}개",makeFoodCount);
        selectedFoodIngrImgs.ForEach(x => x.UpdateInfo());
        CheckAmount();
    }

    public void MakeFood()  //음식 제작
    {
        if (Inventory.Instance.CanCombine(selectedFoodBtn.FoodData.id, makeFoodCount))
            Global.ActionTrigger(Global.MakeFood, new ItemInfo(selectedFoodBtn.FoodData.id, makeFoodCount));
        else UIManager.Instance.RequestSystemMsg("인벤토리에 전부 담을 수 없습니다.");
    }

    public void MakeFoodInfoUIReset()
    {
        if (selectedFoodBtn)
        {
            selectedFoodBtn.selecteLine.enabled = false;
            selectedFoodBtn = null;
        }
    }

    public void OnPointerFoodImage(bool on)  //음식 제작 --> 만들 음식의 이미지에 마우스 대거나 뗼때
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
