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
    CHANGEABLEMOBLIST
}

public enum KeyAction
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ATTACK,
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
}

public enum EventKeyCheck
{
    VOID,
    OBJECT,
    MONO
}
