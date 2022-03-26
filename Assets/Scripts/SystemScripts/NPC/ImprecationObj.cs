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
        UIManager.Instance.RequestSelectionWindow("어떤 효과를 적용하시겠습니까?", imprecationActions, new List<string>() { "체력 40% 감소", "랜덤으로 저주 적용", "랜덤 아이템 손실" });
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
        UIManager.Instance.RequestLeftBottomMsg("저주로 임의의 인벤토리 한 칸의 아이템을 모두 잃었습니다.");
        Inventory.Instance.RemoveRandomItem();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);
        fsOut.gameObject.SetActive(on);
    }
}
