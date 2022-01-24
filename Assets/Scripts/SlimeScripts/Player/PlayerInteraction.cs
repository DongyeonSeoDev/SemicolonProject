using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private PlayerInput playerInput = null;

    [SerializeField]
    private LayerMask interactionableObjLayers;

    [SerializeField]
    private float interactionableDistance = 3f;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    void Update()
    {
        if(playerInput.IsInterraction)
        {
            Interaction();
        }
    }
    private void Interaction() // 상호작용
    {
        playerInput.IsInterraction = false;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, interactionableDistance, Vector2.zero, 0f, interactionableObjLayers);

        // 상호작용을 실행하는 코드

    }
}
