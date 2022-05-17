
using UnityEngine;

public class TutorialStageDoor : MonoBehaviour, IDamageableBySlimeBodySlap
{
    [SerializeField] private NormalNPC npc;
    [SerializeField] private StageDoor door;

    private int hp = 2;

    public void GetDamage(int damage, float charging)
    {
        if(door.IsOpen)
        {
            gameObject.SetActive(false);
            return;
        }

        if(charging < Global.GetSlimePos.GetComponent<PlayerBodySlap>().MaxChargingTime)
        {
            TalkManager.Instance.SetTalkData(npc._NPCInfo, npc.transform, 1);
        }
        else
        {
            if(--hp == 0)  //�� �븸 �¾Ҵµ��� �� �ڵ尡 ����Ǵ� ���� ����
            {
                npc._NPCInfo.talkId = 3;
            }
        }
    }
}
