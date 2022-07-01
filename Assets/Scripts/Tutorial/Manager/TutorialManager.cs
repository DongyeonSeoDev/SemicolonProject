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

    //처음에 안보일 UI들 (KeyAction으로 처리하지 않을 것들)
    public Transform hpUI, energeBarUI;
    public Transform quikSlotUI;
    public Transform[] skillUIArr;
    public Transform[] changeableBodysUIArr;

    //튜토리얼 진행중인가
    public bool IsTutorialStage => (!(GameManager.Instance.savedData.tutorialInfo.isEnded || isTestMode));
    //튜토리얼 진행시키는데 업데이트에서 처리해야할 데이터들
    private List<TutorialPhase> tutorialPhases = new List<TutorialPhase>();

    #region 1
    private Light2D playerFollowLight;  //플레이어 따라다니는 라이트
    #endregion

    #region 2
    public Sprite cursorSpr, clickedCursorSpr;
    #endregion

    public Pair<GameObject, Transform> acqDropIconPair; //UI획득하고 획득 연출 뜰 아이콘 오브젝트

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
        });  //플레이어가 방향키 하나 얻었을 때의 이벤트

        //플레이어가 작은 슬라임 흡수했을 때 이벤트
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

        //EventManager.StartListening("GetRushAttack", GetRushAttack);  //돌진 주는 NPC와 대화후에
        EventManager.StartListening("EndTalkRushMaster", EndTalkRushMaster); //돌진 주는 NPC와 대화후에
        EventManager.StartListening("GetInventoryUI", GetInventoryUI);  //인벤 주는 NPC와 대화하고 얻을 때
        EventManager.StartListening("GetStatUI", GetStatUI);  //스탯 주는 NPC와 대화하고 얻을 때
        EventManager.StartListening("GetMonsterCollectionUI", GetMonsterCollectionUI); //도감 주는 NPC와 대화하고 얻을 때

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
                    um.RequestLogMsg("설정을 획득했습니다");
                });
            }));
        }

        //um.StartLoadingIn();
    }

    public void TutoEnemyDrainCSEvent() //슬라임 흡수 컷씬 중에 슬라임 나오는 연출 시작
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

    public void UIOn(KeyAction type)  //UI보이게 하고 기능 켜줌
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

    private Vector3 GetUITutorialReady(RectTransform rt, Vector3 newPos) //ordinary canvas의 UI의 원래 위치 반환, 새 위치로 이동, 옵젝 켜주고 alpha만 0으로 해줌
    {
        Vector3 startPos = rt.anchoredPosition;
        rt.GetComponent<CanvasGroup>().alpha = 0;
        rt.gameObject.SetActive(true);
        rt.anchoredPosition = newPos;
        return startPos;
    }

    private RectTransform SetUIEmphasisEffect(Transform parent) //강조 UI 이펙트 표시
    {
        RectTransform emphRectTr = PoolManager.GetItem<RectTransform>("UIEmphasisEff");
        emphRectTr.transform.parent = parent;
        emphRectTr.anchoredPosition = Vector3.zero;
        return emphRectTr; 
    }

    private void GetUIIcon(UIType type, Vector3 startPos) //설정, 인벤, 스탯, 도감 등의 메뉴 UI획득
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
                            um.RequestLogMsg("인벤토리를 획득했습니다");
                            break;
                        case UIType.STAT:
                            um.RequestLogMsg("스탯을 획득했습니다");
                            break;
                        case UIType.MONSTER_COLLECTION:
                            um.RequestLogMsg("몬스터 도감을 획득했습니다");
                            break;
                    }
                });
        adi.transform.localScale = new Vector3(3.5f, 3.5f, 1f);
    }

    #region Tutorial Function

    public void GetSkill2() //돌진 획득
    {
        GetUITween(new Vector2(1189, 343), skillUIArr[1], () =>
        {
            EventManager.TriggerEvent("Skill1TutoClear");
            EventManager.TriggerEvent("UpdateKeyCodeUI");
            KeyActionManager.Instance.GetElement(InitGainType.SKILL2);
        });
    }
    public void GetSkill1() //일반공격 획득
    {
        GetUITween(new Vector2(1100, 300), skillUIArr[0], () =>
        {
            EventManager.TriggerEvent("Skill0TutoClear");
            EventManager.TriggerEvent("UpdateKeyCodeUI");
            KeyActionManager.Instance.GetElement(InitGainType.SKILL1);
        });
    }

    public void GetQuikSlot()  //퀵 슬롯 획득
    {
        GetUITween(new Vector2(1000, 300), quikSlotUI, () =>
        {
            EventManager.TriggerEvent("UpdateKeyCodeUI");
            KeyActionManager.Instance.GetElement(InitGainType.QUIKSLOT);
        });
    }

    public void OnCloseQuikSlotGetPanel() //퀵 슬롯 획득 UI 닫고나서
    {
        Util.DelayFunc(() =>
        {
            InteractionHandler.canUseQuikSlot = true;
            tutorialPhases.Add(new QuikSlotTutorialPhase(SetUIEmphasisEffect(quikSlotUI)));
        }, 4f, this);
    }

    public void GetCharUI()  //캐릭터 UI얻음 => HP Energe MobSlot
    {
        for (int i=0; i<changeableBodysUIArr.Length; i++)
        {
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().alpha = 0;
            changeableBodysUIArr[i].gameObject.SetActive(true);
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().DOFade(i==0?1f:0.4f, 0.5f);
        }

        //HP 얻음
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

    public void GetUITween(Vector2 newPos, Transform ui, Action end) //UI 획득 트윈 연출 (Default)
    {
        Vector3 startPos = GetUITutorialReady(ui.GetComponent<RectTransform>(), newPos);

        Sequence seq = DOTween.Sequence();
        seq.Append(ui.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetEase(Ease.OutCirc))
            .Join(ui.GetComponent<RectTransform>().DOAnchorPos(startPos, 0.4f).SetEase(Ease.OutCirc));
        seq.AppendCallback(() => end?.Invoke()).Play();
    }


    public void EndTalkRushMaster() //돌진 NPC와 대화 후에
    {
        if(!StoredData.HasValueKey("EndTalkRushMaster"))
        {
            StoredData.SetValueKey("EndTalkRushMaster", true);
            RectTransform emphRectTr = SetUIEmphasisEffect(skillUIArr[1].transform);
            tutorialPhases.Add(new RushTutorialPhase(emphRectTr.gameObject));

            (ObjectManager.Instance.itrObjDic["Rush Master"] as NormalNPC)._NPCInfo.talkId = 2;

            TalkManager.Instance.SetSubtitle("처음에는 무서워 보였는데 생각보다 오르드씨는 좋으신 분 같아", 0.2f, 2f);
        }
    }

    private void GetInventoryUI() //인벤 얻음
    {
        GetUIIcon(UIType.INVENTORY, ObjectManager.Instance.itrObjDic["Merchant"].transform.position);
    }

    private void GetStatUI() //스탯 UI 획득
    {
        GetUIIcon(UIType.STAT, ObjectManager.Instance.itrObjDic["StatNPC"].transform.position);
        
    }

    private void GetMonsterCollectionUI() //몬스터 도감 UI 획득
    {
        GetUIIcon(UIType.MONSTER_COLLECTION, ObjectManager.Instance.itrObjDic["MonsterCollectionObjNPC"].transform.position);
        
    }

    public void GetBodyChangeSlot()  //변신 슬롯 획득(2,3번째)  
    {
        for(int i=1; i<changeableBodysUIArr.Length; i++)
        {
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().alpha = 0;
            changeableBodysUIArr[i].gameObject.SetActive(true);
            changeableBodysUIArr[i].GetComponent<CanvasGroup>().DOFade(i==1 ? 1f : 0.4f, 0.5f);
        }

        UIManager.Instance.RequestLogMsg("변신 슬롯을 얻었습니다.");
    }
    #endregion
}







