using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Water;
using FkTweening;

[System.Serializable]
public class ValueInRangeInfo
{
    [System.Serializable]
    public struct ValueInRange
    {
        public float range;
        public float value;
    }

    public ValueInRange[] rangeInfo;

    public float GetValue(float f)
    {
        if (rangeInfo == null || rangeInfo.Length == 0) return 0f;

        for(int i = 0; i < rangeInfo.Length; i++)
        {
            if(f < rangeInfo[i].range)
            {
                return rangeInfo[i].value;
            }
        }

        return rangeInfo[rangeInfo.Length - 1].value;
    }
}

public class GameRecord : MonoBehaviour
{
    public struct StatUpdateRecord
    {
        public ushort id;
        public int level;
        public int sell;

        public StatUpdateRecord(ushort id, int level, int sell)
        {
            this.id = id;
            this.level = level;
            this.sell = sell;
        }
    }

    public TextMeshProUGUI gameResultTMP;  //���� ����/����

    public VertexGradient clearVG, failVG;

    public Text playTimeTxt;  //�÷��� Ÿ��
    public Text KillTxt;  //�� ���� Ƚ�� (���� ����)
    public Text charCountTxt;  //���� Ư�� �� (�Ǹ��� �͵� ����)
    public Text expTxt;  //ȹ���� �� ��������Ʈ ����ġ (�� óġ�� �̼� �Ϸ� ����)

    public Text restStatPointTxt;  //���� ����Ʈ

    private float playTime;
    private int killCount;
    private int charCount;
    private float exp;

    [SerializeField] private ValueInRangeInfo[] textFillSecPerOffset;  //�ؽ�Ʈ ���� ������ ȿ���� �� �����´� �󸶸�ŭ�� �ð��� �ɸ�����

    private bool showingStatRecords;
    private int totalPoint;
    private float showTime = 0f;

    private Dictionary<ushort, StatRecord> statRecordDic = new Dictionary<ushort, StatRecord>();  //���� ��� ������ �� ���� â�� �����ִ� ���� ��ϵ�
    private Dictionary<ushort, bool> checkCharDic = new Dictionary<ushort, bool>();  //� Ư���� �� ���̶� ������� üũ��

    private Queue<StatUpdateRecord> statRecordQueue = new Queue<StatUpdateRecord>(); //���ȵ��� ���¸� ������ ������ ��Ƶδ� ť

    //private List<GameObject> invisibleStatRecords = new List<GameObject>();
    public GameObject statResultLastElement;  //statResultTxt�� �θ� ����
    private Text statResultTxt;  //���� ����Ʈ + ���� ������ �� ����Ʈ + Ư�� �Ǹ��ϸ� ������ ����Ʈ

    private void Awake()
    {
        statResultTxt = statResultLastElement.transform.GetChild(0).GetComponent<Text>();

        Restart();

        /*for(int i=0; i<4; i++)
        {
            GameObject obj = Instantiate(statResultLastElement, statResultLastElement.transform.parent);
            invisibleStatRecords.Add(obj);
            obj.GetComponent<StatRecord>().DeleteChild();
            Destroy(obj.GetComponent<StatRecord>());
        }*/
    }

    private void CreateHistory(in StatUpdateRecord record)  //���� �ø� ��� UI ���� ������
    {
        totalPoint += record.sell;
        StatRecord sr = PoolManager.GetItem<StatRecord>("StatRecord");
        statRecordDic.Add(record.id, sr);
        sr.ResetUI();
        sr.Record(record);

        showTime = Time.unscaledTime + 0.3f;
    }

    private void Update()
    {
        playTime += Time.deltaTime;  //�Ͻ����� ���� ���� ��(��Ȱ ��) ��Ž ���

        if(showingStatRecords)  //���� �����丮 �����ִ� ��
        {
            if(Time.unscaledTime > showTime)
            {
                if (statRecordQueue.Count > 0)  //��� ��������
                {
                    StatUpdateRecord record = statRecordQueue.Dequeue();
                    if(statRecordDic.ContainsKey(record.id))  //�ش� ���̵� ���� ��� UI�� �̹� ����ٸ�
                    {
                        totalPoint += record.sell;
                        statRecordDic[record.id].Record(record);

                        showTime = Time.unscaledTime + 0.3f;
                    }
                    else
                    {
                        if (NGlobal.playerStatUI.eternalStatDic.ContainsKey(record.id) && NGlobal.playerStatUI.eternalStatDic[record.id].first.statLv > 1)
                        {
                            CreateHistory(record);  //(����)�����̸鼭 ����� �����̰� �� �� �̻� ������ ��Ŵ
                        }
                        else if(NGlobal.playerStatUI.choiceStatDic.ContainsKey(record.id) && NGlobal.playerStatUI.choiceStatDic[record.id].isUnlock)
                        {
                            CreateHistory(record);  //(���ý���)Ư���̸鼭 �ر��� �� ���´�
                        }
                    }
                }
                if(statRecordQueue.Count == 0)  //���� ���� �����丮 �� ���������� ��� �� ����Ʈ �ѷ� ������
                {
                    showingStatRecords = false;
                    Util.DelayFunc(() => statResultTxt.text = "POINT : <color=#B0A94D>" + totalPoint.ToString() + "</color>", 0.4f, this, true);
                }
            }
        }
    }

