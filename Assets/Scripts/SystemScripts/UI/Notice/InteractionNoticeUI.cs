using UnityEngine.UI;
using UnityEngine;

public class InteractionNoticeUI : MonoBehaviour
{
    private InteractionObj obj;

    [SerializeField] private TextColorRepeat tcr;

    public Text actionText;

    //[SerializeField] private AnimationCurve animCurve;

    public void Set(InteractionObj obj)
    {
        this.obj = obj;
        actionText.text = string.Concat(obj.action, '(', KeySetting.keyDict[KeyAction.INTERACTION].ToString(),')');
        UIManager.Instance.itrNoticeList.Add(this);

        tcr.Transition(true);
    }

    public void Set()
    {
        if(obj)
        {
            actionText.text = string.Concat(obj.action, '(', KeySetting.keyDict[KeyAction.INTERACTION].ToString(), ')');
        }
    }

    private void Update()
    {
        if (obj)
        {
            transform.position = Util.WorldToScreenPoint(obj.transform.position + obj.itrUIOffset);
        }
    }

    private void OnDisable()
    {
        if (obj)
        {
            UIManager.Instance.itrNoticeList.Remove(this);
            obj = null;
        }
        tcr.Transition(false);
    }
}
