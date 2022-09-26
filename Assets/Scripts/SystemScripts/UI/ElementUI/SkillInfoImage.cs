using UnityEngine.UI;
using UnityEngine;

public class SkillInfoImage : MonoBehaviour
{
    public Triple<Image, Text, Image> skillImgCoolTxtImgTriple;
    public Text keyCodeTxt;
    public NameInfoFollowingCursor nifc;
    public UIScale us;
    [SerializeField] CanvasGroup cvsg;

    [SerializeField] private string skillEx;

    [SerializeField] private SkillType skillType;
    public SkillType _SkillType => skillType;

    public CustomContentsSizeFilter ccsf;

    [SerializeField] private UIInfoDelay UIInfoDelayScr;

    private UICommand skillUICmd;
    public bool Registered { get; set; }
    public bool DisableSlot { get; private set; }


    private void Awake()
    {
        UIInfoDelayScr.mouseOverEvent += () => SkillUIManager.Instance.OnClickSkillButton(skillImgCoolTxtImgTriple.first.sprite, nifc.explanation, skillEx);
    }


    public void Register(SkillInfo info)
    {
        cvsg.alpha = !DisableSlot ? 1 : 0.3f;
        skillImgCoolTxtImgTriple.first.sprite = info.skillSpr;
        string skillName = info.skillName;
        skillEx = info.skillExplanation;

        skillUICmd = new SkillUICommand(skillImgCoolTxtImgTriple.third, skillImgCoolTxtImgTriple.second, (int)skillType);

        switch (skillType)
        {
            case SkillType.ATTACK:
                
                skillName += "(�⺻����)";
                break;
            case SkillType.SPECIALATTACK1:
                
                skillName += "(Ư������1)";
                break;
            case SkillType.SPECIALATTACK2:
                
                skillName += "(Ư������2)";
                break;
        }

        nifc.explanation = skillName;
        Registered = true;
        UIInfoDelayScr.transitionEnable = true;
        us.transitionEnable = true;
        nifc.transitionEnable = true;
    }

    public void Unregister()
    {
        Registered = false;
        skillImgCoolTxtImgTriple.second.gameObject.SetActive(false);
        skillImgCoolTxtImgTriple.third.fillAmount = 0;
        skillImgCoolTxtImgTriple.first.sprite = SkillUIManager.Instance.emptySkillSpr;
        cvsg.alpha = !DisableSlot ? 0.4f : 0.3f;
        UIInfoDelayScr.transitionEnable = false;
        us.transitionEnable = false;
        nifc.transitionEnable = false;
    }

    public void SetEnableSlot(bool on) //�ش� ���Կ� ��ų�� Ȱ��ȭ �Ǿ��ֵ� �ƴϵ� �ߵ� ���ϰ� ������ ó��
    {
        cvsg.alpha = on ? (Registered ? 1 : 0.4f) : 0.3f;
        UIInfoDelayScr.transitionEnable = on;
        DisableSlot = !on;
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
