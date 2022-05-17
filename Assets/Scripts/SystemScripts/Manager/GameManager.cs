using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public partial class GameManager : MonoSingleton<GameManager>  
{
    public GameObject emptyPrefab;

    //[HideInInspector] public List<Pick> pickList = new List<Pick>();

#if UNITY_EDITOR
    [Space(10)]  //È®ÀÎ¿ë
    public CheckGameStringKeys checkGameStringKeys = new CheckGameStringKeys();

    public List<Pair<string,InteractionObj>> checkItrObjDic = new List<Pair<string, InteractionObj>>();
#endif

}