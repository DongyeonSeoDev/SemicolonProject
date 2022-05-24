using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ChoiceStatData
{
    public string id; // 스탯 기획 문서에 작성된 변수 명으로 작성한다.
    public string name; // UI에 뜰 한글 이름
    public string targetStat; // 이 ChocieStat에 의해 오르게 될 스탯의 Name 값

    public int firstValue; // 초기값
    public int unlockStatValue; // 이 스탯을 휙듣하기 위한 해당 값의 양

    public int upAmount; // 특정 값이 upAmount이상일 때 마다 스탯 수치 상승
    public float upTargetStatPerChoiceStat; // 이 ChoiceStat의 값 1 당 오르는 대상 스탯의 값
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
    /// 지금까지 받은 총 데미지
    /// </summary>
    public float TotalDamage
    {
        get { return totalDamage; }
        set { totalDamage = value; }
    }

    private int attackMissedNum = 0;
    /// <summary>
    /// 공격이 빗나간 횟수
    /// </summary>
    public int AttackMissedNum
    {
        get { return attackMissedNum; }
        set { attackMissedNum = value; }
    }

    private int attackNum = 0;
    /// <summary>
    /// 공격이 적중한 횟수
    /// </summary>
    public int AttackNum
    {
        get { return attackNum; }
        set { attackNum = value; }
    }

    private int bodySlapNum = 0;
    /// <summary>
    /// 돌진한 횟수
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
                // 처음 이 스탯이 생김
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
                // 처음 이 스탯이 생김

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
                // 이 스탯이 처음 생김
                Debug.Log("Momentom True Wireless Earbuds 2"); // 추진력 해금 체크용 코드 // 참고로 좋은 무선이어폰임 추천함
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
        // 몬스터 외의 것들을 때렸을 경우

        attackMissedNum++;
    }
    public void UpAttackNum()
    {
        // 몬스터를 때렸을 경우

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
