using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuButton : UITransition  //메뉴에서 상단쪽에 있는 인벤 도감 스태 설정 선택버튼에 붙음
{
    public Color transitionColor;
    private Color originColor;

    private Image img;
    private Image childImg;

    public UIType uiType;

    public bool Selected { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        if (img == null)
        {
            img = GetComponent<Image>();
        }
        childImg = transform.GetChild(0).GetComponent<Image>();
        originColor = Color.white.SetColorAlpha(0);
    }

    public void OnSelected(bool onClick)
    {
        if(img == null)
        {
            img = GetComponent<Image>();    
        }

        if (!onClick)
        {
            UIManager.Instance.gameUIList[(int)uiType].gameObject.SetActive(false);
            img.color = img.color.SetColorAlpha(0f);
            childImg.transform.localScale = Vector3.one;
        }
        else
        {
            img.DOKill();
            childImg.DOKill();
            childImg.transform.localScale = SVector3.onePointTwo;
            img.color = img.color.SetColorAlpha(100f);
        }

        transitionEnable = !onClick;
        Selected = onClick;
    }

    public void OnClickBtn()
    {
        if (Selected) return;

        //UIManager.Instance.OnUIInteract(uiType);
        UIManager.Instance.OnClickMenuBtn(uiType);
    }

    public override void Transition(bool on)
    {
        if (transitionEnable)
        {
            img.DOColor(on ? transitionColor : originColor, 0.3f).SetUpdate(true);
            childImg.transform.DOScale(on ? SVector3.onePointTwo : Vector3.one, 0.2f).SetUpdate(true);

            if (on)
            {
                SoundManager.Instance.PlaySoundBox("UIMouseEnterSFX4");
            }
        }
    }
}
