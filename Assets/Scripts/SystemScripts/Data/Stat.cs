using System;

[Serializable]
public class Stat
{
    public EternalStat eternalStat = new EternalStat();
    public EternalStat additionalEternalStat = new EternalStat();
    public ChoiceStat choiceStat = new ChoiceStat();

    public int Hp
    {
        get { return eternalStat.hp + additionalEternalStat.hp; }
    }
    public int Damage
    {
        get { return eternalStat.damage + additionalEternalStat.damage; }
    }
    public int Defense
    {
        get { return eternalStat.defense + additionalEternalStat.defense; }
    }



    public float Speed
    {
        get { return eternalStat.speed + additionalEternalStat.speed; }
    }
}

[Serializable]
public class EternalStat
{
    public int hp;
    //public uint mp;

    public int damage;

    public int defense;

    public int intellect;

    public float speed;

    public float criticalRate;
    public float criticalDamage;

    public void SetDefaultStat()
    {
        hp = 100;
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
