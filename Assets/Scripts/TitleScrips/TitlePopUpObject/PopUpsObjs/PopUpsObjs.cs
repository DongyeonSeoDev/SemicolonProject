using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class PopUpsObjs : MonoBehaviour
{
    protected RectTransform rectTrm = null;
    protected CanvasGroup cvsg = null;
    protected Vector3 originPos;

    public virtual void Awake()
    {
        rectTrm = GetComponent<RectTransform>();
        originPos = rectTrm.anchoredPosition;
        cvsg = GetComponent<CanvasGroup>();
    }

    public virtual void OnEnable()
    {
        FadeIn();
    }
    public virtual void OnDisable()
    {
        
    }
    public virtual void FadeIn()
    {
        rectTrm.anchoredPosition = originPos - new Vector3(100, 0);
        cvsg.alpha = 0.2f;

        rectTrm.DOAnchorPos(originPos, 0.3f).SetUpdate(true);
        cvsg.DOFade(1, 0.33f).SetUpdate(true);
    }
    public virtual void FadeOut()
    {
       
    }
}
