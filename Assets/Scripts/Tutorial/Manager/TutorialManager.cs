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

    //처음에 안보일 UI들 (KeyAction으로 처리하지 않을 것들)
    public Transform hpUI, energeBarUI;
    public Transform[] skillUIArr;
    public Transform[] changeableBodysUIArr;

    //튜토리얼 진행중인가
    public bool IsTutorialStage => !GameManager.Instance.savedData.tutorialInfo.isEnded;
    //튜토리얼 진행시키는데 업데이트에서 처리해야할 데이터들
    private List<TutorialPhase> tutorialPhases = new List<TutorialPhase>();

    #region 1
    private Light2D playerFollowLight;  //플레이어 따라다니는 라이트
    #endregion

    public Pair<GameObject, Transform> acqDropIconPair; //UI획득하고 획득 연출 뜰 아이콘 오브젝트


    [Header("Test")]
    [SerializeField] private bool isTestMode = false;
    public bool IsTestMode => isTestMode;

    private void Awake()
    {
        PoolManager.CreatePool(acqDropIconPair.first, acqDropIconPair.second, 2, "AcqDropIcon");

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
            //적 HP바 UI 생성 후 적 위치로 가져옴
            RectTransform teHpBar;
            Canvas ordCvs = UIManager.Instance.ordinaryCvsg.GetComponent<Canvas>(); 
            if(StoredData.HasGameObjectKey("Tuto EnemyHp UI"))
            {
                teHpBar = StoredData.GetGameObjectData<RectTransform>("Tuto EnemyHp UI");
            }
            else
            {
                teHpBar = Instantiate(Resources.Load<GameObject>("Tutorial/UI/TutorialEnemyHP"), ordCvs.transform).GetComponent<RectTransform>();
                StoredData.SetGameObjectKey("Tuto EnemyHp UI", teHpBar.gameObject);
            }

            teHpBar.gameObject.SetActive(false);
            teHpBar.anchoredPosition = Util.WorldToScreenPosForScreenSpace(enemyPos, ordCvs);

            //Init
            ordCvs.GetComponent<CanvasGroup>().alpha = 1;  //컷씬 시작상태라 alpha값이 0인 상태이므로 1로 켜줌. 
            teHpBar.GetComponent<CanvasGroup>().alpha = 1;

            hpUI.GetComponent<CanvasGroup>().alpha = 0; //아직 HP바 못얻은 상태이므로 alpha만 0으로 하고 옵젝 켜줌 (파티클은 alpha 0이어도 보이므로 아직은 꺼진 상태 유지) 
            hpUI.gameObject.SetActive(true);

            changeableBodysUIArr[0].GetComponent<CanvasGroup>().alpha = 0;  //alpha만 0으로 하고 켜줌
            changeableBodysUIArr[0].gameObject.SetActive(true);

            float hpFillEffectMaskCenterInitScale = StoredData.GetValueData<float>("hpFillEffectMaskCenterInitScale");

            UIManager.Instance.playerHPInfo.first.fillAmount = 0;
            UIManager.Instance.playerHPInfo.third.fillAmount = 0;
            EffectManager.Instance.hpFillEffectMaskCenter.localScale = new Vector3(0, hpFillEffectMaskCenterInitScale, hpFillEffectMaskCenterInitScale);

            Vector3 special2SkillSlotPos = skillUIArr[2].GetComponent<RectTransform>().anchoredPosition;
            skillUIArr[2].GetComponent<CanvasGroup>().alpha = 0;
            skillUIArr[2].gameObject.SetActive(true);
            skillUIArr[2].GetComponent<RectTransform>().anchoredPosition = Util.WorldToScreenPosForScreenSpace(enemyPos, ordCvs);

            //UtilEditor.PauseEditor();

            //HP바 tweening

            Util.DelayFunc(() =>  //슬라임 흡수하고 원래 사이즈로 되돌아올 때까지 대기
            {
                teHpBar.gameObject.SetActive(true);
                EffectManager.Instance.hpFillEffect.gameObject.SetActive(true);

                Sequence seq = DOTween.Sequence();
                seq.Append(teHpBar.DOAnchorPos(Util.WorldToScreenPosForScreenSpace(Global.GetSlimePos.position + Vector3.down, ordCvs), 0.3f))
                .AppendInterval(0.5f); //슬라임 밑으로 체력바 옮김
                seq.Append(teHpBar.DOAnchorPos(new Vector2(-819.3f, 474f), 0.9f).SetEase(Ease.InQuad))
                .AppendInterval(0.4f);  //HP UI 있는곳으로 옮김
                seq.Append(teHpBar.GetComponent<CanvasGroup>().DOFade(0, 0.3f))
                .AppendInterval(0.15f); //옮겨진 UI 안보이게
                seq.Append(hpUI.GetComponent<CanvasGroup>().DOFade(1, 0.4f))
                .Join(changeableBodysUIArr[0].GetComponent<CanvasGroup>().DOFade(1, 0.3f))  
                .Join(skillUIArr[2].GetComponent<RectTransform>().DOAnchorPos(special2SkillSlotPos, 1f).SetEase(Ease.InOutBack))
                .Join(skillUIArr[2].GetComponent<CanvasGroup>().DOFade(1, 0.6f).SetEase(Ease.OutCubic))
                .AppendInterval(0.2f);   //원래 HPUI랑 첫번째 변신 슬롯 보이게 + 흡수 스킬 슬롯 얻음
                seq.Append(UIManager.Instance.playerHPInfo.first.DOFillAmount(1, 0.7f))  //HP Fill 차오르게
                .Join(EffectManager.Instance.hpFillEffectMaskCenter.DOScaleX(hpFillEffectMaskCenterInitScale, 0.75f)) //이펙트 마스크 넓힘 (이펙트도 점점 보이게)
                .AppendCallback(() =>
                {
                    UIManager.Instance.playerHPInfo.third.fillAmount = 1;
                });
                seq.Play();
            }, 3f);
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
