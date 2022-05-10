using UnityEngine;

public class NormalEventObject : InteractionObj
{
    [SerializeField] private string interactionEventKey;  //��ȣ�ۿ뿡�� �߻���ų �̺�Ʈ�� Ű

    [SerializeField] private FakeSpriteOutline fsOut;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interaction()
    {
        //�� �� ��ȣ�ۿ��ϸ� ������ �̺�Ʈ �߻���Ű�� �� �̻� ��ȣ�ۿ� �ȵ�(�̸��� ������ ��ȣ�ۿ� ǥ�ô� �ȶ�)
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
