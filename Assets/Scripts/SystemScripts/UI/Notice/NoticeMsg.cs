using DG.Tweening;
using UnityEngine;
using TMPro;

public class NoticeMsg : MonoBehaviour  
{
    [SerializeField] RectTransform rectTrm;
    [SerializeField] private CanvasGroup cvsg;
    public TextMeshProUGUI msgTmp;

    private Sequence seq;

    public void Set(NoticeUISet nus)
    {
        seq.Kill();
        seq = DOTween.Sequence();

        rectTrm.anchoredPosition = Global.noticeMsgOriginRectPos - new Vector2(200, 0);
        cvsg.alpha = 0;

        msgTmp.text = nus.msg;
        msgTmp.fontSize = nus.fontSize;
        nus.endAction += () => gameObject.SetActive(false);

        /*if (!changeVertexGradient)
        {
            msgTmp.colorGradient = UIManager.Instance.noticeMsgGrd;
        }
        else
        {
            *//*msgTmp.colorGradientPreset.topLeft = grd[0];
            msgTmp.colorGradientPreset.topRight= grd[1];
            msgTmp.colorGradientPreset.bottomLeft = grd[2];
            msgTmp.colorGradientPreset.bottomRight = grd[3];*//*
        }*/

        msgTmp.colorGradient = !nus.changeVertexGradient ? UIManager.Instance.noticeMsgGrd : nus.vg;

        seq.Append(rectTrm.DOAnchorPos(Global.noticeMsgOriginRectPos, 0.3f).SetEase(Ease.InQuart))
            .Join(cvsg.DOFade(1, 0.3f)).AppendInterval(2f);
        seq.Append(rectTrm.DOAnchorPos(Global.noticeMsgOriginRectPos + new Vector2(200, 0), 0.3f).SetEase(Ease.OutQuart))
            .Join(cvsg.DOFade(0, 0.3f)).AppendCallback(() => nus.endAction());
        seq.SetUpdate(true).Play();
    }
}
