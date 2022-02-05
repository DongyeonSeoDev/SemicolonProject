using DG.Tweening;
using UnityEngine;
using TMPro;

public class NoticeMsg : MonoBehaviour  
{
    [SerializeField] RectTransform rectTrm;
    [SerializeField] private CanvasGroup cvsg;
    public TextMeshProUGUI msgTmp;

    private Sequence seq;

    public void Set(string msg, float fontSize, Color[] grd, System.Action endAction)
    {
        seq.Kill();
        seq = DOTween.Sequence();

        rectTrm.anchoredPosition = Global.noticeMsgOriginRectPos - new Vector2(200, 0);
        cvsg.alpha = 0;

        msgTmp.text = msg;
        msgTmp.fontSize = fontSize;
        endAction += () => gameObject.SetActive(false);

        if (grd == null)
        {
            msgTmp.colorGradient = UIManager.Instance.noticeMsgGrd;
        }
        else
        {
            msgTmp.colorGradientPreset.topLeft = grd[0];
            msgTmp.colorGradientPreset.topRight= grd[1];
            msgTmp.colorGradientPreset.bottomLeft = grd[2];
            msgTmp.colorGradientPreset.bottomRight = grd[3];
        }


        seq.Append(rectTrm.DOAnchorPos(Global.noticeMsgOriginRectPos, 0.3f).SetEase(Ease.InQuart))
            .Join(cvsg.DOFade(1, 0.3f)).AppendInterval(2f);
        seq.Append(rectTrm.DOAnchorPos(Global.noticeMsgOriginRectPos + new Vector2(200, 0), 0.3f).SetEase(Ease.OutQuart))
            .Join(cvsg.DOFade(0, 0.3f)).AppendCallback(() => endAction());
        seq.SetUpdate(true).Play();
    }
}
