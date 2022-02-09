using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Water;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInput playerInput = null;

    private List<InteractionObj> nearNPCList = new List<InteractionObj>();
    public List<InteractionObj> NearNPCList
    {
        get { return nearNPCList; }
        set { nearNPCList = value; }
    }

    private bool isNearByNPC = false;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {
        if (nearNPCList.Count > 0)
        {
            isNearByNPC = true;
        }
        else
        {
            isNearByNPC = false;
        }

        if (playerInput.IsInterraction)
        {
            if (isNearByNPC)
            {
                Interaction();
            }
            else
            {
                playerInput.IsInterraction = false;
            }
        }

    }
    private void Interaction() // 상호작용
    {
        playerInput.IsInterraction = false;

        // 상호작용을 실행하는 코드

        nearNPCList[0].Interaction(); // 일단은 맨 처음의 것만 실행, 내일 팀원과 상의해서 상호작용 NPC의 우선순위 기준을 정할 것
    }
}
