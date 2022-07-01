using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
using FkTweening;
using DG.Tweening;
using Water;
using System;

public class TutorialManager : MonoSingleton<TutorialManager> 
{
    //Manager Object
    private GameManager gm;
    private UIManager um;

    //ó���� �Ⱥ��� UI�� (KeyAction���� ó������ ���� �͵�)
    public Transform hpUI, energeBarUI;
    public Transform quikSlotUI;
    public Transform[] skillUIArr;
    public Transform[] changeableBodysUIArr;

    //Ʃ�丮�� �������ΰ�
    public bool IsTutorialStage => (!(GameManager.Instance.savedData.tutorialInfo.isEnded || isTestMode));
    //Ʃ�丮�� �����Ű�µ� ������Ʈ���� ó���ؾ��� �����͵�
    private List<TutorialPhase> tutorialPhases = new List<TutorialPhase>();

    #region 1
    private Light2D playerFollowLight;  //�÷��̾� ����ٴϴ� ����Ʈ
    #endregion

    #region 2
    public Sprite cursorSpr, clickedCursorSpr;
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
            StageManager.Instance.SetClearStage();
        });  //�÷��̾ ����Ű �ϳ� ����� ���� �̺�Ʈ

        //�÷��̾ ���� ������ ������� �� �̺�Ʈ
        EventManager.StartListening("DrainTutorialEnemyDrain", (Action<Vector2>)(enemyPos =>
        {
            Action logAction = null;
            logAction = () =>
            {
                KeyActionManager.Instance.GetElement(InitGainType.SKILL3);
                EventManager.StopListening("EndCutScene", logAction);
            };
            EventManager.StartListening("EndCutScene", logAction);
            
            Canvas ordCvs = UIManager.Instance.ordinaryCvsg.GetComponent<Canvas>();
            ordCvs.GetComponent<CanvasGroup>().alpha = 1;
            Vector3 special2SkillSlotPos = GetUITutorialReady(skillUIArr[2].GetComponent<RectTransform>(), new Vector2(1135, 389));

            skillUIArr[2].GetComponent<RectTransform>().DOAnchorPos(special2SkillSlotPos, 1f).SetEase(Ease.OutCubic);
            skillUIArr[2].GetComponent<CanvasGroup>().DOFade(1, 0.6f).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                EventManager.TriggerEvent("Skill2TutoClear");
                EventManager.TriggerEvent("UpdateKeyCodeUI");
            });
        }));

        //EventManager.StartListening("GetRushAttack", GetRushAttack);  //���� �ִ� NPC�� ��ȭ�Ŀ�
        EventManager.StartListening("EndTalkRushMaster", EndTalkRushMaster); //���� �ִ� NPC�� ��ȭ�Ŀ�
        EventManager.StartListening("GetInventoryUI", GetInventoryUI);  //�κ� �ִ� NPC�� ��ȭ�ϰ� ���� ��
        EventManager.StartListening("GetStatUI", GetStatUI);  //���� �ִ� NPC�� ��ȭ�ϰ� ���� ��
        EventManager.StartListening("GetMonsterCollectionUI", GetMonsterCollectionUI); //���� �ִ� NPC�� ��ȭ�ϰ� ���� ��

        EventManager.StartListening("GainQuikSlotUI", GetQuikSlot);
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
        quikSlotUI.gameObject.SetActive(active); 
        
        for (int i = 0; i < skillUIArr.Length; i++)
        {
            skillUIArr[i].gameObject.SetActive(active);
            //changeableBodysUIArr[i].gameObject.SetActive(active);
        }

        //changeableBodysUIArr[0].gameObject.SetActive(active);
        /*for(int i=1; i< changeableBodysUIArr.Length; i++)
        {
            changeableBodysUIArr[i].gameObject.SetActive(gm.savedData.userInfo.isGainBodyChangeSlot);   
        }*/

        for(int i = 0; i < changeableBodysUIArr.Length; i++)
        {
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

            tutorialPhases.Add(new StartPhase(playerFollowLight,1));
            //EffectManager.Instance.OnTouchEffect("TouchEffect1");
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
                    um.RequestLogMsg("������ ȹ���߽��ϴ�");
                });
            }));
        }

        //um.StartLoadingIn();
    }

    public void TutoEnemyDrainCSEvent() //������ ��� �ƾ� �߿� ������ ������ ���� ����
    {
        StageManager.Instance.StageClear();
        GameObject enemy = StageManager.Instance.CurrentStageGround.objSpawnPos.gameObject;
        Vector3 target = Global.GetSlimePos.position;

        Util.DelayFunc(() =>
        {
            enemy.gameObject.SetActive(true);
            enemy.transform.DOMove(target, 5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                enemy.transform.DOScale(Vector3.zero, 0.4f).OnComplete(() =>
                {
                    UIManager.Instance.worldUICvsg.alpha = 1;
                    KeyActionManager.Instance.ShowExclamationMark();
                    Util.DelayFunc(() => EventManager.TriggerEvent("DrainTutorialEnemyDrain", (Vector2)target), 1);
                    enemy.SetActive(false);
                });
            });
        }, 1.5f, this);
    }

    public void UIOn(KeyAction type)  //UI���̰� �ϰ� ��� ����
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

    private Vector3 GetUITutorialReady(RectTransform rt, Vector3 newPos) //ordinary canvas�� UI�� ���� ��ġ ��ȯ, �� ��ġ�� �̵�, ���� ���ְ� alpha�� 0���� ����
    {
        Vector3 startPos = rt.anchoredPosition;
        rt.GetComponent<CanvasGroup>().alpha = 0;
        rt.gameObject.SetActive(true);
        rt.anchoredPosition = newPos;
        return startPos;
    }

    private RectTransform SetUIEmphasisEffect(Transform parent) //���� UI ����Ʈ ǥ��
    {
        RectTransform emphRectTr = PoolManager.GetItem<RectTransform>("UIEmphasisEff");
        emphRectTr.transform.parent = parent;
        emphRectTr.anchoredPosition = Vector3.zero;
        return emphRectTr; 
    }

    private void GetUIIcon(UIType type, Vector3 startPos) //����, �κ�, ����, ���� ���� �޴� UIȹ��
    {
        KeyAction kt = Util.EnumParse<KeyAction>(type.ToString());

        if (gm.savedData.userInfo.uiActiveDic[kt]) return;

        AcquisitionDropIcon adi = PoolManager.GetItem<AcquisitionDropIcon>("AcqDropIcon");
        adi.Set(UIManager.Instance.GetInterfaceSprite(type),
                startPos, 2.4f, ScriptHelper.RandomVector(new Vector2(-6.5f, 4f), new Vector2(6.5f, 7f)), () =>
                {
                    UIOn(kt);
                    gm.savedData.userInfo.uiActiveDic[kt] = true;

                    switch(type)
                    {
                        case UIType.INVENTORY:
                            um.RequestLogMsg("�κ��丮�� ȹ���߽��ϴ�");
                            break;
                        case UIType.STAT:
                            um.RequestLogMsg("������ ȹ���߽��ϴ�");
                            break;
                        case UIType.MONSTER_COLLECTION:
                            um.RequestLogMsg("���� ������ ȹ���߽��ϴ�");
                            break;
                    }
                });
        adi.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
    }

    #region Tutorial Function

    public void GetSkill2() //���� ȹ��
    {
        GetUITween(new Vector2(1189, 343), skillUIArr[1], () =>
        {
            EventManager.TriggerEvent("Skill1TutoClear");
            EventManager.TriggerEvent("UpdateKeyCodeUI");
            KeyActionManager.Instance.GetElement(InitGainType.SKILL2);
        });
    }
    public void GetSkill1() //�Ϲݰ��� ȹ��
    {
        GetUITween(new Vector2(1100, 300), skillUIArr[0], () =>
        {
            EventManager.TriggerEvent("Skill0TutoClear");
            EventManager.TriggerEvent("UpdateKeyCodeUI");
            KeyActionManager.Instance.GetElement(InitGainType.SKILL1);
        });
    }

    public void GetQuikSlot()  //�� ���� ȹ��
    {
        GetUITween(new Vector2(1000, 300), quikSlotUI, () =>
        {
            EventManager.TriggerEvent("UpdateKeyCodeUI");
            KeyActionManager.Instance.GetElement(InitGainType.QUIKSLOT);
        });
    }

    public void OnCloseQuikSlotGetPanel() //�� ���� ȹ�� UI �ݰ���
    {
        Util.DelayFunc(() =>
        {
            InteractionHandler.canUseQuikSlot = true;
            tutorialPhases.Add(new QuikSlotTutorialPhase(SetUIEmphasisEffect(quikSlotUI)));
        }, 4f, this);
    }

    public void GetCharUI()  //ĳ���� UI���� => HP Energe MobSlot
    {
        for (int i=0; i<changeableBodysUIArr.Length; i++)
        {
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().alpha = 0;
            changeableBodysUIArr[i].gameObject.SetActive(true);
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().DOFade(i==0?1f:0.4f, 0.5f);
        }

        //HP ����
        hpUI.GetComponent<CanvasGroup>().alpha = 1;
        hpUI.gameObject.SetActive(true);
        UIManager.Instance.playerHPInfo.third.fillAmount = 0;
        EffectManager.Instance.hpFillEffect.gameObject.SetActive(true);
        float hpFillEffectMaskCenterInitScale = StoredData.GetValueData<float>("hpFillEffectMaskCenterInitScale", true);
        EffectManager.Instance.hpFillEffectMaskCenter.localScale = new Vector3(0, hpFillEffectMaskCenterInitScale, hpFillEffectMaskCenterInitScale);

        UIManager.Instance.playerHPInfo.third.DOFillAmount(1, 0.7f);
        EffectManager.Instance.hpFillEffectMaskCenter.DOScaleX(hpFillEffectMaskCenterInitScale, 0.75f);

        SkillUIManager sum = SkillUIManager.Instance;
        Vector3 orgEnergeEffMaskScl = StoredData.GetValueData<Vector3>("orgEnergeEffMaskScl", true);
        sum.IsAutoFitEnergeBar = false;
        sum.energeFill.fillAmount = 0;
        sum.energeEffMask.localScale = new Vector3(0, orgEnergeEffMaskScl.y, orgEnergeEffMaskScl.z);
        sum.energeBarAndEff.second.gameObject.SetActive(true);
        sum.energeBarAndEff.first.GetComponent<CanvasGroup>().alpha = 1;
        sum.energeBarAndEff.first.SetActive(true);

        sum.energeFill.DOFillAmount(1, 0.7f);
        sum.energeEffMask.DOScaleX(orgEnergeEffMaskScl.x, 0.75f).OnComplete(() =>
        {
            sum.IsAutoFitEnergeBar = true;
            EventManager.TriggerEvent("UpdateKeyCodeUI");
            StageManager.Instance.StageClear();
            KeyActionManager.Instance.GetElement(InitGainType.CHAR_UI);
        });
    }

    public void GetUITween(Vector2 newPos, Transform ui, Action end) //UI ȹ�� Ʈ�� ���� (Default)
    {
        Vector3 startPos = GetUITutorialReady(ui.GetComponent<RectTransform>(), newPos);

        Sequence seq = DOTween.Sequence();
        seq.Append(ui.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.OutCirc))
            .Join(ui.GetComponent<RectTransform>().DOAnchorPos(startPos, 0.4f).SetEase(Ease.OutCirc));
        seq.AppendCallback(() => end?.Invoke()).Play();
    }


    public void EndTalkRushMaster() //���� NPC�� ��ȭ �Ŀ�
    {
        if(!StoredData.HasValueKey("EndTalkRushMaster"))
        {
            StoredData.SetValueKey("EndTalkRushMaster", true);
            RectTransform emphRectTr = SetUIEmphasisEffect(skillUIArr[1].transform);
            tutorialPhases.Add(new RushTutorialPhase(emphRectTr.gameObject));

            (ObjectManager.Instance.itrObjDic["Rush Master"] as NormalNPC)._NPCInfo.talkId = 2;

            TalkManager.Instance.SetSubtitle("ó������ ������ �����µ� �������� �����徾�� ������ �� ����", 0.2f, 2f);
        }
    }

    private void GetInventoryUI() //�κ� ����
    {
        GetUIIcon(UIType.INVENTORY, ObjectManager.Instance.itrObjDic["Merchant"].transform.position);
    }

    private void GetStatUI() //���� UI ȹ��
    {
        GetUIIcon(UIType.STAT, ObjectManager.Instance.itrObjDic["StatNPC"].transform.position);
        
    }

    private void GetMonsterCollectionUI() //���� ���� UI ȹ��
    {
        GetUIIcon(UIType.MONSTER_COLLECTION, ObjectManager.Instance.itrObjDic["MonsterCollectionObjNPC"].transform.position);
        
    }

    public void GetBodyChangeSlot()  //���� ���� ȹ��(2,3��°)  
    {
        for(int i=1; i<changeableBodysUIArr.Length; i++)
        {
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().alpha = 0;
            changeableBodysUIArr[i].gameObject.SetActive(true);
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().DOFade(i==1 ? 1f : 0.4f, 0.5f);
        }

        UIManager.Instance.RequestLogMsg("���� ������ ������ϴ�.");
    }
    #endregion
}







