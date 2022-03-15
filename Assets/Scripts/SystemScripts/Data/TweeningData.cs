using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

//나중에 전체적으로 코드 리펙토링 필요함
public static class TweeningData    
{
    public static void UpdateUIStack(GameUI self,bool add = true)
    {
        UIManager.Instance.UpdateUIStack(self, add);
    }

    public static void DOScale(GameUIFields fields, bool active)
    {
        if (active)
        {
            fields.cvsg.alpha = 0f;
            fields.transform.localScale = SVector3.zeroPointSeven;

            fields.transform.DOScale(Vector3.one, Global.fullScaleTransitionTime03).SetEase(Ease.OutBack).SetUpdate(true);
            fields.cvsg.DOFade(1, Global.fullAlphaTransitionTime04)
            .SetUpdate(true).OnComplete(() => UpdateUIStack(fields.self));
        }
        else
        {
            float time = Global.fullAlphaTransitionTime04;
            fields.transform.DOScale(SVector3.zeroPointSeven, time).SetEase(Ease.InBack).SetUpdate(true);
            fields.cvsg.DOFade(0, time).SetUpdate(true).OnComplete(() => UpdateUIStack(fields.self, false));
        }
    }

    public static void DOMove(GameUIFields fields, bool active)
    {
        if (active)
        {
            fields.cvsg.alpha = 0f;
            fields.rectTrm.anchoredPosition = new Vector2(fields.originPos.x - 150f, fields.originPos.y);

            fields.rectTrm.DOAnchorPos(fields.originPos, Global.slideTransitionTime03).SetUpdate(true);
            fields.cvsg.DOFade(1, Global.fullAlphaTransitionTime04)
            .SetUpdate(true).OnComplete(() => UpdateUIStack(fields.self));
        }
        else
        {
            fields.rectTrm.DOAnchorPos(new Vector2(fields.originPos.x - 150f, fields.originPos.y), Global.slideTransitionTime03).SetUpdate(true);
            fields.cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(fields.self, false));
        }
    }

    public static void DOFade(GameUIFields fields, bool active)
    {
        if (active)
        {
            fields.cvsg.alpha = 0;
            fields.cvsg.DOFade(1, 2.3f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => UpdateUIStack(fields.self));
        }
        else
        {
            fields.cvsg.DOFade(0, 1).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => UpdateUIStack(fields.self, false));
        }
    }

    public static void DOFadeAndDissolve(GameUIFields fields, bool active)
    {
        if (active)
        {
            DOScale(fields,true);
            DissolveCtrl dc = fields.self.GetComponent<DissolveCtrl>();
            float t = 0f;
            float calc = Time.unscaledDeltaTime / 0.6f;

            Util.ExecuteFunc(() =>
            {
                t += calc;
                dc.SetFade(t);
            }, 0, 0.6f, fields.self, null, () => dc.SetFade(1), true);
        }
        else
        {
            DOScale(fields, false);
            DissolveCtrl dc = fields.self.GetComponent<DissolveCtrl>();
            float t = 1f;
            float calc = Time.unscaledDeltaTime / 0.4f;

            Util.ExecuteFunc(() =>
            {
                t -= calc;
                dc.SetFade(t);
            }, 0, 0.4f, fields.self, null, () => dc.SetFade(0), true);
        }
    }

    public static void DOMoveSequence(GameUIFields fields, bool active)
    {
        Sequence seq = DOTween.Sequence();
        if (active)
        {
            fields.cvsg.alpha = 0;
            fields.transform.localScale = SVector3.half;
            fields.rectTrm.anchoredPosition = new Vector2(fields.originPos.x + 600f, fields.originPos.y);

            seq.Append(fields.childGameUI.UIFields.cvsg.DOFade(0.4f, 0.5f))
            .Join(fields.childGameUI.transform.DOScale(SVector3.half, 0.5f))
            .Join(fields.childGameUI.UIFields.rectTrm.DOAnchorPos(new Vector2(fields.childGameUI.UIFields.originPos.x - 600f, fields.childGameUI.UIFields.originPos.y), 0.5f));

            seq.AppendInterval(0.2f);
            seq.Append(fields.childGameUI.UIFields.cvsg.DOFade(0, 0.4f));
            seq.Join(fields.cvsg.DOFade(1, 0.5f)).Join(fields.transform.DOScale(Vector3.one, 0.5f)).Join(fields.rectTrm.DOAnchorPos(fields.originPos, 0.3f));
            seq.AppendCallback(() => { fields.childGameUI.gameObject.SetActive(false); UpdateUIStack(fields.self); }).SetUpdate(true).Play();
        }
        else
        {
            fields.childGameUI.gameObject.SetActive(true);
            seq.Append(fields.cvsg.DOFade(0.4f, 0.5f))
            .Join(fields.transform.DOScale(SVector3.half, 0.5f))
            .Join(fields.rectTrm.DOAnchorPos(new Vector2(fields.originPos.x + 600f, fields.originPos.y), 0.5f));

            seq.AppendInterval(0.2f);
            seq.Append(fields.cvsg.DOFade(0, 0.4f));
            seq.Join(fields.childGameUI.UIFields.cvsg.DOFade(1, 0.5f)).Join(fields.childGameUI.transform.DOScale(Vector3.one, 0.5f)).Join(fields.childGameUI.UIFields.rectTrm.DOAnchorPos(fields.childGameUI.UIFields.originPos, 0.3f));
            seq.AppendCallback(() => UpdateUIStack(fields.self,false)).SetUpdate(true).Play();
        }
    }

    public static void DOQuaternion(GameUIFields fields, bool active)
    {
        CanvasGroup chCvsg;
        if(active)
        {
            chCvsg = fields.transform.GetChild(0).GetComponent<CanvasGroup>();
            fields.cvsg.alpha = 0;
            fields.transform.rotation = SQuaternion.Y90;
            chCvsg.alpha = 0;
            fields.childGameUI.transform.DORotateQuaternion(SQuaternion.Y90, 0.3f).OnComplete(() =>
            {
                fields.childGameUI.UIFields.cvsg.alpha = 0;
                fields.cvsg.alpha = 1;
                fields.transform.DORotateQuaternion(SQuaternion.Y180, 0.3f).OnComplete(() =>
                {
                    fields.transform.rotation = Quaternion.identity;
                    chCvsg.DOFade(1, 0.2f).OnComplete(() => UpdateUIStack(fields.self)).SetUpdate(true);
                }).SetUpdate(true);
            }).SetUpdate(true);
        }
        else
        {
            chCvsg = fields.childGameUI.transform.GetChild(0).GetComponent<CanvasGroup>();
            chCvsg.alpha = 0;
            
            fields.transform.DORotateQuaternion(SQuaternion.Y90, 0.3f).OnComplete(() =>
            {
                fields.cvsg.alpha = 0;
                fields.childGameUI.UIFields.cvsg.alpha = 1;
                fields.childGameUI.transform.DORotateQuaternion(Quaternion.identity, 0.3f).OnComplete(() =>
                {
                    chCvsg.DOFade(1, 0.2f).OnComplete(() => UpdateUIStack(fields.self,false)).SetUpdate(true);
                }).SetUpdate(true);
            }).SetUpdate(true);
        }
    }
}
