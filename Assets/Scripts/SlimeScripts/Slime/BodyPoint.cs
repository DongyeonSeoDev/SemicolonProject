using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPoint : MonoBehaviour
{
    [SerializeField]
    private float returnToOriginSpeed = 2f;

    private Vector2 originLocalPosition = Vector2.zero;

    [SerializeField]
    private bool isMiddlePoint = false;

    private bool isWall = false;
    public bool IsWall
    {
        get { return isWall; }
    }

    private void Start()
    {
        originLocalPosition = transform.localPosition;
    }
    private void Update()
    {
        if (!isWall && !isMiddlePoint)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, originLocalPosition, Time.deltaTime * returnToOriginSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isWall = true;
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            isWall = false;
        }
    }
}
