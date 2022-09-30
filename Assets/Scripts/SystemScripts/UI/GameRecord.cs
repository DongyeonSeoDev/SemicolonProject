using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
using Water;

public class GameRecord : MonoBehaviour
{
    public TextMeshProUGUI gameResultTMP;  //모험 성공/실패

    public VertexGradient clearVG, failVG;

    public Text playTimeTxt;  //플레이 타임
    public Text KillTxt;  //적 죽인 횟수 (보스 포함)
    public Text charCountTxt;  //얻은 특성 수 (판매한 것도 포함)
    public Text expTxt;  //획득한 총 스탯포인트 경험치 (몹 처치와 미션 완료 보상)

    public Text restStatPointTxt;

    private float playTime;
    private int killCount;
    private int charCount;
    private float exp;

    private Dictionary<ushort, bool> checkCharDic = new Dictionary<ushort, bool>();

    private List<GameObject> invisibleStatRecords = new List<GameObject>();
    public GameObject statResultLastElement;

    private void Awake()
    {
        Restart();

        for(int i=0; i<4; i++)
        {
            GameObject obj = Instantiate(statResultLastElement, statResultLastElement.transform.parent);
            invisibleStatRecords.Add(obj);
            obj.GetComponent<StatRecord>().DeleteChild();
            Destroy(obj.GetComponent<StatRecord>());
        }
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
    }

    private void Record()
    {
        //왼쪽 창
        //왼쪽창 결과들은 0부터 올라가는 연출 있으면 좋을 것 같음
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

        KillTxt.text = killCount.ToString();
        charCountTxt.text = charCount.ToString();
        expTxt.text = ((int)exp).ToString();

        //오른쪽 창
        Stat stat = GameManager.Instance.savedData.userInfo.playerStat;
        EternalStat eternal = stat.eternalStat;
        ChoiceStat choice = stat.choiceStat;

        PoolManager.PoolObjSetActiveFalse("StatRecord");
        restStatPointTxt.text = stat.currentStatPoint.ToString();
        int point = stat.currentStatPoint;

        for(int i=0; i<eternal.AllStats.Count; i++)
        {
            if(eternal.AllStats[i].statLv > 1)
            {
                point += PoolManager.GetItem<StatRecord>("StatRecord").Record(eternal.AllStats[i].id, true);
            }
        }

        for(int i=0; i<choice.AllStats.Count; i++)
        {
            if(choice.AllStats[i].isUnlock)
            {
                point += PoolManager.GetItem<StatRecord>("StatRecord").Record(choice.AllStats[i].id, false);
            }
        }

        //밑에 안보이는 칸 하나 두고 그 밑에 포인트 총량 기록 필요

        for(int i=0; i<invisibleStatRecords.Count; i++)
        {
            invisibleStatRecords[i].transform.SetAsLastSibling();
        }

        statResultLastElement.transform.GetChild(0).GetComponent<Text>().text = "POINT : <color=#B0A94D>" + point.ToString() + "</color>";
        statResultLastElement.transform.SetAsLastSibling();
    }

    public void EndGame(bool clear)
    {
        gameResultTMP.text = clear ? "모험 성공" : "모험 실패";
        gameResultTMP.colorGradient = clear ? clearVG : failVG;

        Record();

        UIManager.Instance.OnUIInteractSetActive(UIType.ENDGAME, true, true);
    }
}
