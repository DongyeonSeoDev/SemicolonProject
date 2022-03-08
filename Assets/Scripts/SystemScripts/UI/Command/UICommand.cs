using UnityEngine;
using UnityEngine.UI;

public abstract class UICommand
{
    protected Image coolFill;
    protected Text coolText;

    protected SlimeGameManager Sgm => SlimeGameManager.Instance;

    public abstract void Execute();

    public UICommand() { }
    public UICommand(Image img, Text txt)
    {
        coolFill = img;
        coolText = txt;
    }
}

public class DefaultAttackUICommand : UICommand
{
    public DefaultAttackUICommand(Image img, Text txt) : base(img,txt)
    {
    }

    public override void Execute()
    {
        if (Sgm.CurrentSkillDelayTimer[0] > 0)
        {
            if (!coolText.gameObject.activeSelf)
                coolText.gameObject.SetActive(true);

            coolFill.fillAmount = Sgm.CurrentSkillDelayTimer[0] / Sgm.SkillDelays[0];
            coolText.text = Sgm.CurrentSkillDelayTimer[0].ToString("F1");
        }
        else
        {
            if (coolText.gameObject.activeSelf)
            {
                coolText.gameObject.SetActive(false);
                coolFill.fillAmount = 0;
            }
        }
    }
}

public class SpecialAttackUICommand : UICommand
{
    public SpecialAttackUICommand(Image img, Text txt) : base(img, txt)
    {
    }
    public override void Execute()
    {
        if (Sgm.CurrentSkillDelayTimer[1] > 0)
        {
            if (!coolText.gameObject.activeSelf)
                coolText.gameObject.SetActive(true);

            coolFill.fillAmount = Sgm.CurrentSkillDelayTimer[1] / Sgm.SkillDelays[1];
            coolText.text = Mathf.RoundToInt(Sgm.CurrentSkillDelayTimer[1]).ToString();
        }
        else
        {
            if (coolText.gameObject.activeSelf)
            {
                coolText.gameObject.SetActive(false);
                coolFill.fillAmount = 0;
            }
        }
    }
}

public class DrainUICommand : UICommand
{
    public DrainUICommand(Image img, Text txt) : base(img, txt)
    {
    }
    public override void Execute()
    {
        if (Sgm.CurrentSkillDelayTimer[2] > 0)
        {
            if (!coolText.gameObject.activeSelf)
                coolText.gameObject.SetActive(true);

            coolFill.fillAmount = Sgm.CurrentSkillDelayTimer[2] / Sgm.SkillDelays[2];
            coolText.text = Mathf.RoundToInt(Sgm.CurrentSkillDelayTimer[2]).ToString();
        }
        else
        {
            if (coolText.gameObject.activeSelf)
            {
                coolText.gameObject.SetActive(false);
                coolFill.fillAmount = 0;
            }
        }
    }
}