using UnityEngine;
using Water;

[CreateAssetMenu(fileName = "Ingredient Data", menuName = "Scriptable Object/Ingredient Data", order = int.MaxValue)]
public class Ingredient : ScriptableObject
{
    private Sprite ingredientSprite;

    public int id;
    public string ingredientName;
    //public string spritePath;  //스프라이트 경로

    public Sprite IngredientSprite
    {
        get
        {
            if (!ingredientSprite)
            {
                ingredientSprite = Resources.Load<Sprite>(Global.ingredientSpritePath+name);
            }
            return ingredientSprite;
        }
    }

}
