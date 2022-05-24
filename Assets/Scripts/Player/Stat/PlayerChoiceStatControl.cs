using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ChoiceStatData
{
    public string id; // ���� ��ȹ ������ �ۼ��� ���� ������ �ۼ��Ѵ�.
    public string name; // UI�� �� �ѱ� �̸�
    public string targetStat; // �� ChocieStat�� ���� ������ �� ������ Name ��

    public int firstValue; // �ʱⰪ
    public int unlockStatValue; // �� ������ �׵��ϱ� ���� �ش� ���� ��

    public int upAmount; // Ư�� ���� upAmount�̻��� �� ���� ���� ��ġ ���
    public float upTargetStatPerChoiceStat; // �� ChoiceStat�� �� 1 �� ������ ��� ������ ��
}
public class PlayerChoiceStatControl : MonoBehaviour
{
    [SerializeField]
    private List<ChoiceStatData> choiceDataList = new List<ChoiceStatData>();

    private Dictionary<string, ChoiceStatData> choiceDataDict = new Dictionary<string, ChoiceStatData>();
    public Dictionary<string, ChoiceStatData> ChoiceDataDict
    {
        get { return choiceDataDict; }
    }

    [SerializeField]
    private int avoidInMomentomNum = 0;
    public int AvoidInMomentomNum
    {
        get { return avoidInMomentomNum; }
        set { avoidInMomentomNum = value; }
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
    private void Start()
    {
        foreach(var item in choiceDataList)
        {
            choiceDataDict.Add(item.id.ToLower(), item);
        }
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
        int num = (int)totalDamage / choiceDataDict["endurance"].upAmount;

        if (pasteEndurance != num && num > 0)
        {
            if(pasteEndurance == 0)
            {
                // ó�� �� ������ ����
                SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.isUnlock = true;
                num = choiceDataDict["endurance"].firstValue;
            }

            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.statValue = num;

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp.statValue += choiceDataDict["endurance"].upTargetStatPerChoiceStat * (num - pasteEndurance);
        }
    }
    public void CheckPatience()
    {
        float pastePatienceNum = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.statValue;
        int num = 0;

        if (pastePatienceNum == 0)
        {
            if (attackMissedNum >= choiceDataDict["patience"].unlockStatValue)
            {
                // ó�� �� ������ ����

                num = choiceDataDict["patience"].firstValue;
                SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.isUnlock = true;

                AttackNumReset();
            }
        }
        else
        {
            num = ((attackNum + attackMissedNum) / choiceDataDict["patience"].upAmount) + choiceDataDict["patience"].firstValue;
        }

        SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.statValue = num;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage.statValue -= choiceDataDict["patience"].upTargetStatPerChoiceStat * pastePatienceNum;
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage.statValue -= choiceDataDict["patience"].upTargetStatPerChoiceStat * pastePatienceNum;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage.statValue += choiceDataDict["patience"].upTargetStatPerChoiceStat * num;
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage.statValue += choiceDataDict["patience"].upTargetStatPerChoiceStat * num;
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
            if(bodySlapNum >= choiceDataDict["momentom"].unlockStatValue)
            {
                // �� ������ ó�� ����
                Debug.Log("Momentom True Wireless Earbuds 2"); // ������ �ر� üũ�� �ڵ� // ����� ���� �����̾����� ��õ��
                num = choiceDataDict["momentom"].firstValue;

                SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock = true;
            }
        }
        else
        {
            num = (avoidInMomentomNum / choiceDataDict["momentom"].upAmount) + choiceDataDict["momentom"].firstValue;
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
