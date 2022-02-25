using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Water;
using TMPro;
using DG.Tweening;

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

    #region damage text effect related
    [Header("데미지 텍스트")]
    public Pair<GameObject, Transform> damageTextPair;

    [System.Serializable]
    public class DamageTextVG
    {
        public VertexGradient normal;
        public VertexGradient cri;
    }

    public DamageTextVG playerDamagedTextVG;
    public DamageTextVG enemyDamagedTextVG;

    public AnimationCurve damageTxtScaleCurve;
    #endregion

    private void Awake()
    {
        fillBackWidth = fillBackRect.rect.width;
        damagedHpUIEffectRect = damagedHpUIEffect.GetComponent<RectTransform>();
        hpFillEffectMaskCenterInitScale = hpFillEffectMaskCenter.localScale.x;

        PoolManager.CreatePool(pickupPlantEffects.first, transform, 2, "PickSuccessEff");
        PoolManager.CreatePool(pickupPlantEffects.second, transform, 2, "PickFailEff");
        PoolManager.CreatePool(damageTextPair.first, damageTextPair.second, 4, "DamageTextEff");

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

    public void OnDamagedUIEffect(float rate) //Damage Particle Effect of HP UI
    {
        damagedHpUIEffect.Stop();
        damagedHpUIEffectRect.anchoredPosition = new Vector2(fillBackWidth * rate, damagedHpUIEffectRect.anchoredPosition.y);
        damagedHpUIEffect.Play();
    }
    
    public void SetHPFillEffectScale(float rate)  //Mask Obj of HPFill Effect Scale change
    {
        hpFillEffectMaskCenter.transform.localScale = new Vector3(hpFillEffectMaskCenterInitScale * rate, hpFillEffectMaskCenterInitScale, hpFillEffectMaskCenterInitScale);
        //hpFillEffect.anchoredPosition = new Vector2(hpFillEffectStartX - fillBackWidth * (1f - rate), hpFillEffect.anchoredPosition.y);
        //if (rate == 0f) hpFillEffect.anchoredPosition = new Vector2(hpFillEffect.anchoredPosition.x - 50f, hpFillEffect.anchoredPosition.y);
    }

    public void OnTopRightBtnEffect(UIType type, bool on) //Top Right Button Effects Active or Inactive
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

    public void OnDamaged(int damage, bool critical, bool isEnemy, Vector2 pos) //When monster or player receive damage, Shows damage text effect UI.
    {
        TextMeshProUGUI tmp = PoolManager.GetItem<TextMeshProUGUI>("DamageTextEff");
        tmp.color = Color.white;
        tmp.transform.localScale = !critical ? Vector3.one : SVector3.onePointTwo;
        tmp.SetText(damage.ToString());
        tmp.colorGradient = isEnemy ? (critical ? enemyDamagedTextVG.cri : enemyDamagedTextVG.normal) : (critical?playerDamagedTextVG.cri:playerDamagedTextVG.normal);
        tmp.transform.position = pos + Vector2.up;
        tmp.transform.SetAsLastSibling();

        //tmp.transform.DOMoveY(2.5f, 0.8f).SetRelative();
        tmp.transform.DOMove(tmp.transform.position + Global.damageTextMove, 0.8f);
        tmp.transform.DOScale(!critical ? SVector3.zeroPointSeven : Vector3.one, 1f).SetEase(damageTxtScaleCurve);
        tmp.DOColor(Color.clear, 1.1f).SetEase(damageTxtScaleCurve).OnComplete(() => tmp.gameObject.SetActive(false));
    }
    public void OnDamaged(int damage, Vector2 pos, Vector3 scale, VertexGradient textColor)
    {
        TextMeshProUGUI tmp = PoolManager.GetItem<TextMeshProUGUI>("DamageTextEff");
        tmp.color = Color.white;
        tmp.transform.localScale = scale;
        tmp.SetText(damage.ToString());
        tmp.colorGradient = textColor;
        tmp.transform.position = pos + Vector2.up;
        tmp.transform.SetAsLastSibling();

        //tmp.transform.DOMoveY(2.5f, 0.8f).SetRelative();
        tmp.transform.DOMove(tmp.transform.position + Global.damageTextMove, 0.8f);
        tmp.transform.DOScale(scale - SVector3.zeroPointThree, 1).SetEase(damageTxtScaleCurve);
        tmp.DOColor(Color.clear, 1.1f).SetEase(damageTxtScaleCurve).OnComplete(() => tmp.gameObject.SetActive(false));
    }
}
