using UnityEngine;
using System.Collections.Generic;

public partial class GameManager : MonoSingleton<GameManager>  
{
    public GameObject emptyPrefab;

    //[HideInInspector] public List<Pick> pickList = new List<Pick>();

    [Space(10)]  //Ȯ�ο�
    [HideInInspector] public CheckGameStringKeys checkGameStringKeys = new CheckGameStringKeys();


}