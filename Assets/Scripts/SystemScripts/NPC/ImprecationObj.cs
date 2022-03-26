using System;
using System.Collections.Generic;
using UnityEngine;

public class ImprecationObj : InteractionObj
{
    private List<Action> imprecationActions = new List<Action>();
    public FakeSpriteOutline fsOut;

    private void Awake()
    {
        imprecationActions.Add(() => DecreasePlayerHp(40));
        imprecationActions.Add(OnStateAbnormal);
        imprecationActions.Add(RemoveRandomItem);

        for (int i = 0; i < imprecationActions.Count; i++)
        {
            imprecationActions[i] += () => StageManager.Instance.SetClearStage();
        }
    }

    public override void Interaction()
    {
        UIManager.Instance.RequestSelectionWindow("� ȿ���� �����Ͻðڽ��ϱ�?", imprecationActions, new List<string>() { "ü�� 40% ����", "�������� ���� ����", "���� ������ �ս�" });
    }

    void DecreasePlayerHp(float value)
    {
        ItemUseMng.DecreaseCurrentHP(value);
    }

    void OnStateAbnormal()
    {
        StateAbnormality state = Global.GetEnumArr<StateAbnormality>().ToList().ToRandomElement();
        StateManager.Instance.StartStateAbnormality(state);
    }

    void RemoveRandomItem()
    {
        UIManager.Instance.RequestLeftBottomMsg("���ַ� ������ �κ��丮 �� ĭ�� �������� ��� �Ҿ����ϴ�.");
        Inventory.Instance.RemoveRandomItem();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        fsOut.gameObject.SetActive(on);
    }
}
