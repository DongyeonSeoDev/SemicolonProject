using UnityEngine;

public class NormalEventObject : InteractionObj
{
    [SerializeField] private string interactionEventKey;  //상호작용에서 발생시킬 이벤트의 키

    [SerializeField] private FakeSpriteOutline fsOut;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interaction()
    {
        //한 번 상호작용하면 적절한 이벤트 발생시키고 더 이상 상호작용 안됨(이름은 뜨지만 상호작용 표시는 안뜸)
        if (notInteractable) return;
        notInteractable = true;
        isHidenItrMark = true;

        interactionEventKey.TriggerEvent();
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

        fsOut.gameObject.SetActive(on);
    }
}
