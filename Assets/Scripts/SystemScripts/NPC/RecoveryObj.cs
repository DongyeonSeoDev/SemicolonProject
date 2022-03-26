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
        UIManager.Instance.RequestSelectionWindow("� ȿ���� �����ðڽ��ϱ�?", recoveryActions, new List<string>() { "ü�� 30% ȸ��", "���� ����" });
    }

    void RecoveryPlayerHP(float value)
    {
        ItemUseMng.IncreaseCurrentHP(value);
    }
    void RemoveImprecation()
    {
        StateManager.Instance.RemoveAllStateAbnormality();
        UIManager.Instance.RequestLeftBottomMsg("��� ���ְ� �����Ǿ����ϴ�.");
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        fsOut.gameObject.SetActive(on);
    }
}
