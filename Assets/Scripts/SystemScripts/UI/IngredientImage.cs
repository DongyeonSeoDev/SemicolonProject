using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Water;

[DisallowMultipleComponent]
public class IngredientImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text cntText, nameText;

    private IngredientCount ingredientInfo;
    public IngredientCount IngredientInfo { get { return ingredientInfo; } }

    public void SetInfo(IngredientCount ingr)  //���� ������ ó�� ������ ��
    {
        ingredientInfo = ingr;
        image.sprite = ingr.ingredient.GetSprite();
        cntText.text = string.Concat(GameManager.Instance.GetItemCount(ingr.ingredient.id),"/",ingr.needCount);
        nameText.text = ingr.ingredient.itemName;
    }

    public void UpdateInfo()  //���� ���� +�ϰų� -�� ��
    {
        cntText.text = string.Concat(GameManager.Instance.GetItemCount(ingredientInfo.ingredient.id), "/", ingredientInfo.needCount * CookingManager.Instance.MakeFoodCount);
    }

    //�� ��ᰡ ������� ������ count�� ����� ���ؼ� ����� �ִ��� Ȯ��
    public bool EnoughCount(int userItemCount, int count) => userItemCount >= ingredientInfo.needCount * count;

    private void OnDisable() //���� ���ص� �Ǳ���
    {
        ingredientInfo = null;
    }
}
