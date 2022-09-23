using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterInfoSlot : MonoBehaviour
{
    private string monsterBodyID;
    public ChangeBodyData BodyData { get; set; }

    [SerializeField] private Image monsterImg;
    [SerializeField] private Image understandingRateFill, understandingOverRateFill;

    [SerializeField] private Image drainProbabilityFill;

    public TextMeshProUGUI nameTMP;

    public float barEffWidth;
    public Pair<RectTransform, RectTransform> barEndEff;  //흡수율, 동화율

    public Transform MobImgBg { get; private set; }

    //public Transform acqMarkPar; //변신 가능 텍스트 UI의 부모
    //[SerializeField] private Text understandingRateText;

    private ItemSO dropItem;

    public MulSpriteColorCtrl msc;
    public GameObject fullAssimEff;

    public void Init(ChangeBodyData data)
    {
        monsterBodyID = data.bodyId.ToString();
        BodyData = data;

        //monsterImg.sprite = data.bodyImg;
        monsterImg.sprite = MonsterCollection.Instance.questionSpr;
        dropItem = data.dropItem;

        MobImgBg = monsterImg.transform.parent;

        monsterImg.GetComponent<Button>().onClick.AddListener(() => MonsterCollection.Instance.Detail(this, monsterBodyID));
    }

    public void SetMonsterSlot(bool set)
    {
        monsterImg.sprite = set ? BodyData.bodyImg : MonsterCollection.Instance.questionSpr;
        nameTMP.text = set ? BodyData.bodyName : "???";
    }

    public void UpdateAssimilationRate(float rate)
    {
        if(rate > 1f)
        {
            understandingRateFill.fillAmount = 1;
            understandingOverRateFill.fillAmount = rate - 1f;

            //fullAssimEff.SetActive(rate >= PlayerEnemyUnderstandingRateManager.Instance.MaxUnderstandingRate); //나중에 풀링으로 ㄱ
        }
        else
        {
            understandingOverRateFill.fillAmount = 0;
            understandingRateFill.fillAmount = rate;
        }

        
    }

    public void UpdateDrainProbability(float prob)
    {
        drainProbabilityFill.fillAmount = Mathf.Clamp(prob/100f, 0, 1f);

        if (prob >= 100f || prob <= 0f)
        {
            barEndEff.first.gameObject.SetActive(false);
        }
        else
        {
            barEndEff.first.gameObject.SetActive(true);
            barEndEff.first.SetPosX(barEffWidth * drainProbabilityFill.fillAmount);
        }
    }

    public void MarkAcqBody(bool on) //변신 가능 텍스트 UI 띄우거나 없앰
    {
        //msc.SetIntensity(on ? 0.6f : 0);
    }
}
