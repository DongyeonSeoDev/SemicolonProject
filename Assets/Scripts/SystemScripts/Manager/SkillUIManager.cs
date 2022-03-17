using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoSingleton<SkillUIManager>
{
    private SkillInfoImage[] skillInfoUIArr;

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
    public Pair<GameObject, GameObject> energeBarAndEff;
    public Transform energeEffMask;
    public Image energeFill;
    private Vector3 orgEnergeEffMaskScl;

    private void Awake()
    {
        skillInfoUIArr = skillImgUIParent.GetComponentsInChildren<SkillInfoImage>();
        orgEnergeEffMaskScl = energeEffMask.localScale;

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
            if(dead){} //bool타입인걸 알리기위한
            skillInfoUIArr.ForEach(x => x.Unregister());
            SkillInfo[] skill = monsterSkillsDic[str];
            foreach(SkillInfo skillInfo in skill)
            {
                switch(skillInfo.skillType)
                {
                    case SkillType.ATTACK:
                        defaultSkill.Register(skillInfo);
                        break;
                    case SkillType.SPECIALATTACK:
                        specialSkill.Register(skillInfo);
                        break;
                    case SkillType.DRAIN:
                        drain.Register(skillInfo);
                        break;
                }
            }

            bool org = str == Global.OriginBodyID;

            energeBarAndEff.first.SetActive(org);
            energeBarAndEff.second.SetActive(org); 

            
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
        }
    }
}