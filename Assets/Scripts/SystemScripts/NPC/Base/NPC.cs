using UnityEngine;

public abstract class NPC : InteractionObj
{
    [SerializeField] protected NPCInfo npcInfo;

    protected void Awake()
    {
        npcInfo.npcName = objName;
    }
    /* protected virtual void OnBecameInvisible()
     {
         SetUI(false);
     }

     protected virtual void OnBecameVisible()
     {
         SetUI(true);
     }*/
}
