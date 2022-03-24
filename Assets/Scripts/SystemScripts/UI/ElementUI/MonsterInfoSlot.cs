using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MonsterInfoSlot : MonoBehaviour
{
    private string monsterBodyID;
    public ChangeBodyData BodyData { get; set; }

    [SerializeField] private Image monsterImg;
    [SerializeField] private Image understandingRateFill, understandingOverRateFill;

    [SerializeField] private Image drainProbabilityFill;

    //public Transform acqMarkPar; //변신 가능 텍스트 UI의 부모
    //[SerializeField] private Text understandingRateText;

    //[SerializeField] private Button transformationBtn;

    //private UIScale btnUS;

    private ItemSO dropItem;

    public MulSpriteColorCtrl msc;
    public GameObject fullAssimEff;

    public void Init(ChangeBodyData data)
    {
        monsterBodyID = data.bodyId.ToString();
        BodyData = data;

        monsterImg.sprite = data.bodyImg;
        dropItem = data.dropItem;

        //btnUS = transformationBtn.GetComponent<UIScale>();

        //transformationBtn.onClick.AddListener(() => SlimeGameManager.Instance.PlayerBodyChange(monsterBodyID));
        monsterImg.GetComponent<Button>().onClick.AddListener(() => MonsterCollection.Instance.Detail(BodyData, monsterBodyID));
    }

    public void UpdateAssimilationRate(float rate)
    {
        if(rate > 1f)
        {
            understandingRateFill.fillAmount = 1;
            understandingOverRateFill.fillAmount = rate - 1f;

            fullAssimEff.SetActive(rate >= PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate); //나중에 풀링으로 ㄱ
        }
        else
        {
            understandingOverRateFill.fillAmount = 0;
            understandingRateFill.fillAmount = rate;
        }

        //Active(rate >= 1f);
    }

    public void UpdateDrainProbability(float prob)
    {
        drainProbabilityFill.fillAmount = Mathf.Clamp(prob/100f, 0, 1f);
    }

    public void MarkAcqBody(bool on) //변신 가능 텍스트 UI 띄우거나 없앰
    {
        msc.SetIntensity(on ? 0.6f : 0);

        /*for(int i=0; i< acqMarkPar.childCount; i++)
            acqMarkPar.GetChild(i).gameObject.SetActive(false);
        if(on)
        {
            Water.PoolManager.GetItem<Transform>("CanTrfMark").SetParent(acqMarkPar);
        }*/
        //acqMarkPar.GetChild(0).gameObject.SetActive(on);  
    }

    /*private void Active(bool active)
    {
        btnUS.transitionEnable = active;
        transformationBtn.interactable = active;
    }*/
}
