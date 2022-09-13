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

        if (InteractionHandler.canInteractObj)
        {
            nearNPCList[0].Interaction();
        }
    }
}
