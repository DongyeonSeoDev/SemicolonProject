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

    [SerializeField] private Pair<string, SkillInfo[]> playerOriginBodySkills;
    [SerializeField] private List<Pair<Enemy.EnemyType, SkillInfo[]>> monsterSkillsList;
    private Dictionary<string, SkillInfo[]> monsterSkillsDic = new Dictionary<string, SkillInfo[]>();

    private void Awake()
    {
        skillInfoUIArr = skillImgUIParent.GetComponentsInChildren<SkillInfoImage>();

        monsterSkillsDic.Add(playerOriginBodySkills.first, playerOriginBodySkills.second);
        for(int i= 0; i < monsterSkillsList.Count; i++)
        {
            monsterSkillsDic.Add(monsterSkillsList[i].first.ToString(), monsterSkillsList[i].second);
        }

        defaultSkill.Register(playerOriginBodySkills.second[0]);
        specialSkill.Register(playerOriginBodySkills.second[1]);
        drain.Register(playerOriginBodySkills.second[2]);

        EventManager.StartListening("ChangeBody", (str, dead) =>
        {
            if(dead){} //bool타입인걸 알리기위한
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
}