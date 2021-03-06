using System;
using System.Collections.Generic;

[Serializable]
public class Stat
{
    public int currentStatPoint;   //현재 가지고 있는 스탯 포인트
    public int accumulateStatPoint;  //지금까지 모은 누적 스탯포인트(정확히는 스탯 올리는데 쓴 비용) -> 스탯 개방에 쓴 포인트는 더하지 않음
    public float currentHp;  //현재 체력
    public float currentExp;  //현재 스탯 포인트 경험치
    public float maxExp; //최대 스탯포인트 경험치
    public EternalStat eternalStat = new EternalStat();  //기본 영구 스탯
    public EternalStat additionalEternalStat = new EternalStat();  //영구 스탯(추가 능력치)
    public ChoiceStat choiceStat = new ChoiceStat();  //기본 선택 스탯
    
    public void ResetAfterRegame()
    {
        currentStatPoint += accumulateStatPoint;
        accumulateStatPoint = 0;
        currentExp = 0;
        additionalEternalStat.ResetAdditional();
        //choiceStat = new ChoiceStat();
        eternalStat.Reset();
        choiceStat.Reset();
    }

    public void SaveStat()
    {
        additionalEternalStat.maxHp.statValue -= eternalStat.maxHp.SaveEternal();
        additionalEternalStat.minDamage.statValue -= eternalStat.minDamage.SaveEternal();
        additionalEternalStat.maxDamage.statValue -= eternalStat.maxDamage.SaveEternal();
        additionalEternalStat.speed.statValue -= eternalStat.speed.SaveEternal();
        additionalEternalStat.attackSpeed.statValue -= eternalStat.attackSpeed.SaveEternal();
        additionalEternalStat.criticalDamage.statValue -= eternalStat.criticalDamage.SaveEternal();
        additionalEternalStat.criticalRate.statValue -= eternalStat.criticalRate.SaveEternal();
        additionalEternalStat.intellect.statValue -= eternalStat.intellect.SaveEternal();
        additionalEternalStat.defense.statValue -= eternalStat.defense.SaveEternal();
    }

    #region default stat + additional stat  property
    public float MaxHp  
    {
        get { return eternalStat.maxHp.statValue + additionalEternalStat.maxHp.statValue; }
    }
    public float MinDamage
    {
        get { return eternalStat.minDamage.statValue + additionalEternalStat.minDamage.statValue; }
    }
    public float MaxDamage
    {
        get { return eternalStat.maxDamage.statValue + additionalEternalStat.maxDamage.statValue; }
    }
    public float Defense
    {
        get { return eternalStat.defense.statValue + additionalEternalStat.defense.statValue; }
    }

    public float Intellect
    {
        get { return eternalStat.intellect.statValue + additionalEternalStat.intellect.statValue; }
    }

    public float Speed
    {
        get { return eternalStat.speed.statValue + additionalEternalStat.speed.statValue; }
    }
    public float AttackSpeed
    {
        get { return eternalStat.attackSpeed.statValue + additionalEternalStat.attackSpeed.statValue; }
    }
    public float CriticalRate
    {
        get { return eternalStat.criticalRate.statValue + additionalEternalStat.criticalRate.statValue; }
    }

    public float CriticalDamage
    {
        get { return eternalStat.criticalDamage.statValue + additionalEternalStat.criticalDamage.statValue; }
    }
    #endregion
}

[Serializable]
public class StatElement  //스탯은 0렙부터 시작. 0렙일 때는 스탯을 개방하지 못한거고 1렙부터 레벨 올릴 경우 스탯 오름 0에서 1은 스탯만 개방하고 스탯 오르진않음
{
    public ushort id;
    public float statValue;  //일단 정수만 쓰이더라도 타입은 다 float으로 해준다
    public float upStatValue; // 특정 스탯 포인트를 투자했을 때 오를 스탯의 값
    public bool isUnlock;  //획득한 스탯인가
    public int statLv;  //스탯 레벨 (이 스탯을 몇 번 올렸는지) (Stat클래스의 eternalStat에서만 쓰일듯함)
    public int savedStatLv;  //저장된 스탯 레벨. 만약에 스테이지 클리어하고 그 때의 스탯레벨 저장한다면 여기에 저장하고 리겜하면 이 값으로 되돌림
    public int maxStatLv; // 스탯 최대 레벨

    public int upStatCount => statLv - 1;  //스탯 올리는 짓을 몇 번 했는지
    public bool isOpenStat => statLv > 0;  //스탯 개방이 되었는지
    public bool isUnlockClose => statLv == 0 && isUnlock;

    public StatElement() { }

    public void Reset()
    {
        if (statLv > 1)
        {
            statLv = savedStatLv;
            //statValue -= upStatCount * upStatValue;
            //statLv = 1;
        }
    }

    public void ResetComplete()
    {
        statValue = 1;
        statLv = 0;
        isUnlock = false;
    }

    public float SaveEternal()
    {
        int tmp = savedStatLv;
        savedStatLv = statLv;
        int sub = savedStatLv - tmp;

        float res = 0f;
        if (sub > 0)
        {
            res = sub * upStatValue;
            statValue += res;
        }
        return res;
    }
}

