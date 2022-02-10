using UnityEngine;

public abstract class NPC : InteractionObj
{
    [SerializeField] protected NPCInfo npcInfo;

    public FakeSpriteOutline fsOut;

    protected void Awake()
    {
        npcInfo.npcName = objName;
    }
    private void Start()
    {
        fsOut.gameObject.SetActive(false);
    }

    public override void SetInteractionUI(bool on)
    {
        base.SetInteractionUI(on);

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
