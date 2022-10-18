using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EternalStatData
{
    public ushort id; // ���� ��ȹ ������ �ۼ��� ���� ������ �ۼ��Ѵ�.
    public string name; // UI�� �� �ѱ� �̸�

    public int unlockStatValue; // �� ������ �׵��ϱ� ���� checkStartValue���� �� ���� �����ϴ� �ش� ���� ��
    public int firstValue; // �ʱⰪ

    public void DataCpy(EternalStatData stat)
    {
        id = stat.id;
        name = stat.name;

        unlockStatValue = stat.unlockStatValue;
        firstValue = stat.firstValue;
    }
}
[Serializable]
public class ChoiceStatData
{
    public ushort id; // ���� ��ȹ ������ �ۼ��� ���� ������ �ۼ��Ѵ�.
    public string name; // UI�� �� �ѱ� �̸�
    public string targetStat; // �� ChocieStat�� ���� ������ �� ������ Name ��

    public int unlockStatValue; // �� ������ �׵��ϱ� ���� checkStartValue���� �� ���� �����ϴ� �ش� ���� ��
    public int firstValue; // �ʱⰪ
    public int checkStartValue; // �ش� ������ ��� ������ üũ�ϱ� ������ ����

    public int upAmount; // Ư�� ���� upAmount�̻��� �� ���� ���� ��ġ ���
    public int changeUpAmount; // upAmount ������ ���õ� ��, ����ϴ� �� ����Ѵ�
    public float upTargetStatPerChoiceStat; // �� ChoiceStat�� �� 1 �� ������ ��� ������ ��

    public int lastMaxStatLv; // ���� ���Ҵ� �ش� ������ ������ ��

    public void DataCpy(ChoiceStatData stat)
    {
        id = stat.id;
        name = stat.name;
        targetStat = stat.targetStat;

        unlockStatValue = stat.unlockStatValue;
        firstValue = stat.firstValue;
        checkStartValue = stat.checkStartValue;

        upAmount = stat.upAmount;
        changeUpAmount = stat.changeUpAmount;
        upTargetStatPerChoiceStat = stat.upTargetStatPerChoiceStat;

        lastMaxStatLv = stat.lastMaxStatLv;
    }
}
public class PlayerChoiceStatControl : MonoBehaviour
{
    #region ChoiceStat Data �ڷᱸ��
    [SerializeField]
    private List<ChoiceStatData> choiceDataList = new List<ChoiceStatData>();

    private Dictionary<ushort, ChoiceStatData> choiceDataDict = new Dictionary<ushort, ChoiceStatData>();
    public Dictionary<ushort, ChoiceStatData> ChoiceDataDict
    {
        get { return choiceDataDict; }
    }
    private Dictionary<ushort, ChoiceStatData> originChoiceDataDict = new Dictionary<ushort, ChoiceStatData>();
    public Dictionary<ushort, ChoiceStatData> OriginChoiceDataDict
    {
        get { return originChoiceDataDict; }
    }
    #endregion

    #region EternalStat Data �ڷᱸ��
    [SerializeField]
    private List<EternalStatData> eternalDataList = new List<EternalStatData>();
    private Dictionary<ushort, EternalStatData> eternalStatDataDict = new Dictionary<ushort, EternalStatData>();
    public Dictionary<ushort, EternalStatData> EternalStatDataDict
    {
        get { return eternalStatDataDict; }
    }
    #endregion

    [SerializeField]
    private int avoidNum = 0;
    public int AvoidNum
    {
        get { return avoidNum; }
    }

    [SerializeField]
    private int avoidInMomentomNum = 0;
    public int AvoidInMomentomNum
    {
        get { return avoidInMomentomNum; }
    }

    [SerializeField]
    private int fakeNum = 0;
    public int FakeNum
    {
        get { return fakeNum; }
    }

    [SerializeField]
    private float totalDamage = 0f;
    /// <summary>
    /// ���ݱ��� ���� �� ������
    /// </summary>
    public float TotalDamage
    {
        get { return totalDamage; }
    }

