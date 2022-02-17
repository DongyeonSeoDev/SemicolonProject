using UnityEngine;

public static class ItemUseMng
{
    //private static Player player;
    
    public static void IncreaseCurrentHP(Player p, int value)
    {
        p.CurrentHp += value;  //플레이어의 회복 함수를 이용해서 회복하고 그 안에서 UIUpdate와 최대 HP 넘지 못하도록 해야함  (사망시 HP가 완충되는 거 수정해야할듯. 부활할 때 완충되야해서)
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("HP " + value + " 회복");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseMaxHP(Player p, int value)
    {
        p.PlayerStat.additionalEternalStat.maxHp += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("최대 HP " + value + " 상승");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseStr(Player p, int value)
    {
        p.PlayerStat.additionalEternalStat.damage += value;
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