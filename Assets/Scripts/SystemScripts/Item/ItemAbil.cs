using UnityEngine;

public static class ItemUseMng
{
    //private static Player player;

    public static void IncreaseMaxHP(Player p, int value)
    {
        p.PlayerStat.additionalEternalStat.maxHp += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("ÃÖ´ë HP " + value + " »ó½Â");
    }

    public static void IncreaseStr(Player p, int value)
    {
        p.PlayerStat.additionalEternalStat.damage += value;
        UIManager.Instance.InsertNoticeQueue("°ø°Ý·Â " + value + " »ó½Â");
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