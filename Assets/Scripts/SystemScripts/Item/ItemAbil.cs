using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}

public class MouseTailJelly : ItemAbil
{
    public override void Use()
    {
        SlimePlayer.PlayerStat.additionalEternalStat.hp += 5;
    }
}

public class SlimeJelly : ItemAbil
{
    public override void Use()
    {
        SlimePlayer.PlayerStat.additionalEternalStat.damage += 1;
    }
}