using UnityEngine;
using System.Collections.Generic;

public class NoTransformationMission : Mission  //»ç¿ë X
{
    private List<KeyAction> cantChangeKeyActions;

    public NoTransformationMission(string title) : base(title)
    {
        missionType = MissionType.NOTRANSFORMATION;
        cantChangeKeyActions = new List<KeyAction>();
    }

    public override void End(bool breakDoor = false)
    {
        InteractionHandler.canTransformEnemy = true;
    }

    public override void Start()
    {
        isEnd = false;
        cantChangeKeyActions.Clear();
        cantChangeKeyActions.Add(KeyAction.CHANGE_SLIME);
        cantChangeKeyActions.Add(KeyAction.CHANGE_MONSTER1);
        cantChangeKeyActions.Add(KeyAction.CHANGE_MONSTER2);
        cantChangeKeyActions.Remove(MonsterCollection.Instance.GetCurBodyKeyAction());
        InteractionHandler.canTransformEnemy = false;
    }

    public override void Update()
    {
        for(int i=0; i < cantChangeKeyActions.Count; i++)
        {
            if(Input.GetKeyDown(KeySetting.keyDict[cantChangeKeyActions[i]]) && !TimeManager.IsTimePaused && MonsterCollection.Instance.HasBodySlot(cantChangeKeyActions[i]))
            {
                BattleUIManager.Instance.ShakeMissionPanel();
            }
        }
    }

    public override void SetLv(DifficultyLevel lv)
    {
        
    }
}
