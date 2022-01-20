using UnityEngine;
using Water;

public class NameInfoFollowingCursor : UITransition
{

    [HideInInspector] public ItemSO data;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Transition(bool on)
    {
        if (on && transitionEnable) UIManager.Instance.SetCursorInfoUI(data.itemName);
        else UIManager.Instance.OffCursorInfoUI();
    }
}
