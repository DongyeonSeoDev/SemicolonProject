using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageDoor : InteractionObj
{
    private SpriteRenderer spr;

    public FakeSpriteOutline fsOut;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
    }

    public override void Interaction()
    {
        
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

        fsOut.gameObject.SetActive(on);
    }
}
