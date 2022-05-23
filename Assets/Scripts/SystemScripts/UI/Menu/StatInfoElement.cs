using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatInfoElement : UITransition
{
    private ushort id;
    private bool isEnter;

    [SerializeField] private Text statLvTxt;
    [SerializeField] private Text curStatTxt;
    [SerializeField] private Text statNameTxt;
    [SerializeField] private Button statUpBtn;

    protected override void Awake()
    {
        EventTrigger eventTrigger = statUpBtn.GetComponent<EventTrigger>();

        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerEnter;
        entry1.callback.AddListener(eventData => Transition(true));

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback.AddListener(eventData => Transition(false));

        eventTrigger.triggers.Add(entry1);
        eventTrigger.triggers.Add(entry2);
    }

    public void InitSet(StatElement info)
    {
        id = info.id;
        statNameTxt.text = NGlobal.playerStatUI.GetStatSOData(id).statName;
        statUpBtn.onClick.AddListener(() =>
        {
            if (NGlobal.playerStatUI.CanStatUp(id))
            {
                NGlobal.playerStatUI.StatUp(id);
                UpdateUI();
            }
            else
            {
                UIManager.Instance.RequestSystemMsg("���� ����Ʈ�� �����մϴ�.");
            }

            if (isEnter)
            {
                Transition(true);
            }
        });

        gameObject.SetActive(info.isUnlock);
    }

    public override void Transition(bool on) //���� ���� ��ư�� ���콺 �� ��
    {
        isEnter = on;
        if (on)
        {
            StatElement el = NGlobal.playerStatUI.eternalStatDic[id].first;
            NGlobal.playerStatUI.OnMouseEnterStatUpBtn((int)Mathf.Pow(2, el.statLv));  //2^���ݱ��� ������ �ø� Ƚ�� = ���ȿø��� ���� �ʿ��� ����Ʈ ��
            UpdatePlusStat(true);
        }
        else
        {
            NGlobal.playerStatUI.OnMouseEnterStatUpBtn(-1);
            UpdatePlusStat(false);
        }
    }

    public void UpdateUI() //���� ���Ȱ� ���� Ƚ�� ����
    {
        curStatTxt.text = NGlobal.playerStatUI.GetCurrentPlayerStat(id).ToString();
        statLvTxt.text = NGlobal.playerStatUI.eternalStatDic[id].first.statLv.ToString();
    }

    public void UpdatePlusStat(bool enter)  //���� ����Ʈ �Ҹ��ϰ� ��� ���콺�� enter�������� üũ�ؼ� UI ����
    {
        curStatTxt.text = enter ? string.Concat(NGlobal.playerStatUI.GetCurrentPlayerStat(id), "<color=green>(+", NGlobal.playerStatUI.eternalStatDic[id].first.upStatValue, ")</color>") : NGlobal.playerStatUI.GetCurrentPlayerStat(id).ToString();
    }

}
