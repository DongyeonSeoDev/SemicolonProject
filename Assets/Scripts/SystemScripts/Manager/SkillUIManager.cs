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

    public Sprite emptySkillSpr; //�� ������ ���� ��������Ʈ (��ų)

    private void Awake()
    {
        skillInfoUIArr = skillImgUIParent.GetComponentsInChildren<SkillInfoImage>();

        defaultSkill.Register(null, "�� ���� ������");
        specialSkill.Register(null, "����");
        drain.Register(null, "���");
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