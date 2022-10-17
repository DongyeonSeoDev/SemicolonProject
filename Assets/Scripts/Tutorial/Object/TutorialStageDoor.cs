
using UnityEngine;

public class TutorialStageDoor : MonoBehaviour, IDamageableBySlimeBodySlap
{
    [SerializeField] private NormalNPC npc;  //���õ� NPC 
    [SerializeField] private StageDoor door; //��¥ ��

    private int hp;
    private bool isDamageable = true;

    private void OnEnable()
    {
        hp = 2;
    }

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
            CinemachineCameraScript.Instance.Shake(2f, 2f, 0.3f);
            EffectManager.Instance.CallGameEffect("DoorHitEff", transform.position, 1.5f);

            if (!StoredData.HasValueKey("TutoDoorHit1"))
            {
                TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto_BreakDoorOneHit"));
                StoredData.SetValueKey("TutoDoorHit1", true);
            }

            if(--hp == 0)   
            {
                npc._NPCInfo.talkId = 3;
                TalkManager.Instance.SetSubtitle(SubtitleDataManager.Instance.GetSubtitle("Tuto_BreakDoor"));
                door.IsBreak = true;
                door.Open();
                door.GetComponent<Collider2D>().enabled = true;
            }
            else
            {
                SoundManager.Instance.PlaySoundBox("Door Hit SFX");
            }

            isDamageable = false;
            Util.DelayFunc(() => isDamageable = true, 0.7f);

            Debug.Log("Tuto_Door Hit Log");
        }
    }
}
