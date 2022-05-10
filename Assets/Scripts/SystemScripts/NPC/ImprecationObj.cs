using System;
using System.Collections.Generic;
using UnityEngine;

public class ImprecationObj : InteractionObj
{
    private List<Action> imprecationActions = new List<Action>();
    public FakeSpriteOutline fsOut;

    private bool canInteract;

    private void ResetActionList()
    {
        imprecationActions.Clear();

        imprecationActions.Add(() => DecreasePlayerHp(40));
        imprecationActions.Add(OnStateAbnormal);
        imprecationActions.Add(RemoveRandomItem);

        for (int i = 0; i < imprecationActions.Count; i++)
        {
            imprecationActions[i] += DefaultFunc;
        }
    }

    private void DefaultFunc()
    {
        StageManager.Instance.SetClearStage();
        EffectManager.Instance.CallFollowTargetGameEffect("ImprecationEff", GameManager.Instance.slimeFollowObj, Vector3.up, 2f);
        SoundManager.Instance.PlaySoundBox("GetDebuffSFX");
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
            UIManager.Instance.RequestSystemMsg("�� �̻� ���� ȿ���� ���� �� �����ϴ�.");
            return;
        }

        ResetActionList();

        UIManager.Instance.RequestSelectionWindow("�̰��� ���ֱ����Դϴ�.\n� ȿ���� �����Ͻðڽ��ϱ�?", imprecationActions, new List<string>() { "DescHp", "RandAntiBuff", "RandItemRm" }, true,
            new List<Func<bool>>() {null, null, () => Inventory.Instance.ActiveSlotCount > 0}, true);
        canInteract = false;
    }

    void DecreasePlayerHp(float value)
    {
        ItemUseMng.DecreaseCurrentHP(value);
    }

    void OnStateAbnormal()
    {
        //���� ���� ���ְ� ���� ������ ������ �� �ڵ����� �ӽ� �ڵ� �ۼ���
        //StateAbnormality antiBuff = (StateAbnormality)UnityEngine.Random.Range(0, Global.EnumCount<StateAbnormality>()-1);

        StateAbnormality antiBuff = StateAbnormality.None;
        do
        {
            antiBuff = (StateAbnormality)UnityEngine.Random.Range(0, Global.EnumCount<StateAbnormality>() - 1);
        } while(antiBuff == StateAbnormality.Poverty);

        StateManager.Instance.StartStateAbnormality(antiBuff);
    }

    void RemoveRandomItem()
    {
        UIManager.Instance.RequestLogMsg("���ַ� ������ �κ��丮 �� ĭ�� �������� ��� �Ҿ����ϴ�.");
        Inventory.Instance.RemoveRandomItem();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        //fsOut.gameObject.SetActive(on);
    }
}
