using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AcquisitionTutorialNotice : MonoBehaviour
{
    [SerializeField] private Text objNameTxt, explanationTxt, addiExpTxt;
    [SerializeField] private Image UIImg;

    public CanvasGroup cvsg;
    public RectTransform rt;

    public void Set(InitAcquisitionData data)
    {
        objNameTxt.text = $"<color=yellow>{data.objName}</color> È¹µæ";
        explanationTxt.text = data.explanation;
        addiExpTxt.text = data.addiExplanation;
        UIImg.sprite = data.UISpr;

        cvsg.alpha = 0;
        transform.localScale = SVector3.zeroPointSeven;

        cvsg.DOFade(1, 0.39f).SetUpdate(true);
        transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetUpdate(true);
    }
}
