using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StatInfoElement : UITransition
{
    private ushort id;
    private StatElement eternal;
    private bool isEnter;

    [SerializeField] private Text statLvTxt;
    [SerializeField] private Text curStatTxt;
    [SerializeField] private Text statNameTxt;
    [SerializeField] private Button statUpBtn;
    private NameInfoFollowingCursor nifc;

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
        nifc = statUpBtn.GetComponent<NameInfoFollowingCursor>();

        eternal = info;
        id = info.id;
        statNameTxt.text = NGlobal.playerStatUI.GetStatSOData(id).statName;

        statUpBtn.onClick.AddListener(() =>
        {
            if (eternal.isOpenStat)
            {
                if (NGlobal.playerStatUI.CanStatUp(id))
                {
                    NGlobal.playerStatUI.StatUp(id);
                    UpdateUI();
                }
            }
            else
            {
                if (NGlobal.playerStatUI.CanStatOpen())
                {
                    NGlobal.playerStatUI.StatOpen(id);
                    statNameTxt.text = NGlobal.playerStatUI.GetStatSOData(id).statName;
                    UpdateUI();
                }
                else
                {
                    UIManager.Instance.RequestSystemMsg("���� ����Ʈ�� �����մϴ�.");
                }
            }

            if (isEnter)
            {
                Transition(true);
            }
        });

        if (!info.isUnlock)
        {
            statNameTxt.text = "???";
            statUpBtn.gameObject.SetActive(false);
        }
        else if (info.statLv == 0)
        {
            UnlockStat();
        }
        else
        {
            nifc.explanation = "���� ������";
        }
    }

    public override void Transition(bool on) //���� ���� ��ư�� ���콺 �� ��
    {
        isEnter = on;
        if (transitionEnable)
        {
            if (on)
            {
                NGlobal.playerStatUI.OnMouseEnterStatUpBtn(eternal.isOpenStat ? UpStatInfoTextAsset.Instance.GetValue(eternal.statLv) : -5);  //2^���ݱ��� ������ �ø� Ƚ�� = ���ȿø��� ���� �ʿ��� ����Ʈ ��(�̾���)
                UpdatePlusStat(true);
            }
            else
            {
                NGlobal.playerStatUI.OnMouseEnterStatUpBtn(-1);
                UpdatePlusStat(false);
            }
        }
    }

    public void UpdateUI() //���� ���Ȱ� ���� Ƚ�� ����
    {
        //curStatTxt.text = NGlobal.playerStatUI.GetCurrentPlayerStat(id).ToString();
        int curStatValue = Mathf.RoundToInt(NGlobal.playerStatUI.GetCurrentPlayerStat(id));
        curStatTxt.text = eternal.isUnlock ? curStatValue.ToString().ToColorStr("#980D0D", () => eternal.statLv == 0 && eternal.isUnlock) : "?" ;
        statLvTxt.text = eternal.statLv.ToString();
    }

    public void SetEnableUpBtn(bool on)  //�����ִ� �� ��ư�� �������� �ϰų� ���� �� �ְ� ��
    {
        if(statUpBtn.gameObject.activeSelf)
        {
            statUpBtn.interactable = on;
            transitionEnable = on;

            if(on && isEnter)
            {
                Transition(true);
            }
        }
    }

    public void UpdatePlusStat(bool enter)  //���� ����Ʈ �Ҹ��ϰ� ��� ���콺�� enter�������� üũ�ؼ� UI ����
    {
        if (enter && !eternal.isOpenStat) return;

        curStatTxt.text = enter ? string.Concat(Mathf.RoundToInt(NGlobal.playerStatUI.GetCurrentPlayerStat(id)), "<color=green>(+", NGlobal.playerStatUI.eternalStatDic[id].first.upStatValue, ")</color>") : Mathf.RoundToInt(NGlobal.playerStatUI.GetCurrentPlayerStat(id)).ToString();
    }

    public void UnlockStat() //�ش� ������ ����. ������ ���� ������´� �ƴ�
    {
        statNameTxt.text = NGlobal.playerStatUI.GetStatSOData(id).statName.ToColorStr("#980D0D");
        statUpBtn.gameObject.SetActive(true);
        nifc.explanation = "�����ϱ�"; 
    }

    public void OpenStat()
    {
        statNameTxt.text = NGlobal.playerStatUI.GetStatSOData(id).statName;
        nifc.explanation = "���� ������";
    }
}
