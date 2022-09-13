using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EternalStatData
{
    public ushort id; // 스탯 기획 문서에 작성된 변수 명으로 작성한다.
    public string name; // UI에 뜰 한글 이름

    public int unlockStatValue; // 이 스탯을 휙득하기 위해 checkStartValue에서 때 부터 얻어야하는 해당 값의 양
    public int firstValue; // 초기값

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
    public ushort id; // 스탯 기획 문서에 작성된 변수 명으로 작성한다.
    public string name; // UI에 뜰 한글 이름
    public string targetStat; // 이 ChocieStat에 의해 오르게 될 스탯의 Name 값

    public int unlockStatValue; // 이 스탯을 휙득하기 위해 checkStartValue에서 때 부터 얻어야하는 해당 값의 양
    public int firstValue; // 초기값
    public int checkStartValue; // 해당 스탯을 얻는 조건을 체크하기 시작한 시점

    public int upAmount; // 특정 값이 upAmount이상일 때 마다 스탯 수치 상승
    public int changeUpAmount; // upAmount 변동과 관련된 값, 사용하는 놈만 사용한다
    public float upTargetStatPerChoiceStat; // 이 ChoiceStat의 값 1 당 오르는 대상 스탯의 값

    public int lastMaxStatLv; // 가장 높았던 해당 스탯의 레벨의 값

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
    #region ChoiceStat Data 자료구조
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

    #region EternalStat Data 자료구조
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
    /// 지금까지 받은 총 데미지
    /// </summary>
    public float TotalDamage
    {
        get { return totalDamage; }
    }

    [SerializeField]
    private int attackMissedNum = 0;
    /// <summary>
    /// 공격이 빗나간 횟수
    /// </summary>
    public int AttackMissedNum
    {
        get { return attackMissedNum; }
    }

    [SerializeField]
    private int attackNum = 0;
    /// <summary>
    /// 공격이 적중한 횟수
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

    ///// <summary>
    ///// 돌진한 횟수
    ///// </summary>
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
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", ChoiceStatControlReset);
        EventManager.StartListening("OnEnemyAttack", UpAttackNum);
        EventManager.StartListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StartListening("Avoid", UpAvoidNum);
        EventManager.StartListening("OnAvoidInMomentom", UpAvoidInMomentomNum);
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
    }

    private void Update()
    {
        if (!TutorialManager.Instance.IsTutorialStage)
        {
            #region ChoiceStat 관련
            CheckMucusMaxTime();

            CheckEndurance();
            CheckProficiency();
            CheckMomentom();
            CheckFrenzy();
            CheckReflection();
            CheckFake();
            CheckMucusRecharge();
            #endregion

            #region EternalStat 관련
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
                // 처음 이 스탯이 생김
                UIManager.Instance.playerStatUI.StatUnlock(stat);

                stat.statValue = choiceDataDict[NGlobal.EnduranceID].firstValue;
                stat.statLv = choiceDataDict[NGlobal.EnduranceID].firstValue;

                ChoiceStatUnlockSetting(stat);

                EnduranceCheckValueReset(true);
            }
        }
    }
    public void EnduranceCheckValueReset(bool aboutUnlock) // unlock체크에 쓰이는 값이랑 upAmount관련 체크에 쓰이는 값이 같으면, 이런식으로 해준다.
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
                // 처음 이 스탯이 생김

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
                // 이 스탯이 처음 생김

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
        // 몬스터 외의 것들을 때렸을 경우

        if (!SlimeGameManager.Instance.Player.PlayerStat.eternalStat.minDamage.isUnlock)
        {
            return;
        }

        attackMissedNum++;
    }
    public void UpAttackNum()
    {
        // 몬스터를 때렸을 경우

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
        }

        ChoiceStatDataListMaxStatLvReset();
    }
    public void WhenTradeStat(ushort statID)
    {
        ChoiceStatCheckStatValueSet(statID);
    }
    private void ChoiceStatCheckStatValueSet(ushort statID)
    {
        #region Swtich문
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
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.maxHp.statValue += choiceDataDict[NGlobal.HealthID].upTargetStatPerChoiceStat;
                    UIManager.Instance.UpdatePlayerHPUI();
                }
                break;
            case NGlobal.StrongID:
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.minDamage.statValue += choiceDataDict[NGlobal.StrongID].upTargetStatPerChoiceStat;
                }
                break;
            case NGlobal.PowerfulID:
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.maxDamage.statValue += choiceDataDict[NGlobal.PowerfulID].upTargetStatPerChoiceStat;
                }
                break;
            case NGlobal.NimbleID:
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.attackSpeed.statValue += choiceDataDict[NGlobal.NimbleID].upTargetStatPerChoiceStat;
                }
                break;
            case NGlobal.ActiveID:
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.speed.statValue += choiceDataDict[NGlobal.ActiveID].upTargetStatPerChoiceStat;
                }
                break;
            case NGlobal.IncisiveID:
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.criticalRate.statValue += choiceDataDict[NGlobal.IncisiveID].upTargetStatPerChoiceStat;
                }
                break;
            case NGlobal.PersistentID:
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.criticalDamage.statValue += choiceDataDict[NGlobal.PersistentID].upTargetStatPerChoiceStat;
                }
                break;
            case NGlobal.HardID:
                {
                    Global.CurrentPlayer.PlayerStat.additionalEternalStat.criticalDamage.statValue += choiceDataDict[NGlobal.HardID].upTargetStatPerChoiceStat;
                }
                break;
        }
        #endregion
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
