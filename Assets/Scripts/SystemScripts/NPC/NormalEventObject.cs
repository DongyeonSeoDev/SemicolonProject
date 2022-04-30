using UnityEngine;

public class NormalEventObject : InteractionObj
{
    [SerializeField] private string interactionEventKey;

    [SerializeField] private FakeSpriteOutline fsOut;

    public override void Interaction()
    {
        if (notInteractable) return;

        notInteractable = true;
        interactionEventKey.TriggerEvent();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

        fsOut.gameObject.SetActive(on);
    }
}
