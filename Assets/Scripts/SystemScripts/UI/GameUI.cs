using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Water
{
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

        private void OnEnable()
        {
            ActiveTransition(_UItype);
        }

        public virtual void ActiveTransition(UIType type)
        {
            switch (type)
            {
                case UIType.CHEF_FOODS_PANEL:
                    DOScale(true);
                    
                    break;

                case UIType.PRODUCTION_PANEL:
                    DOMove(true);
                    break;

                case UIType.INVENTORY:
                    cvsg.alpha = 0f;
                    cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(()=> UpdateUIStack());
                    childGameUI.ActiveTransition(_UItype);
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
            if(active)
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
            if(active)
            {
                cvsg.alpha = 0;
                cvsg.DOFade(1, 2.3f).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => UpdateUIStack());
            }
            else
            {
                cvsg.DOFade(0, 1).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
            }
        }

        #endregion
    }
}