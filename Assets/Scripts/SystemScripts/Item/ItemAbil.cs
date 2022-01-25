using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemAbil
{
    /*protected Stat SlimePlayer
    {
        get => SlimeGameManager.Instance.PlayerStat;
    }*/

    public abstract void Use();
}

public class JellyBomb : ItemAbil
{
    public override void Use()
    {
        //공+3
    }
}

public class LizardGrilledwholemeat : ItemAbil
{
    public override void Use()
    {
        //체 +10
    }
}

public class MouseTailJelly : ItemAbil
{
    public override void Use()
    {
       //체 +5
    }
}

public class SlimeJelly : ItemAbil
{
    public override void Use()
    {
        //공 +1
    }
}