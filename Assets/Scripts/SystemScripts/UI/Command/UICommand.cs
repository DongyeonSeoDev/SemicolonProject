using UnityEngine;
using UnityEngine.UI;

public abstract class UICommand
{
    protected SlimeGameManager Sgm => SlimeGameManager.Instance;

    public abstract void Execute();
}

public class SkillUICommand : UICommand
{
    private Image coolImg;
    private Text coolTxt;
    private int index;

    public SkillUICommand(Image img, Text txt, int index) 
    {
        coolImg = img;
        coolTxt = txt;  
        this.index = index;
    }

    public override void Execute()
    {
        if (Sgm.CurrentSkillDelayTimer[index] > 0)
        {
            if (!coolTxt.gameObject.activeSelf)
                coolTxt.gameObject.SetActive(true);

            coolImg.fillAmount = Sgm.CurrentSkillDelayTimer[index] / Sgm.SkillDelays[index];
            coolTxt.text = Sgm.CurrentSkillDelayTimer[index].ToString("F1");
        }
        else
        {
            if (coolTxt.gameObject.activeSelf)
            {
                coolTxt.gameObject.SetActive(false);
                coolImg.fillAmount = 0;
            }
        }
    }
}