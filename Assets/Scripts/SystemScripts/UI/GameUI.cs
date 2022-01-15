using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Water
{
    public class GameUI : MonoBehaviour
    {
        public UIType _UItype;

        protected virtual void OnEnable()
        {
            ActiveTransition(_UItype);
        }

        public virtual void ActiveTransition(UIType type)
        {
            switch (type)
            {
                case UIType.CHEF_FOODS_PANEL:
                    UIManager.Instance.UpdateUIStack(this); //Test Code
                    break;

                case UIType.PRODUCTION_PANEL:
                    break;

                case UIType.INVENTORY:
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
                    UIManager.Instance.UpdateUIStack(this,false); //Test Code
                    break;

                case UIType.PRODUCTION_PANEL:
                    break;

                case UIType.INVENTORY:
                    break;

                default:
                    break;
            }
        }
    }
}