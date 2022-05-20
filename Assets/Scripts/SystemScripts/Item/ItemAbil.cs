using UnityEngine;

public class ItemUseMng
{
    public static Player PlayerObj => SlimeGameManager.Instance.Player;
    
    public static void IncreaseCurrentHP(float value)
    {
        value *= PlayerObj.PlayerStat.MaxHp * 0.01f;
        int iValue = Mathf.CeilToInt(value);
        PlayerObj.GetHeal(iValue);
        UIManager.Instance.RequestLogMsg("HP " + iValue + " ȸ��");
        UIManager.Instance.UpdatePlayerHPInInven();
        //EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void DecreaseCurrentHP(float value)
    {
        value *= PlayerObj.PlayerStat.MaxHp * 0.01f;
        int iValue = Mathf.FloorToInt(value);
        PlayerObj.GetDamage(iValue, PlayerObj.transform.position, Vector2.zero, Vector3.one, true, false, true);
        UIManager.Instance.RequestLogMsg("���ַ� ���ؼ� HP " + iValue + "%��ŭ �Ҿ����ϴ�.");
        UIManager.Instance.UpdatePlayerHPInInven();
    }

    public static void IncreaseMaxHP(float value)
    {
        PlayerObj.PlayerStat.additionalEternalStat.maxHp.statValue += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.RequestLogMsg("�ִ� HP " + value + " ���");
        PlayerObj.GetHeal((int)value);
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
        UIManager.Instance.UpdatePlayerHPInInven();
    }

    public static void IncreaseStr(float value)
    {
        PlayerObj.PlayerStat.additionalEternalStat.minDamage.statValue += value;
        PlayerObj.PlayerStat.additionalEternalStat.maxDamage.statValue += value;

        UIManager.Instance.RequestLogMsg("���ݷ� " + value + " ���");
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