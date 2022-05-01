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

    public string action = "상호작용";

    [Space(15)]

    public Vector3 uiOffset = Vector3.down;  //이름 UI 오프셋
    public Vector3 itrUIOffset = Vector3.up;  //상호작용 표시 UI 오프셋

    //NPC이름과 상호작용 표시가 뜨는 범위가 다를 수 있으므로 분리함
    [HideInInspector] public NPCUI npcUI;
    [HideInInspector] public InteractionNoticeUI itrUI;

    public abstract void Interaction();

    public virtual void SetUI(bool on)  //상호작용 옵젝의 이름 활성화/비활성화
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
