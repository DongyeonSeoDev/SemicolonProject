using UnityEngine;
using UnityEngine.UI;

public class FoodButton : MonoBehaviour
{
    [HideInInspector] public Button button;
    [SerializeField] private Food foodData;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
        {
            CookingManager.Instance.SelectFoodBtn(this);
        });
    }
}
