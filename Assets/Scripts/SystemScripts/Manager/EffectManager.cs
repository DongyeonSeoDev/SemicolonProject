using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectManager : MonoSingleton<EffectManager>
{
    private float fillBackWidth;
    private RectTransform damagedHpUIEffectRect;

    public ParticleSystem damagedHpUIEffect;
    public RectTransform fillBackRect;

    [Space(10)]
    [Header("HP Fill 이펙트")]

    //private float hpFillEffectStartX;

    public RectTransform hpFillEffect;
    public Transform hpFillEffectMaskCenter;
    //public ScreenToWorldPos hpFillEffectMaskObj;

    [Space(20)][Header("TopRight Btn Effects")]
    public GameObject inventoryBtnEffect;
    public GameObject statBtnEffect;

    private void Awake()
    {
        fillBackWidth = fillBackRect.rect.width;
        damagedHpUIEffectRect = damagedHpUIEffect.GetComponent<RectTransform>();
        //hpFillEffectStartX = hpFillEffect.anchoredPosition.x;
        //hpFillEffectMaskObj.screenPoint = new Vector2(fillBackRect.anchoredPosition.x - fillBackWidth * 0.5f, fillBackRect.anchoredPosition.y);
    }

    private void Start()
    {
        Global.AddMonoAction(Global.AcquisitionItem, item => {
            ((Item)item).FollowEffect();
            OnTopRightBtnEffect(UIType.INVENTORY, true);
        });
        Global.AddAction(Global.MakeFood, unusedValue => OnTopRightBtnEffect(UIType.INVENTORY, true));
    }

    public void OnDamagedUIEffect(float rate)
    {
        damagedHpUIEffectRect.anchoredPosition = new Vector2(fillBackWidth * rate, damagedHpUIEffectRect.rect.y);
        damagedHpUIEffect.Play();
    }
    
    public void SetHPFillEffectPos(float rate) //HPFill 이펙트의 위치를 HPFillAmount에 맞게 설정 (Mask옵젝 만들고 크기를 왼쪽으로 키워서 처리할 수도 있음)
    {
        hpFillEffectMaskCenter.transform.localScale = new Vector3(rate, 1, 1);
        //hpFillEffect.anchoredPosition = new Vector2(hpFillEffectStartX - fillBackWidth * (1f - rate), hpFillEffect.anchoredPosition.y);
        //if (rate == 0f) hpFillEffect.anchoredPosition = new Vector2(hpFillEffect.anchoredPosition.x - 50f, hpFillEffect.anchoredPosition.y);
    }

    public void OnTopRightBtnEffect(UIType type, bool on)
    {
        if (on)
        {
            if (UIManager.Instance.gameUIList[(int)type].gameObject.activeSelf) return;
        }
        switch (type)
        {
            case UIType.INVENTORY:
                inventoryBtnEffect.SetActive(on);
                break;
            case UIType.STAT:
                statBtnEffect.SetActive(on);
                break;
        }
    }
}
