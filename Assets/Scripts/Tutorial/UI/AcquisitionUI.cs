using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcquisitionUI : MonoBehaviour
{
    public UIType uiType;
    [HideInInspector] public bool on = false;
    private CanvasGroup cvsg;

    private void Awake()
    {
        cvsg = GetComponent<CanvasGroup>();
        //OnUIVisible(false);
    }

    public void OnUIVisible(bool on)
    {
        this.on = on;
        cvsg.alpha = on ? 1 : 0;
        cvsg.interactable = on;
        cvsg.blocksRaycasts = on;
    }

    private void Start()
    {
        if(GameManager.Instance.savedData.tutorialInfo.isEnded || TutorialManager.Instance.IsTestMode)
        {
            UIActiveData.Instance.uiActiveDic[uiType] = true;
        }
        OnUIVisible(!TutorialManager.Instance.IsTutorialStage);
    }
}
