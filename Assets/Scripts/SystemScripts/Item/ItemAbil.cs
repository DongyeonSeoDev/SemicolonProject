using UnityEngine;

public static class ItemUseMng
{
    //private static Player player;
    
    public static void IncreaseCurrentHP(float value)
    {
        value *= SlimeGameManager.Instance.Player.PlayerStat.MaxHp * 0.01f;
        int iValue = Mathf.CeilToInt(value);
        SlimeGameManager.Instance.Player.GetHeal(iValue);
        UIManager.Instance.InsertNoticeQueue("HP " + iValue + "% ȸ��");
        //EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void DecreaseCurrentHP(float value)
    {
        value *= SlimeGameManager.Instance.Player.PlayerStat.MaxHp * 0.01f;
        int iValue = Mathf.FloorToInt(value);
        SlimeGameManager.Instance.Player.GetDamage(iValue, false, true);  //�̰� �����ϰ� �ּ� Ǯ��� ��
        UIManager.Instance.RequestLeftBottomMsg("���ַ� ���ؼ� HP " + iValue + "%��ŭ �Ҿ����ϴ�.");
    }

    public static void IncreaseMaxHP(float value)
    {
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp += value;
        UIManager.Instance.UpdatePlayerHPUI();
        UIManager.Instance.InsertNoticeQueue("�ִ� HP " + value + " ���");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }

    public static void IncreaseStr(int value)
    {
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage += value;
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage += value;

        UIManager.Instance.InsertNoticeQueue("���ݷ� " + value + " ���");
        EffectManager.Instance.OnTopRightBtnEffect(UIType.STAT, true);
    }
}

public abstract class ItemAbil
{
    /*protected Player SlimePlayer
    {
        get => SlimeGameManager.Instance.Player;
    }*/

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
        ItemUseMng.IncreaseCurrentHP( 40);
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