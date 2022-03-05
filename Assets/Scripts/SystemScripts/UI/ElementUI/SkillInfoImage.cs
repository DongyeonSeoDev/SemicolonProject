using UnityEngine.UI;
using UnityEngine;

public class SkillInfoImage : MonoBehaviour
{
    public Triple<Image, Text, Image> skillImgCoolTxtImgTriple;
    public Text keyCodeTxt;
    public NameInfoFollowingCursor nifc;
    [SerializeField] Button skillBtn;
    [SerializeField] CanvasGroup cvsg;

    [SerializeField] private string skillEx;

    [SerializeField] private SkillType skillType;

    public CustomContentsSizeFilter ccsf;

    private UICommand skillUICmd;
    public bool Registered { get; set; }


    private void Awake()
    {
        skillBtn.onClick.AddListener(() => SkillUIManager.Instance.OnClickSkillButton(skillImgCoolTxtImgTriple.first.sprite, nifc.explanation, skillEx));
    }


    public void Register(SkillInfo info)
    {
        cvsg.alpha = 1;
        skillImgCoolTxtImgTriple.first.sprite = info.skillSpr;
        string skillName = info.skillName;
        skillEx = info.skillExplanation;

        switch (skillType)
        {
            case SkillType.ATTACK:
                skillUICmd = new DefaultAttackUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second);
                skillName += "(기본공격)";
                break;
            case SkillType.SPECIALATTACK:
                skillUICmd = new SpecialAttackUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second);
                skillName += "(특수공격1)";
                break;
            case SkillType.DRAIN:
                skillUICmd = new DrainUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second);
                skillName += "(특수공격2)";
                break;
        }

        nifc.explanation = skillName;
        Registered = true;
    }

    public void Unregister()
    {
        Registered = false;
        skillImgCoolTxtImgTriple.second.gameObject.SetActive(false);
        skillImgCoolTxtImgTriple.third.fillAmount = 0;
        skillImgCoolTxtImgTriple.first.sprite = SkillUIManager.Instance.emptySkillSpr;
        cvsg.alpha = 0.4f;
    }

    public void UpdateKeyCode()
    {
        keyCodeTxt.text = KeyCodeToString.GetString(KeySetting.GetKeyCode(Util.EnumParse<KeyAction>(skillType.ToString())));
        Util.DelayFunc(() => ccsf.UpdateSize(), 0.2f);
    }

    private void Update()
    {
        if(Registered)
        {
            skillUICmd.Execute();
        }
    }
}
