using System;

[Serializable]
public class Stat
{
    public EternalStat eternalStat = new EternalStat();
    public EternalStat additionalEternalStat = new EternalStat();
    public ChoiceStat choiceStat = new ChoiceStat();

    #region default stat + additional stat  property
    public int Hp  //나중에 이름을 MaxHp로 바꿀 것
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
    public int maxHp;
    //public int mp;

    public int damage;

    public int defense;

    public int intellect;

    public float speed;

    public float criticalRate;
    public float criticalDamage;

    public void SetDefaultStat()
    {
        maxHp = 100;
        damage = 10;
        defense = 2;

        speed = 8f;
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
