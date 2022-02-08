using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour
{
    private InteractionObj npc;

    public Text nameText;

    public void Set(InteractionObj npc)
    {
        this.npc = npc;
        nameText.text = npc.ObjName;
    }

    private void Update()
    {
        if(npc)
        {
            transform.position = Util.WorldToScreenPoint(npc.transform.position + npc.uiOffset);
        }
    }

    private void OnDisable()
    {
        npc = null;
    }
}
