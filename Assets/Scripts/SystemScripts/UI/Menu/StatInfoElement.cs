using UnityEngine;
using UnityEngine.UI;

public class StatInfoElement : UITransition
{
    [SerializeField] private Text usedStatPointTxt;
    [SerializeField] private Text curStatTxt;
    [SerializeField] private Text statNameTxt;
    [SerializeField] private Button statUpBtn;

    public void InitSet(string statName)
    {
        statNameTxt.text = statName;
        statUpBtn.onClick.AddListener(() =>
        {

        });
    }

    public override void Transition(bool on)
    {

    }

    public void UpdateUI()
    {

    }


}
