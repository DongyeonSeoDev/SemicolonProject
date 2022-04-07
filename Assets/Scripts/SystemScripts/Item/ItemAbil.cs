using UnityEngine;

public class ItemUseMng
{
    public static Player PlayerObj => SlimeGameManager.Instance.Player;
    
    public static void IncreaseCurrentHP(float value)
    {
        value *= PlayerObj.PlayerStat.MaxHp * 0.01f;
        int iValue = Mathf.CeilToInt(value);
        PlayerObj.GetHeal(iValue);
        UIManager.Instance.InsertNoticeQueue("HP " + iValue + " 회복");
        //EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void DecreaseCurrentHP(float value)
    {
        value *= PlayerObj.PlayerStat.MaxHp * 0.01f;
        int iValue = Mathf.FloorToInt(value);
        PlayerObj.GetDamage(iValue, false, true);  
        UIManager.Instance.RequestLeftBottomMsg("저주로 인해서 HP " + iValue + "%만큼 잃었습니다.");
    }

    public static void IncreaseMaxHP(float value)
    {
        PlayerObj.PlayerStat.additionalEternalStat.maxHp += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("최대 HP " + value + " 상승");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseStr(int value)
    {
        PlayerObj.PlayerStat.additionalEternalStat.minDamage += value;
        PlayerObj.PlayerStat.additionalEternalStat.maxDamage += value;

        UIManager.Instance.InsertNoticeQueue("공격력 " + value + " 상승");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }
}

public abstract class ItemAbil
{
    public abstract void Use();
}

public class JellyBomb : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseStr(3);
    }
}

public class LizardGrilledwholemeat : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseMaxHP(10);
    }
}

public class MouseTailJelly : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseMaxHP(5);
    }
}

public class SlimeJelly : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseStr(1);
    }
}

public class Restorativeherb : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseCurrentHP(25);
    }
}

public class Yellowherb : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseCurrentHP( 10);
    }
}

public class RecoveryPotion : ItemAbil
{
    public override void Use()
    {
        ItemUseMng.IncreaseCurrentHP( 80);
    }
}