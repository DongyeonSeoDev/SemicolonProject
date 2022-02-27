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
        
    }
}

public class SpecialAttackUICommand : UICommand
{
    public SpecialAttackUICommand(Image img, Text txt) : base(img, txt)
    {
    }
    public override void Execute()
    {
       
    }
}

public class DrainUICommand : UICommand
{
    public DrainUICommand(Image img, Text txt) : base(img, txt)
    {
    }
    public override void Execute()
    {
        
    }
}