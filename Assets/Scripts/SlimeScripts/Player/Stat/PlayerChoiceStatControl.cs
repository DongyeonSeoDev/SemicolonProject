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

    [Header("�γ� ������ �ʱ� ����")]
    [SerializeField]
    private int firstPatienceValue = 1;

    [Header("���� Ƚ���� �� �� �̻��� �� ���� �γ� ��ġ ���")]
    [SerializeField]
    private int patienceUpAmount = 100;

    [Header("1 �γ��� ������ ���ݷ��� ��")]
    [SerializeField]
    private int upDamagePerPatience = 1;

    private float totalDamage = 0f;
    /// <summary>
    /// ���ݱ��� ���� �� ������
    /// </summary>
    public float TotalDamage
    {
        get { return totalDamage; }
        set { totalDamage = value; }
    }

    private int attackMissedNum = 0;
    public int AttackMissedNum
    {
        get { return attackMissedNum; }
        set { attackMissedNum = value; }
    }

    private int attackNum = 0;
    public int AttackNum
    {
        get { return attackNum; }
        set { attackNum = value; }
    }
    private void OnEnable()
    {
        EventManager.StartListening("OnEnemyAttack", UpAttackNum);
        EventManager.StartListening("OnAttackMiss", UpAttackMissedNum);
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnEnemyAttack", UpAttackNum);
        EventManager.StopListening("OnAttackMiss", UpAttackMissedNum);
    }
    private void Update()
    {
        CheckPatience();
    }
    public void CheckEndurance()
    {
        int pasteEndurance = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance;
        int num = (int)totalDamage / enduranceUpAmount;

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
    public void UpAttackMissedNum()
    {
        // ���� ���� �͵��� ������ ���

        attackMissedNum++;
    }
    public void UpAttackNum()
    {
        // ���͸� ������ ���

        if(SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience == 0)
        {
            attackMissedNum = 0;

            return;
        }

        attackNum++;
    }
    public void CheckPatience()
    {
        int pastePatienceNum = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience;
        int num = 0;

        if (pastePatienceNum == 0)
        {
            if (attackMissedNum >= 100)
            {
                // ó�� �� ������ ����

                SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience = firstPatienceValue;
                num = firstPatienceValue;

                AttackNumReset();
            }
        }
        else
        {
            num = ((attackNum + attackMissedNum) / patienceUpAmount) + firstPatienceValue;
        }

        SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience = num;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage += upDamagePerPatience * (num - pastePatienceNum);
    }
    private void AttackNumReset()
    {
        attackMissedNum = 0;
        attackNum = 0;
    }
}
