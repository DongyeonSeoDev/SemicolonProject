using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPoint : MonoBehaviour
{
    [SerializeField]
    private LayerMask whatIsWall;
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

    private void Awake() 
    {
        SlimeEventManager.StartListening("BodyPointCrash", BodyPointCrash);
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
    private void OnDisable() 
    {
        SlimeEventManager.StopListening("BodyPointCrash", BodyPointCrash);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (whatIsWall.CompareGameObjectLayer(other.gameObject))
        {
            isWall = true;
        }

        SlimeEventManager.TriggerEvent("BodyPointCrash", other.gameObject);
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (whatIsWall.CompareGameObjectLayer(other.gameObject))
        {
            isWall = false;
        }
    }
    private void BodyPointCrash(GameObject targetObject)
    {

    }
}
