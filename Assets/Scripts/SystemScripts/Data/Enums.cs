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
    CHANGEABLEMOBLIST
}

public enum KeyAction
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ATTACK,
    ESCAPE,
    //��������� ����Ű

    INVENTORY,
    STAT,
    MONSTER_COLLECTION,
    INTERACTION,
    SPECIALATTACK,
    DRAIN,
    MANASTONE,
    SETTING,
    CHANGE_SLIME,
    CHANGE_MONSTER1,
    CHANGE_MONSTER2,

    NULL  //�ƹ��͵� ���� ��



    //CHANGEABLEBODYS
}

public enum SkillType
{
    ATTACK,
    SPECIALATTACK,
    DRAIN,         //�ι�° Ư�� ��ų
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
    NONE
}

public enum StateAbnormality
{
    Pain,
    Scar,
    Poverty,
    Blind,
    None  // ��� ������ �������� ��ġ
}

public enum DoorDirType
{
    FRONT,
    RIGHT,
    LEFT,
    BACK
}


public enum EventKeyCheck
{
    OBJECT,
    MONO
}
