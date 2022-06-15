using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;

public class GameStart : TitleObject
{
    public override void DoWork()
    {
        Debug.Log("DoWork! Num : " + curTitleObjIdx);

        EditorSceneManager.LoadScene("StageScene");
    }
}
