using UnityEngine;
using UnityEngine.UI;

public class StatInfoElement : UITransition
{
    private ushort id;

    [SerializeField] private Text usedStatPointTxt;
    [SerializeField] private Text curStatTxt;
    [SerializeField] private Text statNameTxt;
    [SerializeField] private Button statUpBtn;

    public void InitSet(StatElement info)
    {
        id = info.id;
        statNameTxt.text = info.statName;
        statUpBtn.onClick.AddListener(() =>
        {
            //올릴 수 있는지 체크하고 스탯 올리고 UI 업데이트
            UpdateUI();
        });
    }

    public override void Transition(bool on)
    {
        
    }

    public void UpdateUI()
    {
        //누적 스탯 포인트 사용 횟수랑 현재 스탯 갱신
    }


}
