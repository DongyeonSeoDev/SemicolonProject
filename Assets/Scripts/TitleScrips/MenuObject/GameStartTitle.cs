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
            Debug.Log("�̹� ���� ������ ���� á���ϴ�.");

            return;
        }

        Debug.Log("DoWork! Num : " + curTitleObjIdx);

        loadingWindowObj.SetActive(true);

        EventManager.TriggerEvent("StartNewGame"); //�� ���� ��ư ������ �� �Ѿ�� �� �̺�Ʈ
        LoadSceneManager.Instance.LoadScene(progressBar, progressText, "StageScene");
    }

    private void OnDisable()
    {
        loadingWindowObj.SetActive(false);
    }
}
