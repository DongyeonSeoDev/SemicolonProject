using UnityEngine;
using Water;

public abstract class InteractionObj : MonoBehaviour
{
    [SerializeField] protected bool isHidenName;
    [SerializeField] protected string objName;
    public string ObjName { get { return objName; } }

    public Vector3 uiOffset = Vector3.down;

    [HideInInspector] public NPCUI npcUI;


    public abstract void Interaction();

    public virtual void SetUI(bool on)
    {
        if (on)
        {
            if (!npcUI)
            {
                npcUI = PoolManager.GetItem("NPCNameUI").GetComponent<NPCUI>();
                npcUI.Set(this);
            }
            else
            {
                npcUI.gameObject.SetActive(true);
            }
        }
        else
        {
            if (npcUI)
                npcUI.gameObject.SetActive(false);
        }
    }
}
