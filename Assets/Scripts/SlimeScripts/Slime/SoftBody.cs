using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class SoftBody : MonoBehaviour
{
    #region Constants
    private const float splineOffset = 0.5f;
    #endregion
    #region Fields
    [SerializeField]
    public SpriteShapeController spriteShapeController = null;
    [SerializeField]
    public List<Transform> points;
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        UpdateVerticies();
    }
    private void Start()
    {
        Debug.Log((0).Limit(11, 14));
        Debug.Log((15).Limit(11, 14));
        
        points.Clear();
        transform.Find("BodyPoints").GetComponentsInChildren<BodyPoint>().ForEach(x => points.Add(x.transform));

        foreach (Transform item in points)
        {
            if (item.GetComponent<MiddlePoint>() != null)
            {
                continue;
            }

            points[0].gameObject.AddComponent<SpringJoint2D>().connectedBody = item.GetComponent<Rigidbody2D>(); // points[0]은 항상 MiddlePoint
        }

        List<Transform> notMiddlePoints = new List<Transform>();

        for (int i = 1; i < points.Count; i++)
        {
            notMiddlePoints.Add(points[i]);
        }

        for (int i = 0; i < notMiddlePoints.Count; i++)
        {
            notMiddlePoints[i].gameObject.AddComponent<SpringJoint2D>().connectedBody = points[0].GetComponent<Rigidbody2D>();
            notMiddlePoints[i].gameObject.AddComponent<SpringJoint2D>().connectedBody = notMiddlePoints[(i - 1).Limit(0, notMiddlePoints.Count - 1)].GetComponent<Rigidbody2D>();
            notMiddlePoints[i].gameObject.AddComponent<SpringJoint2D>().connectedBody = notMiddlePoints[(i + 1).Limit(0, notMiddlePoints.Count - 1)].GetComponent<Rigidbody2D>();
        }

    }
    private void Update()
    {
        UpdateVerticies();
    }
    #endregion

    #region Private Methods
    private void UpdateVerticies()
    {
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector2 _vertex = points[i].localPosition;

            Vector2 _towardsCenter = (-_vertex).normalized;

            float _colliderRadius = points[i].gameObject.GetComponent<CircleCollider2D>().radius;

            try
            {
                spriteShapeController.spline.SetPosition(i, (_vertex - _towardsCenter * _colliderRadius));
            }
            catch
            {
                Debug.Log("Spline Points들이 서로 너무 가깝습니다.. recalculate");

                spriteShapeController.spline.SetPosition(i, (_vertex - _towardsCenter * (_colliderRadius + splineOffset)));
            }

            Vector2 _lt = spriteShapeController.spline.GetLeftTangent(i);

            Vector2 _newRt = Vector2.Perpendicular(_towardsCenter) * _lt.magnitude;
            Vector2 _newLt = -_newRt;

            spriteShapeController.spline.SetRightTangent(i, _newRt);
            spriteShapeController.spline.SetLeftTangent(i, _newLt);
        }
    }
    #endregion
}
