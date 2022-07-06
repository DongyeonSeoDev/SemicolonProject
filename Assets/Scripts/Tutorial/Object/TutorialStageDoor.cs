
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

        TalkManager.Instance.SetSubtitle("�������� ���� �� �����ѵ�", 0.15f, 2f);
        if (charging < Global.GetSlimePos.GetComponent<PlayerBodySlap>().MaxChargingTime) //������ Ǯ��¡ �ƴϸ�
        {
            //TalkManager.Instance.SetTalkData(npc._NPCInfo, npc.transform, 1);
        }
        else
        {
            if(--hp == 0)  
            {
                npc._NPCInfo.talkId = 3;
                TalkManager.Instance.SetSubtitle("���!", 0.1f, 1f);
            }

            isDamageable = false;
            Util.DelayFunc(() => isDamageable = true, 1f);
        }
    }
}
