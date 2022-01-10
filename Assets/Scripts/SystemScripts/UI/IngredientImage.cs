using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IngredientImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text text;

    private IngredientCount ingredientInfo;
    public IngredientCount IngredientInfo { get { return ingredientInfo; } }

    public void SetInfo(IngredientCount ingr)  //만들 음식을 처음 눌렀을 때
    {
        ingredientInfo = ingr;
        image.sprite = ingr.ingredient.IngredientSprite;
        text.text = string.Concat(CookingManager.Instance.GetItemCount(ingr.ingredient.id),"/",ingr.needCount);        
    }

    public void UpdateInfo()  //만들 개수 +하거나 -할 때
    {
        text.text = string.Concat(CookingManager.Instance.GetItemCount(ingredientInfo.ingredient.id), "/", ingredientInfo.needCount * CookingManager.Instance.MakeFoodCount);
    }

    public bool EnoughCount(int userItemCount, int count) => userItemCount >= ingredientInfo.needCount * count;

    private void OnDisable()
    {
        ingredientInfo = null;
    }
}
