using UnityEngine;

public static class ItemUseMng
{
    //private static Player player;
    
    public static void IncreaseCurrentHP(Player p, int value)
    {
        p.CurrentHp += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("HP " + value + " ȸ��");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseMaxHP(Player p, int value)
    {
        p.PlayerStat.additionalEternalStat.maxHp += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("�ִ� HP " + value + " ���");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseStr(Player p, int value)
    {
        p.PlayerStat.additionalEternalStat.damage += value;
        UIManager.Instance.InsertNoticeQueue("���ݷ� " + value + " ���");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }
}

public abstract class ItemAbil
{
    protected Player SlimePlayer
    {
        get => SlimeGameManager.Instance.Player;
    }

    public abstract void Use();
}

public class JellyBomb : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseStr(SlimePlayer,3);
    }
}

public class LizardGrilledwholemeat : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseMaxHP(SlimePlayer,10);
    }
}

public class MouseTailJelly : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseMaxHP(SlimePlayer, 5);
    }
}

public class SlimeJelly : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseStr(SlimePlayer, 1);
    }
}

public class Restorativeherb : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseCurrentHP(SlimePlayer, 40);
    }
}

public class Yellowherb : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseCurrentHP(SlimePlayer, 10);
    }
}

public class RecoveryPotion : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseCurrentHP(SlimePlayer, 80);
    }
}