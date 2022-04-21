using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoiceStatControl : MonoBehaviour
{
    [Header("누적 피해량이 이 값 이상일 때 마다 맷집값이 상승함")]
    [SerializeField]
    private int enduranceUpAmount = 100;

    [Header("1 맷집당 오르는 최대체력의 양")]
    [SerializeField]
    private float upMaxHpPerEnduranceUpAmount = 5f;

    public void CheckEndurance()
    {
        int pasteEndurance = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance;
        int num = (int)SlimeGameManager.Instance.Player.TotalDamage / enduranceUpAmount;

        if (pasteEndurance != num && num > 0)
        {
            if(pasteEndurance == 0)
            {
                // 처음 이 스탯이 생김
            }

            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance = num;

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp += upMaxHpPerEnduranceUpAmount * (num - pasteEndurance);
        }
    }
}
