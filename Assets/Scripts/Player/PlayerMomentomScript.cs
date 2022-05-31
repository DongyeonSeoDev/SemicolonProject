using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMomentomScript : MonoBehaviour
{ 
    private PlayerChoiceStatControl playerChoiceStatControl = null;

    private float lastUpSpeed = 0f;

    [SerializeField]
    private float momentomTime = 3f;
    [SerializeField]
    private float momentomSpeed = 3f;
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
        momentomTimer = momentomTime + 
            playerChoiceStatControl.ChoiceDataDict[NGlobal.MomentomID].upTargetStatPerChoiceStat * SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom.statValue;

        // 추진력 사용 이펙트 넣을것
        // 이전값 저장해서 추진력 끝나면 그 값 빼기
        lastUpSpeed = momentomSpeed;
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.speed.statValue += lastUpSpeed;

        Debug.Log("추진력!!!!!!!!" + lastUpSpeed + " 만큼 이속 상승");
    }
    private void EndMomentom()
    {
        momentomStarted = false;
        SlimeGameManager.Instance.Player.PlayerState.IsInMomentom = false;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.speed.statValue -= lastUpSpeed;
        momentomTimer = 0f;

        Debug.Log("추진력 멈춰!" + lastUpSpeed + "만큼 이동속도 감소");
        lastUpSpeed = 0f;
    }
}
