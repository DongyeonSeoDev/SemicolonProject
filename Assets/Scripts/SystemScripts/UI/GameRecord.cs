using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Water;
using FkTweening;

public class GameRecord : MonoBehaviour
{
    struct StatUpdateRecord
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

    private void Update()
    {
        playTime += Time.deltaTime;
    }

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

    public void AddStatInfo(ushort id, int lv, int cost)
    {
        statRecordQueue.Enqueue(new StatUpdateRecord(id, lv, cost));
    }

    public void Restart()
    {
        playTime = 0f;
        killCount = 0;
        charCount = 0;
        exp = 0f;

        for (ushort i = NGlobal.CStatStartID; i <= NGlobal.CStatEndID; i += NGlobal.StatIDOffset)
        {
            checkCharDic[i] = false;
        }

        statRecordQueue.Clear();
    }

    //����â ������� 0���� �ö󰡴� ���� ������ ���� �� ����
    //������ â�� �ø� �ø� ������� �ؽ�Ʈ ���ϴ� ���� �ʿ�
    //Ž�� ���� ���д� ũ�� ������ ����ϸ� ������ ���� �ʿ�
    //���� ����Ʈ, �� ����Ʈ�� �������� ���� �ʿ�
    private void Record()
    {
        //���� â
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

        KillTxt.DOFillText(0, killCount, 2, 0.02f, true, () =>
        {
            charCountTxt.DOFillText(0, charCount, 1, 0.07f, true, () =>
            {
                expTxt.DOFillText(0, (int)exp, 200, 0.05f, true, () =>
                {
                    RecordStats();
                });
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

    private void RecordStats()
    {
        //������ â
        Stat stat = GameManager.Instance.savedData.userInfo.playerStat;
        EternalStat eternal = stat.eternalStat;
        ChoiceStat choice = stat.choiceStat;

        PoolManager.PoolObjSetActiveFalse("StatRecord");
        restStatPointTxt.text = stat.currentStatPoint.ToString();
        int point = stat.currentStatPoint;

        for (int i = 0; i < eternal.AllStats.Count; i++)
        {
            if (eternal.AllStats[i].statLv > 1)
            {
                point += PoolManager.GetItem<StatRecord>("StatRecord").Record(eternal.AllStats[i].id, true);
            }
        }

        for (int i = 0; i < choice.AllStats.Count; i++)
        {
            if (choice.AllStats[i].isUnlock)
            {
                point += PoolManager.GetItem<StatRecord>("StatRecord").Record(choice.AllStats[i].id, false);
            }
        }

        statResultLastElement.transform.GetChild(0).GetComponent<Text>().text = "POINT : <color=#B0A94D>" + point.ToString() + "</color>";
    }

    private void ResetUI()
    {
        playTimeTxt.text = "00:00:00";
        KillTxt.text = "0";
        charCountTxt.text = "0";
        expTxt.text = "0";

        restStatPointTxt.text = "0";
        statResultTxt.text = "0";
    }

    public void EndGame(bool clear)
    {
        gameResultTMP.text = clear ? "���� ����" : "���� ����";
        gameResultTMP.colorGradient = clear ? clearVG : failVG;

        ResetUI();
        Util.DelayFunc(Record, 0.5f, this, true);

        UIManager.Instance.OnUIInteractSetActive(UIType.ENDGAME, true, true);
    }
}
