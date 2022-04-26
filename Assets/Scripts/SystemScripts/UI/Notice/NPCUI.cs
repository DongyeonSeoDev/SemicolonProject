using UnityEngine;
using UnityEngine.UI;

public class NPCUI : MonoBehaviour
{
    private RectTransform rectTr;
    private InteractionObj npc;

    public Text nameText;

    private void Awake()
    {
        rectTr = GetComponent<RectTransform>();
    }

    public void Set(InteractionObj npc)
    {
        this.npc = npc;
        nameText.text = npc.ObjName;
    }

    private void Update()
    {
        if(npc)
        {
            //transform.position = Util.WorldToScreenPoint(npc.transform.position + npc.uiOffset);  //overlay일때
            //rectTr.anchoredPosition = RectTransformUtility.WorldToScreenPoint(Util.MainCam, npc.transform.position + npc.uiOffset);  //camera일때

            rectTr.anchoredPosition = Util.WorldToScreenPosForScreenSpace(npc.transform.position + npc.uiOffset, Util.WorldCvs);
        }
    }

    private void OnDisable()
    {
        npc = null;
    }
}
