using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddlePoint : BodyPoint
{
    [SerializeField]
    private EdgeCollider2D triggerEdgeCollider2D = null;

    private Vector2[] notMiddlePointsPositions;

    private SoftBody softBody = null;
    public SoftBody SoftBody
    {
        get { return softBody; }
        set { softBody = value; }
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
