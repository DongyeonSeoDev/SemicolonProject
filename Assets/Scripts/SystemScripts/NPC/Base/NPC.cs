using UnityEngine;

public abstract class NPC : InteractionObj
{
    [SerializeField] protected NPCInfo npcInfo;
    public NPCInfo _NPCInfo => npcInfo;

    public FakeSpriteOutline fsOut;

    public string npcId => name;

    protected void Awake()
    {
        npcInfo.npcName = objName;
    }
    private void Start()
    {
        if(fsOut)
           fsOut.gameObject.SetActive(false);
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

        if(fsOut)
           fsOut.gameObject.SetActive(on);
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
