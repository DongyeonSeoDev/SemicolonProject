using UnityEngine;

public static class ItemUseMng
{
    //private static Player player;
    
    public static void IncreaseCurrentHP(Player p, float value)
    {
        value *= p.PlayerStat.MaxHp * 0.01f;
        int iValue = Mathf.CeilToInt(value);
        p.GetHeal(iValue);
        UIManager.Instance.InsertNoticeQueue("HP " + iValue + "% 회복");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseMaxHP(Player p, float value)
    {
        p.PlayerStat.additionalEternalStat.maxHp += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("최대 HP " + value + " 상승");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseStr(Player p, int value)
    {
        p.PlayerStat.additionalEternalStat.minDamage += value;
        p.PlayerStat.additionalEternalStat.maxDamage += value;

        UIManager.Instance.InsertNoticeQueue("공격력 " + value + " 상승");
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