#region �ּ�
//�÷��̾ ���� ������ ������� �� �̺�Ʈ
/*EventManager.StartListening("DrainTutorialEnemyDrain", enemyPos =>
{
    if (!StoredData.HasValueKey("DrainTutorialEnemyDrain1"))
    {
        Action logAction = null;
        logAction = () =>
        {
            KeyActionManager.Instance.GetElement(InitGainType.SKILL3);
            EventManager.StopListening("EndCutScene", logAction);
        };
        EventManager.StartListening("EndCutScene", logAction);

        StoredData.SetValueKey("DrainTutorialEnemyDrain1", true);

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
        cursorImg.sprite = cursorSpr;

        hpUI.GetComponent<CanvasGroup>().alpha = 0; //���� HP�� ������ �����̹Ƿ� alpha�� 0���� �ϰ� ���� ���� (��ƼŬ�� alpha 0�̾ ���̹Ƿ� ������ ���� ���� ����) 
        hpUI.gameObject.SetActive(true);

        changeableBodysUIArr[0].GetComponent<CanvasGroup>().alpha = 0;  //alpha�� 0���� �ϰ� ����
        changeableBodysUIArr[0].gameObject.SetActive(true);

        float hpFillEffectMaskCenterInitScale = StoredData.GetValueData<float>("hpFillEffectMaskCenterInitScale", true);

        UIManager.Instance.playerHPInfo.first.fillAmount = 0;
        UIManager.Instance.playerHPInfo.third.fillAmount = 0;
        EffectManager.Instance.hpFillEffectMaskCenter.localScale = new Vector3(0, hpFillEffectMaskCenterInitScale, hpFillEffectMaskCenterInitScale);

        //�̰� ��� ��ĥ��. anchor�� �޶� ��ġ�� ����� �ȳ���(anchor�� �߰��̾�� ������ �ڵ尡 �ߵ�) //Util.WorldToScreenPosForScreenSpace(enemyPos, ordCvs) - new Vector3(960f, -67f);
        // anchor�� �θ� anchor���� ����ؼ� � ����ŭ ���ų� ���ϴ½����� ���ľ��Ұ� ����
        Vector3 special2SkillSlotPos = GetUITutorialReady(skillUIArr[2].GetComponent<RectTransform>(), new Vector2(1135, 389));

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
                .AppendInterval(0.4f).AppendCallback(() => cursorImg.sprite = clickedCursorSpr);  //���콺 Ŀ�� �̹��� ����
            seq.Append(teHpBar.DOAnchorPos(new Vector2(-640f, 474f), 0.9f).SetEase(Ease.InQuad))
                .AppendInterval(0.4f).AppendCallback(() =>
                {
                    cursorImg.transform.parent = ordCvs.transform;
                    cursorImg.sprite = cursorSpr;
                });  //HP UI �ִ°����� �ű�
            seq.AppendInterval(0.3f);
            seq.Append(teHpBar.DOScale(SVector3.two, 0.4f)).AppendInterval(0.3f);
            seq.Append(teHpBar.GetComponent<CanvasGroup>().DOFade(0, 0.3f))
            .AppendInterval(0.15f); //�Ű��� UI �Ⱥ��̰�
            seq.Append(hpUI.GetComponent<CanvasGroup>().DOFade(1, 0.4f))
                .Join(cursorImg.DOColor(Color.clear, 0.3f))
                .Join(changeableBodysUIArr[0].GetComponent<CanvasGroup>().DOFade(1, 0.3f))
                .AppendInterval(0.6f);
            seq.Append(UIManager.Instance.playerHPInfo.first.DOFillAmount(1, 0.7f))  //HP Fill ��������
            .Join(EffectManager.Instance.hpFillEffectMaskCenter.DOScaleX(hpFillEffectMaskCenterInitScale, 0.75f)) //����Ʈ ����ũ ���� (����Ʈ�� ���� ���̰�)
            .AppendInterval(0.5f);
            seq.Append(skillUIArr[2].GetComponent<RectTransform>().DOAnchorPos(special2SkillSlotPos, 1f).SetEase(Ease.OutCubic))
            .Join(skillUIArr[2].GetComponent<CanvasGroup>().DOFade(1, 0.6f).SetEase(Ease.OutCubic))
            .AppendInterval(0.2f)  //���� HPUI�� ù��° ���� ���� ���̰� + ��� ��ų ���� ����
            .AppendCallback(() =>
            {
                UIManager.Instance.playerHPInfo.third.fillAmount = 1;
                Destroy(teHpBar.gameObject);
                Destroy(cursorImg.gameObject);
                EventManager.TriggerEvent("Skill2TutoClear");
                EventManager.TriggerEvent("UpdateKeyCodeUI");
            });
            seq.Play();
        }, 3f);  //end of Util
    }  //end of if
});  //�÷��̾ Ʃ�丮��(HP�� ��� ��) ���� ���͸� ������� ���� �̺�Ʈ*/

