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

    [SerializeField] private ValueInRangeInfo[] textFillSecPerOffset;  //텍스트 숫자 오르는 효과할 때 오프셋당 얼마만큼의 시간이 걸리는지

    private bool showingStatRecords;
    private int totalPoint;
    private float showTime = 0f;

    private Dictionary<ushort, StatRecord> statRecordDic = new Dictionary<ushort, StatRecord>();  //스탯 기록 보여줄 때 현재 창에 나와있는 스탯 기록들
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

    private void CreateHistory(in StatUpdateRecord record)  //스탯 올린 기록 UI 새로 보여줌
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
        playTime += Time.deltaTime;  //일시정지 제외 시작 후(부활 후) 플탐 기록

        if(showingStatRecords)  //스탯 히스토리 보여주는 중
        {
            if(Time.unscaledTime > showTime)
            {
                if (statRecordQueue.Count > 0)  //기록 남아있음
                {
                    StatUpdateRecord record = statRecordQueue.Dequeue();
                    if(statRecordDic.ContainsKey(record.id))  //해당 아이디에 대한 기록 UI를 이미 띄웠다면
                    {
                        totalPoint += record.sell;
                        statRecordDic[record.id].Record(record);

                        showTime = Time.unscaledTime + 0.3f;
                    }
                    else
                    {
                        if (NGlobal.playerStatUI.eternalStatDic.ContainsKey(record.id) && NGlobal.playerStatUI.eternalStatDic[record.id].first.statLv > 1)
                        {
                            CreateHistory(record);  //(고정)스탯이면서 개방된 스탯이고 한 번 이상 렙업을 시킴
                        }
                        else if(NGlobal.playerStatUI.choiceStatDic.ContainsKey(record.id) && NGlobal.playerStatUI.choiceStatDic[record.id].isUnlock)
                        {
                            CreateHistory(record);  //(선택스탯)특성이면서 해금을 한 상태다
                        }
                    }
                }
                if(statRecordQueue.Count == 0)  //스탯 렙업 히스토리 다 보여줬으면 잠시 후 포인트 총량 보여줌
                {
                    showingStatRecords = false;
                    Util.DelayFunc(() => statResultTxt.text = "POINT : <color=#B0A94D>" + totalPoint.ToString() + "</color>", 0.4f, this, true);
                }
            }
        }
    }

    //킬 수, 획득 경험치, 획득 특성 수 기록  (밑에 있는 함수 3개)
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

    public void AddStatInfo(ushort id, int lv, int cost)  //오른쪽 창의 스탯 정보들 히스토리 기록
    {
        statRecordQueue.Enqueue(new StatUpdateRecord(id, lv, cost));
    }

    public void Restart()  //데이터 초기화. 처음 시작할 때나 부활할 때 호출
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

    private void Record()        //왼쪽 창 정보를 띄워줌
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
                expTxt.DOFillText(0, (int)exp, 100, textFillSecPerOffset[2].GetValue(exp), true, RecordStats);  //텍스트 세 개의 연출이 끝나면 잠시후 오른쪽 창 정보 띄움
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

    private void RecordStats()  //오른쪽 창의 정보를 띄워줌
    {
        Stat stat = GameManager.Instance.savedData.userInfo.playerStat;
        EternalStat eternal = stat.eternalStat;
        ChoiceStat choice = stat.choiceStat;

        totalPoint = stat.currentStatPoint;
        restStatPointTxt.DOFillText(0, stat.currentStatPoint, 1, textFillSecPerOffset[3].GetValue(stat.currentStatPoint), true, () =>
        {
            showTime = Time.unscaledTime + 0.1f;
            showingStatRecords = true;
        });  //보유 포인트의 양을 보여주고 스탯 히스토리 보여줌
    }

    private void ResetUI()  //결과창 띄울 때 UI들을 처음에 리셋 시켜줌
    {
        playTimeTxt.text = "00:00:00";
        KillTxt.text = "0";
        charCountTxt.text = "0";
        expTxt.text = "0";

        restStatPointTxt.text = "0";
        statResultTxt.text = "0";

        PoolManager.PoolObjSetActiveFalse("StatRecord");
    }

    public void EndGame(bool clear)  //죽거나 최종스테이지까지 클리어하면 호출하는 함수
    {
        gameResultTMP.text = clear ? "모험 성공" : "모험 실패";
        gameResultTMP.colorGradient = clear ? clearVG : failVG;

        ResetUI();
        Util.DelayFunc(Record, 2f, this, true);

        UIManager.Instance.OnUIInteractSetActive(UIType.ENDGAME, true, true);
    }
}
