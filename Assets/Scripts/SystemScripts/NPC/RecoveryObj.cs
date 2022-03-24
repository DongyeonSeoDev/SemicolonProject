using System;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryObj : InteractionObj
{
    private List<Action> recoveryActions = new List<Action>();

    private void Awake()
    {
        recoveryActions.Add(() => RecoveryPlayerHP(30));
        recoveryActions.Add(RemoveImprecation);
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

    }
}
