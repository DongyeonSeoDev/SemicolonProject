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
        EffectManager.Instance.CallFollowTargetGameEffect("ImprecationEff", SlimeGameManager.Instance.CurrentPlayerBody.transform, Vector3.up, 2f);
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
            UIManager.Instance.RequestSystemMsg("더 이상 저주 효과를 받을 수 없습니다.");
            return;
        }

        ResetActionList();

        UIManager.Instance.RequestSelectionWindow("어떤 효과를 적용하시겠습니까?", imprecationActions, new List<string>() { "체력 40% 감소", "랜덤으로 저주 적용", "랜덤 아이템 손실" }, true,
            new List<Func<bool>>() {null, null, () => Inventory.Instance.ActiveSlotCount > 0});
        canInteract = false;
    }

    void DecreasePlayerHp(float value)
    {
        ItemUseMng.DecreaseCurrentHP(value);
    }

    void OnStateAbnormal()
    {
        StateAbnormality state = (StateAbnormality)UnityEngine.Random.Range(0, Global.EnumCount<StateAbnormality>()-1);
        StateManager.Instance.StartStateAbnormality(state);
    }

    void RemoveRandomItem()
    {
        UIManager.Instance.RequestLeftBottomMsg("저주로 임의의 인벤토리 한 칸의 아이템을 모두 잃었습니다.");
        Inventory.Instance.RemoveRandomItem();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        fsOut.gameObject.SetActive(on);
    }
}
