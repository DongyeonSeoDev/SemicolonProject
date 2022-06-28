using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTitleObject : TitleMenuObject
{
    public override void DoWork()
    {
        base.DoWork();

        Debug.Log("DoWork!");
    }
}
