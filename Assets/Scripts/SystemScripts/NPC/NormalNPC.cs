using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalNPC : NPC
{



    public override void Interaction()
    {
        TalkManager.Instance.SetTalkData(npcInfo, transform);
    }

    public override void SetUI(bool on)
    {
        base.SetUI(on);
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
    }
}