    [SerializeField]
    private int attackMissedNum = 0;
    /// <summary>
    /// ������ ������ Ƚ��
    /// </summary>
    public int AttackMissedNum
    {
        get { return attackMissedNum; }
    }

    [SerializeField]
    private int attackNum = 0;
    /// <summary>
    /// ������ ������ Ƚ��
    /// </summary>
    public int AttackNum
    {
        get { return attackNum; }
    }

    private bool mucusChargeEnergyMax = false;
    public bool MucusChargeEnergyMax
    {
        get { return mucusChargeEnergyMax; }
        set { mucusChargeEnergyMax = value; }
    }

    [SerializeField]
    private float mucusChargeEnergyMaxTime = 0;
    public float MucusChargeEnergyMaxTime
    {
        get
        {
            return mucusChargeEnergyMaxTime;
        }
    }

    /// <summary>
    /// -���� Ư���� �������� ���� Ư�� �ൿ ī��Ʈ üũ
    /// </summary>
    private Dictionary<ushort, int> minusStatCountDict = new Dictionary<ushort, int>();

    /// <summary>
    /// -���� Ư���� �� �� �ٽ� �ǵ��� �޴� ����. -> ���Ĵ�� �׳� ���� �÷��ָ� �ּڰ��� �����ϴ� ���� ó���� �Ͼ�� ��� �������� ���� �������⶧���� �̷��� ó����
    /// </summary>
    private Dictionary<ushort, float> accumStatDic = new Dictionary<ushort, float>();  

    private void Start()
    {
        if (TutorialManager.Instance.IsTutorialStage)
        {
            StopListenings();
        }

        for (int i = 0; i < choiceDataList.Count; i++)
        {
            choiceDataList[i].id = (ushort)(100 + i * 5);
        }

        ChoiceStatData choiceStatData;
        foreach (var item in choiceDataList)
        {
            choiceDataDict.Add(item.id, item);

            choiceStatData = new ChoiceStatData();
            choiceStatData.DataCpy(item);

            originChoiceDataDict.Add(item.id, choiceStatData);
        }

        foreach(var item in eternalDataList)
        {
            eternalStatDataDict.Add(item.id, item);
        }

        foreach(ushort id in ChoiceDataDict.Keys)
        {
            if (NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(id).IsMinusStatProp)
            {
                minusStatCountDict.Add(id, 0);
                accumStatDic.Add(id, 0f);
            }
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", ChoiceStatControlReset);
        EventManager.StartListening("OnEnemyAttack", UpAttackNum);
        EventManager.StartListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StartListening("Avoid", UpAvoidNum);
        EventManager.StartListening("OnAvoidInMomentom", UpAvoidInMomentomNum);

        EventManager.StartListening("ExitCurrentMap", UpExitRoomCount);
        EventManager.StartListening("EnemyDead", UpEnemyDeadCount);
    }
    private void OnDisable()
    {
        StopListenings();
    }

    private void StopListenings()
    {
        EventManager.StopListening("PlayerDead", ChoiceStatControlReset);
        EventManager.StopListening("OnEnemyAttack", UpAttackNum);
        EventManager.StopListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StopListening("Avoid", UpAvoidNum);
        EventManager.StopListening("OnAvoidInMomentom", UpAvoidInMomentomNum);

        EventManager.StopListening("ExitCurrentMap", UpExitRoomCount);
        EventManager.StopListening("EnemyDead", UpEnemyDeadCount);
    }

    private void Update()
    {
        if (!TutorialManager.Instance.IsTutorialStage)
        {
            #region ChoiceStat ����
            CheckMucusMaxTime();

            CheckEndurance();
            CheckProficiency();
            CheckMomentom();
            CheckFrenzy();
            CheckReflection();
            CheckFake();
            CheckMucusRecharge();
            #endregion

            #region EternalStat ����
            CheckAttackSpeed();
            CheckDefense();
            CheckSpeed();
            CheckCriticalRate();
            CheckCriticalDamage();
            #endregion
        }
    }
    public void CheckAttackSpeed()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.eternalStat.attackSpeed;

        if (stat.isUnlock)
        {
            return;
        }

        if(eternalStatDataDict[NGlobal.AttackSpeedID].unlockStatValue <= PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Slime_01.ToString()))
        {
            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }
    public void CheckDefense()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.eternalStat.defense;

