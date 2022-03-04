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

    private void Awake()
    {
        skillInfoUIArr = skillImgUIParent.GetComponentsInChildren<SkillInfoImage>();

        defaultSkill.Register(null, "몸 조각 던지기");
        specialSkill.Register(null, "돌진");
        drain.Register(null, "흡수");
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