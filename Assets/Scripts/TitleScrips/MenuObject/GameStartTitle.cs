using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameStartTitle : TitleMenuObject
{
    [SerializeField]
    private TitleDataController controller;
    [SerializeField]
    private GameObject loadingWindowObj = null;

    public Image progressBar = null;
    public TextMeshProUGUI progressText = null;

    public override void DoWork()
    {
        if(controller.loadedSlotNum >= controller.maxSlotNum)
        {
            Debug.Log("이미 저장 슬롯이 가득 찼습니다.");

            return;
        }

        Debug.Log("DoWork! Num : " + curTitleObjIdx);

        loadingWindowObj.SetActive(true);

        EventManager.TriggerEvent("StartNewGame"); //새 게임 버튼 누르고 씬 넘어가기 전 이벤트
        LoadSceneManager.Instance.LoadScene(progressBar, progressText, "StageScene");
    }

    private void OnDisable()
    {
        loadingWindowObj.SetActive(false);
    }
}
