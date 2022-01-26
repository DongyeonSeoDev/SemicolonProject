using UnityEngine;
using Water;

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
        SlimePlayer.PlayerStat.additionalEternalStat.damage += 3;
    }
}

public class LizardGrilledwholemeat : ItemAbil
{
    public override void Use()
    {
        SlimePlayer.PlayerStat.additionalEternalStat.hp += 10;
        UIManager.Instance.UpdatePlayerHPUI();
    }
}

public class MouseTailJelly : ItemAbil
{
    public override void Use()
    {
        SlimePlayer.PlayerStat.additionalEternalStat.hp += 5;
        UIManager.Instance.UpdatePlayerHPUI();
    }
}

public class SlimeJelly : ItemAbil
{
    public override void Use()
    {
        SlimePlayer.PlayerStat.additionalEternalStat.damage += 1;
    }
}