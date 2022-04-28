using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using FkTweening;
using DG.Tweening;
using Water;

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
    public bool IsTutorialStage => (!(GameManager.Instance.savedData.tutorialInfo.isEnded || isTestMode));
    //Ʃ�丮�� �����Ű�µ� ������Ʈ���� ó���ؾ��� �����͵�
    private List<TutorialPhase> tutorialPhases = new List<TutorialPhase>();

    #region 1
    private Light2D playerFollowLight;  //�÷��̾� ����ٴϴ� ����Ʈ
    #endregion

    public Pair<GameObject, Transform> acqDropIconPair; //UIȹ���ϰ� ȹ�� ���� �� ������ ������Ʈ

    [Header("Test")]
    [SerializeField] private bool isTestMode = false;
    public bool IsTestMode => isTestMode;

    private void Awake()
    {
        PoolManager.CreatePool(acqDropIconPair.first, acqDropIconPair.second, 2, "AcqDropIcon");

        DefineAction();
    }

    private void DefineAction()
    {
        EventManager.StartListening("Tuto_GainArrowKey", () =>
        {
            playerFollowLight.DOInnerRadius(20f, 1.5f, false);
            playerFollowLight.DOOuterRadius(20f, 1.5f, false, () =>
            {
                playerFollowLight.gameObject.SetActive(false);
                Environment.Instance.mainLight.intensity = 1;
            });
        });  //�÷��̾ ����Ű �ϳ� ����� ���� �̺�Ʈ

        EventManager.StartListening("DrainTutorialEnemyDrain", enemyPos =>
        {
            //�� HP�� UI ���� �� �� ��ġ�� ������
            RectTransform teHpBar;
            Canvas ordCvs = UIManager.Instance.ordinaryCvsg.GetComponent<Canvas>();

            teHpBar = Instantiate(Resources.Load<GameObject>("Tutorial/UI/TutorialEnemyHP"), ordCvs.transform).GetComponent<RectTransform>();
            teHpBar.gameObject.SetActive(false);
            teHpBar.anchoredPosition = Util.WorldToScreenPosForScreenSpace(enemyPos, ordCvs);

            //Cursor Img
            Image cursorImg = teHpBar.GetChild(1).GetComponent<Image>();

            //Init
            ordCvs.GetComponent<CanvasGroup>().alpha = 1;  //�ƾ� ���ۻ��¶� alpha���� 0�� �����̹Ƿ� 1�� ����. 
            teHpBar.GetComponent<CanvasGroup>().alpha = 1;
            cursorImg.color = Color.clear;

            hpUI.GetComponent<CanvasGroup>().alpha = 0; //���� HP�� ������ �����̹Ƿ� alpha�� 0���� �ϰ� ���� ���� (��ƼŬ�� alpha 0�̾ ���̹Ƿ� ������ ���� ���� ����) 
            hpUI.gameObject.SetActive(true);

            changeableBodysUIArr[0].GetComponent<CanvasGroup>().alpha = 0;  //alpha�� 0���� �ϰ� ����
            changeableBodysUIArr[0].gameObject.SetActive(true);

            float hpFillEffectMaskCenterInitScale = StoredData.GetValueData<float>("hpFillEffectMaskCenterInitScale");
            StoredData.DeleteValueKey("hpFillEffectMaskCenterInitScale");

            UIManager.Instance.playerHPInfo.first.fillAmount = 0;
            UIManager.Instance.playerHPInfo.third.fillAmount = 0;
            EffectManager.Instance.hpFillEffectMaskCenter.localScale = new Vector3(0, hpFillEffectMaskCenterInitScale, hpFillEffectMaskCenterInitScale);

            Vector3 special2SkillSlotPos = skillUIArr[2].GetComponent<RectTransform>().anchoredPosition;
            skillUIArr[2].GetComponent<CanvasGroup>().alpha = 0;
            skillUIArr[2].gameObject.SetActive(true);
            skillUIArr[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(951, 740); //�̰� ��� ��ĥ��. anchor�� �޶� ��ġ�� ����� �ȳ���(anchor�� �߰��̾�� ������ �ڵ尡 �ߵ�) //Util.WorldToScreenPosForScreenSpace(enemyPos, ordCvs) - new Vector3(960f, -67f);

            //UtilEditor.PauseEditor();

            //HP�� tweening

            Util.DelayFunc(() =>  //������ ����ϰ� ���� ������� �ǵ��ƿ� ������ ���
            {
                teHpBar.gameObject.SetActive(true);
                EffectManager.Instance.hpFillEffect.gameObject.SetActive(true);

                Sequence seq = DOTween.Sequence();
                seq.Append(teHpBar.DOAnchorPos(Util.WorldToScreenPosForScreenSpace(Global.GetSlimePos.position + Vector3.down, ordCvs), 0.3f))
                .AppendInterval(0.7f); //������ ������ ü�¹� �ű�
                seq.Append(cursorImg.DOColor(Color.white, 0.5f))
                .AppendInterval(0.4f);  //���콺 Ŀ�� �̹��� ����
                seq.Append(teHpBar.DOAnchorPos(new Vector2(-819.3f, 474f), 0.9f).SetEase(Ease.InQuad))
                .AppendInterval(0.4f).AppendCallback(()=>cursorImg.transform.parent = ordCvs.transform);  //HP UI �ִ°����� �ű�
                seq.Append(teHpBar.GetComponent<CanvasGroup>().DOFade(0, 0.3f))
                .AppendInterval(0.15f); //�Ű��� UI �Ⱥ��̰�
                seq.Append(hpUI.GetComponent<CanvasGroup>().DOFade(1, 0.4f))
                .Join(cursorImg.DOColor(Color.clear, 0.3f))
                .Join(changeableBodysUIArr[0].GetComponent<CanvasGroup>().DOFade(1, 0.3f));
                seq.Append(skillUIArr[2].GetComponent<RectTransform>().DOAnchorPos(special2SkillSlotPos, 1f).SetEase(Ease.OutCubic))
                .Join(skillUIArr[2].GetComponent<CanvasGroup>().DOFade(1, 0.6f).SetEase(Ease.OutCubic))
                .AppendInterval(0.2f);   //���� HPUI�� ù��° ���� ���� ���̰� + ��� ��ų ���� ����
                seq.Append(UIManager.Instance.playerHPInfo.first.DOFillAmount(1, 0.7f))  //HP Fill ��������
                .Join(EffectManager.Instance.hpFillEffectMaskCenter.DOScaleX(hpFillEffectMaskCenterInitScale, 0.75f)) //����Ʈ ����ũ ���� (����Ʈ�� ���� ���̰�)
                .AppendCallback(() =>
                {
                    UIManager.Instance.playerHPInfo.third.fillAmount = 1;
                    Destroy(teHpBar.gameObject);
                    Destroy(cursorImg.gameObject);
                });
                seq.Play();
            }, 3f);
        });  //�÷��̾ Ʃ�丮��(HP�� ��� ��) ���� ���͸� ������� ���� �̺�Ʈ

        EventManager.StartListening("Tuto_CanDrainObject", () =>
        {
            TimeManager.LerpTime(1f, 0f, () =>
            {
                RectTransform emphRectTr = PoolManager.GetItem<RectTransform>("UIEmphasisEff");
                emphRectTr.transform.parent = skillUIArr[2].transform;
                emphRectTr.anchoredPosition = Vector3.zero;
                tutorialPhases.Add(new AbsorptionPhase(() => 
                {
                    emphRectTr.gameObject.SetActive(false);
                    TimeManager.TimeResume(() =>
                    {
                        Global.GetSlimePos.GetComponent<PlayerDrain>().DoDrainByTuto();
                        Destroy(Global.GetSlimePos.GetComponentInChildren<PlayerCanDrainCheckCollider>().gameObject);
                        EventManager.TriggerEvent("StartCutScene");

                        Util.DelayFunc(() =>
                        {
                            Canvas ordCvs = UIManager.Instance.ordinaryCvsg.GetComponent<Canvas>();
                            ordCvs.GetComponent<CanvasGroup>().alpha = 1;

                            Vector3 startPos = skillUIArr[0].GetComponent<RectTransform>().anchoredPosition;
                            skillUIArr[0].GetComponent<CanvasGroup>().alpha = 0;
                            skillUIArr[0].gameObject.SetActive(true);
                            skillUIArr[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(951, 740);

                            Sequence seq = DOTween.Sequence();
                            seq.Append(skillUIArr[0].GetComponent<RectTransform>().DOAnchorPos(startPos, 0.4f).SetEase(Ease.InQuad))
                            .Join(skillUIArr[0].GetComponent<CanvasGroup>().DOFade(1, 0.3f))
                            .AppendInterval(0.3f);

                        }, 1.5f, this);

                    }, 1f);
                }));
            });
        });
    }

    private void Start()
    {
        GameManager.Instance.testKeyInputActionDict.Add(KeyCode.B, () => Debug.Log(Time.timeScale));

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

            UIManager.Instance.RequestLogMsg("<b>[����Ư��]</b> ���콺�� Ű���带 ȹ���Ͽ����ϴ�(?)", 4f);
        }

        if (!gm.savedData.userInfo.uiActiveDic[KeyAction.SETTING])
        {
            tutorialPhases.Add(new SettingPhase(10, () =>
            {
                PoolManager.GetItem<AcquisitionDropIcon>("AcqDropIcon").Set(UIManager.Instance.GetInterfaceSprite(UIType.SETTING),
                Global.GetSlimePos.position - new Vector3(10f, 0), 3f, new Vector2(12, 12), () =>
                {
                    UIOn(KeyAction.SETTING);
                    gm.savedData.userInfo.uiActiveDic[KeyAction.QUIT] = true;
                });
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
