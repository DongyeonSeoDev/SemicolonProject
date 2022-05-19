using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoiceStatControl : MonoBehaviour
{
    [Header("맷집 스탯의 초기값")]
    [SerializeField]
    private int firstEnduranceValue = 1;
    [Header("인내 스탯의 초기값")]
    [SerializeField]
    private int firstPatienceValue = 1;
    [Header("추진력 스탯의 초기값")]
    [SerializeField]
    private int firstMomentomValue = 1;

    [Header("맷집 스탯을 휙득하기 위한 해당 값의 양")]
    [SerializeField]
    private int unlockEnduranceStatValue = 100;
    [Header("인내 스탯을 휙득하기 위한 해당 값의 양")]
    [SerializeField]
    private int unlockPatienceStatValue = 100;
    [Header("추진력 스탯을 휙득하기 위한 해당 값의 양")]
    [SerializeField]
    private int unlockMomentomStatValue = 150;


    [Header("누적 피해량이 이 값 이상일 때 마다 맷집값이 상승함")]
    [SerializeField]
    private int enduranceUpAmount = 100;

    [Header("1 맷집당 오르는 최대체력의 양")]
    [SerializeField]
    private float upMaxHpPerEnduranceUpAmount = 5f;


    [Header("공격 횟수가 이 값 이상일 때 마다 인내 수치 상승")]
    [SerializeField]
    private int patienceUpAmount = 100;

    [Header("1 인내당 오르는 공격력의 양")]
    [SerializeField]
    private int upDamagePerPatience = 1;


    [SerializeField]
    private int avoidInMomentomNum = 0;
    public int AvoidInMomentomNum
    {
        get { return avoidInMomentomNum; }
        set { avoidInMomentomNum = value; }
    }

    [Header("회피 횟수가 이 값 이상일 때 추진력 스탯 상승")]
    [SerializeField]
    private int momentomAmount = 10;

    [Header("1 추진력당 오르는 추진력 사용 시의 추가 이동속도 값")]
    [SerializeField]
    private float upMomentomSpeedPerMomentom = 0.2f;
    public float UpMomentomSpeedPerMomentom
    {
        get { return upMomentomSpeedPerMomentom; }
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
                // 처음 이 스탯이 생김
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
                // 처음 이 스탯이 생김

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
                // 이 스탯이 처음 생김
                Debug.Log("Momentom True Wireless Earbuds 2"); // 추진력 해금 체크용 코드 // 참고로 좋은 무선이어폰임 추천함
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
