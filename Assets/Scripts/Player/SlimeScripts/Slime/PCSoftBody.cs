using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PCSoftBody : SoftBody
{
    #region AfterImageSoftBody관련 변수들
    [SerializeField]
    private GameObject afterImageSoftBody = null;
    #endregion

    #region Fields
    [SerializeField]
    private List<Transform> notMiddlePoints = new List<Transform>();
    public List<Transform> NotMiddlePoints
    {
        get { return notMiddlePoints; }
    }

    private List<BodyPoint> upNotMiddlePoints = new List<BodyPoint>();
    public List<BodyPoint> UpNotMiddlePoints
    {
        get { return upNotMiddlePoints; }
    }
    private List<BodyPoint> downNotMiddlePoints = new List<BodyPoint>();
    public List<BodyPoint> DownNotMiddlePoints
    {
        get { return downNotMiddlePoints; }
    }

    private BodyPoint leftestDownNotMiddlePoint = null;
    private float leftestDownNotMiddlePointDistance = 0f;

    private BodyPoint rightestDownNotMiddlePoint = null;
    private float rightestDownNotMiddlePointDistance = 0f;


    [Header("MiddlePoint와 다른 Point들 사이의 SpringJoint의 Frequency값")]
    [SerializeField]
    private float middleToOtherSpringJointFrequency = 10f;
    [Header("MiddlePoint를 제외한 Point들간의 SpringJoint의 Frequency값")]
    private float pointToPointSpringJointFrequency = 1f;
    #endregion

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        //UpdateVerticies();
        SetPoints();

        for (int i = 0; i < notMiddlePoints.Count; i++)
        {
            CheckPointType(i);
            SetJoints(i);
        }
    }
    private void OnEnable()
    {
        EventManager.StartListening("SpawnAfterImageSoftBody", SpawnAfterImageSoftBody);
    }
    private void OnDisable()
    {
        EventManager.StopListening("SpawnAfterImageSoftBody", SpawnAfterImageSoftBody);
    }
    private void SpawnAfterImageSoftBody()
    {
        AfterImageSoftBody softBody = null;
        GameObject obj = null;

        bool found = false;

        (obj, found) = SlimePoolManager.Instance.Find(afterImageSoftBody);

        if (found)
        {
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(afterImageSoftBody, SlimePoolManager.Instance.transform);
        }

        obj.transform.position = transform.position;
        softBody = obj.GetComponent<AfterImageSoftBody>();
        softBody.OnSpawn(notMiddlePoints);
    }

    private void SetPoints()
    {
        points.Clear();
        transform.GetComponentsInChildren<BodyPoint>().ForEach(x => points.Add(x.transform));

        MiddlePoint middlePoint = null;
        foreach (Transform item in points)
        {
            middlePoint = item.GetComponent<MiddlePoint>();
            if (middlePoint == null)
            {
                notMiddlePoints.Add(item);

                Rigidbody2D itemRigid = item.GetComponent<Rigidbody2D>();

                DistanceJoint2D distanceJoint2D = points[0].gameObject.AddComponent<DistanceJoint2D>();
                distanceJoint2D.connectedBody = itemRigid;
                distanceJoint2D.maxDistanceOnly = true;
            }
            else
            {
                middlePoint.SoftBody = this;
            }
        }
    }

    private void CheckPointType(int i)
    {
        BodyPoint bodyPoint = notMiddlePoints[i].GetComponent<BodyPoint>();

        Transform bodyPointTrm = notMiddlePoints[i];

        if (bodyPointTrm.position.y >= transform.position.y)
        {
            upNotMiddlePoints.Add(bodyPoint);
        }
        else
        {
            float distance = (bodyPointTrm.position.x - transform.position.x).Abs();

            if (bodyPointTrm.position.x >= transform.position.x)
            {
                if (rightestDownNotMiddlePointDistance < distance)
                {
                    if (rightestDownNotMiddlePoint != null)
                    {
                        rightestDownNotMiddlePoint.SetTrueisDownBodyPoint();
                        downNotMiddlePoints.Add(rightestDownNotMiddlePoint);
                    }

                    rightestDownNotMiddlePoint = bodyPoint;
                    rightestDownNotMiddlePointDistance = distance;
                }
                else
                {
                    bodyPoint.SetTrueisDownBodyPoint();
                    downNotMiddlePoints.Add(bodyPoint);
                }
            }
            else
            {
                if (leftestDownNotMiddlePointDistance < distance)
                {
                    if (leftestDownNotMiddlePoint != null)
                    {
                        leftestDownNotMiddlePoint.SetTrueisDownBodyPoint();
                        downNotMiddlePoints.Add(leftestDownNotMiddlePoint);
                    }

                    leftestDownNotMiddlePoint = bodyPoint;
                    leftestDownNotMiddlePointDistance = distance;
                }
                else
                {
                    bodyPoint.SetTrueisDownBodyPoint();
                    downNotMiddlePoints.Add(bodyPoint);
                }
            }
        }
    }

    private void SetJoints(int i)
    {
        //int upNotMiddleBodyPointNum = (i - 1).Limit(0, notMiddlePoints.Count - 1);
        int downNotMiddleBodyPointnum = (i + 1).Limit(0, notMiddlePoints.Count - 1);

        //Transform upPoint = notMiddlePoints[upNotMiddleBodyPointNum];
        Transform downPoint = notMiddlePoints[downNotMiddleBodyPointnum];

        RelativeJoint2D notMiddleRelativeJoint2D = null;
        notMiddleRelativeJoint2D = notMiddlePoints[i].gameObject.AddComponent<RelativeJoint2D>();
        notMiddleRelativeJoint2D.connectedBody = downPoint.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UpdateVerticies();
    }
    #endregion

    #region Private Methods
    public override void UpdateVerticies()
    {
        base.UpdateVerticies();

        for (int i = 0; i < notMiddlePoints.Count; i++)
        {
            Vector2 _vertex = notMiddlePoints[i].transform.localPosition;

            Vector2 _towardsCenter = (-_vertex).normalized;

            try
            {
                spriteShapeController.spline.SetPosition(i, (_vertex - _towardsCenter * radius));
            }
            catch
            {
                Debug.Log("Spline Points들이 서로 너무 가깝습니다.. recalculate");

                spriteShapeController.spline.SetPosition(i, (_vertex - _towardsCenter * (radius + splineOffset)));
            }

            spriteShapeController.spline.SetLeftTangent(i, GetTangentVec(i, true) / 4f);
            spriteShapeController.spline.SetRightTangent(i, -GetTangentVec(i, true) / 4f);

            spriteShapeController.spline.SetTangentMode(i, UnityEngine.U2D.ShapeTangentMode.Continuous);

            //Vector2 _lt = spriteShapeController.spline.GetLeftTangent(i);

            //Vector2 _newRt = Vector2.Perpendicular(_towardsCenter) * _lt.magnitude;
            //Vector2 _newLt = -_newRt;

            ////spriteShapeController.spline.SetRightTangent(i, _newRt);
            //spriteShapeController.spline.SetLeftTangent(i, _newLt);
        }
    }
    private Vector2 GetTangentVec(int idx, bool isPaste)
    {
        return (isPaste ? spriteShapeController.spline.GetPosition((idx - 1).Limit(0, spriteShapeController.spline.GetPointCount() - 1))
            : spriteShapeController.spline.GetPosition((idx + 1).Limit(0, spriteShapeController.spline.GetPointCount() - 1))) 
            - spriteShapeController.spline.GetPosition(idx); 
    }
    //private void CheckPasteNext(int idx)
    //{
    //    Vector2 curPos = spriteShapeController.spline.GetPosition(idx);
    //    Vector2 pastePos = spriteShapeController.spline.GetPosition((idx - 1).Limit(0, spriteShapeController.spline.GetPointCount() - 1));
    //    Vector2 nextPos = spriteShapeController.spline.GetPosition((idx + 1).Limit(0, spriteShapeController.spline.GetPointCount() + 1));

    //    Vector2 cur_paste_gap = curPos - pastePos;
    //    Vector2 cur_next_gap = curPos - nextPos;

    //    if(cur_paste_gap.x < 0.1f)
    //    {

    //    }
    //}
    #endregion
}
