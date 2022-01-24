using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAction : MonoBehaviour
{
    protected PlayerState playerStatus = null;
    protected PlayerInput playerInput = null;

    protected Rigidbody2D rigid = null;
    protected List<Rigidbody2D> childRigids = new List<Rigidbody2D>();

    public virtual void Start()
    {
        playerStatus = GetComponent<PlayerState>();
        playerInput = GetComponent<PlayerInput>();

        rigid = GetComponent<Rigidbody2D>();

        childRigids = transform.GetComponentsInChildren<Rigidbody2D>().ToList();
    }
}
