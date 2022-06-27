using UnityEngine;
using System.Collections.Generic;
using Water;
using System;

public partial class GameManager : MonoSingleton<GameManager>  
{
#if UNITY_EDITOR
    [Space(10)]  //È®ÀÎ¿ë
    public CheckGameStringKeys checkGameStringKeys = new CheckGameStringKeys();

    public List<Pair<string,InteractionObj>> checkItrObjDic = new List<Pair<string, InteractionObj>>();

    public Dictionary<KeyCode, Action> testKeyInputActionDict = new Dictionary<KeyCode, Action>();
#endif
}