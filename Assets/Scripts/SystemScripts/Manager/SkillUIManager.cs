using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SkillUIManager : MonoSingleton<SkillUIManager>
{
    private SkillInfoImage[] skillInfoUIArr;

    [SerializeField] private Transform skillImgUIParent;

    public SkillInfoImage defaultSkill;
    public SkillInfoImage specialSkill;
    public SkillInfoImage drain;

    public Triple<Image, Text, Text> skillDetailImgNameEx;

    public Sprite emptySkillSpr; //�� ������ ���� ��������Ʈ (��ų)

    //[SerializeField] private Pair<string, SkillInfo[]> playerOriginBodySkills;
    //[SerializeField] private List<Pair<Enemy.EnemyType, SkillInfo[]>> monsterSkillsList;
    [SerializeField] private SkillsInfoSO skillsSO;
    private Dictionary<string, SkillInfo[]> monsterSkillsDic = new Dictionary<string, SkillInfo[]>();

    //��������
    public Pair<GameObject, GameObject> energeBarAndEff;
    public Transform energeEffMask;
    public Image energeFill;
    private Vector3 orgEnergeEffMaskScl;

    [Space(15)]
    //�������� ���� ������ �� ȿ��
    //public CanvasGroup energeBarCvsg;
    private bool isLowEnerge = false;
    private float lowEnergeRate;
    private ParticleSystem.MainModule energeBarEffMainModule;
    private ParticleSystem.ColorOverLifetimeModule energePsCOLT;
    private Color energeDefaultColor, energeImgDefaultColor;
    private Gradient defaultEnergeGrad;

    public float lowEnergeRatePercent = 10f;
    public ParticleSystem energeParticleEff;
    public Color lowEnergeColor;
    public Gradient lowEnergeGrad;
    

    private void Awake()
    {
        skillInfoUIArr = skillImgUIParent.GetComponentsInChildren<SkillInfoImage>();
        orgEnergeEffMaskScl = energeEffMask.localScale;

        lowEnergeRate = lowEnergeRatePercent * 0.01f;
        energeBarEffMainModule = energeParticleEff.main;
        energePsCOLT = energeParticleEff.colorOverLifetime;

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

        EventManager.StartListening("ChangeBody", (str, dead) =>
        {
            if(dead){} //boolŸ���ΰ� �˸�������
            skillInfoUIArr.ForEach(x => x.Unregister());
            SkillInfo[] skill = monsterSkillsDic[str];
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

            bool org = str == Global.OriginBodyID;

            if (org)
            {
                energeBarAndEff.first.SetActive(true);
                energeBarAndEff.first.transform.DOScaleX(1, 0.3f);
            }
            else
            {
                energeBarAndEff.first.transform.DOScaleX(0, 0.3f).OnComplete(() => energeBarAndEff.first.SetActive(false));
            }
        });
    }

    public void UpdateSkillKeyCode()
    {
        for(int i=0; i<skillInfoUIArr.Length; i++)
        {
            skillInfoUIArr[i].UpdateKeyCode();
        }
    }

    public void OnClickSkillButton(Sprite spr, string sName, string ex)
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

    private void UpdateEnergeBarUI()
    {
        if(energeBarAndEff.first.activeSelf)
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
}