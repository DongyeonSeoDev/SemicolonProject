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

    //�� ������ �ʿ������ ������ �������� �ִ��� Ȯ���Ѵ�
    public bool CanMake()  //�� ������ ���� �� �ִ��� üũ
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
