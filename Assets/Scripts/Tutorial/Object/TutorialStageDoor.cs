
using UnityEngine;

public class TutorialStageDoor : MonoBehaviour, IDamageableBySlimeBodySlap
{
    [SerializeField] private NormalNPC npc;  //관련된 NPC 
    [SerializeField] private StageDoor door; //진짜 문

    private int hp = 2;
    private bool isDamageable = true;  

    public void GetDamage(int damage, float charging)
    {
        if (!isDamageable) return;
        if(door.IsOpen) //진짜 문이 열려있는 상태에서 한 번 더 박으면 가짜문 옵젝 꺼줌
        {
            npc._NPCInfo.talkId = 3;
            gameObject.SetActive(false);
            return;
        }

        TalkManager.Instance.SetSubtitle("생각보다 문이 좀 딱딱한데", 0.2f, 2f);
        if (charging < Global.GetSlimePos.GetComponent<PlayerBodySlap>().MaxChargingTime) //에너지 풀차징 아니면
        {
            //TalkManager.Instance.SetTalkData(npc._NPCInfo, npc.transform, 1);
        }
        else
        {
            if(--hp == 0)  
            {
                npc._NPCInfo.talkId = 3;
                TalkManager.Instance.SetSubtitle("우왓!", 0.2f, 1f);
            }

            isDamageable = false;
            Util.DelayFunc(() => isDamageable = true, 1f);
        }
    }
}
