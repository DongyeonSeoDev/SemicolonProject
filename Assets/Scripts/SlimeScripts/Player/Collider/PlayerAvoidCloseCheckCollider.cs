using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAvoidCloseCheckCollider : MonoBehaviour
{
    private MiddlePoint middlePoint = null;

    [SerializeField]
    private LayerMask checkLayer;

    [SerializeField]
    private float distanceToMiddle = 2f;

    private EdgeCollider2D edgeCollider2D = null;
    private List<Vector2> pointList = new List<Vector2> ();

    void Awake()
    {
        edgeCollider2D = GetComponent<EdgeCollider2D>();

        pointList.Clear();
        edgeCollider2D.SetPoints(pointList);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("¿¿æ÷");
    }
    public void SetMiddlePoint(MiddlePoint middlePoint)
    {
        this.middlePoint = middlePoint;
    }
    public void InsertEdgePoint(Vector2 pos)
    {
        pointList.Add(pos + pos.normalized * distanceToMiddle);

        edgeCollider2D.SetPoints(pointList);
    }
    public void SetEdgePoints(Vector2[] poses)
    {
        pointList = new List<Vector2>();

        foreach (var item in poses)
        {
            pointList.Add(item + item.normalized * distanceToMiddle); 
        }

        edgeCollider2D.SetPoints(pointList);
    }
    public void SetEdgePoints(List<Vector2> poses)
    {
        pointList = new List<Vector2>();

        foreach (var item in poses)
        {
            pointList.Add(item + item.normalized * distanceToMiddle);
        }

        edgeCollider2D.SetPoints(pointList);
    }
}
