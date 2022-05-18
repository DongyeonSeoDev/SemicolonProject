using System;

[Serializable]
public class Stat
{
    public int currentStatPoint;   //현재 가지고 있는 스탯 포인트
    public int accumulateStatPoint;  //지금까지 모은 누적 스탯포인트
    public float currentHp;  //현재 체력
    public float currentExp;  //현재 경험치
    public EternalStat eternalStat = new EternalStat();  //기본 영구 스탯
    public EternalStat additionalEternalStat = new EternalStat();  //영구 스탯(추가 능력치)
    public ChoiceStat choiceStat = new ChoiceStat();  //기본 선택 스탯

    #region default stat + additional stat  property
    public float MaxHp  
    {
        get { return eternalStat.maxHp + additionalEternalStat.maxHp; }
    }
    public int MinDamage
    {
        get { return eternalStat.minDamage + additionalEternalStat.minDamage; }
    }
    public int MaxDamage
    {
        get { return eternalStat.maxDamage + additionalEternalStat.maxDamage; }
    }
    public int Defense
    {
        get { return eternalStat.defense + additionalEternalStat.defense; }
    }

    public int Intellect
    {
        get { return eternalStat.intellect + additionalEternalStat.intellect; }
    }

    public float Speed
    {
        get { return eternalStat.speed + additionalEternalStat.speed; }
    }
    public float AttackSpeed
    {
        get { return eternalStat.attackSpeed + additionalEternalStat.attackSpeed; }
    }
    public float CriticalRate
    {
        get { return eternalStat.criticalRate + additionalEternalStat.criticalRate; }
    }

    public int CriticalDamage
    {
        get { return eternalStat.criticalDamage + additionalEternalStat.criticalDamage; }
    }
    #endregion
}

[Serializable]
public class StatElement<T>  // 메모(이거 확인하면 이 주석 지우고) : string 넣어서 그냥 class로 바꿈.
{
    public T statValue;
    public bool isUnlock;  //획득한 스탯인가
    public int usedStatPoint;  //이 스탯에 사용된 스탯포인트 (Stat클래스의 eternalStat에서만 쓰일듯함)
    public string statName;  //이 스탯의 이름 (예 : 체력, 공격력, 방어력)
}

[Serializable]
public class EternalStat
{
    public static EternalStat operator +(EternalStat a, EternalStat b) => new EternalStat(a.maxHp + b.maxHp, a.minDamage + b.minDamage, a.maxDamage + b.maxDamage, a.defense + b.defense, a.intellect + b.intellect, a.speed + b.speed, a.attackSpeed + b.attackSpeed, a.criticalRate + b.criticalRate, a.criticalDamage + b.criticalDamage);
    public static EternalStat operator -(EternalStat a, EternalStat b) => new EternalStat(a.maxHp - b.maxHp, a.minDamage - b.minDamage, a.maxDamage - b.maxDamage, a.defense - b.defense, a.intellect - b.intellect, a.speed - b.speed, a.attackSpeed - b.attackSpeed, a.criticalRate - b.criticalRate, a.criticalDamage - b.criticalDamage);
    public static EternalStat operator *(EternalStat a, EternalStat b) => new EternalStat(a.maxHp * b.maxHp, a.minDamage * b.minDamage, a.maxDamage * b.maxDamage, a.defense * b.defense, a.intellect * b.intellect, a.speed * b.speed, a.attackSpeed * b.attackSpeed, a.criticalRate * b.criticalRate, a.criticalDamage * b.criticalDamage);
    public static EternalStat operator *(EternalStat a, int b) => new EternalStat(a.maxHp * b, a.minDamage * b, a.maxDamage * b, a.defense * b, a.intellect * b, a.speed * b, a.attackSpeed * b, a.criticalRate * b, a.criticalDamage * b);
    public static EternalStat operator *(EternalStat a, float b) => new EternalStat((a.maxHp * b), (int)(a.minDamage * b), (int)(a.maxDamage * b), (int)(a.defense * b), (int)(a.intellect * b), (int)(a.speed * b), (int)(a.attackSpeed * b), (int)(a.criticalRate * b), (int)(a.criticalDamage * b));
    public static EternalStat operator /(EternalStat a, EternalStat b) => new EternalStat(a.maxHp / b.maxHp, a.minDamage / b.minDamage, a.maxDamage / b.maxDamage, a.defense / b.defense, a.intellect / b.intellect, a.speed / b.speed, a.attackSpeed / b.attackSpeed, a.criticalRate / b.criticalRate, a.criticalDamage / b.criticalDamage);
    public static EternalStat operator /(EternalStat a, int b) => new EternalStat(a.maxHp / b, a.minDamage / b, a.maxDamage / b, a.defense / b, a.intellect / b, a.speed / b, a.attackSpeed / b, a.criticalRate / b, a.criticalDamage / b);
    public static EternalStat operator /(EternalStat a, float b) => new EternalStat((a.maxHp / b), (int)(a.minDamage / b), (int)(a.maxDamage / b), (int)(a.defense / b), (int)(a.intellect / b), (int)(a.speed / b), (int)(a.attackSpeed / b), (int)(a.criticalRate / b), (int)(a.criticalDamage / b));
    
    //public int currentHp;
    public float maxHp;
    //public int mp;

    public int minDamage;
    public int maxDamage;

    public int defense;

    public int intellect;

    public float speed;
    public float attackSpeed;

    public float criticalRate;
    public int criticalDamage;
    public EternalStat()
    {

    }
    public EternalStat(float mh, int mind, int maxd, int df, int intell, float s, float aS, float cr, int cd)
    {
        maxHp = mh;
        minDamage = mind;
        maxDamage = maxd;

        defense = df;
        intellect = intell;
        speed = s;
        attackSpeed = aS;
        criticalRate = cr;
        criticalDamage = cd;
    }
}

[Serializable]
public class ChoiceStat
{
    //public int overweight; // 과체중
    public StatElement<int> patience; // 인내력

    public StatElement<int> momentom; // 추진력

    public StatElement<int> endurance; // 맷집

    //public int luck; // 운

    //public float viscosity; // 점성
    //public float humidity; // 습도

    //public float affinity; // 친밀도
    //public float dominion; // 지배력

    //public float marineLifeFitness; // 해양생물 적합력
    //public float landCreatureFitness; // 지상생물 적합력

    //public float cookingAbility;
}


