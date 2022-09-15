using System;
using System.Collections.Generic;
using System.Linq;

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
    
    public void UseStatPoint(int point)  //포인트 사용하고 누적에 저장
    {
        currentStatPoint -= point;
        if(currentStatPoint<0) currentStatPoint = 0;
        accumulateStatPoint += point;
    }

    public void GetStatPoint(int point)  //포인트 받고 누적에서 빼줌
    {
        currentStatPoint += point;
        accumulateStatPoint -= point;
        if(accumulateStatPoint < 0) accumulateStatPoint = 0;
    }

    public void ResetAfterRegame()  //튜토리얼 종료라면 이렇게 데이터 초기화
    {
        currentStatPoint += accumulateStatPoint;
        accumulateStatPoint = 0;
        currentExp = 0;
        additionalEternalStat.ResetAdditional();
        eternalStat.Reset();
        choiceStat.Reset();
    }

    public void SaveStat()  //안씀
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
    public bool isUnlock;  //획득한 스탯인가
    public int statLv;  //스탯 레벨 (이 스탯을 몇 번 올렸는지) (Stat클래스의 eternalStat에서만 쓰일듯함)
    public int savedStatLv;  //저장된 스탯 레벨. 만약에 스테이지 클리어하고 그 때의 스탯레벨 저장한다면 여기에 저장하고 리겜하면 이 값으로 되돌림

    public int upStatCount => statLv - 1;  //스탯 올리는 짓을 몇 번 했는지
    public bool isOpenStat => statLv > 0;  //스탯 개방이 되었는지
    public bool isUnlockClose => statLv == 0 && isUnlock;  //스탯을 획득했지만 개방은 안했는지

    public int maxStatLv => NGlobal.playerStatUI.GetStatSOData(id).maxStatLv; // 스탯 최대 레벨
    public float UpStatValue => NGlobal.playerStatUI.GetStatSOData(id).upStatValue; // 특정 스탯 포인트를 투자했을 때 오를 스탯의 값

    public string StatName => NGlobal.playerStatUI.GetStatSOData(id).statName;
    public UnityEngine.Sprite StatSprite => NGlobal.playerStatUI.GetStatSOData(id).statSpr;

    public StatElement() { }

    public void Reset()  //스탯 (구: 고정스탯) 초기화
    {
        if (statLv > 1)
        {
            statLv = savedStatLv;
        }
    }

    public void ResetComplete()  //특성 (구: 선택스탯) 초기화
    {
        statValue = 1;
        statLv = 0;
        isUnlock = false;
    }

    public float SaveEternal()  //안씀
    {
        int tmp = savedStatLv;
        savedStatLv = statLv;
        int sub = savedStatLv - tmp;

        float res = 0f;
        if (sub > 0)
        {
            res = sub * UpStatValue;
            statValue += res;
        }
        return res;
    }
}

[Serializable]
public class EternalStat
{
    public static EternalStat operator *(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue * b.maxHp.statValue, a.minDamage.statValue * b.minDamage.statValue, a.maxDamage.statValue * b.maxDamage.statValue, a.defense.statValue * b.defense.statValue, a.intellect.statValue * b.intellect.statValue, a.speed.statValue * b.speed.statValue, a.attackSpeed.statValue * b.attackSpeed.statValue, a.criticalRate.statValue * b.criticalRate.statValue, a.criticalDamage.statValue * b.criticalDamage.statValue);
    public static EternalStat operator *(EternalStat a, int b) =>         new EternalStat(a.maxHp .statValue* b, a.minDamage.statValue * b, a.maxDamage.statValue * b, a.defense.statValue * b, a.intellect.statValue * b, a.speed.statValue * b, a.attackSpeed.statValue * b, a.criticalRate.statValue * b, a.criticalDamage.statValue * b);
    public static EternalStat operator *(EternalStat a, float b) =>       new EternalStat((a.maxHp.statValue * b), (int)(a.minDamage.statValue * b), (int)(a.maxDamage.statValue * b), (int)(a.defense.statValue * b), (int)(a.intellect.statValue * b), (int)(a.speed.statValue * b), (int)(a.attackSpeed.statValue * b), (int)(a.criticalRate.statValue * b), (int)(a.criticalDamage.statValue * b));
    public static EternalStat operator /(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue/ b.maxHp.statValue, a.minDamage.statValue / b.minDamage.statValue, a.maxDamage.statValue / b.maxDamage.statValue, a.defense.statValue / b.defense.statValue, a.intellect.statValue / b.intellect.statValue, a.speed.statValue / b.speed.statValue, a.attackSpeed.statValue / b.attackSpeed.statValue, a.criticalRate.statValue / b.criticalRate.statValue, a.criticalDamage.statValue / b.criticalDamage.statValue);
    public static EternalStat operator /(EternalStat a, int b) =>         new EternalStat(a.maxHp .statValue/ b, a.minDamage.statValue / b, a.maxDamage.statValue / b, a.defense.statValue / b, a.intellect.statValue / b, a.speed.statValue / b, a.attackSpeed.statValue / b, a.criticalRate.statValue / b, a.criticalDamage.statValue / b);
    public static EternalStat operator /(EternalStat a, float b) =>       new EternalStat((a.maxHp.statValue / b), (int)(a.minDamage.statValue / b), (int)(a.maxDamage.statValue / b), (int)(a.defense.statValue / b), (int)(a.intellect.statValue / b), (int)(a.speed.statValue / b), (int)(a.attackSpeed.statValue / b), (int)(a.criticalRate.statValue / b), (int)(a.criticalDamage.statValue / b));
    
