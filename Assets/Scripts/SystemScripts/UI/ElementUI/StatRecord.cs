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

        statName.text = string.Format("LV.{0} {1}", info.level, stat.StatName);
        point += info.sell;
        statRecord.text = string.Concat('+', point);
    }

    public void ResetUI()
    {
        point = 0;
    }
}
