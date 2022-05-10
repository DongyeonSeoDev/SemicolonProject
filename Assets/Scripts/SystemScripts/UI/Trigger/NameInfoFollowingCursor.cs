
[UnityEngine.RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]
public class NameInfoFollowingCursor : UITransition
{
    public string explanation;

    public int fontSize = 30;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Transition(bool on)
    {
        if (on && transitionEnable) UIManager.Instance.SetCursorInfoUI(explanation, fontSize);
        else UIManager.Instance.OffCursorInfoUI();
    }
}
