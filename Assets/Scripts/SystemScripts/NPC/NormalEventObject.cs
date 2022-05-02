using UnityEngine;

public class NormalEventObject : InteractionObj
{
    [SerializeField] private string interactionEventKey;

    [SerializeField] private FakeSpriteOutline fsOut;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interaction()
    {
        /*if (notInteractable) return;
        notInteractable = true;
        isHidenItrMark = true;*/

        interactionEventKey.TriggerEvent();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

        fsOut.gameObject.SetActive(on);
    }
}