[Serializable]
public class EternalStat
{
    //public static EternalStat operator +(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue + b.maxHp.statValue, a.minDamage.statValue + b.minDamage.statValue, a.maxDamage.statValue + b.maxDamage.statValue, a.defense.statValue + b.defense.statValue, a.intellect.statValue + b.intellect.statValue, a.speed.statValue + b.speed.statValue, a.attackSpeed.statValue + b.attackSpeed.statValue, a.criticalRate.statValue + b.criticalRate.statValue, a.criticalDamage.statValue + b.criticalDamage.statValue);
    //public static EternalStat operator -(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue - b.maxHp.statValue, a.minDamage.statValue - b.minDamage.statValue, a.maxDamage.statValue - b.maxDamage.statValue, a.defense.statValue - b.defense.statValue, a.intellect.statValue - b.intellect.statValue, a.speed.statValue - b.speed.statValue, a.attackSpeed.statValue - b.attackSpeed.statValue, a.criticalRate.statValue - b.criticalRate.statValue, a.criticalDamage.statValue - b.criticalDamage.statValue);
    public static EternalStat operator *(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue * b.maxHp.statValue, a.minDamage.statValue * b.minDamage.statValue, a.maxDamage.statValue * b.maxDamage.statValue, a.defense.statValue * b.defense.statValue, a.intellect.statValue * b.intellect.statValue, a.speed.statValue * b.speed.statValue, a.attackSpeed.statValue * b.attackSpeed.statValue, a.criticalRate.statValue * b.criticalRate.statValue, a.criticalDamage.statValue * b.criticalDamage.statValue);
    public static EternalStat operator *(EternalStat a, int b) =>         new EternalStat(a.maxHp .statValue* b, a.minDamage.statValue * b, a.maxDamage.statValue * b, a.defense.statValue * b, a.intellect.statValue * b, a.speed.statValue * b, a.attackSpeed.statValue * b, a.criticalRate.statValue * b, a.criticalDamage.statValue * b);
    public static EternalStat operator *(EternalStat a, float b) =>       new EternalStat((a.maxHp.statValue * b), (int)(a.minDamage.statValue * b), (int)(a.maxDamage.statValue * b), (int)(a.defense.statValue * b), (int)(a.intellect.statValue * b), (int)(a.speed.statValue * b), (int)(a.attackSpeed.statValue * b), (int)(a.criticalRate.statValue * b), (int)(a.criticalDamage.statValue * b));
    public static EternalStat operator /(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue/ b.maxHp.statValue, a.minDamage.statValue / b.minDamage.statValue, a.maxDamage.statValue / b.maxDamage.statValue, a.defense.statValue / b.defense.statValue, a.intellect.statValue / b.intellect.statValue, a.speed.statValue / b.speed.statValue, a.attackSpeed.statValue / b.attackSpeed.statValue, a.criticalRate.statValue / b.criticalRate.statValue, a.criticalDamage.statValue / b.criticalDamage.statValue);
    public static EternalStat operator /(EternalStat a, int b) =>         new EternalStat(a.maxHp .statValue/ b, a.minDamage.statValue / b, a.maxDamage.statValue / b, a.defense.statValue / b, a.intellect.statValue / b, a.speed.statValue / b, a.attackSpeed.statValue / b, a.criticalRate.statValue / b, a.criticalDamage.statValue / b);
    public static EternalStat operator /(EternalStat a, float b) =>       new EternalStat((a.maxHp.statValue / b), (int)(a.minDamage.statValue / b), (int)(a.maxDamage.statValue / b), (int)(a.defense.statValue / b), (int)(a.intellect.statValue / b), (int)(a.speed.statValue / b), (int)(a.attackSpeed.statValue / b), (int)(a.criticalRate.statValue / b), (int)(a.criticalDamage.statValue / b));
    
    //public int currentHp;
    public StatElement maxHp = new StatElement();
    //public int mp;

    public StatElement minDamage = new StatElement();
    public StatElement maxDamage = new StatElement();

    public StatElement defense = new StatElement();

    public StatElement intellect = new StatElement();

    public StatElement speed = new StatElement();
    public StatElement attackSpeed = new StatElement();

    public StatElement criticalRate = new StatElement();
    public StatElement criticalDamage = new StatElement();
    public EternalStat()
    {

    }
    public EternalStat(float mh, float mind, float maxd, float df, float intell, float s, float aS, float cr, float cd)
    {
        maxHp.statValue = mh;
        minDamage.statValue = mind;
        maxDamage.statValue = maxd;

        defense.statValue = df;
        intellect.statValue = intell;
        speed.statValue = s;
        attackSpeed.statValue = aS;
        criticalRate.statValue = cr;
        criticalDamage.statValue = cd;
    }

    public void Reset()
    {
        maxHp.Reset();
        minDamage.Reset();
        maxDamage.Reset();
        defense.Reset();
        intellect.Reset();
        speed.Reset();
        attackSpeed.Reset();
        criticalRate.Reset();
        criticalDamage.Reset();
    }

