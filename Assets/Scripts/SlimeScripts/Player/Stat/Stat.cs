using System;

[Serializable]
public class Stat
{
    public int currentStatPoint;   //���� ������ �ִ� ���� ����Ʈ
    public int accumulateStatPoint;  //���ݱ��� ���� ���� ��������Ʈ
    public float currentHp;  //���� ü��
    public float currentExp;  //���� ����ġ
    public EternalStat eternalStat = new EternalStat();  //�⺻ ���� ����
    public EternalStat additionalEternalStat = new EternalStat();  //���� ����(�߰� �ɷ�ġ)
    public ChoiceStat choiceStat = new ChoiceStat();  //�⺻ ���� ����

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
public class StatElement<T>  // �޸�(�̰� Ȯ���ϸ� �� �ּ� �����) : string �־ �׳� class�� �ٲ�.
{
    public T statValue;
    public bool isUnlock;  //ȹ���� �����ΰ�
    public int usedStatPoint;  //�� ���ȿ� ���� ��������Ʈ (StatŬ������ eternalStat������ ���ϵ���)
    public string statName;  //�� ������ �̸� (�� : ü��, ���ݷ�, ����)
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
    //public int overweight; // ��ü��
    public StatElement<int> patience; // �γ���

    public StatElement<int> momentom; // ������

    public StatElement<int> endurance; // ����

    //public int luck; // ��

    //public float viscosity; // ����
    //public float humidity; // ����

    //public float affinity; // ģ�е�
    //public float dominion; // �����

    //public float marineLifeFitness; // �ؾ���� ���շ�
    //public float landCreatureFitness; // ������� ���շ�

    //public float cookingAbility;
}


