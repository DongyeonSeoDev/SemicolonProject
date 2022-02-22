using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Water;

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
    private float hpFillEffectMaskCenterInitScale;
    //public ScreenToWorldPos hpFillEffectMaskObj;

    [Space(20)][Header("TopRight Btn Effects")]
    public GameObject inventoryBtnEffect;
    public GameObject statBtnEffect;
    //public GameObject mobColtBtnEffect;
    public GameObject changeableMobListBtnEffect;

    [Space(20)]
    [Header("채집 성공/실패 이펙트")]
    public Pair<GameObject, GameObject> pickupPlantEffects;  //채집 성공, 실패 이펙트

    private void Awake()
    {
        fillBackWidth = fillBackRect.rect.width;
        damagedHpUIEffectRect = damagedHpUIEffect.GetComponent<RectTransform>();
        hpFillEffectMaskCenterInitScale = hpFillEffectMaskCenter.localScale.x;

        PoolManager.CreatePool(pickupPlantEffects.first, transform, 2, "PickSuccessEff");
        PoolManager.CreatePool(pickupPlantEffects.second, transform, 2, "PickFailEff");

        EventManager.StartListening("PlayerRespawn", Respawn);

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

    private void Respawn(Vector2 unusedValue)
    {
        inventoryBtnEffect.SetActive(false);
        statBtnEffect.SetActive(false);
    }

    public void OnDamagedUIEffect(float rate)
    {
        damagedHpUIEffect.Stop();
        damagedHpUIEffectRect.anchoredPosition = new Vector2(fillBackWidth * rate, damagedHpUIEffectRect.anchoredPosition.y);
        damagedHpUIEffect.Play();
    }
    
    public void SetHPFillEffectScale(float rate) 
    {
        hpFillEffectMaskCenter.transform.localScale = new Vector3(hpFillEffectMaskCenterInitScale * rate, hpFillEffectMaskCenterInitScale, hpFillEffectMaskCenterInitScale);
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
            case UIType.CHANGEABLEMOBLIST:
                changeableMobListBtnEffect.SetActive(on);
                break;
        }
    }
}