/*private void ShowTargetSortingLayers() //Test
{
    int[] arr = (int[])playerFollowLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").GetValue(playerFollowLight);

    StringBuilder sb = new StringBuilder();

    foreach (int r in arr)
        sb.Append(r + ", ");

    Debug.Log(sb.ToString());
}*/

//�÷��̾�� Ʃ�� ������ ���� �Ÿ��� ���� ����(��� ���� �Ÿ�)�� �� �̺�Ʈ
/*EventManager.StartListening("Tuto_CanDrainObject", () =>
{
    if (!StoredData.HasValueKey("Tuto_CanDrainObject1"))
    {
        StoredData.SetValueKey("Tuto_CanDrainObject1", true);
        SlimeGameManager.Instance.Player.PlayerInput.CantPlaySkill2 = true;

        TimeManager.LerpTime(2f, 0f, () =>
        {
            RectTransform emphRectTr = SetUIEmphasisEffect(skillUIArr[2].transform);
            tutorialPhases.Add(new AbsorptionPhase(() =>
            {
                emphRectTr.gameObject.SetActive(false);
                TimeManager.TimeResume(() =>
                {
                    SlimeGameManager.Instance.Player.PlayerInput.CantPlaySkill2 = false;
                    Global.GetSlimePos.GetComponent<PlayerDrain>().DoDrainByTuto();
                    Destroy(Global.GetSlimePos.GetComponentInChildren<PlayerCanDrainCheckCollider>().gameObject);

                    EventManager.TriggerEvent("StartCutScene");
                    Canvas ordCvs = UIManager.Instance.ordinaryCvsg.GetComponent<Canvas>();
                    ordCvs.GetComponent<CanvasGroup>().alpha = 1;
                    EffectManager.Instance.hpFillEffect.gameObject.SetActive(true);

                    Util.DelayFunc(() =>
                    {
                        //Init
                        Vector3 startPos = GetUITutorialReady(skillUIArr[0].GetComponent<RectTransform>(), new Vector2(918, 393)); //���� ��ġ(�� ��ũ�� ��ǥ)��

                        Vector3 orgEnergeEffMaskScl = StoredData.GetValueData<Vector3>("orgEnergeEffMaskScl", true);
                        //StoredData.DeleteValueKey("orgEnergeEffMaskScl");
                        SkillUIManager sum = SkillUIManager.Instance;
                        sum.IsAutoFitEnergeBar = false;
                        sum.energeFill.fillAmount = 0;
                        sum.energeEffMask.localScale = new Vector3(0, orgEnergeEffMaskScl.y, orgEnergeEffMaskScl.z);
                        sum.energeBarAndEff.second.gameObject.SetActive(true);
                        sum.energeBarAndEff.first.GetComponent<CanvasGroup>().alpha = 0;
                        sum.energeBarAndEff.first.SetActive(true);

                        //Tween Sequence

                        Sequence seq = DOTween.Sequence();
                        seq.Append(skillUIArr[0].GetComponent<RectTransform>().DOAnchorPos(startPos, 0.4f).SetEase(Ease.InQuad))
                        .Join(skillUIArr[0].GetComponent<CanvasGroup>().DOFade(1, 0.3f))
                        .AppendInterval(0.5f);
                        seq.Append(sum.energeBarAndEff.first.GetComponent<CanvasGroup>().DOFade(1, 0.4f));
                        seq.Append(sum.energeFill.DOFillAmount(1, 0.75f))
                        .Join(sum.energeEffMask.DOScaleX(orgEnergeEffMaskScl.x, 0.68f));
                        seq.AppendInterval(0.6f).AppendCallback(() =>
                        {
                            EventManager.TriggerEvent("EndCutScene");
                            EventManager.TriggerEvent("Skill0TutoClear");
                            sum.IsAutoFitEnergeBar = true;

                            um.RequestLogMsg("���� �������� ������ϴ�.");
                            um.RequestLogMsg("�⺻ ������ ������ϴ�.");
                            EventManager.TriggerEvent("UpdateKeyCodeUI");
                        });  

                    }, 2.5f, this); //end of util

                }, 1f);  //end of time resume event
            }));  //end of AbsorptionPhase event
        });  //end of lerp time end event
    }  //end of if 
});  //end of event*/

/*public void GetRushAttack()  //������ ����
    {
        if (StoredData.HasValueKey("GetRushAttack")) return;

        (ObjectManager.Instance.itrObjDic["Rush Master"] as NormalNPC)._NPCInfo.talkId = 2;

        Vector3 startPos = GetUITutorialReady(skillUIArr[1].GetComponent<RectTransform>(), new Vector2(1189, 343));

        Sequence seq = DOTween.Sequence();
        seq.Append(skillUIArr[1].GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.OutCirc))
            .Join(skillUIArr[1].GetComponent<RectTransform>().DOAnchorPos(startPos, 0.4f).SetEase(Ease.OutCirc));
        seq.AppendCallback(() =>
        {
            EventManager.TriggerEvent("Skill1TutoClear");
            RectTransform emphRectTr = SetUIEmphasisEffect(skillUIArr[1].transform);
            tutorialPhases.Add(new RushTutorialPhase(emphRectTr.gameObject));
            um.RequestLogMsg("������ ������ϴ�.");
            EventManager.TriggerEvent("UpdateKeyCodeUI");
        }).Play();

        StoredData.SetValueKey("GetRushAttack", true);
    }*/

#endregion