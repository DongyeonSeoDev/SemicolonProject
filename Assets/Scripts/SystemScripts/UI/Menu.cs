using UnityEngine;

public class Menu : MonoBehaviour
{
    /*[HideInInspector] public UIType menuType;

    private void Awake()
    {
        menuType = GetComponent<GameUI>()._UItype;
    }*/

    private void Start()
    {
        UIManager.Instance.gameMenuList.Add(this);
    }
}
