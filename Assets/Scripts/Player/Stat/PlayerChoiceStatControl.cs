using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChoiceStatData
{
    public ushort id; // 스탯 기획 문서에 작성된 변수 명으로 작성한다.
    public string name; // UI에 뜰 한글 이름
    public string targetStat; // 이 ChocieStat에 의해 오르게 될 스탯의 Name 값

    public int firstValue; // 초기값
    public int checkStartValue; // 해당 스탯을 얻는 조건을 체크하기 시작한 시점
    public int unlockStatValue; // 이 스탯을 휙득하기 위해 checkStartValue에서 때 부터 얻어야하는 해당 값의 양

    public int upAmount; // 특정 값이 upAmount이상일 때 마다 스탯 수치 상승
    public float upTargetStatPerChoiceStat; // 이 ChoiceStat의 값 1 당 오르는 대상 스탯의 값

}
public class PlayerChoiceStatControl : MonoBehaviour
{
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

    private float totalDamage = 0f;
    /// <summary>
    /// 지금까지 받은 총 데미지
    /// </summary>
    public float TotalDamage
    {
        get { return totalDamage; }
    }

    private int attackMissedNum = 0;
    /// <summary>
    /// 공격이 빗나간 횟수
    /// </summary>
    public int AttackMissedNum
    {
        get { return attackMissedNum; }
    }

    private int attackNum = 0;
    /// <summary>
    /// 공격이 적중한 횟수
    /// </summary>
    public int AttackNum
    {
        get { return attackNum; }
    }

    //private int bodySlapNum = 0;
    ///// <summary>
    ///// 돌진한 횟수
    ///// </summary>
    //public int BodySlapNum
    //{
    //    get { return bodySlapNum; }
    //}
    private void Start()
    {
        for(int i = 0; i < choiceDataList.Count; i++)
        {
            choiceDataList[i].id = (ushort)(100 + i * 5);
        }

        foreach (var item in choiceDataList)
        {
            choiceDataDict.Add(item.id, item);
            originChoiceDataDict.Add(item.id, item);
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening("PlayerDead", ChoiceStatControlReset);
        EventManager.StartListening("OnEnemyAttack", UpAttackNum);
        EventManager.StartListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StartListening("Avoid", UpAvoidNum);
        EventManager.StartListening("OnAvoidInMomentom", UpAvoidInMomentomNum);
        //EventManager.StartListening("OnBodySlap", UpBodySlapNum);
    }
    private void OnDisable()
    {
        EventManager.StopListening("PlayerDead", ChoiceStatControlReset);
        EventManager.StopListening("OnEnemyAttack", UpAttackNum);
        EventManager.StopListening("OnAttackMiss", UpAttackMissedNum);

        EventManager.StopListening("Avoid", UpAvoidNum);
        EventManager.StopListening("OnAvoidInMomentom", UpAvoidInMomentomNum);
        //EventManager.StopListening("OnBodySlap", UpBodySlapNum);
    }
    private void Update()
    {
        CheckPatience();
        CheckMomentom();
        CheckFrenzy();
        CheckReflection();
    }
    public void CheckEndurance()
    {
        float pasteEndurance = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.statValue;
        int num = 0;
        StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance;

        if(stat.isUnlock)
        {
            num = ((int)totalDamage - choiceDataDict[NGlobal.EnduranceID].checkStartValue) / choiceDataDict[NGlobal.EnduranceID].upAmount;
        }
        else
        {
            num = ((int)totalDamage - choiceDataDict[NGlobal.EnduranceID].checkStartValue) / choiceDataDict[NGlobal.EnduranceID].unlockStatValue;
        }

        if (num > 0)
        {
            if(!stat.isUnlock && !SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.isUnlock)
            {
                // 처음 이 스탯이 생김
                stat.isUnlock = true;

                UIManager.Instance.playerStatUI.StatUnlock(stat);

                num = choiceDataDict[NGlobal.EnduranceID].firstValue;
            }

            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.statValue += num;
            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance.statLv += num;

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp.statValue += choiceDataDict[NGlobal.EnduranceID].upTargetStatPerChoiceStat * (num);

            EnduranceCheckValueReset();
        }
    }
    public void EnduranceCheckValueReset()
    {
        choiceDataDict[NGlobal.EnduranceID].checkStartValue = (int)totalDamage;
    }
    public void CheckPatience()
    {
        float pastePatienceNum = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency.statValue;
        int num = 0;

        if (!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency.isUnlock)
        {
            if (attackMissedNum - choiceDataDict[NGlobal.PatienceID].checkStartValue >= choiceDataDict[NGlobal.PatienceID].unlockStatValue)
            {
                // 처음 이 스탯이 생김

                num = choiceDataDict[NGlobal.PatienceID].firstValue;
                StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency;
                stat.isUnlock = true;

                UIManager.Instance.playerStatUI.StatUnlock(stat);

                ProficiencyCheckValueReset();
            }
        }
        else
        {
            num = ((attackNum + attackMissedNum - choiceDataDict[NGlobal.PatienceID].checkStartValue) / (choiceDataDict[NGlobal.PatienceID].upAmount *
                (SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency.statLv)));
        }

        if (num != 0)
        {
            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency.statValue += num;
            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.proficiency.statLv += num;

            ProficiencyCheckValueReset();

            choiceDataDict[NGlobal.PatienceID].upAmount = originChoiceDataDict[NGlobal.PatienceID].upAmount * num;

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.minDamage.statValue += choiceDataDict[NGlobal.PatienceID].upTargetStatPerChoiceStat * (num);
            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxDamage.statValue += choiceDataDict[NGlobal.PatienceID].upTargetStatPerChoiceStat * (num);
        }
    }
    private void ProficiencyCheckValueReset()
    {
        choiceDataDict[NGlobal.PatienceID].checkStartValue = attackNum + attackMissedNum;
    }

    public void CheckMomentom()
    {
        int num = 0;

        if(!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.isUnlock)
        {
            if(PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Rat_02.ToString())
                >= choiceDataDict[NGlobal.MomentomID].unlockStatValue)
            {
                // 이 스탯이 처음 생김
                Debug.Log("Momentom True Wireless Earbuds 3"); // 추진력 해금 체크용 코드 // 참고로 좋은 무선이어폰임 추천함
                num = choiceDataDict[NGlobal.MomentomID].firstValue;

                StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom;
                stat.isUnlock = true;

                MomentomCheckValueReset();

                UIManager.Instance.playerStatUI.StatUnlock(stat);
            }
        }
        else
        {
            num = ((avoidInMomentomNum - choiceDataDict[NGlobal.MomentomID].checkStartValue) / (choiceDataDict[NGlobal.MomentomID].upAmount));
        }

        if (0 != num)
        {
            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.statValue += num;
            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.statLv += num;

            MomentomCheckValueReset();

            choiceDataDict[NGlobal.MomentomID].upAmount += originChoiceDataDict[NGlobal.MomentomID].upAmount * num;
        }
    }

