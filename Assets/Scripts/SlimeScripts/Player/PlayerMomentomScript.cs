using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMomentomScript : MonoBehaviour
{ 
    private PlayerChoiceStatControl playerChoiceStatControl = null;

    private float lastUpSpeed = 0f;

    [SerializeField]
    private float momentomTime = 3f;
    private float momentomTimer = 0f;

    private bool momentomStarted = false;

    private void Start()
    {
        playerChoiceStatControl = GetComponent<PlayerChoiceStatControl>();
    }
    void Update()
    {
        if(momentomStarted)
        {
            momentomTimer -= Time.deltaTime;

            if(momentomTimer < 0f)
            {
                EndMomentom();
            }
        }

        if(!momentomStarted && SlimeGameManager.Instance.Player.PlayerState.IsInMomentom)
        {
            StartMomentom();
        }
    }
    private void StartMomentom()
    {
        momentomStarted = true;
        momentomTimer = momentomTime;

        // ������ ��� ����Ʈ ������
        // ������ �����ؼ� ������ ������ �� �� ����
        lastUpSpeed = playerChoiceStatControl.UpMomentomSpeedPerMomentom * SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.statValue;
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.speed += lastUpSpeed;

        Debug.Log("������!!!!!!!!" + lastUpSpeed + " ��ŭ �̼� ���");
    }
    private void EndMomentom()
    {
        momentomStarted = false;
        SlimeGameManager.Instance.Player.PlayerState.IsInMomentom = false;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.speed -= lastUpSpeed;
        momentomTimer = 0f;

        Debug.Log("������ ����!" + lastUpSpeed + "��ŭ �̵��ӵ� ����");
        lastUpSpeed = 0f;
    }
}