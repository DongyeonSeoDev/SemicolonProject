using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameStartTitle : TitleObject
{
    [SerializeField]
    private GameObject loadingWindowObj = null;

    public Image progressBar = null;
    public TextMeshProUGUI progressText = null;

    public override void DoWork()
    {
        Debug.Log("DoWork! Num : " + curTitleObjIdx);

        loadingWindowObj.SetActive(true);

        LoadSceneManager.Instance.LoadScene(progressBar, progressText, "StageScene");
    }

    private void OnDisable()
    {
        loadingWindowObj.SetActive(false);
    }
}
