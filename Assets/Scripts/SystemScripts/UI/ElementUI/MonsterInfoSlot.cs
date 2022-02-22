using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MonsterInfoSlot : MonoBehaviour
{
    private string monsterBodyID;
    public PlayerEnemyUnderstandingRateManager.ChangeBodyData bodyData { get; set; }

    [SerializeField] private Image monsterImg;
    [SerializeField] private Image understandingRateFill, understandingOverRateFill;
    //[SerializeField] private Text understandingRateText;

    [SerializeField] private Button transformationBtn;

    private UIScale btnUS;

    private ItemSO dropItem;

    public void Init(PlayerEnemyUnderstandingRateManager.ChangeBodyData data)
    {
        monsterBodyID = data.bodyId.ToString();
        bodyData = data;

        monsterImg.sprite = data.bodyImg;
        //dropItem = data.

        btnUS = transformationBtn.GetComponent<UIScale>();

        transformationBtn.onClick.AddListener(() => SlimeGameManager.Instance.PlayerBodyChange(monsterBodyID));
        monsterImg.GetComponent<Button>().onClick.AddListener(() => MonsterCollection.Instance.Detail(bodyData, monsterBodyID));
    }

    public void UpdateRate(float rate)
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

        Active(rate >= 1f);
    }

    private void Active(bool active)
    {
        btnUS.transitionEnable = active;
        transformationBtn.interactable = active;
    }
}
