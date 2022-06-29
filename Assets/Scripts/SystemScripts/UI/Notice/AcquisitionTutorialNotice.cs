using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AcquisitionTutorialNotice : MonoBehaviour
{
    [SerializeField] private Text objNameTxt, explanationTxt, addiExpTxt;
    [SerializeField] private Image UIImg;

    public CanvasGroup cvsg;
    public RectTransform rt;

    private System.Action closeAction;

    public void Set(InitAcquisitionData data)
    {
        objNameTxt.text = $"<color=yellow>{data.objName}</color> È¹µæ";
        explanationTxt.text = data.explanation;
        addiExpTxt.text = data.addiExplanation;
        UIImg.sprite = data.UISpr;
        closeAction = () => data.endEvent.Invoke();

        cvsg.alpha = 0;
        transform.localScale = SVector3.zeroPointSeven;

        cvsg.DOFade(1, 0.39f).SetUpdate(true);
        transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void Close()
    {
        transform.DOScale(SVector3.zeroPointSeven, 0.4f).SetEase(Ease.InBack).SetUpdate(true);
        cvsg.DOFade(0, 0.41f).SetUpdate(true).OnComplete(() =>
        {
            KeyActionManager.Instance.enableProcessGainUINotice = true;
            TimeManager.TimeResume();
            closeAction?.Invoke();
            gameObject.SetActive(false);
        });
    }
}
