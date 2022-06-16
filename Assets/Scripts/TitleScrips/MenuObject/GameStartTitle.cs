using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameStartTitle : TitleObject
{
    public Image progressBar = null;
    public override void DoWork()
    {
        Debug.Log("DoWork! Num : " + curTitleObjIdx);

        LoadSceneManager.Instance.LoadScene(progressBar, "StageScene");
    }
}
