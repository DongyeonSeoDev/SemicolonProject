using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction : MonoBehaviour
{
    protected Player player = null;
    protected PlayerState playerState = null;
    protected PlayerInput playerInput = null;
    protected PCSoftBody softBody = null;

    protected Rigidbody2D rigid = null;
    protected List<Rigidbody2D> childRigids = new List<Rigidbody2D>();

    public virtual void Awake()
    {
        player = SlimeGameManager.Instance.Player;
        playerState = player.GetComponent<PlayerState>();
        playerInput = player.GetComponent<PlayerInput>();
        softBody = GetComponent<PCSoftBody>();

        rigid = GetComponent<Rigidbody2D>();

        childRigids = transform.GetComponentsInChildren<Rigidbody2D>().ToList();
    }
}