#region 주석
//플레이어가 작은 슬라임 흡수했을 때 이벤트
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

        //적 HP바 UI 생성 후 적 위치로 가져옴
        RectTransform teHpBar;
        Canvas ordCvs = UIManager.Instance.ordinaryCvsg.GetComponent<Canvas>();

        teHpBar = Instantiate(Resources.Load<GameObject>("Tutorial/UI/TutorialEnemyHP"), ordCvs.transform).GetComponent<RectTransform>();
        teHpBar.gameObject.SetActive(false);
        teHpBar.anchoredPosition = Util.WorldToScreenPosForScreenSpace(enemyPos, ordCvs);

        //Cursor Img
        Image cursorImg = teHpBar.GetChild(1).GetComponent<Image>();

        //Init
        ordCvs.GetComponent<CanvasGroup>().alpha = 1;  //컷씬 시작상태라 alpha값이 0인 상태이므로 1로 켜줌. 
        teHpBar.GetComponent<CanvasGroup>().alpha = 1;
        cursorImg.color = Color.clear;
        cursorImg.sprite = cursorSpr;

        hpUI.GetComponent<CanvasGroup>().alpha = 0; //아직 HP바 못얻은 상태이므로 alpha만 0으로 하고 옵젝 켜줌 (파티클은 alpha 0이어도 보이므로 아직은 꺼진 상태 유지) 
        hpUI.gameObject.SetActive(true);

        changeableBodysUIArr[0].GetComponent<CanvasGroup>().alpha = 0;  //alpha만 0으로 하고 켜줌
        changeableBodysUIArr[0].gameObject.SetActive(true);

        float hpFillEffectMaskCenterInitScale = StoredData.GetValueData<float>("hpFillEffectMaskCenterInitScale", true);

        UIManager.Instance.playerHPInfo.first.fillAmount = 0;
        UIManager.Instance.playerHPInfo.third.fillAmount = 0;
        EffectManager.Instance.hpFillEffectMaskCenter.localScale = new Vector3(0, hpFillEffectMaskCenterInitScale, hpFillEffectMaskCenterInitScale);

        //이걸 어떻게 고칠까. anchor가 달라서 위치가 제대로 안나옴(anchor가 중간이어야 오른쪽 코드가 잘됨) //Util.WorldToScreenPosForScreenSpace(enemyPos, ordCvs) - new Vector3(960f, -67f);
        // anchor와 부모 anchor까지 고려해서 어떤 값만큼 빼거나 더하는식으로 고쳐야할것 같음
        Vector3 special2SkillSlotPos = GetUITutorialReady(skillUIArr[2].GetComponent<RectTransform>(), new Vector2(1135, 389));

        //UtilEditor.PauseEditor();

        //HP바 tweening

        Util.DelayFunc(() =>  //슬라임 흡수하고 원래 사이즈로 되돌아올 때까지 대기
        {
            teHpBar.gameObject.SetActive(true);
            EffectManager.Instance.hpFillEffect.gameObject.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.Append(teHpBar.DOAnchorPos(Util.WorldToScreenPosForScreenSpace(Global.GetSlimePos.position + Vector3.down, ordCvs), 0.3f))
            .AppendInterval(0.7f); //슬라임 밑으로 체력바 옮김
            seq.Append(cursorImg.DOColor(Color.white, 0.5f))
                .AppendInterval(0.4f).AppendCallback(() => cursorImg.sprite = clickedCursorSpr);  //마우스 커서 이미지 보임
            seq.Append(teHpBar.DOAnchorPos(new Vector2(-640f, 474f), 0.9f).SetEase(Ease.InQuad))
                .AppendInterval(0.4f).AppendCallback(() =>
                {
                    cursorImg.transform.parent = ordCvs.transform;
                    cursorImg.sprite = cursorSpr;
                });  //HP UI 있는곳으로 옮김
            seq.AppendInterval(0.3f);
            seq.Append(teHpBar.DOScale(SVector3.two, 0.4f)).AppendInterval(0.3f);
            seq.Append(teHpBar.GetComponent<CanvasGroup>().DOFade(0, 0.3f))
            .AppendInterval(0.15f); //옮겨진 UI 안보이게
            seq.Append(hpUI.GetComponent<CanvasGroup>().DOFade(1, 0.4f))
                .Join(cursorImg.DOColor(Color.clear, 0.3f))
                .Join(changeableBodysUIArr[0].GetComponent<CanvasGroup>().DOFade(1, 0.3f))
                .AppendInterval(0.6f);
            seq.Append(UIManager.Instance.playerHPInfo.first.DOFillAmount(1, 0.7f))  //HP Fill 차오르게
            .Join(EffectManager.Instance.hpFillEffectMaskCenter.DOScaleX(hpFillEffectMaskCenterInitScale, 0.75f)) //이펙트 마스크 넓힘 (이펙트도 점점 보이게)
            .AppendInterval(0.5f);
            seq.Append(skillUIArr[2].GetComponent<RectTransform>().DOAnchorPos(special2SkillSlotPos, 1f).SetEase(Ease.OutCubic))
            .Join(skillUIArr[2].GetComponent<CanvasGroup>().DOFade(1, 0.6f).SetEase(Ease.OutCubic))
            .AppendInterval(0.2f)  //원래 HPUI랑 첫번째 변신 슬롯 보이게 + 흡수 스킬 슬롯 얻음
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
});  //플레이어가 튜토리얼(HP바 얻는 곳) 전용 몬스터를 흡수했을 때의 이벤트*/