    //ų ��, ȹ�� ����ġ, ȹ�� Ư�� �� ���  (�ؿ� �ִ� �Լ� 3��)
    public void KillEnemy() => killCount++;
    public void GetStatPointExp(float value) => exp += value;

    public void CheckGetChar(ushort id)
    {
        if (!checkCharDic[id])
        {
            checkCharDic[id] = true;
            charCount++;
        }
    }

    public void AddStatInfo(ushort id, int lv, int cost)  //������ â�� ���� ������ �����丮 ���
    {
        statRecordQueue.Enqueue(new StatUpdateRecord(id, lv, cost));
    }

    public void Restart()  //������ �ʱ�ȭ. ó�� ������ ���� ��Ȱ�� �� ȣ��
    {
        showingStatRecords = false;
        totalPoint = 0;

        playTime = 0f;
        killCount = 0;
        charCount = 0;
        exp = 0f;

        for (ushort i = NGlobal.CStatStartID; i <= NGlobal.CStatEndID; i += NGlobal.StatIDOffset)
        {
            checkCharDic[i] = false;
        }

        statRecordQueue.Clear();
        statRecordDic.Clear();
    }

    private void Record()        //���� â ������ �����
    {
        int s = (int)playTime % 60;
        int m = ((int)playTime / 60) % 60;
        int h = (int)playTime / 3600;

        if(h > 99) 
        {
            playTimeTxt.text = "00:00:00";
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(h.ToString("00"));
            sb.Append(":");
            sb.Append(m.ToString("00"));
            sb.Append(":");
            sb.Append(s.ToString("00"));
            playTimeTxt.text = sb.ToString();
        }

        KillTxt.DOFillText(0, killCount, 2, textFillSecPerOffset[0].GetValue(killCount), true, () =>
        {
            charCountTxt.DOFillText(0, charCount, 1, textFillSecPerOffset[1].GetValue(charCount), true, () =>
            {
                expTxt.DOFillText(0, (int)exp, 100, textFillSecPerOffset[2].GetValue(exp), true, RecordStats);  //�ؽ�Ʈ �� ���� ������ ������ ����� ������ â ���� ���
            });
        });

        #region �ּ�
        //�ؿ� �Ⱥ��̴� ĭ �ϳ� �ΰ� �� �ؿ� ����Ʈ �ѷ� ��� �ʿ�

        /*for(int i=0; i<invisibleStatRecords.Count; i++)
        {
            invisibleStatRecords[i].transform.SetAsLastSibling();
        }*/

        //statResultLastElement.transform.SetAsLastSibling();
        #endregion
    }

    private void RecordStats()  //������ â�� ������ �����
    {
        Stat stat = GameManager.Instance.savedData.userInfo.playerStat;
        EternalStat eternal = stat.eternalStat;
        ChoiceStat choice = stat.choiceStat;

        totalPoint = stat.currentStatPoint;
        restStatPointTxt.DOFillText(0, stat.currentStatPoint, 1, textFillSecPerOffset[3].GetValue(stat.currentStatPoint), true, () =>
        {
            showTime = Time.unscaledTime + 0.1f;
            showingStatRecords = true;
        });  //���� ����Ʈ�� ���� �����ְ� ���� �����丮 ������
    }

    private void ResetUI()  //���â ��� �� UI���� ó���� ���� ������
    {
        playTimeTxt.text = "00:00:00";
        KillTxt.text = "0";
        charCountTxt.text = "0";
        expTxt.text = "0";

        restStatPointTxt.text = "0";
        statResultTxt.text = "0";

        PoolManager.PoolObjSetActiveFalse("StatRecord");
    }

    public void EndGame(bool clear)  //�װų� ���������������� Ŭ�����ϸ� ȣ���ϴ� �Լ�
    {
        gameResultTMP.text = clear ? "���� ����" : "���� ����";
        gameResultTMP.colorGradient = clear ? clearVG : failVG;

        ResetUI();
        Util.DelayFunc(Record, 2f, this, true);

        UIManager.Instance.OnUIInteractSetActive(UIType.ENDGAME, true, true);
    }
}
