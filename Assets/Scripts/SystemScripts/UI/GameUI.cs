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
                    cvsg.alpha = 0f;
                    rectTrm.anchoredPosition = new Vector2(originPos.x - 150f, originPos.y);

                    rectTrm.DOAnchorPos(originPos, Global.slideTransitionTime03).SetUpdate(true);
                    cvsg.DOFade(1, Global.fullAlphaTransitionTime04)
                    .SetUpdate(true).OnComplete(() => UpdateUIStack());
                    break;

                case UIType.INVENTORY:
                    cvsg.alpha = 0f;
                    cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(()=> UpdateUIStack());
                    childGameUI.ActiveTransition(_UItype);
                    break;

                case UIType.FOOD_DETAIL:
                    DOScale(true);
                    break;

                default:
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
                    rectTrm.DOAnchorPos(new Vector2(originPos.x - 150f, originPos.y), Global.slideTransitionTime03).SetUpdate(true);
                    cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
                    break;

                case UIType.INVENTORY:
                    childGameUI.InActiveTransition();
                    cvsg.DOFade(0, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => UpdateUIStack(false));
                    break;

                case UIType.FOOD_DETAIL:
                    DOScale(false);
                    break;

                default:
                    break;
            }
        }

        public void UpdateUIStack(bool add = true)
        {
            UIManager.Instance.UpdateUIStack(this, add);
        }

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
    }
}