using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using DG.Tweening;
using Water;

public class CookingManager : MonoSingleton<CookingManager>
{
    private List<Food> allFoodList;  //이 게임의 모든 음식 데이터들
    private Dictionary<int, Food> foodDic = new Dictionary<int, Food>();  //키: 음식 아이디, 값: 음식

    private Dictionary<Food, FoodButton> foodBtnDic = new Dictionary<Food, FoodButton>();

    private FoodButton selectedFoodBtn;  //음식 만들기 창에서 자신이 선택한 음식 버튼
    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (음식 제작 창에서) 음식 버튼 리스트

    private int makeFoodCount; //만드려는(선택한) 음식 개수

    public Image foodImg;  //만드려는(선택한) 음식 이미지
    public Text makeFoodCountText; //만드려는(선택한) 음식 개수 텍스트

    private void Awake()
    {
        SetData();
    }

    private void SetData()
    {
        allFoodList = new List<Food>(Resources.LoadAll<Food>("System/FoodData/"));
        
        for(int i=0; i< allFoodList.Count; ++i)
        {
            foodDic.Add(allFoodList[i].id, allFoodList[i]);
        }

        foodBtnList.ForEach(x =>
        {
            foodBtnDic.Add(x.FoodData, x);
        });
    }

    public Food GetFood(int id) => foodDic[id];

    public void ShowFoodList(Chef currentChef) //대화한 요리사가 만들 수 있는 음식 리스트 표시
    {
        foodBtnList.ForEach(x => x.gameObject.SetActive(false));
        currentChef.CanFoodList.ForEach(x =>
        {
            foodBtnDic[x].gameObject.SetActive(true);
        });
    }

    public void SelectFoodBtn(FoodButton foodBtn)  //음식 제작 창에서 만들 음식 선택
    {
        if (selectedFoodBtn == foodBtn) return;

        selectedFoodBtn = foodBtn;
        Food selectedFood = selectedFoodBtn.FoodData;

        makeFoodCount = 1;
        foodImg.sprite = selectedFood.GetSprite();
        makeFoodCountText.text = "1";
    }
}
