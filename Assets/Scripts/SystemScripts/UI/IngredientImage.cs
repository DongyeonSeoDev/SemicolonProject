using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class IngredientImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text text;

    private IngredientCount ingredientInfo;
    public IngredientCount IngredientInfo { get { return ingredientInfo; } }

    public void SetInfo(IngredientCount ingr)  //���� ������ ó�� ������ ��
    {
        ingredientInfo = ingr;
        image.sprite = ingr.ingredient.IngredientSprite;
        text.text = string.Concat(CookingManager.Instance.GetItemCount(ingr.ingredient.id),"/",ingr.needCount);        
    }

    public void UpdateInfo()  //���� ���� +�ϰų� -�� ��
    {
        text.text = string.Concat(CookingManager.Instance.GetItemCount(ingredientInfo.ingredient.id), "/", ingredientInfo.needCount * CookingManager.Instance.MakeFoodCount);
    }

    public bool EnoughCount(int userItemCount, int count) => userItemCount >= ingredientInfo.needCount * count;

    private void OnDisable()
    {
        ingredientInfo = null;
    }
}
