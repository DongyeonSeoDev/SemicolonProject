using System;
using System.Collections.Generic;
using UnityEngine;

public class RecoveryObj : InteractionObj
{
    private List<Action> recoveryActions = new List<Action>();
    public FakeSpriteOutline fsOut;

    private bool canInteract;

    private void ResetActionList()
    {
        recoveryActions.Clear();  //�ʱ�ȭ�ϴ� ������ �Ű������� List<Action>�� �����ϰ� �� �Լ� ������ �� � �Լ��� �߰��ϰ� �Ǵµ� Clear�� ���ϸ� �� �߰��� �Լ��� �� �߰��ǰ� �̰� ���̰� �ȴ�.

        recoveryActions.Add(() => RecoveryPlayerHP(30));
        recoveryActions.Add(RemoveImprecation);

        for (int i = 0; i < recoveryActions.Count; i++)
        {
            recoveryActions[i] += () => StageManager.Instance.SetClearStage();
        }
    }

    private void OnEnable()
    {
        canInteract = true;
    }
    private void OnDisable()
    {
        canInteract = false;
    }

    public override void Interaction()
    {
        if (!canInteract)
        {
            UIManager.Instance.RequestSystemMsg("�� �̻� ȸ�� ȿ���� ���� �� �����ϴ�.");
            return;
        }

        ResetActionList();  

        UIManager.Instance.RequestSelectionWindow("� ȿ���� �����ðڽ��ϱ�?", recoveryActions, new List<string>() { "ü�� 30% ȸ��", "���� ����" });
        canInteract = false;
    }

    void RecoveryPlayerHP(float value)
    {
        ItemUseMng.IncreaseCurrentHP(value);
    }
    void RemoveImprecation()
    {
        StateManager.Instance.RemoveAllStateAbnormality();
        //UIManager.Instance.RequestLeftBottomMsg("��� ���ְ� �����Ǿ����ϴ�.");
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        fsOut.gameObject.SetActive(on);
    }
}
