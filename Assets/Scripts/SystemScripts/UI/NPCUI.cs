using UnityEngine;
using UnityEngine.UI;
using Water;

public class NPCUI : MonoBehaviour
{
    private NPC npc;

    public Text nameText;

    public void Set(NPC npc)
    {
        this.npc = npc;
        nameText.text = npc.NPCName;
        transform.position = Util.WorldToScreenPoint(npc.transform.position + npc.uiOffset);
    }
}
