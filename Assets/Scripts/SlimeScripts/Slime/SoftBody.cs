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
    private SpriteShapeController spriteShapeController = null;
    [SerializeField]
    private List<Transform> points;
    [SerializeField]
    private List<Transform> notMiddlePoints = new List<Transform>();
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
        transform.GetComponentsInChildren<BodyPoint>().ForEach(x => points.Add(x.transform));

        foreach (Transform item in points)
        {
            if (item.GetComponent<MiddlePoint>() == null)
            {
                notMiddlePoints.Add(item);
            }

            Rigidbody2D itemRigid = item.GetComponent<Rigidbody2D>();

            SpringJoint2D springJoint2D = points[0].gameObject.AddComponent<SpringJoint2D>(); // points[0]은 항상 MiddlePoint
            springJoint2D.connectedBody = itemRigid;
            springJoint2D.frequency = 20f;
            
            DistanceJoint2D distanceJoint2D = points[0].gameObject.AddComponent<DistanceJoint2D>();
            distanceJoint2D.connectedBody = itemRigid;
            distanceJoint2D.maxDistanceOnly = true;
        }

        for (int i = 0; i < notMiddlePoints.Count; i++)
        {
            SpringJoint2D notMiddleSpringJoint2D = notMiddlePoints[i].gameObject.AddComponent<SpringJoint2D>();
            notMiddleSpringJoint2D.connectedBody = points[0].GetComponent<Rigidbody2D>();
            notMiddleSpringJoint2D.frequency = 5f;

            // DistanceJoint2D notMiddleDistanceJoint2D = null;
            // // notMiddlePoints[i].gameObject.AddComponent<DistanceJoint2D>();
            // // notMiddleDistanceJoint2D.connectedBody = points[0].GetComponent<Rigidbody2D>();
            // // notMiddleDistanceJoint2D.maxDistanceOnly = true;

            // // notMiddlePoints[i].gameObject.AddComponent<DistanceJoint2D>().connectedBody = points[0].GetComponent<Rigidbody2D>();

            // notMiddleDistanceJoint2D = notMiddlePoints[i].gameObject.AddComponent<DistanceJoint2D>();
            // notMiddleDistanceJoint2D.connectedBody = notMiddlePoints[(i - 1).Limit(0, notMiddlePoints.Count - 1)].GetComponent<Rigidbody2D>();
            // notMiddleDistanceJoint2D.maxDistanceOnly = true;

            // notMiddleDistanceJoint2D = notMiddlePoints[i].gameObject.AddComponent<DistanceJoint2D>();
            // notMiddleDistanceJoint2D.connectedBody = notMiddlePoints[(i + 1).Limit(0, notMiddlePoints.Count - 1)].GetComponent<Rigidbody2D>();
            // notMiddleDistanceJoint2D.maxDistanceOnly = true;

            SpringJoint2D upNotMiddleSpringJoint2D = notMiddlePoints[i].gameObject.AddComponent<SpringJoint2D>();
            upNotMiddleSpringJoint2D.connectedBody = notMiddlePoints[(i - 1).Limit(0, notMiddlePoints.Count - 1)].GetComponent<Rigidbody2D>();
            upNotMiddleSpringJoint2D.frequency = 1f;

            SpringJoint2D downNotMiddleSpringJoint2D = notMiddlePoints[i].gameObject.AddComponent<SpringJoint2D>();
            downNotMiddleSpringJoint2D.connectedBody = notMiddlePoints[(i + 1).Limit(0, notMiddlePoints.Count - 1)].GetComponent<Rigidbody2D>();
            downNotMiddleSpringJoint2D.frequency = 1f;
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
        for (int i = 0; i < notMiddlePoints.Count; i++)
        {
            Vector2 _vertex = notMiddlePoints[i].localPosition;

            Vector2 _towardsCenter = (-_vertex).normalized;

            float _colliderRadius = notMiddlePoints[i].gameObject.GetComponent<CircleCollider2D>().radius;

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