/*private void ShowTargetSortingLayers() //Test
{
    int[] arr = (int[])playerFollowLight.GetFieldInfo<Light2D>("m_ApplyToSortingLayers").GetValue(playerFollowLight);

    StringBuilder sb = new StringBuilder();

    foreach (int r in arr)
        sb.Append(r + ", ");

    Debug.Log(sb.ToString());
}*/

//플레이어와 튜토 슬라임 사이 거리가 일정 이하(흡수 판정 거리)일 때 이벤트
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
                        Vector3 startPos = GetUITutorialReady(skillUIArr[0].GetComponent<RectTransform>(), new Vector2(918, 393)); //몬스터 위치(의 스크린 좌표)로

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

                            um.RequestLogMsg("공격 에너지를 얻었습니다.");
                            um.RequestLogMsg("기본 공격을 얻었습니다.");
                            EventManager.TriggerEvent("UpdateKeyCodeUI");
                        });  

                    }, 2.5f, this); //end of util

                }, 1f);  //end of time resume event
            }));  //end of AbsorptionPhase event
        });  //end of lerp time end event
    }  //end of if 
});  //end of event*/

/*public void GetRushAttack()  //돌진을 얻음
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
            um.RequestLogMsg("돌진을 얻었습니다.");
            EventManager.TriggerEvent("UpdateKeyCodeUI");
        }).Play();

        StoredData.SetValueKey("GetRushAttack", true);
    }*/

#endregion