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

    private void Start()
    {
        playerDrain = GetComponent<PlayerDrain>();
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
            notMiddlePointsPositions[i.Limit(0, softBody.NotMiddlePoints.Count - 1)] = softBody.NotMiddlePoints[i.Limit(0, softBody.NotMiddlePoints.Count - 1)].localPosition;
        }
    }
    private void FixedUpdateEdgeCollider()
    {
        triggerEdgeCollider2D.SetPoints(notMiddlePointsPositions.ToList());
    }
}
