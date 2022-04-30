
using UnityEngine;

public class TutorialStageDoor : MonoBehaviour, IDamageableBySlimeBodySlap
{
    [SerializeField] private NormalNPC npc;

    public void GetDamage(int damage, float charging)
    {
        if(charging < Global.GetSlimePos.GetComponent<PlayerBodySlap>().MaxChargingTime)
        {
            TalkManager.Instance.SetTalkData(npc._NPCInfo, npc.transform, 1);
        }
    }
}
