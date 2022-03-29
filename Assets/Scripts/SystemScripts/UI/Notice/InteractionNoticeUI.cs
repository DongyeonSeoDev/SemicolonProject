using UnityEngine.UI;
using UnityEngine;

public class InteractionNoticeUI : MonoBehaviour
{
    private InteractionObj obj;

    [SerializeField] private TextColorRepeat tcr;

    private RectTransform rectTr;

    public Text actionText;

    //[SerializeField] private AnimationCurve animCurve;

    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
    }

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

    private void LateUpdate()
    {
        if (obj)
        {
            //transform.position = Util.WorldToScreenPoint(obj.transform.position + obj.itrUIOffset);
            //rectTr.anchoredPosition = RectTransformUtility.WorldToScreenPoint(Util.MainCam, obj.transform.position + obj.itrUIOffset); --> 이 경우에는 anchor를 left bottom으로

            rectTr.anchoredPosition = Util.ScreenToWorldPosForScreenSpace(obj.transform.position + obj.itrUIOffset, Util.WorldCvs);
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
