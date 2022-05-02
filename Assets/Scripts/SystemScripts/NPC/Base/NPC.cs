using UnityEngine;

public abstract class NPC : InteractionObj
{
    [SerializeField] protected NPCInfo npcInfo;
    public NPCInfo _NPCInfo => npcInfo;

    public FakeSpriteOutline fsOut;

    public string npcId => name;

    protected override void Awake()
    {
        if (string.IsNullOrEmpty(npcInfo.npcName))
            npcInfo.npcName = objName;
        if (string.IsNullOrEmpty(npcInfo.id))
            npcInfo.id = objId;

        objName = npcInfo.npcName;
        objId = npcInfo.id;

        base.Awake();
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
