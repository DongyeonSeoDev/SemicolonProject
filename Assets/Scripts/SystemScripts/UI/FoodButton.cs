using UnityEngine;
using UnityEngine.UI;
using Water;
using TMPro;

[DisallowMultipleComponent]
public class FoodButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    [SerializeField] private Food foodData;

    public Image foodImg;
    public Text foodNameText;

    public Food FoodData 
    { 
        get { return foodData; } 
        set
        {
            foodData = value;
            foodImg.sprite = foodData.GetSprite();
            foodNameText.text = foodData.itemName;
        }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            CookingManager.Instance.SelectFoodBtn(this);
        });
    }

    //이 음식의 필요재료들과 개수가 유저한테 있는지 확인한다
    public bool CanMake()  //이 음식을 만들 수 있는지 체크
    {
        for(int i=0; i<foodData.needIngredients.Count; i++)
        {
            if(GameManager.Instance.GetItemCount(foodData.needIngredients[i].ingredient.id)< foodData.needIngredients[i].needCount)
            {
                GetComponent<UIScale>().transitionEnable = false;
                return false;
            }
        }
        GetComponent<UIScale>().transitionEnable = true;
        return true;
    }
}
