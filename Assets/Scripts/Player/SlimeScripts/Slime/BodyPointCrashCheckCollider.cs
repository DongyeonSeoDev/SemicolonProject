using System.Linq;
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
        EventManager.StartListening("OnBodySlap", OnBodySlap);
        EventManager.StartListening("ExitCurrentMap", ExitCurrentMap);
    }
    private void OnDisable()
    {
        EventManager.StopListening("OnBodySlap", OnBodySlap);
        EventManager.StopListening("ExitCurrentMap", ExitCurrentMap);
    }

    void Update()
    {
        if (bodyPoint.GetComponent<MiddlePoint>() == null)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        if (whatIsWall.CompareGameObjectLayer(obj))
        {
            bodyPoint.IsWall = true;

            return;
        }

        if (SlimeGameManager.Instance.Player.PlayerState.BodySlapping)
        {
            EventManager.TriggerEvent("BodyPointCrash", obj);
        }
        else
        {
            if (bodyPoint.MiddlePoint != null)
            {
                bodyPoint.MiddlePoint.WillCrashList.Add(obj);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject obj = other.gameObject;

        if (whatIsWall.CompareGameObjectLayer(obj))
        {
            bodyPoint.IsWall = false;
        }

        if(bodyPoint.MiddlePoint != null)
        {
            bodyPoint.MiddlePoint.WillCrashList.Remove(obj);
        }
    }
    private void OnBodySlap()
    {
        if (bodyPoint.MiddlePoint != null)
        {
            List<GameObject> objList = bodyPoint.MiddlePoint.WillCrashList.Distinct().ToList();

            foreach (var item in objList)
            {
                EventManager.TriggerEvent("BodyPointCrash", item);
            }
        }
    }
    private void ExitCurrentMap()
    {
        if (bodyPoint.MiddlePoint != null)
        {
            bodyPoint.MiddlePoint.WillCrashList.Clear();
        }
    }
}
