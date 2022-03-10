using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddlePoint : BodyPoint
{
    [SerializeField]
    private EdgeCollider2D edgeCollider2D = null;

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

        for(int i = 0; i < softBody.NotMiddlePoints.Count; i++)
        {
            notMiddlePointsPositions[i] = softBody.NotMiddlePoints[i].localPosition;
        }

        notMiddlePointsPositions[notMiddlePointsPositions.Length - 1] = notMiddlePointsPositions[0];
    }
    private void FixedUpdateEdgeCollider()
    {
        edgeCollider2D.SetPoints(notMiddlePointsPositions.ToList());
    }
}
