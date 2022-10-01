using UnityEngine;
using UnityEngine.UI;

public class StatRecord : MonoBehaviour
{
    public Text statName;
    public Text statRecord;

    public int Record(ushort id, bool eternal)
    {
        int point = 0;
        if(eternal)
        {
            StatElement eStat = NGlobal.playerStatUI.eternalStatDic[id].first;
            statName.text = string.Format("LV.{0} {1}", eStat.statLv, eStat.StatName);
            point = NGlobal.playerStatUI.usedStatUpPointDic[id];
            statRecord.text = string.Concat('+', point);
        }
        else
        {
            StatElement cStat = NGlobal.playerStatUI.choiceStatDic[id];
            statName.text = string.Format("LV.{0} {1}", cStat.statLv, cStat.StatName);
            point = NGlobal.playerStatUI.GetSellPoint(cStat.id);
            statRecord.text = string.Concat('+', point);
        }

        return point;
    }

    /*public void DeleteChild()
    {
        Destroy(transform.GetChild(0).gameObject);
    }*/
}
