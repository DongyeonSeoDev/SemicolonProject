using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoiceStatControl : MonoBehaviour
{
    [Header("���� ���ط��� �� �� �̻��� �� ���� �������� �����")]
    [SerializeField]
    private int enduranceUpAmount = 100;

    [Header("1 ������ ������ �ִ�ü���� ��")]
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
                // ó�� �� ������ ����
            }

            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance = num;

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp += upMaxHpPerEnduranceUpAmount * (num - pasteEndurance);
        }
    }
}
