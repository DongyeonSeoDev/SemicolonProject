using System;

[Serializable]
public class Stat
{
    public EternalStat eternalStat = new EternalStat();
    public EternalStat additionalEternalStat = new EternalStat();
    public ChoiceStat choiceStat = new ChoiceStat();

    #region default stat + additional stat  property
    public int MaxHp  
    {
        get { return eternalStat.maxHp + additionalEternalStat.maxHp; }
    }
    public int Damage
    {
        get { return eternalStat.damage + additionalEternalStat.damage; }
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

    public float CriticalRate
    {
        get { return eternalStat.criticalRate + additionalEternalStat.criticalRate; }
    }

    public float CriticalDamage
    {
        get { return eternalStat.criticalDamage + additionalEternalStat.criticalDamage; }
    }
    #endregion
}

[Serializable]
public class EternalStat
{
    public static EternalStat operator +(EternalStat a, EternalStat b) => new EternalStat(a.maxHp + b.maxHp, a.damage + b.damage, a.defense + b.defense, a.intellect + b.intellect, a.speed + b.speed, a.criticalRate + b.criticalRate, a.criticalDamage + b.criticalDamage);
    public static EternalStat operator -(EternalStat a, EternalStat b) => new EternalStat(a.maxHp - b.maxHp, a.damage - b.damage, a.defense - b.defense, a.intellect - b.intellect, a.speed - b.speed, a.criticalRate - b.criticalRate, a.criticalDamage - b.criticalDamage);
    public static EternalStat operator *(EternalStat a, EternalStat b) => new EternalStat(a.maxHp * b.maxHp, a.damage * b.damage, a.defense * b.defense, a.intellect * b.intellect, a.speed * b.speed, a.criticalRate * b.criticalRate, a.criticalDamage * b.criticalDamage);
    public static EternalStat operator *(EternalStat a, int b) => new EternalStat(a.maxHp * b, a.damage * b, a.defense * b, a.intellect * b, a.speed * b, a.criticalRate * b, a.criticalDamage * b);
    public static EternalStat operator *(EternalStat a, float b) => new EternalStat((int)(a.maxHp * b), (int)(a.damage * b), (int)(a.defense * b), (int)(a.intellect * b), (int)(a.speed * b), (int)(a.criticalRate * b), (int)(a.criticalDamage * b));
    public static EternalStat operator /(EternalStat a, EternalStat b) => new EternalStat(a.maxHp / b.maxHp, a.damage / b.damage, a.defense / b.defense, a.intellect / b.intellect, a.speed / b.speed, a.criticalRate / b.criticalRate, a.criticalDamage / b.criticalDamage);
    public static EternalStat operator /(EternalStat a, int b) => new EternalStat(a.maxHp / b, a.damage / b, a.defense / b, a.intellect / b, a.speed / b, a.criticalRate / b, a.criticalDamage / b);
    public static EternalStat operator /(EternalStat a, float b) => new EternalStat((int)(a.maxHp / b), (int)(a.damage / b), (int)(a.defense / b), (int)(a.intellect / b), (int)(a.speed / b), (int)(a.criticalRate / b), (int)(a.criticalDamage / b));
    
    //public int currentHp;
    public int maxHp;
    //public int mp;

    public int damage;

    public int defense;

    public int intellect;

    public float speed;

    public float criticalRate;
    public float criticalDamage;
    public EternalStat()
    {

    }
    public EternalStat(int mh, int d, int df, int intell, float s, float cr, float cd)
    {
        maxHp = mh;
        damage = d;
        defense = df;
        intellect = intell;
        speed = s;
        criticalRate = cr;
        criticalDamage = cd;
    }
    public void SetDefaultStat() //���� �⺻��
    {
        maxHp = 100;
        damage = 10;
        intellect = 1;
        defense = 2;

        speed = 8f;
        criticalRate = 5;
        criticalDamage = 1;
    }
}

[Serializable]
public class ChoiceStat
{
    public int overweight;
    public int patience;
    public int luck;

    public float viscosity;
    public float humidity;

    public float affinity;
    public float dominion;

    public float marineLifeFitness;
    public float landCreatureFitness;

    public float cookingAbility;
}
