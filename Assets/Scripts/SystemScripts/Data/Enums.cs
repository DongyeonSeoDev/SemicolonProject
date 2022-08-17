
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
    FAIL,  //흡수하고 장착 실패
    SUCCESS,  //흡수하고 장착 성공
    ALREADY,  //흡수했는데 이미 장착된 놈
    UNDERSTANDING  //동화율 n%마다 알리는 놈
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
    CHAR_UI,  //HP, Energe, MobSlot(3칸 다) 3개 
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
    //여기까지가 고정키

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

    NONE  //아무것도 없는 값



    //CHANGEABLEBODYS
}

public enum SkillType
{
    ATTACK,
    SPECIALATTACK1,  //첫번째 특수 스킬
    SPECIALATTACK2,         //두번째 특수 스킬
    MANASTONE,     //마석 스킬 (여기에 넣어야할진 아직 미정)
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
    None  // 얘는 무조건 마지막에 배치
}

public enum BuffType
{
    None  // 얘는 무조건 마지막에 배치
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
