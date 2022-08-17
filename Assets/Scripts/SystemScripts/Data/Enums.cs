
public enum MissionType
{
    NONE,
    ALLKILL,
    SURVIVAL1,
    NOTRANSFORMATION,
    NOQUIKSLOT,
    ALLABSORPTION,
    ABSORPTIONTUTORIAL,
    ABSORPTIONTUTORIAL2,
    NODAMAGED,
    SIMULABSORPTION,
    CONSEKILL,
    TIMELIMITKILL
}

public enum DifficultyLevel
{
    EASY,
    NORMAL,
    HARD
}

public enum KillNoticeType
{
    FAIL,  //����ϰ� ���� ����
    SUCCESS,  //����ϰ� ���� ����
    ALREADY,  //����ߴµ� �̹� ������ ��
    UNDERSTANDING  //��ȭ�� n%���� �˸��� ��
}

public enum ItemType
{
    EQUIP,
    CONSUME,
    ETC,
    NONE
}

public enum UIType
{
    CHEF_FOODS_PANEL,
    PRODUCTION_PANEL,
    INVENTORY,
    FOOD_DETAIL,
    ITEM_DETAIL,
    COMBINATION,
    REMOVE_ITEM,
    STAT,
    QUIT,
    DEATH,
    CLEAR,
    SETTING,
    KEYSETTING,
    RESOLUTION,
    SOUND,
    HELP,
    CREDIT,
    MONSTER_COLLECTION,
    MONSTERINFO_DETAIL,
    MONSTERINFO_DETAIL_STAT,
    MONSTERINFO_DETAIL_ITEM,
    SKILLDETAIL,
    CHANGEABLEMOBLIST,
    STATEINFO,
    MINIGAME_PICKUP,
    UIOFFCONFIRM,
    MENU,
    MONSTERINFO_DETAIL_FEATURE
}

public enum InitGainType
{
    NONE,
    HP,
    ENERGE,
    QUIKSLOT,
    MOBSLOT,
    SKILL1,
    SKILL2,
    SKILL3,
    CHAR_UI,  //HP, Energe, MobSlot(3ĭ ��) 3�� 
    INVENTORY_UI,
    STAT_UI,
    MONCOL_UI,
    SETTING_UI,
    HP_STAT
}

public enum KeyAction
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ATTACK,
    FIXED_SPECIALATTACK1,
    SETTING,
    //��������� ����Ű

    QUIT,
    MENU,
    INVENTORY,
    STAT,
    MONSTER_COLLECTION,
    INTERACTION,
    EVENT,
    SPECIALATTACK1,
    SPECIALATTACK2,
    MANASTONE,
    CHANGE_SLIME,
    CHANGE_MONSTER1,
    CHANGE_MONSTER2,
    ITEM_QUIKSLOT,

    NONE  //�ƹ��͵� ���� ��



    //CHANGEABLEBODYS
}

public enum SkillType
{
    ATTACK,
    SPECIALATTACK1,  //ù��° Ư�� ��ų
    SPECIALATTACK2,         //�ι�° Ư�� ��ų
    MANASTONE,     //���� ��ų (���⿡ �־������ ���� ����)
    NULL
}

public enum ApplyMatCompoType
{
    SPRITERENDERER,
    IMAGE,
    RAWIMAGE
}

public enum RandomRoomType
{
    MONSTER,
    RECOVERY,
    IMPRECATION
}

public enum AreaType
{
    START,
    MONSTER,
    CHEF,
    PLANTS,
    RANDOM,
    BOSS,
    RECOVERY,
    IMPRECATION,
    NONE,
    LOBBY,
    STAT
}

public enum StateAbnormality
{
    Pain,
    Scar,
    Poverty,
    Blind,
    None  // ��� ������ �������� ��ġ
}

public enum BuffType
{
    None  // ��� ������ �������� ��ġ
}

public enum DoorDirType
{
    FRONT,
    RIGHT,
    LEFT,
    BACK
}

public enum EnemySpecies
{
    NONE,
    RAT,
    SLIME,
    SKELETON
}


public enum EventKeyCheck
{
    OBJECT,
    MONO
}
