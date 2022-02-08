using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class InteractionNoticeUI : MonoBehaviour
{
    private InteractionObj obj;

    public Text actionText;

    //[SerializeField] private AnimationCurve animCurve;
    private Sequence seq;

    private void Awake()
    {
        seq = DOTween.Sequence();
        seq.SetLoops(-1, LoopType.Restart);
        seq.Append(actionText.DOColor(Color.green, 1.5f));
        seq.AppendInterval(0.8f);
        seq.Append(actionText.DOColor(Color.white, 1.5f));
        seq.AppendInterval(0.8f);
    }

    public void Set(InteractionObj obj)
    {
        this.obj = obj;
        actionText.text = string.Concat(obj.action, '(', KeySetting.keyDict[KeyAction.INTERACTION].ToString(),')');
        UIManager.Instance.itrNoticeList.Add(this);

        seq.Play();
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
        UIManager.Instance.itrNoticeList.Remove(this);
        obj = null;
        seq.Pause();
    }
}
