using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class SkillUIManager : MonoSingleton<SkillUIManager>
{
    private SkillInfoImage[] skillInfoUIArr;
    [SerializeField] private GridLayoutGroup bottomRightBarGrid;

    [SerializeField] private Transform skillImgUIParent;

    public SkillInfoImage defaultSkill;
    public SkillInfoImage specialSkill;
    public SkillInfoImage drain;

    public Triple<Image, Text, Text> skillDetailImgNameEx;

    public Sprite emptySkillSpr; //빈 슬롯일 때의 스프라이트 (스킬)

    //[SerializeField] private Pair<string, SkillInfo[]> playerOriginBodySkills;
    //[SerializeField] private List<Pair<Enemy.EnemyType, SkillInfo[]>> monsterSkillsList;
    [SerializeField] private SkillsInfoSO skillsSO;
    private Dictionary<string, SkillInfo[]> monsterSkillsDic = new Dictionary<string, SkillInfo[]>();

    //에너지바
    public Pair<GameObject, ParticleSystem> energeBarAndEff;
    public Transform energeEffMask;
    public Image energeFill;
    private Vector3 orgEnergeEffMaskScl;

    public Color slimeAtkEnergeColor, assimilationBarColor;  //에너지바 색상 : 슬라임일 때랑 변신한 몹일 때
    public Color slimeEnergePsColor, assimPsColor; // 파티클 색상 : 슬라임일 때와 변신상태일 때
    public Gradient slimeEnergeGd, assimGd; //파티클의 Color over Lifetime 값 : 슬라임일 때와 변신상태일 때
    public Text curAssimText;  //동화율 최대치 표시 텍스트

    [Space(15)]
    //에너지바 일정 이하일 때 효과
    //public CanvasGroup energeBarCvsg;
    private bool isLowEnerge = false;
    private float lowEnergeRate;
    private ParticleSystem.MainModule energeBarEffMainModule;
    private ParticleSystem.ColorOverLifetimeModule energePsCOLT;
    private Color energeDefaultColor, energeImgDefaultColor;
    private Gradient defaultEnergeGrad;

    public float lowEnergeRatePercent = 10f;
    public Color lowEnergeColor;
    public Gradient lowEnergeGrad;

    public bool IsAutoFitEnergeBar { get; set; }
    public bool IsOriginSlime { get; private set; }

    private void Awake()
    {
        skillInfoUIArr = skillImgUIParent.GetComponentsInChildren<SkillInfoImage>();
        orgEnergeEffMaskScl = energeEffMask.localScale;
        StoredData.SetValueKey("orgEnergeEffMaskScl", orgEnergeEffMaskScl);

        lowEnergeRate = lowEnergeRatePercent * 0.01f;
        energeBarEffMainModule = energeBarAndEff.second.main;
        energePsCOLT = energeBarAndEff.second.colorOverLifetime;

        energeDefaultColor = energeBarEffMainModule.startColor.color;
        energeImgDefaultColor = energeFill.color;
        defaultEnergeGrad = energePsCOLT.color.gradient;

        monsterSkillsDic.Add(skillsSO.playerOriginBodySkills.first, skillsSO.playerOriginBodySkills.second);
        for(int i= 0; i < skillsSO.monsterSkillsList.Count; i++)
        {
            monsterSkillsDic.Add(skillsSO.monsterSkillsList[i].first.ToString(), skillsSO.monsterSkillsList[i].second);
        }

        defaultSkill.Register(skillsSO.playerOriginBodySkills.second[0]);
        specialSkill.Register(skillsSO.playerOriginBodySkills.second[1]);
        drain.Register(skillsSO.playerOriginBodySkills.second[2]);

        IsAutoFitEnergeBar = true;
        IsOriginSlime = true;

        EventManager.StartListening("ChangeBody", (id, dead) =>
        {
            if(dead){} //bool타입인걸 알리기위한

            //현재 스킬UI정보를 없애고 변신한 몸체의 스킬 정보 가져와서 세팅
            skillInfoUIArr.ForEach(x => x.Unregister());
            SkillInfo[] skill = monsterSkillsDic[id];
            foreach(SkillInfo skillInfo in skill)
            {
                switch(skillInfo.skillType)
                {
                    case SkillType.ATTACK:
                        defaultSkill.Register(skillInfo);
                        break;
                    case SkillType.SPECIALATTACK1:
                        specialSkill.Register(skillInfo);
                        break;
                    case SkillType.SPECIALATTACK2:
                        drain.Register(skillInfo);
                        break;
                }
            }

            //체력바 밑의 에너지바에서 슬라임이면 필색상과 이펙트 색상을 초록으로, 흡수한 몹이면 파란색으로
            bool org = id == Global.OriginBodyID;
            IsOriginSlime = org;
            energeFill.color = org ? slimeAtkEnergeColor : assimilationBarColor;
            curAssimText.gameObject.SetActive(!org);

            energeBarEffMainModule.startColor = org ? slimeEnergePsColor : assimPsColor;
            energePsCOLT.color = org ? slimeEnergeGd : assimGd;

            energeEffMask.DOKill();
            energeFill.DOKill();
            UpdateUnderstandingBar();
        });

        EventManager.StartListening("StartCutScene", () => SetActiveSlimeEnergeEffect(false));
        EventManager.StartListening("EndCutScene", () => SetActiveSlimeEnergeEffect(true));
        EventManager.StartListening("EnemyDead", (Action<GameObject, string, bool>)((obj, id, dead) => UpdateUnderstandingBar()));
        EventManager.StartListening("UpdateKeyCodeUI", UpdateSkillKeyCode);
    }

    private void Start()
    {
        Util.DelayFunc(() => bottomRightBarGrid.enabled = false, 0.5f, this, true);
    }

    void SetActiveSlimeEnergeEffect(bool active) => energeBarAndEff.second.gameObject.SetActive(active);


    public void UpdateSkillKeyCode()  //스킬 슬롯들에 존재하는 상호작용 키를 현재 키세팅 상태에 맞게 갱신
    {
        for(int i=0; i<skillInfoUIArr.Length; i++)
        {
            skillInfoUIArr[i].UpdateKeyCode();
        }
    }

    public void OnClickSkillButton(Sprite spr, string sName, string ex)  //스킬 정보 자세히보기
    {
        UIManager.Instance.OnUIInteractSetActive(UIType.SKILLDETAIL, true);

        skillDetailImgNameEx.first.sprite = spr;
        skillDetailImgNameEx.second.text = sName;
        skillDetailImgNameEx.third.text = ex;
    }

    private void LateUpdate()
    {
        UpdateEnergeBarUI();
    }

    private void UpdateEnergeBarUI()  //현재 슬라임 상태일 때 공격 에너지 양 UI 갱신
    {
        if(IsAutoFitEnergeBar && IsOriginSlime)
        {
            float rate = Global.CurrentPlayer.CurrentEnergy / Global.CurrentPlayer.MaxEnergy;
            energeFill.fillAmount = rate;
            energeEffMask.localScale = new Vector3(orgEnergeEffMaskScl.x * rate,orgEnergeEffMaskScl.y,orgEnergeEffMaskScl.z);

            if(rate <= lowEnergeRate && !isLowEnerge)
            {
                isLowEnerge = true;
                energeBarEffMainModule.startColor = lowEnergeColor;
                energeFill.color = lowEnergeColor;
                energePsCOLT.color = lowEnergeGrad;

                //energeBarCvsg.DOFade(0.5f, 0.4f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            }
            else if(rate > lowEnergeRate && isLowEnerge)
            {
                isLowEnerge = false;
                energeBarEffMainModule.startColor = energeDefaultColor;
                energeFill.color = energeImgDefaultColor;
                energePsCOLT.color = defaultEnergeGrad;

                //energeBarCvsg.DOKill();
                //energeBarCvsg.alpha = 1;
            }
        }
    }

    public void UpdateUnderstandingBar() //현재 변신 상태의 몬스터 동화율 UI 갱신
    {
        if (IsOriginSlime) return;

        float rate = PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(SlimeGameManager.Instance.CurrentBodyId) % 51 * 0.02f;
        energeFill.DOFillAmount(rate, 0.3f);
        energeEffMask.DOScaleX(orgEnergeEffMaskScl.x * rate, 0.3f);
    }

    public void SetEnableSlot(SkillType type, bool on)
    {
        for(int i= 0; i < skillInfoUIArr.Length; i++)
        {
            if(skillInfoUIArr[i]._SkillType == type)
            {
                skillInfoUIArr[i].SetEnableSlot(on);
                break;
            }
        }
    }
}