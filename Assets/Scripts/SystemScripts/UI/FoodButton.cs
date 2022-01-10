using UnityEngine;
using UnityEngine.UI;

public class FoodButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    [SerializeField] private Food foodData;

    public Food FoodData { get { return foodData; } set { foodData = value; } }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            CookingManager.Instance.SelectFoodBtn(this);
        });
    }

    public bool CanMake()
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
