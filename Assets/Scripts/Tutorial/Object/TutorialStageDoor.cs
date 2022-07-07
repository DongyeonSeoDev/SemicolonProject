
using UnityEngine;

public class TutorialStageDoor : MonoBehaviour, IDamageableBySlimeBodySlap
{
    [SerializeField] private NormalNPC npc;  //���õ� NPC 
    [SerializeField] private StageDoor door; //��¥ ��

    private int hp = 2;
    private bool isDamageable = true;  

    public void GetDamage(int damage, float charging)
    {
        if (!isDamageable) return;
        if(door.IsOpen) //��¥ ���� �����ִ� ���¿��� �� �� �� ������ ��¥�� ���� ����
        {
            npc._NPCInfo.talkId = 3;
            gameObject.SetActive(false);
            return;
        }

        if (charging < Global.GetSlimePos.GetComponent<PlayerBodySlap>().MaxChargingTime) //������ Ǯ��¡ �ƴϸ�
        {
            //TalkManager.Instance.SetTalkData(npc._NPCInfo, npc.transform, 1);
        }
        else
        {
            if(!StoredData.HasValueKey("TutoDoorHit1"))
            {
                TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto_BreakDoorOneHit"));
                StoredData.SetValueKey("TutoDoorHit1", true);
            }

            if(--hp == 0)  
            {
                npc._NPCInfo.talkId = 3;
                TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto_BreakDoor"));
            }

            isDamageable = false;
            Util.DelayFunc(() => isDamageable = true, 1f);
        }
    }
}
