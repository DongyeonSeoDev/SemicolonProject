using UnityEngine;
using UnityEngine.UI;
using Water;
using TMPro;

[DisallowMultipleComponent]
public class FoodButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    [SerializeField] private Food foodData;

    public Outline selecteLine;
    public Image foodImg;
    public TextMeshProUGUI foodNameTmp;

    private bool isEnoughLoot;
    public bool IsEnoughLoot { get { return isEnoughLoot; } }

    public Food FoodData 
    { 
        get { return foodData; } 
        set
        {
            foodData = value;
            foodImg.sprite = foodData.GetSprite();
            foodNameTmp.SetText(foodData.itemName);
        }
    }

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            CookingManager.Instance.SelectFoodBtn(this);
            selecteLine.enabled = true;
        });
    }

    //�� ������ �ʿ������ ������ �������� �ִ��� Ȯ���Ѵ�
    public bool CanMake()  //�� ������ ���� �� �ִ��� üũ
    {
        for(int i=0; i<foodData.needIngredients.Count; i++)
        {
            if(GameManager.Instance.GetItemCount(foodData.needIngredients[i].ingredient.id)< foodData.needIngredients[i].needCount)
            {
                //GetComponent<UIScale>().transitionEnable = false;
                isEnoughLoot = false;
                return false;
            }
        }
        //GetComponent<UIScale>().transitionEnable = true;
        isEnoughLoot = true;
        return true;
    }
}
