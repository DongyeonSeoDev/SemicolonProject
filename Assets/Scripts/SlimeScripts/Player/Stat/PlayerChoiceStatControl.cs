using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoiceStatControl : MonoBehaviour
{
    [Header("���� ������ �ʱⰪ")]
    [SerializeField]
    private int firstEnduranceValue = 1;
    [Header("�γ� ������ �ʱⰪ")]
    [SerializeField]
    private int firstPatienceValue = 1;
    [Header("������ ������ �ʱⰪ")]
    [SerializeField]
    private int firstMomentomValue = 1;

    [Header("���� ������ �׵��ϱ� ���� �ش� ���� ��")]
    [SerializeField]
    private int unlockEnduranceStatValue = 100;
    [Header("�γ� ������ �׵��ϱ� ���� �ش� ���� ��")]
    [SerializeField]
    private int unlockPatienceStatValue = 100;
    [Header("������ ������ �׵��ϱ� ���� �ش� ���� ��")]
    [SerializeField]
    private int unlockMomentomStatValue = 150;


    [Header("���� ���ط��� �� �� �̻��� �� ���� �������� �����")]
    [SerializeField]
    private int enduranceUpAmount = 100;

    [Header("1 ������ ������ �ִ�ü���� ��")]
    [SerializeField]
    private float upMaxHpPerEnduranceUpAmount = 5f;


    [Header("���� Ƚ���� �� �� �̻��� �� ���� �γ� ��ġ ���")]
    [SerializeField]
    private int patienceUpAmount = 100;

    [Header("1 �γ��� ������ ���ݷ��� ��")]
    [SerializeField]
    private int upDamagePerPatience = 1;


    [SerializeField]
    private int avoidInMomentomNum = 0;
    public int AvoidInMomentomNum
    {
        get { return avoidInMomentomNum; }
        set { avoidInMomentomNum = value; }
    }

    [Header("ȸ�� Ƚ���� �� �� �̻��� �� ������ ���� ���")]
    [SerializeField]
    private int momentomAmount = 10;

    [Header("1 �����´� ������ ������ ��� ���� �߰� �̵��ӵ� ��")]
    [SerializeField]
    private float upMomentomSpeedPerMomentom = 0.2f;
    public float UpMomentomSpeedPerMomentom
    {
        get { return upMomentomSpeedPerMomentom; }
    }


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
    /// <summary>
    /// ������ ������ Ƚ��
    /// </summary>
    public int AttackMissedNum
    {
        get { return attackMissedNum; }
        set { attackMissedNum = value; }
    }

    private int attackNum = 0;
    /// <summary>
    /// ������ ������ Ƚ��
    /// </summary>
    public int AttackNum
    {
        get { return attackNum; }
        set { attackNum = value; }
    }

    private int bodySlapNum = 0;
    /// <summary>
    /// ������ Ƚ��
    /// </summary>
    public int BodySlapNum
    {
        get { return bodySlapNum; }
        set { bodySlapNum = value; }
    }

    private void OnEnable()
    {
        EventManager.StartListening("OnEnemyAttack", UpAttackNum);
        EventManager.StartListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StartListening("OnAvoidInMomentom", UpAvoidInMomentomNum);
        EventManager.StartListening("OnBodySlap", UpBodySlapNum);
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnEnemyAttack", UpAttackNum);
        EventManager.StopListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StopListening("OnAvoidInMomentom", UpAvoidInMomentomNum);
        EventManager.StopListening("OnBodySlap", UpBodySlapNum);
    }
    private void Update()
    {
        CheckPatience();
        CheckMomentom();
    }
    public void CheckEndurance()
    {
        float pasteEndurance = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.statValue;
        int num = (int)totalDamage / enduranceUpAmount;

        if (pasteEndurance != num && num > 0)
        {
            if(pasteEndurance == 0)
            {
                // ó�� �� ������ ����
                SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.isUnlock = true;
                num = firstEnduranceValue;
            }

            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.statValue = num;

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp.statValue += upMaxHpPerEnduranceUpAmount * (num - pasteEndurance);
        }
    }
    public void CheckPatience()
    {
        float pastePatienceNum = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.statValue;
        int num = 0;

        if (pastePatienceNum == 0)
        {
            if (attackMissedNum >= 100)
            {
                // ó�� �� ������ ����

                num = firstPatienceValue;
                SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.isUnlock = true;

                AttackNumReset();
            }
        }
        else
        {
            num = ((attackNum + attackMissedNum) / patienceUpAmount) + firstPatienceValue;
        }

        Debug.Log(SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat);
        SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.statValue = num;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage.statValue -= upDamagePerPatience * pastePatienceNum;
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage.statValue -= upDamagePerPatience * pastePatienceNum;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage.statValue = upDamagePerPatience * num;
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage.statValue = upDamagePerPatience * num;
    }
    private void AttackNumReset()
    {
        attackMissedNum = 0;
        attackNum = 0;
    }

    public void CheckMomentom()
    {
        float pasteMomentomNum = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.statValue;
        int num = 0;

        if(pasteMomentomNum == 0)
        {
            if(bodySlapNum >= unlockMomentomStatValue)
            {
                // �� ������ ó�� ����
                Debug.Log("Momentom True Wireless Earbuds 2"); // ������ �ر� üũ�� �ڵ� // ����� ���� �����̾����� ��õ��
                num = firstMomentomValue;

                SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock = true;
            }
        }
        else
        {
            num = (avoidInMomentomNum / momentomAmount) + firstMomentomValue;
        }

        SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.statValue = num;
    }
    public void UpAttackMissedNum()
    {
        // ���� ���� �͵��� ������ ���

        attackMissedNum++;
    }
    public void UpAttackNum()
    {
        // ���͸� ������ ���

        if (SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.isUnlock)
        {
            attackMissedNum = 0;

            return;
        }

        attackNum++;
    }
    public void UpAvoidInMomentomNum()
    {
        if(!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock)
        {
            return;
        }

        avoidInMomentomNum++;
    }
    public void UpBodySlapNum()
    {
        bodySlapNum++;
    }
}
