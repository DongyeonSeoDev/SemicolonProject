using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

    public void SetInfo(IngredientCount ingr)  //만들 음식을 처음 눌렀을 때
    {
        ingredientInfo = ingr;
        image.sprite = ingr.ingredient.GetSprite();

        int count = GameManager.Instance.GetItemCount(ingr.ingredient.id);
        if (count >= ingr.needCount)
            cntText.text = string.Concat(count, "/", ingr.needCount);
        else
            cntText.text = string.Concat("<color=red>", count, "</color>/", ingr.needCount);

        //nameText.text = ingr.ingredient.itemName;
        if(!nifc) nifc = GetComponent<NameInfoFollowingCursor>();
        nifc.explanation = ingr.ingredient.itemName;
    }

    public void UpdateInfo()  //만들 개수 +하거나 -할 때
    {
        cntText.text = string.Concat(GameManager.Instance.GetItemCount(ingredientInfo.ingredient.id), "/", ingredientInfo.needCount * CookingManager.Instance.MakeFoodCount);
    }

    //이 재료가 만들려는 음식을 count개 만들기 위해서 충분히 있는지 확인
    public bool EnoughCount(int userItemCount, int count) => userItemCount >= ingredientInfo.needCount * count;

    private void OnDisable() //굳이 안해도 되긴함
    {
        ingredientInfo = null;
    }
}
