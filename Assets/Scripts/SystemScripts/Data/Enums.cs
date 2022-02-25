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
    //여기까지가 고정키

    INVENTORY,
    STAT,
    MONSTER_COLLECTION,
    CHANGEABLEBODYS,
    INTERACTION,
    SPECIALATTACK,
    DRAIN,
    MANASTONE,
    SETTING,
    CHANGE_SLIME,
    CHANGE_MONSTER1,
    CHANGE_MONSTER2,

    NULL  //아무것도 없는 값
}

public enum SkillType
{
    ATTACK,
    SPECIALATTACK,
    DRAIN,
    MANASTONE,
    NULL
}


public enum EventKeyCheck
{
    VOID,
    OBJECT,
    MONO
}
