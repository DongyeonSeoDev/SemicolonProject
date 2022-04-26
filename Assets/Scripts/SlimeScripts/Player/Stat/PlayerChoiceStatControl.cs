using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChoiceStatControl : MonoBehaviour
{
    [Header("누적 피해량이 이 값 이상일 때 마다 맷집값이 상승함")]
    [SerializeField]
    private int enduranceUpAmount = 100;

    [Header("1 맷집당 오르는 최대체력의 양")]
    [SerializeField]
    private float upMaxHpPerEnduranceUpAmount = 5f;

    [Header("인내 스탯의 초기 스탯")]
    [SerializeField]
    private int firstPatienceValue = 1;

    [Header("공격 횟수가 이 값 이상일 때 마다 인내 수치 상승")]
    [SerializeField]
    private int patienceUpAmount = 100;

    [Header("1 인내당 오르는 공격력의 양")]
    [SerializeField]
    private int upDamagePerPatience = 1;

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
                // 처음 이 스탯이 생김
            }

            SlimeGameManager.Instance.Player.PlayerStat.choiceStat.endurance = num;

            SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.maxHp += upMaxHpPerEnduranceUpAmount * (num - pasteEndurance);
        }
    }
    public void UpAttackMissedNum()
    {
        // 몬스터 외의 것들을 때렸을 경우

        attackMissedNum++;
    }
    public void UpAttackNum()
    {
        // 몬스터를 때렸을 경우

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
                // 처음 이 스탯이 생김

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
