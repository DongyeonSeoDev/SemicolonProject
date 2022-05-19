

public static class NGlobal
{
    public static ushort MaxHpID => 5;
    public static ushort MinDamageID => 10;
    public static ushort MaxDamageID => 15;
    public static ushort DefenseID => 20;
    public static ushort IntellectID => 25;
    public static ushort SpeedID => 30;
    public static ushort AttackSpeedID => 35;
    public static ushort CriticalRate => 40;
    public static ushort CriticalDamage => 45;

    public const ushort EStatStartID = 5;
    public const ushort EStatEndID = 45;
    public const ushort EStatIDOffset = 5;



    public static PlayerStatUI playerStatUI => UIManager.Instance.playerStatUI;
}


