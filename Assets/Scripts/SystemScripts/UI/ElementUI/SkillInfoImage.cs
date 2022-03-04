using UnityEngine.UI;
using UnityEngine;
using System;

public class SkillInfoImage : MonoBehaviour
{
    public Triple<Image, Text, Image> skillImgCoolTxtImgTriple;
    public Text keyCodeTxt;
    public NameInfoFollowingCursor nifc;
    [SerializeField] Button skillBtn;
    [SerializeField] CanvasGroup cvsg;

    [SerializeField] private SkillType skillType;

    private UICommand skillUICmd;
    public bool Registered { get; set; }


    private void Awake()
    {
        skillBtn.onClick.AddListener(() => SkillUIManager.Instance.OnClickSkillButton());
        Register(null, ""); //�ӽ���
    }


    public void Register(Sprite skillSpr, string skillName)
    {
        gameObject.SetActive(true);
        skillImgCoolTxtImgTriple.first.sprite = skillSpr;

        switch (skillType)
        {
            case SkillType.ATTACK:
                skillUICmd = new DefaultAttackUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second);
                skillName += "(�⺻����)";
                break;
            case SkillType.SPECIALATTACK:
                skillUICmd = new SpecialAttackUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second);
                skillName += "(Ư������)";
                break;
            case SkillType.DRAIN:
                skillUICmd = new DrainUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second);
                //skillName += "(������ ����)";
                break;
        }

        nifc.explanation = skillName;
        Registered = true;
    }

    public void Unregister()
    {
        Registered = false;
        gameObject.SetActive(false);
    }

    public void UpdateKeyCode()
    {
        keyCodeTxt.text = KeyCodeToString.GetString(KeySetting.GetKeyCode(Util.EnumParse<KeyAction>(skillType.ToString())));
    }

    private void Update()
    {
        if(Registered)
        {
            skillUICmd.Execute();
        }
    }
}
