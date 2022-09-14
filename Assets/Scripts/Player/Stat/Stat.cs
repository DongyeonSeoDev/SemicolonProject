using System;
using System.Collections.Generic;
using System.Linq;

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
    
    public void UseStatPoint(int point)  //����Ʈ ����ϰ� ������ ����
    {
        currentStatPoint -= point;
        if(currentStatPoint<0) currentStatPoint = 0;
        accumulateStatPoint += point;
    }

    public void GetStatPoint(int point)  //����Ʈ �ް� �������� ����
    {
        currentStatPoint += point;
        accumulateStatPoint -= point;
        if(accumulateStatPoint < 0) accumulateStatPoint = 0;
    }

    public void ResetAfterRegame()  //Ʃ�丮�� ������ �̷��� ������ �ʱ�ȭ
    {
        currentStatPoint += accumulateStatPoint;
        accumulateStatPoint = 0;
        currentExp = 0;
        additionalEternalStat.ResetAdditional();
        eternalStat.Reset();
        choiceStat.Reset();
    }

    public void SaveStat()  //�Ⱦ�
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
public class StatElement  //������ 0������ ����. 0���� ���� ������ �������� ���ѰŰ� 1������ ���� �ø� ��� ���� ���� 0���� 1�� ���ȸ� �����ϰ� ���� ����������
{
    public ushort id;
    public float statValue;  //�ϴ� ������ ���̴��� Ÿ���� �� float���� ���ش�
    public bool isUnlock;  //ȹ���� �����ΰ�
    public int statLv;  //���� ���� (�� ������ �� �� �÷ȴ���) (StatŬ������ eternalStat������ ���ϵ���)
    public int savedStatLv;  //����� ���� ����. ���࿡ �������� Ŭ�����ϰ� �� ���� ���ȷ��� �����Ѵٸ� ���⿡ �����ϰ� �����ϸ� �� ������ �ǵ���

    public int upStatCount => statLv - 1;  //���� �ø��� ���� �� �� �ߴ���
    public bool isOpenStat => statLv > 0;  //���� ������ �Ǿ�����
    public bool isUnlockClose => statLv == 0 && isUnlock;  //������ ȹ�������� ������ ���ߴ���

    public int maxStatLv => NGlobal.playerStatUI.GetStatSOData(id).maxStatLv; // ���� �ִ� ����
    public float UpStatValue => NGlobal.playerStatUI.GetStatSOData(id).upStatValue; // Ư�� ���� ����Ʈ�� �������� �� ���� ������ ��

    public string StatName => NGlobal.playerStatUI.GetStatSOData(id).statName;
    public UnityEngine.Sprite StatSprite => NGlobal.playerStatUI.GetStatSOData(id).statSpr;

    public StatElement() { }

    public void Reset()  //���� (��: ��������) �ʱ�ȭ
    {
        if (statLv > 1)
        {
            statLv = savedStatLv;
        }
    }

    public void ResetComplete()  //Ư�� (��: ���ý���) �ʱ�ȭ
    {
        statValue = 1;
        statLv = 0;
        isUnlock = false;
    }

    public float SaveEternal()  //�Ⱦ�
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

    public List<StatElement> AllStats  //��� ���ȵ��� ������ ����Ʈ
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

    //������ ��ġ�� 0���� ū ���ȵ��� ����
    public int NoZeroStatCount => AllStats.Count(stat => stat.statValue > 0);  //������ ���� 0���� ū �͵��� ����

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
    public StatElement proficiency = new StatElement(); // ���õ�

    public StatElement momentom = new StatElement(); // ������

    public StatElement endurance = new StatElement(); // ����

    public StatElement frenzy = new StatElement(); // ������ �ʴ� ����

    public StatElement reflection = new StatElement(); // �ݻ�

    public StatElement mucusRecharge = new StatElement() ; // ���� ������

    public StatElement fake = new StatElement(); // ����ô

    public StatElement multipleShots = new StatElement() ; // ���� �߻�

    public StatElement health = new StatElement();  //�ǰ���
    public StatElement strong = new StatElement();  //����
    public StatElement powerful = new StatElement();  //������
    public StatElement nimble = new StatElement();  //������
    public StatElement active = new StatElement();  //Ȱ����
    public StatElement incisive = new StatElement();  //������
    public StatElement persistent = new StatElement();  //������
    public StatElement hard = new StatElement();  //�ܴ���

    public StatElement weak = new StatElement(); //�����
    public StatElement soft = new StatElement(); //������
    public StatElement feeble = new StatElement(); //�����
    public StatElement tiredness = new StatElement(); //�Ƿ���
    public StatElement fear = new StatElement(); //�η���
    public StatElement sluggish = new StatElement(); //����
    public StatElement dull = new StatElement(); //��Ź��
    public StatElement terror = new StatElement(); //������

    private List<StatElement> elements;

    public List<StatElement> AllStats  //��� Ư��(���ý���)���� ������ ����Ʈ
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
