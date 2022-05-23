using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyScript : MonoBehaviour
{
    private readonly string playerLayerName = "Player";

    private SpriteRenderer spriteRenderer = null;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        SlimeGameManager.Instance.CurrentPlayerBody = gameObject;
        spriteRenderer.sortingLayerName = playerLayerName;
    }
    private void Start() 
    {
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

        AvoidCloseCheckCollider z = GetComponentInChildren<AvoidCloseCheckCollider>();

        if(z == null)
        {
            Instantiate(Resources.Load<GameObject>("Player/PlayerCollider/BoxAvoidCloseCheckCollider"), transform);
        }
    }
}
