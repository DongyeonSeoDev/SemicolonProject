using System;
using System.Collections.Generic;

[Serializable]
public class Stat
{
    public int currentStatPoint;   //���� ������ �ִ� ���� ����Ʈ
    public int accumulateStatPoint;  //���ݱ��� ���� ���� ��������Ʈ(��Ȯ���� ���� �ø��µ� �� ���) -> ���� ���濡 �� ����Ʈ�� ������ ����
    public float currentHp;  //���� ü��
    public float currentExp;  //���� ���� ����Ʈ ����ġ
    public float maxExp; //�ִ� ��������Ʈ ����ġ
    public EternalStat eternalStat = new EternalStat();  //�⺻ ���� ����
    public EternalStat additionalEternalStat = new EternalStat();  //���� ����(�߰� �ɷ�ġ)
    public ChoiceStat choiceStat = new ChoiceStat();  //�⺻ ���� ����
    
    public void ResetAfterRegame()
    {
        currentStatPoint += accumulateStatPoint;
        accumulateStatPoint = 0;
        currentExp = 0;
        additionalEternalStat = new EternalStat();
        //choiceStat = new ChoiceStat();
        eternalStat.Reset();
        choiceStat.Reset();
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
public class StatElement  //������ 0������ ����. 0���� ���� ������ �������� ���ѰŰ� 1������ ���� �ø� ��� ���� ���� 0���� 1�� ���ȸ� �����ϰ� ���� ����������
{
    public ushort id;
    public float statValue;  //�ϴ� ������ ���̴��� Ÿ���� �� float���� ���ش�
    public float upStatValue; // Ư�� ���� ����Ʈ�� �������� �� ���� ������ ��
    public bool isUnlock;  //ȹ���� �����ΰ�
    public int statLv;  //���� ���� (�� ������ �� �� �÷ȴ���) (StatŬ������ eternalStat������ ���ϵ���)
    public int maxStatLv; // ���� �ִ� ����

    public int upStatCount => statLv - 1;  //���� �ø��� ���� �� �� �ߴ���
    public bool isOpenStat => statLv > 0;

    public StatElement() { }

    public void Reset()
    {
        if (statLv > 1)
        {
            statValue -= upStatCount * upStatValue;
            statLv = 1;
        }
    }

}

[Serializable]
public class EternalStat
{
    public static EternalStat operator +(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue + b.maxHp.statValue, a.minDamage.statValue + b.minDamage.statValue, a.maxDamage.statValue + b.maxDamage.statValue, a.defense.statValue + b.defense.statValue, a.intellect.statValue + b.intellect.statValue, a.speed.statValue + b.speed.statValue, a.attackSpeed.statValue + b.attackSpeed.statValue, a.criticalRate.statValue + b.criticalRate.statValue, a.criticalDamage.statValue + b.criticalDamage.statValue);
    public static EternalStat operator -(EternalStat a, EternalStat b) => new EternalStat(a.maxHp.statValue - b.maxHp.statValue, a.minDamage.statValue - b.minDamage.statValue, a.maxDamage.statValue - b.maxDamage.statValue, a.defense.statValue - b.defense.statValue, a.intellect.statValue - b.intellect.statValue, a.speed.statValue - b.speed.statValue, a.attackSpeed.statValue - b.attackSpeed.statValue, a.criticalRate.statValue - b.criticalRate.statValue, a.criticalDamage.statValue - b.criticalDamage.statValue);
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

}

[Serializable]
public class ChoiceStat
{
    //public int overweight; // ��ü��
    public StatElement patience = new StatElement(); // �γ���

    public StatElement momentom = new StatElement(); // ������

    public StatElement endurance = new StatElement(); // ����

    //public int luck; // ��

    //public float viscosity; // ����
    //public float humidity; // ����

    //public float affinity; // ģ�е�
    //public float dominion; // �����

    //public float marineLifeFitness; // �ؾ���� ���շ�
    //public float landCreatureFitness; // ������� ���շ�

    //public float cookingAbility;

    public void Reset()
    {
        patience.Reset();
        momentom.Reset();
        endurance.Reset();
    }

}
