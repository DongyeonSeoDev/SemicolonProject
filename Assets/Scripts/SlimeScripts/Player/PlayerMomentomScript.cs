using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMomentomScript : MonoBehaviour
{ 
    private PlayerChoiceStatControl playerChoiceStatControl = null;

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

        // 추진력 사용 이펙트 넣을것
        //스킬 스크립트 따로 빠져있으면 PlayerSkill 상속해서 작업하는게 편한데 그게
        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.speed += playerChoiceStatControl.UpMomentomSpeedPerMomentom * SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom;
        Debug.Log("추진력!!!!!!!!" + playerChoiceStatControl.UpMomentomSpeedPerMomentom * SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom + " 만큼 이속 상승");
    }
    private void EndMomentom()
    {
        momentomStarted = false;
        SlimeGameManager.Instance.Player.PlayerState.IsInMomentom = false;

        SlimeGameManager.Instance.Player.PlayerStat.additionalEternalStat.speed -= playerChoiceStatControl.UpMomentomSpeedPerMomentom * SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom;
        momentomTimer = 0f;

        Debug.Log("추진력 멈춰!" + playerChoiceStatControl.UpMomentomSpeedPerMomentom * SlimeGameManager.Instance.Player.PlayerStat.choiceStat.momentom + "만큼 이동속도 감소");
    }
}
