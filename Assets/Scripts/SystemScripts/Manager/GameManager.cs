using UnityEngine;
using System.Collections.Generic;

public partial class GameManager : MonoSingleton<GameManager>  
{
    [HideInInspector] public List<Pick> pickList = new List<Pick>();

    [Header("È®ÀÎ¿ë")]
    public CheckGameStringKeys checkGameStringKeys = new CheckGameStringKeys();


}