using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Water;

[DisallowMultipleComponent]
public class IngredientImage : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Text cntText;

    private NameInfoFollowingCursor nifc;

    private IngredientCount ingredientInfo;
    public IngredientCount IngredientInfo { get { return ingredientInfo; } }

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => CookingManager.Instance.OnClickIngrImg(ingredientInfo.ingredient));
        
    }

    public void SetInfo(IngredientCount ingr)  //���� ������ ó�� ������ ��
    {
        ingredientInfo = ingr;
        image.sprite = ingr.ingredient.GetSprite();
        cntText.text = string.Concat(GameManager.Instance.GetItemCount(ingr.ingredient.id),"/",ingr.needCount);
        //nameText.text = ingr.ingredient.itemName;

        if(!nifc) nifc = GetComponent<NameInfoFollowingCursor>();
        nifc.data = ingr.ingredient;
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
