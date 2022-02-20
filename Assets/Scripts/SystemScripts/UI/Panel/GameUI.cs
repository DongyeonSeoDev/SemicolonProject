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

    private GameUIFields gameUIFields;

    public GameUIFields UIFields { get { return gameUIFields; } }

    public void ResetPos() => rectTrm.anchoredPosition = originPos;

    private void Awake()
    {
        rectTrm = GetComponent<RectTransform>();
        originPos = rectTrm.anchoredPosition;
        gameUIFields = new GameUIFields() 
        { childGameUI = childGameUI, cvsg = cvsg, originPos = originPos, rectTrm = rectTrm, _UItype = _UItype, transform = transform, self = this };
    }

    /* private void OnEnable()
     {
         ActiveTransition(_UItype);
     }*/

    public virtual void ActiveTransition()
    {
        gameObject.SetActive(true);
        UIManager.Instance.uiTweeningDic[_UItype] = true;
        switch (_UItype)
        {
            case UIType.CHEF_FOODS_PANEL:
                //DOScale(true);
                TweeningData.DOScale(gameUIFields, true);
                break;

            case UIType.PRODUCTION_PANEL:
                //DOMove(true);
                TweeningData.DOMove(gameUIFields, true);
                break;

            case UIType.INVENTORY:
                cvsg.alpha = 0f;
                cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack());
                childGameUI.ActiveTransition();
                break;

            case UIType.FOOD_DETAIL:
                //DOScale(true);
                TweeningData.DOScale(gameUIFields, true);
                break;

            case UIType.ITEM_DETAIL:
                //DOMove(true);
                TweeningData.DOMove(gameUIFields, true);
                break;

            case UIType.COMBINATION:
                cvsg.alpha = 0f;
                transform.localScale = SVector3.onePointSix;
                transform.DOScale(Vector3.one, Global.fullScaleTransitionTime03).SetEase(Ease.InExpo).SetUpdate(true);
                cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack());
                break;

            case UIType.DEATH:
                //DOFade(true);
                TweeningData.DOFade(gameUIFields, true);
                break;

            case UIType.CLEAR:
                //DOFade(true);
                TweeningData.DOFade(gameUIFields, true);
                break;

            case UIType.KEYSETTING:
                //DOMoveSequence(true);
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;
            case UIType.RESOLUTION:
                //DOMoveSequence(true);
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;

            case UIType.SOUND:
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;

            case UIType.HELP:
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;

            case UIType.CREDIT:
                TweeningData.DOMoveSequence(gameUIFields, true);
                break;

            case UIType.SETTING:
                //DOFadeAndDissolve(true);
                TweeningData.DOFadeAndDissolve(gameUIFields, true);
                break;

            default:
                //DOScale(true);
                TweeningData.DOScale(gameUIFields, true);
                break;
        }
    }

    public virtual void InActiveTransition()
    {
        UIManager.Instance.uiTweeningDic[_UItype] = true;
        switch (_UItype)
        {
            case UIType.CHEF_FOODS_PANEL:
                //DOScale(false);
                TweeningData.DOScale(gameUIFields, false);
                break;

            case UIType.PRODUCTION_PANEL:
                //DOMove(false);
                TweeningData.DOMove(gameUIFields, false);
                break;

            case UIType.INVENTORY:
                childGameUI.InActiveTransition();
                cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
                break;

            case UIType.FOOD_DETAIL:
                //DOScale(false);
                TweeningData.DOScale(gameUIFields, false);
                break;

            case UIType.ITEM_DETAIL:
                //DOMove(false);
                TweeningData.DOMove(gameUIFields, false);
                break;

            case UIType.COMBINATION:
                transform.DOScale(SVector3.onePointSix, Global.fullScaleTransitionTime03).SetEase(Ease.OutQuad).SetUpdate(true);
                cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
                break;

            case UIType.DEATH:
                //DOFade(false);
                TweeningData.DOFade(gameUIFields, false);
                break;

            case UIType.CLEAR:
                //DOFade(false);
                TweeningData.DOFade(gameUIFields, false);
                break;

            case UIType.SETTING:
                //DOFadeAndDissolve(false);
                TweeningData.DOFadeAndDissolve(gameUIFields, false);
                break;

            case UIType.KEYSETTING:
                //DOMoveSequence(false);
                TweeningData.DOMoveSequence(gameUIFields, false);
                break;
            case UIType.RESOLUTION:
                //DOMoveSequence(false);
                TweeningData.DOMoveSequence(gameUIFields, false);
                break;

            case UIType.SOUND:
                TweeningData.DOMoveSequence(gameUIFields, false);
                break;

            case UIType.HELP:
                TweeningData.DOMoveSequence(gameUIFields, false);
                break;

            case UIType.CREDIT:
                TweeningData.DOMoveSequence(gameUIFields, false);
                break;

            default:
                //DOScale(false);
                TweeningData.DOScale(gameUIFields, false);
                break;
        }
    }

    public void UpdateUIStack(bool add = true)
    {
        UIManager.Instance.UpdateUIStack(this, add);
    }

    #region 주석
    /* #region 트위닝 함수
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
             }, 0, 0.6f, this, null, () => mat.SetFloat("_Fade", 1), true);
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
             }, 0, 0.4f, this, null, () => mat.SetFloat("_Fade", 0), true);
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
     #endregion*/
    #endregion
}
