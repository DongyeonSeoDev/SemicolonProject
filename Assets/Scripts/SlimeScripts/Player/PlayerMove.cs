using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerInput playerInput = null;
    private Rigidbody2D rigid = null;
    private List<Rigidbody2D> childRigids = new List<Rigidbody2D>();

    [SerializeField]
    private float speed = 0f;
    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();

        rigid = GetComponent<Rigidbody2D>();

        childRigids = transform.GetComponentsInChildren<Rigidbody2D>().ToList();
    }

    private void FixedUpdate()
    {
        Vector2 MoveVec = playerInput.MoveVector * speed;
        rigid.velocity = MoveVec;

        childRigids.ForEach(x => x.velocity = MoveVec);
    }

}
