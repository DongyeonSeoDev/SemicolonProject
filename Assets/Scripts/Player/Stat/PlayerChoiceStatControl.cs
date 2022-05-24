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
    }

    private float totalDamage = 0f;
    /// <summary>
    /// ���ݱ��� ���� �� ������
    /// </summary>
    public float TotalDamage
    {
        get { return totalDamage; }
    }

    private int attackMissedNum = 0;
    /// <summary>
    /// ������ ������ Ƚ��
    /// </summary>
    public int AttackMissedNum
    {
        get { return attackMissedNum; }
    }

    private int attackNum = 0;
    /// <summary>
    /// ������ ������ Ƚ��
    /// </summary>
    public int AttackNum
    {
        get { return attackNum; }
    }

    //private int bodySlapNum = 0;
    ///// <summary>
    ///// ������ Ƚ��
    ///// </summary>
    //public int BodySlapNum
    //{
    //    get { return bodySlapNum; }
    //}
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
        //EventManager.StartListening("OnBodySlap", UpBodySlapNum);
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnEnemyAttack", UpAttackNum);
        EventManager.StopListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StopListening("OnAvoidInMomentom", UpAvoidInMomentomNum);
        //EventManager.StopListening("OnBodySlap", UpBodySlapNum);
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
            if(!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.isUnlock)
            {
                // ó�� �� ������ ����
                StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance;
                stat.isUnlock = true;

                UIManager.Instance.playerStatUI.StatUnlock(stat);

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

        if (!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.isUnlock)
        {
            if (attackMissedNum >= choiceDataDict["patience"].unlockStatValue)
            {
                // ó�� �� ������ ����

                num = choiceDataDict["patience"].firstValue;
                StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience;
                stat.isUnlock = true;

                UIManager.Instance.playerStatUI.StatUnlock(stat);

                AttackNumReset();
            }
        }
        else
        {
            num = ((attackNum + attackMissedNum) / choiceDataDict["patience"].upAmount) + choiceDataDict["patience"].firstValue;
        }

        SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.statValue = num;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage.statValue += choiceDataDict["patience"].upTargetStatPerChoiceStat * (num- pastePatienceNum);
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage.statValue += choiceDataDict["patience"].upTargetStatPerChoiceStat * (num - pastePatienceNum);
    }
    private void AttackNumReset()
    {
        attackMissedNum = 0;
        attackNum = 0;
    }

    public void CheckMomentom()
    {
        int num = 0;

        if(!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock)
        {
            if(PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Rat_02.ToString())
                >= choiceDataDict["momentom"].unlockStatValue)
            {
                // �� ������ ó�� ����
                Debug.Log("Momentom True Wireless Earbuds 2"); // ������ �ر� üũ�� �ڵ� // ����� ���� �����̾����� ��õ��
                num = choiceDataDict["momentom"].firstValue;

                StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom;
                stat.isUnlock = true;

                UIManager.Instance.playerStatUI.StatUnlock(stat);
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

        if (!SlimeGameManager.Instance.Player.PlayerStat.eternalStat.minDamage.isUnlock)
        {
            return;
        }

        attackMissedNum++;
    }
    public void UpAttackNum()
    {
        // ���͸� ������ ���

        if(!SlimeGameManager.Instance.Player.PlayerStat.eternalStat.minDamage.isUnlock)
        {
            return;
        }

        if (!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.patience.isUnlock)
        {
            attackMissedNum = 0;

            return;
        }

        attackNum++;
    }
    public void UpAvoidInMomentomNum()
    {
        if(!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock || !SlimeGameManager.Instance.Player.PlayerStat.eternalStat.speed.isUnlock)
        {
            return;
        }

        avoidInMomentomNum++;
    }
    //public void UpBodySlapNum()
    //{
    //    if(!SlimeGameManager.Instance.Player.PlayerStat.eternalStat.speed.isUnlock)
    //    {
    //        return;
    //    }

    //    bodySlapNum++;
    //}
    public void UpTotalDamage(float value)
    {
        if(!SlimeGameManager.Instance.Player.PlayerStat.eternalStat.maxHp.isUnlock)
        {
            return;
        }

        totalDamage += value;
    }
}
