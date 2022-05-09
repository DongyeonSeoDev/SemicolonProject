using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvoidCloseCheckCollider : MonoBehaviour
{
    private EdgeCollider2D edgeCollider2D = null;
    private List<Vector2> pointList = new List<Vector2> ();

    void Start()
    {
        edgeCollider2D = GetComponent<EdgeCollider2D>();
    }

    void Update()
    {
        
    }
    public void InsertEdgePoint(Vector2 pos)
    {
        pointList.Add(pos);

        edgeCollider2D.SetPoints(pointList);
    }
}
