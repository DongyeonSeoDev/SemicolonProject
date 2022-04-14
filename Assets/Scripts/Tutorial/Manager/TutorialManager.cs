using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using Water;
using System.Text;
using System;
using FkTweening;

public class TutorialManager : MonoSingleton<TutorialManager> 
{
    //Manager Object
    private GameManager gm;
    private UIManager um;

    //처음에 안보일 UI들
    public Transform hpUI, energeBarUI;
    public Transform[] skillUIArr;

    //튜토리얼 진행중인가
    public bool IsTutorialStage => !GameManager.Instance.savedData.tutorialInfo.isEnded;
    //튜토리얼 진행시키는데 업데이트에서 처리해야할 데이터들
    private List<TutorialPhase> tutorialPhases = new List<TutorialPhase>();

    #region 1
    private Light2D playerFollowLight;  //플레이어 따라다니는 라이트
    #endregion

    [Header("Test")]
    [SerializeField] private bool isTestMode = false;
    public bool IsTestMode => isTestMode;

    private void Awake()
    {
        EventManager.StartListening("Tuto_GainAllArrowKey", () =>
        {
            playerFollowLight.DOInnerRadius(20f, 1.5f, false);
            playerFollowLight.DOOuterRadius(20f, 1.5f, false, () =>
            {
                playerFollowLight.gameObject.SetActive(false);
                Environment.Instance.mainLight.intensity = 1;
            });
        });
    }

    private void Start()
    {
        
        gm = GameManager.Instance;
        um = UIManager.Instance;

        bool active = gm.savedData.tutorialInfo.isEnded;

        if (isTestMode)  //Test
        {
            um.StartLoadingIn();
            return;
        }

        //Init Etc UI Active
        hpUI.gameObject.SetActive(active);
        energeBarUI.gameObject.SetActive(active);
        for (int i = 0; i < skillUIArr.Length; i++)
        {
            skillUIArr[i].gameObject.SetActive(active);
        }

        //PlayerFollowLight Init Setting
        playerFollowLight = PoolManager.GetItem<Light2D>("NormalPointLight2D");
        playerFollowLight.gameObject.SetActive(!active);
        playerFollowLight.gameObject.AddComponent<SlimeFollowObj>();
        playerFollowLight.color = Color.white;

        playerFollowLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").SetValue(playerFollowLight, new int[8]
        {
            0, -1221289887, 573196299, -992757899, -1418907605, -1646225281, -1158705011, 521365507
        });

        if (!active)
        {
            

            Environment.Instance.mainLight.intensity = 0;
            playerFollowLight.intensity = 1;
            playerFollowLight.pointLightInnerRadius = 0;
            playerFollowLight.pointLightOuterRadius = 0;

            tutorialPhases.Add(new StartPhase(playerFollowLight,2));
            EffectManager.Instance.OnTouchEffect("TouchEffect1");
        }

        if(!active || !gm.savedData.userInfo.uiActiveDic[UIType.SETTING])
        {
            tutorialPhases.Add(new SettingPhase(null, 10, () => UIOn(UIType.SETTING)));
        }

        um.StartLoadingIn();
    }

    private void ShowTargetSortingLayers() //Test
    {
        int[] arr = (int[])playerFollowLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").GetValue(playerFollowLight);

        StringBuilder sb = new StringBuilder();

        foreach (int r in arr)
            sb.Append(r + ", ");

        Debug.Log(sb.ToString());
    }

    public void UIOn(UIType type)
    {
        UIManager.Instance.PreventItrUI(0.4f);
        GameManager.Instance.savedData.userInfo.uiActiveDic[type] = true;
        UIManager.Instance.acqUIList.Find(x => x.uiType == type).GetComponent<AcquisitionUI>().OnUIVisible(true);
    }

    private void Update()
    {
        if(IsTutorialStage)
        {
            int i;
            for(i = 0; i < tutorialPhases.Count; i++)
            {
                tutorialPhases[i].DoPhaseUpdate();
            }
            for (i = 0; i < tutorialPhases.Count; i++)
            {
                if(tutorialPhases[i].IsEnded)
                {
                    tutorialPhases.RemoveAt(i);
                    i--;
                }
            }
        }

        
    }
}