    public StatElement maxHp = new StatElement();

    public StatElement minDamage = new StatElement();
    public StatElement maxDamage = new StatElement();

    public StatElement defense = new StatElement();

    public StatElement intellect = new StatElement();

    public StatElement speed = new StatElement();
    public StatElement attackSpeed = new StatElement();

    public StatElement criticalRate = new StatElement();
    public StatElement criticalDamage = new StatElement();

    private List<StatElement> elements;

    public List<StatElement> AllStats  //모든 스탯들의 데이터 리스트
    {
        get
        {
            if(elements == null)
            {
                elements = new List<StatElement>();
                elements.Add(maxHp);
                elements.Add(minDamage);
                elements.Add(maxDamage);
                elements.Add(defense);
                elements.Add(intellect);
                elements.Add(speed);
                elements.Add(attackSpeed);
                elements.Add(criticalRate);
                elements.Add(criticalDamage);
            }
            return elements;
        }
    }

    //스탯의 수치가 0보다 큰 스탯들의 개수
    public int NoZeroStatCount => AllStats.Count(stat => stat.statValue > 0);  //스탯의 값이 0보다 큰 것들의 개수

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
    public StatElement proficiency = new StatElement(); // 숙련도

    public StatElement momentom = new StatElement(); // 추진력

    public StatElement endurance = new StatElement(); // 맷집

    public StatElement frenzy = new StatElement(); // 멈추지 않는 돌진

    public StatElement reflection = new StatElement(); // 반사

    public StatElement mucusRecharge = new StatElement() ; // 점액 재충전

    public StatElement fake = new StatElement(); // 맞은척

    public StatElement multipleShots = new StatElement() ; // 다중 발사

    public StatElement health = new StatElement();  //건강함
    public StatElement strong = new StatElement();  //강함
    public StatElement powerful = new StatElement();  //강력함
    public StatElement nimble = new StatElement();  //날렵함
    public StatElement active = new StatElement();  //활발함
    public StatElement incisive = new StatElement();  //예리함
    public StatElement persistent = new StatElement();  //집요함
    public StatElement hard = new StatElement();  //단단함

    public StatElement weak = new StatElement(); //허약함
    public StatElement soft = new StatElement(); //나약함
    public StatElement feeble = new StatElement(); //쇠약함
    public StatElement tiredness = new StatElement(); //피로함
    public StatElement fear = new StatElement(); //두려움
    public StatElement sluggish = new StatElement(); //둔함
    public StatElement dull = new StatElement(); //둔탁함
    public StatElement terror = new StatElement(); //공포감

    private List<StatElement> elements;

    public List<StatElement> AllStats  //모든 특성(선택스탯)들의 데이터 리스트
    {
        get
        {
            if (elements == null)
            {
                elements = new List<StatElement>();
                elements.Add(proficiency);
                elements.Add(momentom);
                elements.Add(endurance);
                elements.Add(frenzy);
                elements.Add(reflection);
                elements.Add(mucusRecharge);
                elements.Add(fake);
                elements.Add(multipleShots);
                elements.Add(health);
                elements.Add(strong);
                elements.Add(powerful);
                elements.Add(nimble);
                elements.Add(active);
                elements.Add(incisive);
                elements.Add(persistent);
                elements.Add(hard);
                elements.Add(weak);
                elements.Add(soft);
                elements.Add(feeble);
                elements.Add(tiredness);
                elements.Add(fear);
                elements.Add(sluggish);
                elements.Add(dull);
                elements.Add(terror);
            }
            return elements;
        }
    }

    public void Reset()
    {
        for(int i=0; i<AllStats.Count; i++)
        {
            AllStats[i].ResetComplete();
        }
    }
}