    public void MomentomCheckValueReset()
    {
        choiceDataDict[NGlobal.MomentomID].checkStartValue = avoidInMomentomNum;
    }

    public void CheckFrenzy()
    {
        if(!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.frenzy.isUnlock && 
            PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Slime_03.ToString())
                >= choiceDataDict[NGlobal.FrenzyID].unlockStatValue)
        {
            StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.frenzy;

            stat.statValue = choiceDataDict[NGlobal.FrenzyID].firstValue;
            stat.statLv = choiceDataDict[NGlobal.FrenzyID].firstValue;

            stat.isUnlock = true;

            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
    }

    public void CheckReflection()
    {
        if (!SlimeGameManager.Instance.Player.PlayerStat.choiceStat.reflection.isUnlock &&
            PlayerEnemyUnderstandingRateManager.Instance.GetUnderstandingRate(Enemy.EnemyType.Slime_01.ToString())
                >= choiceDataDict[NGlobal.ReflectionID].unlockStatValue)
        {
            StatElement stat = SlimeGameManager.Instance.Player.PlayerStat.choiceStat.reflection;

            stat.statValue = choiceDataDict[NGlobal.ReflectionID].firstValue;
            stat.statLv = choiceDataDict[NGlobal.ReflectionID].firstValue;

            stat.isUnlock = true;

            UIManager.Instance.playerStatUI.StatUnlock(stat);
        }
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
            ProficiencyCheckValueReset();

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
    public void ChoiceStatControlReset()
    {
        avoidInMomentomNum = 0;
        totalDamage = 0;
        attackMissedNum = 0;
        attackNum = 0;
    }
}
