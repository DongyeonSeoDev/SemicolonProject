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
        //skillBtn.onClick.AddListener(() => SkillUIManager.Instance.OnClickSkillButton(skillImgCoolTxtImgTriple.first.sprite, nifc.explanation, skillEx));
        GetComponent<UIInfoDelay>().mouseOverEvent += () => SkillUIManager.Instance.OnClickSkillButton(skillImgCoolTxtImgTriple.first.sprite, nifc.explanation, skillEx);
    }


    public void Register(SkillInfo info)
    {
        cvsg.alpha = 1;
        skillImgCoolTxtImgTriple.first.sprite = info.skillSpr;
        string skillName = info.skillName;
        skillEx = info.skillExplanation;

        skillUICmd = new SkillUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second, (int)skillType);

        switch (skillType)
        {
            case SkillType.ATTACK:
                
                skillName += "(기본공격)";
                break;
            case SkillType.SPECIALATTACK1:
                
                skillName += "(특수공격1)";
                break;
            case SkillType.SPECIALATTACK2:
                
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
        ccsf.UpdateSizeDelay();
    }

    private void Update()
    {
        if(Registered)
        {
            skillUICmd.Execute();
        }
    }
}