        if (stat.isUnlock)
        {
            return;
        }

        if (eternalStatDataDict[NGlobal.DefenseID].unlockStatValue <= PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Slime_03.ToString()))
        {
            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }
    public void CheckIntellect()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.eternalStat.intellect;

        if (stat.isUnlock)
        {
            return;
        }

        if (eternalStatDataDict[NGlobal.IntellectID].unlockStatValue <= PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.SkeletonMage_06.ToString()))
        {
            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }
    public void CheckSpeed()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.eternalStat.speed;

        if (stat.isUnlock)
        {
            return;
        }

        if (eternalStatDataDict[NGlobal.SpeedID].unlockStatValue <= PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Rat_02.ToString()))
        {
            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }
    public void CheckCriticalRate()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.eternalStat.criticalRate;

        if (stat.isUnlock)
        {
            return;
        }

        if (eternalStatDataDict[NGlobal.CriticalRate].unlockStatValue <= PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.SkeletonArcher_04.ToString()))
        {
            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }
    public void CheckCriticalDamage()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.eternalStat.criticalDamage;

        if (stat.isUnlock)
        {
            return;
        }

        if (eternalStatDataDict[NGlobal.CriticalDamage].unlockStatValue <= PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.SkeletonArcher_04.ToString()))
        {
            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }
    public void CheckMucusMaxTime()
    {
        if(mucusChargeEnergyMax)
        {
            mucusChargeEnergyMaxTime += Time.deltaTime;
        }
    }
    public void CheckEndurance()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance;

        if (!CheckCanUpChoiceStat(stat))
        {
            return;
        }

        if (stat.isUnlock)
        {
            if ((int)totalDamage - choiceDataDict[NGlobal.EnduranceID].checkStartValue >= choiceDataDict[NGlobal.EnduranceID].upAmount)
            {
                if (stat.statLv >= stat.maxStatLv)
                {
                    return;
                }

                UpChoiceStatLv(stat);

                SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp.statValue += choiceDataDict[NGlobal.EnduranceID].upTargetStatPerChoiceStat;

                EnduranceCheckValueReset(false);
            }
        }
        else
        {
            if (!SlimeGameManager.Instance.Player.PlayerStat.eternalStat.maxHp.isUnlock)
            {
                return;
            }

            if (((int)totalDamage - choiceDataDict[NGlobal.EnduranceID].checkStartValue) >= choiceDataDict[NGlobal.EnduranceID].unlockStatValue)
            {
                // ó�� �� ������ ����
                UIManager.Instance.playerStatUI.StatUnlock(stat);

                stat.statValue = choiceDataDict[NGlobal.EnduranceID].firstValue;
                stat.statLv = choiceDataDict[NGlobal.EnduranceID].firstValue;

                ChoiceStatUnlockSetting(stat);

                EnduranceCheckValueReset(true);
            }
        }
    }
    public void EnduranceCheckValueReset(bool aboutUnlock) // unlocküũ�� ���̴� ���̶� upAmount���� üũ�� ���̴� ���� ������, �̷������� ���ش�.
    {
        if (aboutUnlock)
        {
            choiceDataDict[NGlobal.EnduranceID].checkStartValue += choiceDataDict[NGlobal.EnduranceID].unlockStatValue;
        }
        else
        {
            choiceDataDict[NGlobal.EnduranceID].checkStartValue += choiceDataDict[NGlobal.EnduranceID].upAmount;
        }
    }
    public void CheckProficiency()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency;

        if (!CheckCanUpChoiceStat(stat))
        {
            return;
        }

        if (SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency.isUnlock)
        {
            if ((attackNum + attackMissedNum - choiceDataDict[NGlobal.ProficiencyID].checkStartValue) >= (choiceDataDict[NGlobal.ProficiencyID].upAmount))
            {
                if (stat.statLv >= stat.maxStatLv)
                {
                    return;
                }

                UpChoiceStatLv(stat);

                ProficiencyCheckValueReset(false);

                choiceDataDict[NGlobal.ProficiencyID].upAmount = originChoiceDataDict[NGlobal.ProficiencyID].upAmount * (int)stat.statValue;

                SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage.statValue += choiceDataDict[NGlobal.ProficiencyID].upTargetStatPerChoiceStat;
                SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage.statValue += choiceDataDict[NGlobal.ProficiencyID].upTargetStatPerChoiceStat;
            }
        }
        else
        {
            if (attackMissedNum - choiceDataDict[NGlobal.ProficiencyID].checkStartValue >= choiceDataDict[NGlobal.ProficiencyID].unlockStatValue)
            {
                // ó�� �� ������ ����

                stat.statValue = choiceDataDict[NGlobal.ProficiencyID].firstValue;
                stat.statLv = choiceDataDict[NGlobal.ProficiencyID].firstValue;

                ChoiceStatUnlockSetting(stat);

                UIManager.Instance.playerStatUI.StatUnlock(stat);

                ProficiencyCheckValueReset(true);
            }
        }
    }
    private void ProficiencyCheckValueReset(bool aboutUnlock)
    {
        if (aboutUnlock)
        {
            choiceDataDict[NGlobal.ProficiencyID].checkStartValue += choiceDataDict[NGlobal.ProficiencyID].unlockStatValue;
        }
        else
        {
            choiceDataDict[NGlobal.ProficiencyID].checkStartValue += choiceDataDict[NGlobal.ProficiencyID].upAmount;
        }
    }

    public void CheckMomentom()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom;

        if (!CheckCanUpChoiceStat(stat))
        {
            return;
        }

        if (SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock)
        {
            if (((avoidInMomentomNum - choiceDataDict[NGlobal.MomentomID].checkStartValue) >= (choiceDataDict[NGlobal.MomentomID].upAmount)))
            {
                if (stat.statLv >= stat.maxStatLv)
                {
                    return;
                }

                UpChoiceStatLv(stat);

                MomentomCheckValueReset();

                choiceDataDict[NGlobal.MomentomID].upAmount += originChoiceDataDict[NGlobal.MomentomID].upAmount;
            }
        }
        else
        {
            if (PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Rat_02.ToString())
                >= choiceDataDict[NGlobal.MomentomID].unlockStatValue)
            {
                // �� ������ ó�� ����

                stat.statValue = choiceDataDict[NGlobal.MomentomID].firstValue;
                stat.statLv = choiceDataDict[NGlobal.MomentomID].firstValue;

                ChoiceStatUnlockSetting(stat);

                MomentomCheckValueReset();

                UIManager.Instance.playerStatUI.StatUnlock(stat);
            }
        }
    }

    public void MomentomCheckValueReset()
    {
        choiceDataDict[NGlobal.MomentomID].checkStartValue += choiceDataDict[NGlobal.MomentomID].upAmount;
    }

    public void CheckFrenzy()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.frenzy;

        if (!CheckCanUpChoiceStat(stat))
        {
            return;
        }

        if (!stat.isUnlock && 
            PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Slime_03.ToString())
                >= choiceDataDict[NGlobal.FrenzyID].unlockStatValue)
        {
            stat.statValue = choiceDataDict[NGlobal.FrenzyID].firstValue;
            stat.statLv = choiceDataDict[NGlobal.FrenzyID].firstValue;

            ChoiceStatUnlockSetting(stat);

            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }

    public void CheckReflection()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.reflection;

        if (!CheckCanUpChoiceStat(stat))
        {
            return;
        }

        if (!stat.isUnlock &&
            PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Slime_01.ToString())
                >= choiceDataDict[NGlobal.ReflectionID].unlockStatValue)
        {
            stat.statValue = choiceDataDict[NGlobal.ReflectionID].firstValue;
            stat.statLv = choiceDataDict[NGlobal.ReflectionID].firstValue;

            ChoiceStatUnlockSetting(stat);

            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }
    public void CheckMucusRecharge()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.mucusRecharge;

        if (!CheckCanUpChoiceStat(stat))
        {
            return;
        }

        if (stat.isUnlock)
        {
            if (mucusChargeEnergyMaxTime - choiceDataDict[NGlobal.MucusRechargeID].checkStartValue
                >= choiceDataDict[NGlobal.MucusRechargeID].upAmount)
            {
                if (stat.statLv >= stat.maxStatLv)
                {
                    return;
                }

                UpChoiceStatLv(stat);

                MucusRechargeValueReset();

                choiceDataDict[NGlobal.MucusRechargeID].upAmount += choiceDataDict[NGlobal.MucusRechargeID].changeUpAmount;
            }
        }
        else
        {
            if (PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.SkeletonArcher_04.ToString())
                >= choiceDataDict[NGlobal.MucusRechargeID].unlockStatValue)
            {
                stat.statValue = choiceDataDict[NGlobal.MucusRechargeID].firstValue;
                stat.statLv = choiceDataDict[NGlobal.MucusRechargeID].firstValue;

                ChoiceStatUnlockSetting(stat);

                UIManager.Instance.playerStatUI.StatUnlock(stat);
            }
        }
    }
    public void MucusRechargeValueReset()
    {
        choiceDataDict[NGlobal.MucusRechargeID].checkStartValue += choiceDataDict[NGlobal.MucusRechargeID].upAmount;
    }

    public void CheckFake()
    {
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.fake;

        if (!CheckCanUpChoiceStat(stat))
        {
            return;
        }

        if (SlimeGameManager.Instance.Player.PlayerStat.choiceStat.fake.isUnlock)
        {
            if ((fakeNum - choiceDataDict[NGlobal.FakeID].checkStartValue) >= choiceDataDict[NGlobal.FakeID].upAmount)
            {
                if (stat.statLv >= stat.maxStatLv)
                {
                    return;
                }

                UpChoiceStatLv(stat);

                choiceDataDict[NGlobal.FakeID].upAmount *= 2;

                FakeCheckValueReset();
            }
        }
        else
        {
            if (avoidNum >= choiceDataDict[NGlobal.FakeID].unlockStatValue)
            {
                stat.statValue = choiceDataDict[NGlobal.FakeID].firstValue;
                stat.statLv = choiceDataDict[NGlobal.FakeID].firstValue;

                ChoiceStatUnlockSetting(stat);

                UIManager.Instance.playerStatUI.StatUnlock(stat);
            }
        }

        SlimeGameManager.Instance.Player.FakePercentage = stat.statValue * choiceDataDict[NGlobal.FakeID].upTargetStatPerChoiceStat;
    }
    public void FakeCheckValueReset()
    {
        choiceDataDict[NGlobal.FakeID].checkStartValue = choiceDataDict[NGlobal.FakeID].upAmount;
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

        if (!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency.isUnlock)
        {
            ProficiencyCheckValueReset(true);

            return;
        }

        attackNum++;
    }
    public void UpAvoidNum()
    {
        avoidNum++;
    }
    public void UpAvoidInMomentomNum()
    {
        if(!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock)
        {
            return;
        }

        avoidInMomentomNum++;
    }
    public void UpFakeNum()
    {
        fakeNum++;
    }

    public void UpTotalDamage(float value)
    {
        if(!SlimeGameManager.Instance.Player.PlayerStat.eternalStat.maxHp.isUnlock)
        {
            return;
        }

        totalDamage += value;
    }

    private void UpExitRoomCount()
    {
        ChoiceStat stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat;
        if (stat.weak.isUnlock)
        {
            CheckMinusStatCount(NGlobal.WeakID);
        }
        if (stat.tiredness.isUnlock)
        {
            CheckMinusStatCount(NGlobal.TirednessID);
        }
        if(stat.fear.isUnlock)
        {
            CheckMinusStatCount(NGlobal.FearID);
        }

        if (StageManager.Instance.IsBattleArea)
        {
            if (stat.soft.isUnlock)
            {
                CheckMinusStatCount(NGlobal.SoftID);
            }
            if (stat.feeble.isUnlock)
            {
                CheckMinusStatCount(NGlobal.FeebleID);
            }
        }
    }

    private void UpEnemyDeadCount(GameObject obj, string str, bool b)
    {
        ChoiceStat stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat;
        if (stat.sluggish.isUnlock)
        {
            CheckMinusStatCount(NGlobal.SluggishID);
        }
        if (stat.dull.isUnlock)
        {
            CheckMinusStatCount(NGlobal.DullID);
        }
        if (stat.terror.isUnlock)
        {
            CheckMinusStatCount(NGlobal.TerrorID);
        }
    }

    private void CheckMinusStatCount(ushort id)
    {
        minusStatCountDict[id]++;
        if (minusStatCountDict[id] >= choiceDataDict[id].upAmount)
        {
            StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
            if (stat.statLv < stat.maxStatLv)
            {
                minusStatCountDict[id] = 0;
                stat.statLv++;
                NGlobal.playerStatUI.StatUp(id);
                ChoiceStatCheckStatValueSet(id);
            }
        }
    }

    public void ChoiceStatControlReset()
    {
        avoidInMomentomNum = 0;
        totalDamage = 0;
        attackMissedNum = 0;
        attackNum = 0;
        mucusChargeEnergyMaxTime = 0;

        foreach (var item in choiceDataDict)
        {
            item.Value.checkStartValue = originChoiceDataDict[item.Key].checkStartValue;

            if (minusStatCountDict.ContainsKey(item.Key))
            {
                minusStatCountDict[item.Key] = 0;
                accumStatDic[item.Key] = 0f;
            }
        }

        ChoiceStatDataListMaxStatLvReset();
    }
    public void WhenTradeStat(ushort statID)
    {
        ChoiceStatCheckStatValueSet(statID);
    }
    private void ChoiceStatCheckStatValueSet(ushort statID)
    {
        #region Swtich��
        switch (statID)
        {
            case NGlobal.MomentomID:
                {
                    choiceDataDict[NGlobal.MomentomID].checkStartValue = avoidInMomentomNum;
                }
                break;
            case NGlobal.FakeID:
                {
                    choiceDataDict[NGlobal.FakeID].checkStartValue = fakeNum;
                }
                break;
            case NGlobal.EnduranceID:
                {
                    choiceDataDict[NGlobal.EnduranceID].checkStartValue = (int)totalDamage;
                }
                break;
            case NGlobal.ProficiencyID:
                {
                    choiceDataDict[NGlobal.ProficiencyID].checkStartValue = attackMissedNum + attackNum;
                }
                break;
            case NGlobal.MucusRechargeID:
                {
                    choiceDataDict[NGlobal.MucusRechargeID].checkStartValue = (int)mucusChargeEnergyMaxTime;
                }
                break;

            case NGlobal.HealthID:
            case NGlobal.StrongID:
            case NGlobal.PowerfulID:
            case NGlobal.NimbleID:
            case NGlobal.ActiveID:
            case NGlobal.IncisiveID:
            case NGlobal.PersistentID:
            case NGlobal.HardID:
                {
                    if (NGlobal.playerStatUI.choiceStatDic[statID].isUnlock)  //����
                    {
                        NGlobal.playerStatUI.eternalStatDic[NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(statID).needStatID].second.statValue
                            += choiceDataDict[statID].upTargetStatPerChoiceStat;
                    }
                    else  //�Ǹ�
                    {
                        ChoiceStatSO so = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(statID);
                        float min = NGlobal.playerStatUI.GetStatSOData<EternalStatSO>(so.needStatID).minStatValue;
                        NGlobal.playerStatUI.eternalStatDic[so.needStatID].second.statValue -= choiceDataDict[statID].upTargetStatPerChoiceStat;
                        if(NGlobal.playerStatUI.GetCurrentPlayerStat(so.needStatID) < min)
                        {
                            NGlobal.playerStatUI.eternalStatDic[so.needStatID].second.statValue = -NGlobal.playerStatUI.eternalStatDic[so.needStatID].first.statValue + min;
                        }
                        if (NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxHpID) < NGlobal.playerStatUI.PlayerStat.currentHp)  //�ִ�ü���� ���� ü�º��� ������ ����ü���� �ִ�ü������ ����
                        {
                            NGlobal.playerStatUI.PlayerStat.currentHp = NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxHpID);
                        }
                    }

                    UIManager.Instance.UpdatePlayerHPUI();
                }
                break;

            case NGlobal.WeakID:
                {
                    if (NGlobal.playerStatUI.choiceStatDic[statID].isUnlock)  //����
                    {
                        float value = NGlobal.playerStatUI.eternalStatDic[NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(statID).needStatID].first.statLv == 1 ?
                        choiceDataDict[statID].firstValue : choiceDataDict[statID].upTargetStatPerChoiceStat;  //���߿� �Ǹ��ϸ� ���� �ǵ��� �޾ƾ��ϹǷ� �� �ǵ��� ������ ������
                        Global.CurrentPlayer.PlayerStat.additionalEternalStat.maxHp.statValue -= value;

                        float min = NGlobal.playerStatUI.GetStatSOData<EternalStatSO>(NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(statID).needStatID).minStatValue;
                        if (NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxHpID) < min)  
                        {
                            value -= (min - NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxHpID));  //�ּڰ� ������ �ϸ� ���� �� ������ϴ� ��ŭ value���� ���� ���߿� ��������
                            Global.CurrentPlayer.PlayerStat.additionalEternalStat.maxHp.statValue = -Global.CurrentPlayer.PlayerStat.eternalStat.maxHp.statValue + min;
                        }

                        if (NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxHpID) < NGlobal.playerStatUI.PlayerStat.currentHp)  //�ִ�ü���� ���� ü�º��� ������ ����ü���� �ִ�ü������ ����
                        {
                            NGlobal.playerStatUI.PlayerStat.currentHp = NGlobal.playerStatUI.GetCurrentPlayerStat(NGlobal.MaxHpID);
                        }

                        accumStatDic[statID] += value;
                    }
                    else  //�Ǹ�
                    {
                        Global.CurrentPlayer.PlayerStat.additionalEternalStat.maxHp.statValue += accumStatDic[statID];
                        accumStatDic[statID] = 0f;
                    }
                    
                    UIManager.Instance.UpdatePlayerHPUI();
                }
                break;
            case NGlobal.SoftID:
            case NGlobal.FeebleID:
            case NGlobal.TirednessID:
            case NGlobal.FearID:
            case NGlobal.SluggishID:
            case NGlobal.DullID:
            case NGlobal.TerrorID:
                {
                    ChoiceStatSO so = NGlobal.playerStatUI.GetStatSOData<ChoiceStatSO>(statID);

                    if (NGlobal.playerStatUI.choiceStatDic[statID].isUnlock)  //Ư�� ����
                    {
                        float min2 = NGlobal.playerStatUI.GetStatSOData<EternalStatSO>(so.needStatID).minStatValue;

                        //���߿� �Ǹ��ϸ� ���� �ǵ��� �޾ƾ��ϹǷ� �� �ǵ��� ������ ������
                        float value2 = NGlobal.playerStatUI.eternalStatDic[so.needStatID].first.statLv == 1 ? choiceDataDict[statID].firstValue : choiceDataDict[statID].upTargetStatPerChoiceStat;
                        NGlobal.playerStatUI.eternalStatDic[so.needStatID].second.statValue -= value2;

                        if (NGlobal.playerStatUI.GetCurrentPlayerStat(so.needStatID) < min2)  //�ִ�ü�� ���� ���ȵ��� �ּ�ġ�� 0���� ��
                        {
                            value2 -= (min2 - NGlobal.playerStatUI.GetCurrentPlayerStat(so.needStatID)); //�ּڰ� ������ �ϸ� ���� �� ������ϴ� ��ŭ value���� ���� ���߿� ��������
                            NGlobal.playerStatUI.eternalStatDic[so.needStatID].second.statValue = -NGlobal.playerStatUI.eternalStatDic[so.needStatID].first.statValue + min2;
                        }

                        //�ּ�ü���� �ִ�ü�º��� ���ų� Ŭ ���� ����ó���� ���� �̰��� ����

                        accumStatDic[statID] += value2;
                    }
                    else  //Ư�� �Ǹ�
                    {
                        NGlobal.playerStatUI.eternalStatDic[so.needStatID].second.statValue += accumStatDic[statID];
                        accumStatDic[statID] = 0f;
                    }
                }
                break;
        }
        #endregion
    }

    public float GetGrowthRate(ushort id)
    {
        if (!choiceDataDict.ContainsKey(id)) return 0f;

        StatElement stat = NGlobal.playerStatUI.choiceStatDic[id];
        if (stat.maxStatLv <= stat.statLv) return 1f;  //����Ұ��ų� ������ ���⼭ 1�� ���� ������

        if (minusStatCountDict.ContainsKey(id))  //���Ȱ��� Ư�� ����� ��ȯ
        {
            return (float)minusStatCountDict[id] / choiceDataDict[id].upAmount;
        }

        float eachValue = 0f;

        switch (id)  //����̳� ���� Ư���� ����� ��ȯ. -> ���� case�� ���� �Ŵ� ����Ұ��ų� ���������ۿ� ���� ���ϴ� ��(���߹߻簰��)
        {
            case NGlobal.MomentomID:
                {
                    eachValue = avoidInMomentomNum;
                }
                break;
            case NGlobal.FakeID:
                {
                    eachValue = fakeNum;
                }
                break;
            case NGlobal.EnduranceID:
                {
                    eachValue = totalDamage;
                }
                break;
            case NGlobal.ProficiencyID:
                {
                    eachValue = attackNum;
                }
                break;
            case NGlobal.MucusRechargeID:
                {
                    eachValue = mucusChargeEnergyMaxTime;
                }
                break;

            default:
                return 0f;  //���߹߻� ������ ���⿡�� ��ȯ��
        }

        return (eachValue - choiceDataDict[id].checkStartValue) / choiceDataDict[id].upAmount;
    }

    private void UpChoiceStatLv(StatElement choiceStat)
    {
        ushort statId = choiceStat.id;

        choiceStat.statLv++;
        choiceStat.statValue++;

        ChoiceStatUnlockSetting(choiceStat);

        NGlobal.playerStatUI.StatUp(choiceStat.id);
    }
    private bool CheckCanUpChoiceStat(StatElement choiceStat)
    {
        ChoiceStatData choiceStatData = choiceDataDict[choiceStat.id];

        return choiceStat.statLv >= choiceStatData.lastMaxStatLv;
    }
    private void ChoiceStatUnlockSetting(StatElement stat)
    {
        if (choiceDataDict[stat.id].lastMaxStatLv < stat.statLv)
        {
            choiceDataDict[stat.id].lastMaxStatLv = stat.statLv;
        }
    }
    private void ChoiceStatDataListMaxStatLvReset()
    {
        foreach(var item in choiceDataDict)
        {
            item.Value.lastMaxStatLv = 0;
        }
    }
}
