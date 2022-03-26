using System;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryObj : InteractionObj
{
    private List<Action> recoveryActions = new List<Action>();
    public FakeSpriteOutline fsOut;

    private void Awake()
    {
        recoveryActions.Add(() => RecoveryPlayerHP(30));
        recoveryActions.Add(RemoveImprecation);

        for(int i = 0; i < recoveryActions.Count; i++)
        {
            recoveryActions[i] += () => StageManager.Instance.SetClearStage();
        }
    }

    public override void Interaction()
    {
        UIManager.Instance.RequestSelectionWindow("어떤 효과를 받으시겠습니까?", recoveryActions, new List<string>() { "체력 30% 회복", "저주 해제" });
    }

    void RecoveryPlayerHP(float value)
    {
        ItemUseMng.IncreaseCurrentHP(value);
    }
    void RemoveImprecation()
    {
        StateManager.Instance.RemoveAllStateAbnormality();
        UIManager.Instance.RequestLeftBottomMsg("모든 저주가 해제되었습니다.");
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        fsOut.gameObject.SetActive(on);
    }
}
