

public static class NGlobal
{
    public const ushort MaxHpID = 5;
    public const ushort MinDamageID = 10;
    public const ushort MaxDamageID = 15;
    public const ushort DefenseID = 20;
    public const ushort IntellectID = 25;
    public const ushort SpeedID = 30;
    public const ushort AttackSpeedID = 35;
    public const ushort CriticalRate = 40;
    public const ushort CriticalDamage = 45;

    public const ushort EStatStartID = 5;
    public const ushort EStatEndID = 45;
    public const ushort StatIDOffset = 5;

    public const ushort ProficiencyID = 100;
    public const ushort MomentomID = 105;
    public const ushort EnduranceID = 110;
    public const ushort FrenzyID = 115;
    public const ushort ReflectionID = 120;
    public const ushort MucusRechargeID = 125;
    public const ushort FakeID = 130;
    public const ushort MultiShotID = 135;

    public const ushort CStatStartID = 100;
    public const ushort CStatEndID = 135; 

    public static PlayerStatUI playerStatUI => UIManager.Instance.playerStatUI;
}


