using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddlePoint : BodyPoint
{
    private PlayerDrain playerDrain = null;
    public PlayerDrain PlayerDrain
    {
        get { return playerDrain; }
    }

    private SlimeAvoidCloseCheckCollider playerAvoidCloseCheckCollider = null;

    [SerializeField]
    private EdgeCollider2D triggerEdgeCollider2D = null;

    private Vector2[] notMiddlePointsPositions;

    private PCSoftBody softBody = null;
    public PCSoftBody SoftBody
    {
        get { return softBody; }
        set { softBody = value; }
    }

    [SerializeField]
    private float minDisWithBodyPoints = 0.2f; // MiddlePoint는 이 값 이하로 BodyPoint와 가까워 질 수 없다.
    public float MinDisWithBodyPoints
    {
        get { return minDisWithBodyPoints;}
    }

    [SerializeField]
    private float maxDisWithBodyPoints = 5f; // MiddlePoint는 이 값 이상으로 BodyPoint와 멀어질 수 없다.
    public float MaxDisWithBodyPoints
    {
        get { return maxDisWithBodyPoints;}
    }

    [SerializeField]
    private bool afterImageSoftBodySpawned = false;
    public bool DrainEffectSpawned
    {
        get { return afterImageSoftBodySpawned;}
        set { afterImageSoftBodySpawned = value; }  
    }

    private List<GameObject> willCrashList = new List<GameObject>();
    public List<GameObject> WillCrashList
    {
        get { return willCrashList; }
    }
    private void Start()
    {
        playerDrain = GetComponent<PlayerDrain>();
        playerAvoidCloseCheckCollider = GetComponentInChildren<SlimeAvoidCloseCheckCollider>();
        playerAvoidCloseCheckCollider.SetMiddlePoint(this);
    }
    private void FixedUpdate()
    {
        FixedUpdatePointsPositions();
        FixedUpdateEdgeCollider();
    }
    private void FixedUpdatePointsPositions()
    {
        notMiddlePointsPositions = new Vector2[softBody.NotMiddlePoints.Count + 1];

        for(int i = 0; i < notMiddlePointsPositions.Length; i++)
        {
            notMiddlePointsPositions[i] = softBody.NotMiddlePoints[i.Limit(0, softBody.NotMiddlePoints.Count - 1)].localPosition;
        }

        playerAvoidCloseCheckCollider.SetEdgePoints(notMiddlePointsPositions);
    }
    private void FixedUpdateEdgeCollider()
    {
        triggerEdgeCollider2D.SetPoints(notMiddlePointsPositions.ToList());
    }
}
