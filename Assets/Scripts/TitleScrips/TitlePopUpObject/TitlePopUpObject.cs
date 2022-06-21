using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class TitlePopUpObject : MonoBehaviour
{
    public EnableTitlePopUpObject lastEnabler = null;

    [SerializeField]
    protected CanvasGroup cvsg = null;

    public List<PopUpsObjs> popUpsObjsList = new List<PopUpsObjs>();

    protected bool fadeInDone = false;
    public bool canFadeOut = true;
    
    public virtual void Awake()
    {
        cvsg = GetComponent<CanvasGroup>();
    }
    public virtual void OnEnable()
    {
        FadeIn();
        popUpsObjsList.ForEach(t => {
            if (t.gameObject.activeSelf)
            {
                t.FadeIn();
            }
            else
            {
                t.gameObject.SetActive(true);
            }
        });
    }
    public virtual void OnDisable()
    {
       
    }
    public virtual void Update()
    {
        if(canFadeOut && (fadeInDone && (Input.GetKeyUp(KeyCode.Escape) || Input.GetKeyUp(KeyCode.Return))))
        {
            FadeOut();
        }
    }

    public virtual void FadeIn()
    {
        fadeInDone = false;
        cvsg.alpha = 0f;
        transform.localScale = SVector3.zeroPointSeven;

        transform.DOScale(Vector3.one, Global.fullScaleTransitionTime03).SetEase(Ease.OutBack).SetUpdate(true);
        cvsg.DOFade(1, Global.fullAlphaTransitionTime04).SetUpdate(true).OnComplete(() => fadeInDone = true);
    }

    public virtual void FadeOut()
    {
        fadeInDone = false;
        float time = Global.fullAlphaTransitionTime04;

        transform.DOScale(SVector3.zeroPointSeven, time).SetEase(Ease.InBack).SetUpdate(true);

        lastEnabler.PlayOnDisableSoundBox();
        cvsg.DOFade(0, time).SetUpdate(true).OnComplete(() => lastEnabler.Disable());
    }
}
