using System.Collections.Generic;
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

    public Transform acqMarkPar; //���� ���� �ؽ�Ʈ UI�� �θ�
    //[SerializeField] private Text understandingRateText;

    //[SerializeField] private Button transformationBtn;

    //private UIScale btnUS;

    private ItemSO dropItem;

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
        
        if(prob >= 1)
        {
            //���� UI ����Ʈ ��
        }
        else
        {
            //���� UI ����Ʈ ��
        }
    }

    public void MarkAcqBody(bool on) //���� ���� �ؽ�Ʈ UI ���ų� ����
    {
        for(int i=0; i< acqMarkPar.childCount; i++)
            acqMarkPar.GetChild(i).gameObject.SetActive(false);

        if(on)
        {
            Water.PoolManager.GetItem<Transform>("CanTrfMark").SetParent(acqMarkPar);
        }
    }

    /*private void Active(bool active)
    {
        btnUS.transitionEnable = active;
        transformationBtn.interactable = active;
    }*/
}
