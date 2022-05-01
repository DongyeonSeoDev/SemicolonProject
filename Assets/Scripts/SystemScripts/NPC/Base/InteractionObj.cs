using UnityEngine;
using Water;

public abstract class InteractionObj : MonoBehaviour
{
    [SerializeField] protected bool isHidenName, isHidenItrMark;
    [SerializeField] protected bool notInteractable;

    [Space(15)]

    [SerializeField] protected string objName;
    public string ObjName { get { return objName; } }

    [SerializeField] protected string objId;
    public string ObjId => objId;

    public string action = "��ȣ�ۿ�";

    [Space(15)]

    public Vector3 uiOffset = Vector3.down;  //�̸� UI ������
    public Vector3 itrUIOffset = Vector3.up;  //��ȣ�ۿ� ǥ�� UI ������

    //NPC�̸��� ��ȣ�ۿ� ǥ�ð� �ߴ� ������ �ٸ� �� �����Ƿ� �и���
    [HideInInspector] public NPCUI npcUI;
    [HideInInspector] public InteractionNoticeUI itrUI;

    public abstract void Interaction();

    public virtual void SetUI(bool on)  //��ȣ�ۿ� ������ �̸� Ȱ��ȭ/��Ȱ��ȭ
    {
        if (on)
        {
            if (!npcUI && !isHidenName)
            {
                npcUI = PoolManager.GetItem("NPCNameUI").GetComponent<NPCUI>();
                npcUI.Set(this);
            }
        }
        else
        {
            if (npcUI)
            {
                npcUI.gameObject.SetActive(false);
                npcUI = null;
            }
        }
    }

    public virtual void SetInteractionUI(bool on)
    {
        if(on && !isHidenItrMark)
        {
            itrUI = PoolManager.GetItem("InteractionMark").GetComponent<InteractionNoticeUI>();
            itrUI.Set(this);
        }
        else
        {
            if(itrUI)
            {
                itrUI.gameObject.SetActive(false);
                itrUI = null;
            }
        }
    }
}
