using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MonsterInfoSlot : MonoBehaviour
{
    private string monsterBodyID;
    public string MonsterBodyID { get => monsterBodyID; }

    [SerializeField] private Image monsterImg;
    [SerializeField] private Image understandingRateFill, understandingOverRateFill;
    //[SerializeField] private Text understandingRateText;

    [SerializeField] private Button transformationBtn;

    private UIScale btnUS;

    public void Init(PlayerEnemyUnderstandingRateManager.ChangeBodyData data)
    {
        monsterBodyID = data.bodyId.ToString();

        //monsterImg.sprite = data.

        btnUS = transformationBtn.GetComponent<UIScale>();
        btnUS.transitionEnable = false;
        transformationBtn.interactable = false;
        transformationBtn.onClick.AddListener(() => SlimeGameManager.Instance.PlayerBodyChange(monsterBodyID));
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
    }
}
