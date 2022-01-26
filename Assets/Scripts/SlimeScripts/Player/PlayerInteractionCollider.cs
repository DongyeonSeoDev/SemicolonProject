using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractionCollider : MonoBehaviour
{
    private PlayerInteraction playerInteraction = null;

    [SerializeField]
    private LayerMask interactionableNPCLayer;
    void Start()
    {
        playerInteraction = SlimeGameManager.Instance.Player.GetComponent<PlayerInteraction>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (interactionableNPCLayer.CompareGameObjectLayer(other.gameObject))
        {
            NPC target = other.GetComponent<NPC>();

            if (target != null)
            {
                playerInteraction.NearNPCList.Add(target);
 
                target.SetUI(true); // NPC가 상호작용 범위 안에 들어갔을 때
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (interactionableNPCLayer.CompareGameObjectLayer(other.gameObject))
        {
            NPC target = other.GetComponent<NPC>();

            if (target != null)
            {
                playerInteraction.NearNPCList.Remove(target);

                target.SetUI(false); // NPC가 상호작용 범위에서 벗어났을 때
            }
        }
    }
}
