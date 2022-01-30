using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPointCrashCheckCollider : MonoBehaviour
{
    [SerializeField]
    private BodyPoint bodyPoint = null; // Set this in Inspector

    [SerializeField]
    private LayerMask whatIsWall;

    private void OnEnable()
    {
        EventManager.StartListening("BodyPointCrash", BodyPointCrash);
    }

    void Update()
    {
        transform.localPosition = bodyPoint.transform.localPosition;
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