    public void ResetAdditional()
    {
        maxHp.statValue = 0;
        minDamage.statValue = 0;
        maxDamage.statValue = 0;
        defense.statValue = 0;
        intellect.statValue = 0;
        speed.statValue = 0;
        attackSpeed.statValue = 0;
        criticalRate.statValue = 0;
        criticalDamage.statValue = 0;
    }

    public void Sum(EternalStat a)
    {
        maxHp.statValue += a.maxHp.statValue;
        minDamage.statValue += a.minDamage.statValue;
        maxDamage.statValue += a.maxDamage.statValue;
        defense.statValue += a.defense.statValue;
        intellect.statValue += a.intellect.statValue;
        speed.statValue += a.speed.statValue;
        attackSpeed.statValue += a.attackSpeed.statValue;
        criticalRate.statValue += a.criticalRate.statValue;
        criticalDamage.statValue += a.criticalDamage.statValue;
    }

    public void Sub(EternalStat a)
    {
        maxHp.statValue -= a.maxHp.statValue;
        minDamage.statValue -= a.minDamage.statValue;
        maxDamage.statValue -= a.maxDamage.statValue;
        defense.statValue -= a.defense.statValue;
        intellect.statValue -= a.intellect.statValue;
        speed.statValue -= a.speed.statValue;
        attackSpeed.statValue -= a.attackSpeed.statValue;
        criticalRate.statValue -= a.criticalRate.statValue;
        criticalDamage.statValue -= a.criticalDamage.statValue;
    }
    public void Multiply(EternalStat a)
    {
        maxHp.statValue *= a.maxHp.statValue;
        minDamage.statValue *= a.minDamage.statValue;
        maxDamage.statValue *= a.maxDamage.statValue;
        defense.statValue *= a.defense.statValue;
        intellect.statValue *= a.intellect.statValue;
        speed.statValue *= a.speed.statValue;
        attackSpeed.statValue *= a.attackSpeed.statValue;
        criticalRate.statValue *= a.criticalRate.statValue;
        criticalDamage.statValue *= a.criticalDamage.statValue;
    }
    public void Multiply(int a)
    {
        maxHp.statValue *= a;
        minDamage.statValue *= a;
        maxDamage.statValue *= a;
        defense.statValue *= a;
        intellect.statValue *= a;
        speed.statValue *= a;
        attackSpeed.statValue *= a;
        criticalRate.statValue *= a;
        criticalDamage.statValue *= a;
    }
    public void Multiply(float a)
    {
        maxHp.statValue *= a;
        minDamage.statValue *= a;
        maxDamage.statValue *= a;
        defense.statValue *= a;
        intellect.statValue *= a;
        speed.statValue *= a;
        attackSpeed.statValue *= a;
        criticalRate.statValue *= a;
        criticalDamage.statValue *= a;
    }

    public void Division(EternalStat a)
    {
        maxHp.statValue /= a.maxHp.statValue;
        minDamage.statValue /= a.minDamage.statValue;
        maxDamage.statValue /= a.maxDamage.statValue;
        defense.statValue /= a.defense.statValue;
        intellect.statValue /= a.intellect.statValue;
        speed.statValue /= a.speed.statValue;
        attackSpeed.statValue /= a.attackSpeed.statValue;
        criticalRate.statValue /= a.criticalRate.statValue;
        criticalDamage.statValue /= a.criticalDamage.statValue;
    }

    public void Division(int a)
    {
        maxHp.statValue /= a;
        minDamage.statValue /= a;
        maxDamage.statValue /= a;
        defense.statValue /= a;
        intellect.statValue /= a;
        speed.statValue /= a;
        attackSpeed.statValue /= a;
        criticalRate.statValue /= a;
        criticalDamage.statValue /= a;
    }

    public void Division(float a)
    {
        maxHp.statValue /= a;
        minDamage.statValue /= a;
        maxDamage.statValue /= a;
        defense.statValue /= a;
        intellect.statValue /= a;
        speed.statValue /= a;
        attackSpeed.statValue /= a;
        criticalRate.statValue /= a;
        criticalDamage.statValue /= a;
    }
}

[Serializable]
public class ChoiceStat
{
    //public int overweight; // 과체중
    public StatElement proficiency = new StatElement(); // 숙련도

    public StatElement momentom = new StatElement(); // 추진력

    public StatElement endurance = new StatElement(); // 맷집

    public StatElement frenzy = new StatElement(); // 멈추지 않는 돌진

    public StatElement reflection = new StatElement(); // 반사

    public StatElement mucusRecharge = new StatElement() ; // 점액 재충전

    public StatElement fake = new StatElement(); // 맞은척

    //public int luck; // 운

    //public float viscosity; // 점성
    //public float humidity; // 습도

    //public float affinity; // 친밀도
    //public float dominion; // 지배력

    //public float marineLifeFitness; // 해양생물 적합력
    //public float landCreatureFitness; // 지상생물 적합력

    //public float cookingAbility;

    public void Reset()
    {
        proficiency.ResetComplete();
        momentom.ResetComplete();
        endurance.ResetComplete();
        frenzy.ResetComplete();
        reflection.ResetComplete();
    }


}
