using UnityEngine;

public class CharacteristicNPC : NPC
{
    public override void Interaction()
    {
        StatStore.Instance.ShowStockList();
    }
}
