using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyScript : MonoBehaviour
{
    private void Start() 
    {
        SlimeGameManager.Instance.CurrentPlayerBody = gameObject;

        PlayerInteractionCollider x = GetComponentInChildren<PlayerInteractionCollider>();

        if(x == null)
        {
            Instantiate(Resources.Load<GameObject>("Player/PlayerCollider/InteractionCollider"), transform);
        }

        PlayerGetItemCollider y = GetComponentInChildren<PlayerGetItemCollider>();

        if(y == null)
        {
            Instantiate(Resources.Load<GameObject>("Player/PlayerCollider/GetItemCollider"), transform);
        }
    }
}
