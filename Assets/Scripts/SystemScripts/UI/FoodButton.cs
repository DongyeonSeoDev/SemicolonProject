using UnityEngine;
using UnityEngine.UI;

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
            foodNameText.text = foodData.foodName;
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
            if(CookingManager.Instance.GetItemCount(foodData.needIngredients[i].ingredient.id)< foodData.needIngredients[i].needCount)
            {
                return false;
            }
        }
        return true;
    }
}
