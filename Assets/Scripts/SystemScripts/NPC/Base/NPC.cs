using Water;
using UnityEngine;

public abstract class NPC : MonoBehaviour
{
    [SerializeField] protected string npcName;
    public string NPCName { get { return npcName; } }

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
            if(npcUI)
               npcUI.gameObject.SetActive(false);
        }
    }
}
