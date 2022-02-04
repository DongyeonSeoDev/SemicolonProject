using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    private RectTransform rectTrm;
    private Vector3 originPos;

    [SerializeField] private CanvasGroup cvsg;

    public UIType _UItype;

    public GameUI childGameUI;

    public void ResetPos() => rectTrm.anchoredPosition = originPos;

    private void Awake()
    {
        rectTrm = GetComponent<RectTransform>();
        originPos = rectTrm.anchoredPosition;
    }

    /* private void OnEnable()
     {
         ActiveTransition(_UItype);
     }*/

    public virtual void ActiveTransition()
    {
        gameObject.SetActive(true);
        switch (_UItype)
        {
            case UIType.CHEF_FOODS_PANEL:
                DOScale(true);

                break;

            case UIType.PRODUCTION_PANEL:
                DOMove(true);
                break;

            case UIType.INVENTORY:
                cvsg.alpha = 0f;
                cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack());
                childGameUI.ActiveTransition();
                break;

            case UIType.FOOD_DETAIL:
                DOScale(true);
                break;

            case UIType.ITEM_DETAIL:
                DOMove(true);
                break;

            case UIType.COMBINATION:
                cvsg.alpha = 0f;
                transform.localScale = Global.onePointSix;
                transform.DOScale(Vector3.one, Global.fullScaleTransitionTime03).SetEase(Ease.InExpo);
                cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack());
                break;

            case UIType.DEATH:
                DOFade(true);
                break;

            case UIType.CLEAR:
                DOFade(true);
                break;

            case UIType.KEYSETTING:
                DOMoveSequence(true);
                break;
            case UIType.RESOLUTION:
                DOMoveSequence(true);
                break;

            case UIType.SETTING:
                DOFadeAndDissolve(true);
                break;

            default:
                DOScale(true);
                break;
        }
    }

    public virtual void InActiveTransition()
    {
        switch (_UItype)
        {
            case UIType.CHEF_FOODS_PANEL:

                DOScale(false);

                break;

            case UIType.PRODUCTION_PANEL:
                DOMove(false);
                break;

            case UIType.INVENTORY:
                childGameUI.InActiveTransition();
                cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
                break;

            case UIType.FOOD_DETAIL:
                DOScale(false);
                break;

            case UIType.ITEM_DETAIL:
                DOMove(false);
                break;

            case UIType.COMBINATION:
                transform.DOScale(Global.onePointSix, Global.fullScaleTransitionTime03).SetEase(Ease.OutQuad);
                cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
                break;

            case UIType.DEATH:
                DOFade(false);
                break;

            case UIType.CLEAR:
                DOFade(false);
                break;

            case UIType.SETTING:
                DOFadeAndDissolve(false);
                break;

            case UIType.KEYSETTING:
                DOMoveSequence(false);
                break;
            case UIType.RESOLUTION:
                DOMoveSequence(false);
                break;

            default:
                DOScale(false);
                break;
        }
    }

    public void UpdateUIStack(bool add = true)
    {
        UIManager.Instance.UpdateUIStack(this, add);
    }

    #region Æ®À§´× ÇÔ¼ö
    protected void DOScale(bool active)
    {
        if (active)
        {
            cvsg.alpha = 0f;
            transform.localScale = Global.zeroPointSeven;

            transform.DOScale(Vector3.one, Global.fullScaleTransitionTime03).SetEase(Ease.OutBack).SetUpdate(true);
            cvsg.DOFade(1, Global.fullAlphaTransitionTime04)
            .SetUpdate(true).OnComplete(() => UpdateUIStack());
        }
        else
        {
            float time = Global.fullAlphaTransitionTime04;
            transform.DOScale(Global.zeroPointSeven, time).SetEase(Ease.InBack).SetUpdate(true);
            cvsg.DOFade(0, time).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
        }
    }

    protected void DOMove(bool active)
    {
        if (active)
        {
            cvsg.alpha = 0f;
            rectTrm.anchoredPosition = new Vector2(originPos.x - 150f, originPos.y);

            rectTrm.DOAnchorPos(originPos, Global.slideTransitionTime03).SetUpdate(true);
            cvsg.DOFade(1, Global.fullAlphaTransitionTime04)
            .SetUpdate(true).OnComplete(() => UpdateUIStack());
        }
        else
        {
            rectTrm.DOAnchorPos(new Vector2(originPos.x - 150f, originPos.y), Global.slideTransitionTime03).SetUpdate(true);
            cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
        }
    }

    protected void DOFade(bool active)
    {
        if (active)
        {
            cvsg.alpha = 0;
            cvsg.DOFade(1, 2.3f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => UpdateUIStack());
        }
        else
        {
            cvsg.DOFade(0, 1).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
        }
    }

    protected void DOFadeAndDissolve(bool active)
    {
        if (active)
        {
            DOScale(true);
            Material mat = GetComponent<Image>().material;
            float t = 0f;
            float calc = Time.deltaTime / 0.6f;

            Util.ExecuteFunc(() =>
            {
                t += calc;
                mat.SetFloat("_Fade", t);
            }, 0, 0.6f, this, null, () => mat.SetFloat("_Fade", 1));
        }
        else
        {
            DOScale(false);
            Material mat = GetComponent<Image>().material;
            float t = 1f;
            float calc = Time.deltaTime / 0.4f;

            Util.ExecuteFunc(() =>
            {
                t -= calc;
                mat.SetFloat("_Fade", t);
            }, 0, 0.4f, this, null, () => mat.SetFloat("_Fade", 0));
        }
    }

    protected void DOMoveSequence(bool active)
    {
        Sequence seq = DOTween.Sequence();
        if (active)
        {
            cvsg.alpha = 0;
            transform.localScale = Global.half;
            rectTrm.anchoredPosition = new Vector2(originPos.x + 600f, originPos.y);

            seq.Append(childGameUI.cvsg.DOFade(0.4f, 0.5f))
            .Join(childGameUI.transform.DOScale(Global.half, 0.5f))
            .Join(childGameUI.rectTrm.DOAnchorPos(new Vector2(childGameUI.originPos.x - 600f, childGameUI.originPos.y), 0.5f));

            seq.AppendInterval(0.2f);
            seq.Append(childGameUI.cvsg.DOFade(0, 0.4f));
            seq.Join(cvsg.DOFade(1, 0.5f)).Join(transform.DOScale(Vector3.one, 0.5f)).Join(rectTrm.DOAnchorPos(originPos, 0.3f));
            seq.AppendCallback(() => { childGameUI.gameObject.SetActive(false); UpdateUIStack(); }).SetUpdate(true).Play();
        }
        else
        {
            childGameUI.gameObject.SetActive(true);
            seq.Append(cvsg.DOFade(0.4f, 0.5f))
            .Join(transform.DOScale(Global.half, 0.5f))
            .Join(rectTrm.DOAnchorPos(new Vector2(originPos.x + 600f, originPos.y), 0.5f));

            seq.AppendInterval(0.2f);
            seq.Append(cvsg.DOFade(0, 0.4f));
            seq.Join(childGameUI.cvsg.DOFade(1, 0.5f)).Join(childGameUI.transform.DOScale(Vector3.one, 0.5f)).Join(childGameUI.rectTrm.DOAnchorPos(childGameUI.originPos, 0.3f));
            seq.AppendCallback(() => UpdateUIStack(false)).SetUpdate(true).Play();
        }
    }
    #endregion
}
