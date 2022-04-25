using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using FkTweening;

public class TutorialManager : MonoSingleton<TutorialManager> 
{
    //Manager Object
    private GameManager gm;
    private UIManager um;

    //ó���� �Ⱥ��� UI�� (KeyAction���� ó������ ���� �͵�)
    public Transform hpUI, energeBarUI;
    public Transform[] skillUIArr;
    public Transform[] changeableBodysUIArr;

    //Ʃ�丮�� �������ΰ�
    public bool IsTutorialStage => !GameManager.Instance.savedData.tutorialInfo.isEnded;
    //Ʃ�丮�� �����Ű�µ� ������Ʈ���� ó���ؾ��� �����͵�
    private List<TutorialPhase> tutorialPhases = new List<TutorialPhase>();

    #region 1
    private Light2D playerFollowLight;  //�÷��̾� ����ٴϴ� ����Ʈ
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

        EventManager.StartListening("DrainTutorialEnemyDrain", enemyPos =>
        {
            RectTransform teHpBar;
            if(StoredData.HasGameObjectKey("Tuto EnemyHp UI"))
            {
                teHpBar = StoredData.GetGameObjectData<RectTransform>("Tuto EnemyHp UI");
            }
            else
            {
                teHpBar = Instantiate(Resources.Load<GameObject>("Tutorial/UI/TutorialEnemyHP"), Util.WorldCvs.transform).GetComponent<RectTransform>();
            }

            teHpBar.gameObject.SetActive(true);
            teHpBar.anchoredPosition = Util.ScreenToWorldPosForScreenSpace(enemyPos, Util.WorldCvs);
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
            gm.savedData.userInfo.uiActiveDic[KeyAction.SETTING] = true;
            gm.savedData.userInfo.uiActiveDic[KeyAction.INVENTORY] = true;
            gm.savedData.userInfo.uiActiveDic[KeyAction.STAT] = true;
            gm.savedData.userInfo.uiActiveDic[KeyAction.MONSTER_COLLECTION] = true;
            gm.savedData.userInfo.uiActiveDic[KeyAction.QUIT] = true;
            return;
        }

        //Init Etc UI Active
        hpUI.gameObject.SetActive(active);
        energeBarUI.gameObject.SetActive(active);
        
        for (int i = 0; i < skillUIArr.Length; i++)
        {
            skillUIArr[i].gameObject.SetActive(active);
            changeableBodysUIArr[i].gameObject.SetActive(active);
        }

        //PlayerFollowLight Init Setting
        playerFollowLight = StoredData.GetGameObjectData<Light2D>("Player Follow Light");
        playerFollowLight.gameObject.SetActive(!active);
      
        /*playerFollowLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").SetValue(playerFollowLight, new int[8]
        {
            0, -1221289887, 573196299, -992757899, -1418907605, -1646225281, -1158705011, 521365507
        });*/

        if (!active)
        {
            Environment.Instance.mainLight.intensity = 0;
            playerFollowLight.intensity = 1;
            playerFollowLight.pointLightInnerRadius = 0;
            playerFollowLight.pointLightOuterRadius = 0;

            tutorialPhases.Add(new StartPhase(playerFollowLight,2));
            EffectManager.Instance.OnTouchEffect("TouchEffect1");
        }

        if(!gm.savedData.userInfo.uiActiveDic[KeyAction.SETTING])
        {
            tutorialPhases.Add(new SettingPhase(10, () =>
            {
                UIOn(KeyAction.SETTING);
                gm.savedData.userInfo.uiActiveDic[KeyAction.QUIT] = true;
            }));
        }

        um.StartLoadingIn();
    }

    /*private void ShowTargetSortingLayers() //Test
    {
        int[] arr = (int[])playerFollowLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").GetValue(playerFollowLight);

        StringBuilder sb = new StringBuilder();

        foreach (int r in arr)
            sb.Append(r + ", ");

        Debug.Log(sb.ToString());
    }*/

    public void UIOn(KeyAction type)
    {
        um.PreventItrUI(0.4f);
        gm.savedData.userInfo.uiActiveDic[type] = true;
        um.acqUIList.Find(x => x.keyType == type).OnUIVisible(true);
    }

    private void Update()
    {
        TutorialUpdate();
    }


    private void TutorialUpdate()
    {
        int i;
        for (i = 0; i < tutorialPhases.Count; i++)
        {
            tutorialPhases[i].DoPhaseUpdate();
        }
        for (i = 0; i < tutorialPhases.Count; i++)
        {
            if (tutorialPhases[i].IsEnded)
            {
                tutorialPhases.RemoveAt(i);
                i--;
            }
        }
    }

    public void ActiveMonsterSlot(int num)
    {
        changeableBodysUIArr[num].gameObject.SetActive(true);  
    }

    public void ActiveSkillUI(int num)
    {
        skillUIArr[num].gameObject.SetActive(true); 
    }
}
