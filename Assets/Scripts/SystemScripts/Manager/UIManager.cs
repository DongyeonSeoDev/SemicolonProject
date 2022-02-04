using UnityEngine;
using System.Collections.Generic;

public partial class UIManager : MonoSingleton<UIManager>
{
    [HideInInspector] public List<Menu> gameMenuList = new List<Menu>();
}