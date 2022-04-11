using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoSingleton<TutorialManager> 
{
    private GameManager gm;
    private UIManager um;

    public Transform hpUI, energeBarUI;
    public Transform[] skillUIArr;

    public bool IsTutorialStage { get; set; }

    private void Awake()
    {
        
    }

    private void Start()
    {
        gm = GameManager.Instance;
        um = UIManager.Instance;

        bool active = gm.savedData.tutorialInfo.isEnded;
        hpUI.gameObject.SetActive(active);
        energeBarUI.gameObject.SetActive(active);
        for (int i = 0; i < skillUIArr.Length; i++)
        {
            skillUIArr[i].gameObject.SetActive(active);
        }

        if (!gm.savedData.tutorialInfo.isEnded)
        {
            IsTutorialStage = true;

            
        }
    }

    public void UIOn(UIType type)
    {
        UIActiveData.Instance.uiActiveDic[type] = true;
        UIManager.Instance.acqUIList.Find(x => x.uiType == type).GetComponent<AcquisitionUI>().OnUIVisible(true);
    }
}
