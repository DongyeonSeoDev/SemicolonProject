using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPointCrashCheckCollider : MonoBehaviour
{
    [SerializeField]
    private BodyPoint bodyPoint = null; // Set this in Inspector
    private CircleCollider2D col = null;
    public CircleCollider2D Col
    {
        get { return col; }
    }


    [SerializeField]
    private LayerMask whatIsWall;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }
    private void OnEnable()
    {
        EventManager.StartListening("BodyPointCrash", BodyPointCrash);
    }

    void Update()
    {
        if (bodyPoint.GetComponent<MiddlePoint>() == null)
        {
            transform.localPosition = Vector3.zero;
        }
    }
    private void OnDisable()
    {
        EventManager.StopListening("BodyPointCrash", BodyPointCrash);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (whatIsWall.CompareGameObjectLayer(other.gameObject))
        {
            bodyPoint.IsWall = true;
        }

        EventManager.TriggerEvent("BodyPointCrash", other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (whatIsWall.CompareGameObjectLayer(other.gameObject))
        {
            bodyPoint.IsWall = false;
        }
    }
    private void BodyPointCrash(GameObject targetObject)
    {

    }
}
