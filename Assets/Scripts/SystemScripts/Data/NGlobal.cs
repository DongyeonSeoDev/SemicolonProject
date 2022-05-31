

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
    public const ushort StatIDOffset = 5;

    public const ushort PatienceID = 100;
    public const ushort MomentomID = 105;
    public const ushort EnduranceID = 110;
    public const ushort FrenzyID = 115;
    public const ushort ReflectionID = 120;

    public const ushort CStatStartID = 100;
    public const ushort CStatEndID = 125; //나중에 값 바꿔야 함

    public static PlayerStatUI playerStatUI => UIManager.Instance.playerStatUI;


    public static string GetChoiceStatAbilExplanation(ushort id)
    {
        switch(id)
        {
            case PatienceID:
                return "인내";
            case MomentomID:
                return "추진력";
            case EnduranceID:
                return "맷집";
        }

        return string.Empty;
    }
}


