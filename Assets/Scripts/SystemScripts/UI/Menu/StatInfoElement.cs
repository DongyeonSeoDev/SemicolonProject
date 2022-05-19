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
            //�ø� �� �ִ��� üũ�ϰ� ���� �ø��� UI ������Ʈ
            UpdateUI();
        });
    }

    public override void Transition(bool on)
    {
        
    }

    public void UpdateUI()
    {
        //���� ���� ����Ʈ ��� Ƚ���� ���� ���� ����
    }


}
