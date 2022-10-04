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

    public TextMeshProUGUI gameResultTMP;  //모험 성공/실패

    public VertexGradient clearVG, failVG;

    public Text playTimeTxt;  //플레이 타임
    public Text KillTxt;  //적 죽인 횟수 (보스 포함)
    public Text charCountTxt;  //얻은 특성 수 (판매한 것도 포함)
    public Text expTxt;  //획득한 총 스탯포인트 경험치 (몹 처치와 미션 완료 보상)

    public Text restStatPointTxt;  //보유 포인트

    private float playTime;
    private int killCount;
    private int charCount;
    private float exp;

    private Dictionary<ushort, bool> checkCharDic = new Dictionary<ushort, bool>();  //어떤 특성을 한 번이라도 얻었는지 체크함

    private Queue<StatUpdateRecord> statRecordQueue = new Queue<StatUpdateRecord>(); //스탯들의 상태를 갱신한 정보를 담아두는 큐

    //private List<GameObject> invisibleStatRecords = new List<GameObject>();
    public GameObject statResultLastElement;  //statResultTxt의 부모 옵젝
    private Text statResultTxt;  //보유 포인트 + 스탯 렙업에 쓴 포인트 + 특성 판매하면 나오는 포인트

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

    //왼쪽창 결과들은 0부터 올라가는 연출 있으면 좋을 것 같음
    //오른쪽 창은 올린 올린 순서대로 텍스트 변하는 연출 필요
    //탐험 성공 실패는 크기 빠르게 축소하며 나오는 연출 필요
    //보유 포인트, 총 포인트도 차오르는 연출 필요
    private void Record()
    {
        //왼쪽 창
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

        #region 주석
        //밑에 안보이는 칸 하나 두고 그 밑에 포인트 총량 기록 필요

        /*for(int i=0; i<invisibleStatRecords.Count; i++)
        {
            invisibleStatRecords[i].transform.SetAsLastSibling();
        }*/

        //statResultLastElement.transform.SetAsLastSibling();
        #endregion
    }

    private void RecordStats()
    {
        //오른쪽 창
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
        gameResultTMP.text = clear ? "모험 성공" : "모험 실패";
        gameResultTMP.colorGradient = clear ? clearVG : failVG;

        ResetUI();
        Util.DelayFunc(Record, 0.5f, this, true);

        UIManager.Instance.OnUIInteractSetActive(UIType.ENDGAME, true, true);
    }
}
