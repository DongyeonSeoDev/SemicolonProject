using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTitleObject : TitleObject
{
    public override void DoWork()
    {
        base.DoWork();

        Debug.Log("DoWork!");
    }
}
