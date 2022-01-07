using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;
using UnityEngine.UI;
using DG.Tweening;
using Water;

public class CookingManager : MonoSingleton<CookingManager>
{
    private List<Food> allFoodList;  //�� ������ ��� ���� �����͵�
    private Dictionary<int, Food> foodDic = new Dictionary<int, Food>();  //Ű: ���� ���̵�, ��: ����

    private Dictionary<Food, FoodButton> foodBtnDic = new Dictionary<Food, FoodButton>();

    private FoodButton selectedFoodBtn;  //���� ����� â���� �ڽ��� ������ ���� ��ư
    [SerializeField] private List<FoodButton> foodBtnList = new List<FoodButton>(); // (���� ���� â����) ���� ��ư ����Ʈ

    private int makeFoodCount; //�������(������) ���� ����

    public Image foodImg;  //�������(������) ���� �̹���
    public Text makeFoodCountText; //�������(������) ���� ���� �ؽ�Ʈ

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

    public void ShowFoodList(Chef currentChef) //��ȭ�� �丮�簡 ���� �� �ִ� ���� ����Ʈ ǥ��
    {
        foodBtnList.ForEach(x => x.gameObject.SetActive(false));
        currentChef.CanFoodList.ForEach(x =>
        {
            foodBtnDic[x].gameObject.SetActive(true);
        });
    }

    public void SelectFoodBtn(FoodButton foodBtn)  //���� ���� â���� ���� ���� ����
    {
        if (selectedFoodBtn == foodBtn) return;

        selectedFoodBtn = foodBtn;
        Food selectedFood = selectedFoodBtn.FoodData;

        makeFoodCount = 1;
        foodImg.sprite = selectedFood.GetSprite();
        makeFoodCountText.text = "1";
    }
}
