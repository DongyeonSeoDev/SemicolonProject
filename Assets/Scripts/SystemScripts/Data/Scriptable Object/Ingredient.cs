using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient Data", menuName = "Scriptable Object/Ingredient Data", order = int.MaxValue)]
public class Ingredient : ScriptableObject
{
    public int id;
    public string ingredientName;
    public string spritePath;
}
