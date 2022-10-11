using UnityEngine;
using UnityEngine.UI;

public class StatRecord : MonoBehaviour
{
    public Text statName;
    public Text statRecord;

    private int point;

    public void Record(GameRecord.StatUpdateRecord info)
    {
        StatElement stat = NGlobal.playerStatUI.eternalStatDic.ContainsKey(info.id) ? NGlobal.playerStatUI.eternalStatDic[info.id].first : NGlobal.playerStatUI.choiceStatDic[info.id];

        statName.text = string.Format("<color=#{0}>LV.{1} {2}</color>", NGlobal.playerStatUI.choiceStatDic.ContainsKey(info.id) ? "E3E3E3" : "B7B6A5", info.level, stat.StatName);
        point += info.sell;
        statRecord.text = string.Concat("<color=#FFFF90>+", point, "</color>");
        //statRecord.color = NGlobal.playerStatUI.choiceStatDic.ContainsKey(info.id) ? Color.blue : Color.green;
    }

    public void ResetUI()
    {
        point = 0;
    }
}
