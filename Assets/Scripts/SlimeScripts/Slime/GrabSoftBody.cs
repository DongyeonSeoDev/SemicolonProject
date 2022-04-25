using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSoftBody : SoftBody
{
    public GameObject targetObj = null;

    private Vector3 offset = Vector3.zero;

    [SerializeField]
    private LayerMask rayCheckingLayer;

    [SerializeField]
    private float shootRayOffest = 10f;

    private int originLayer = 0;

    private Vector2 upestLocalPos = Vector2.zero;
    private Vector2 downestLocalPos = Vector2.zero;
    private Vector2 leftestLocalPos = Vector2.zero;
    private Vector2 rightestLocalPos = Vector2.zero;

    private float gapAboutLefstestToRightest = 0f; // x
    private float gapAboutUpestToDownest = 0f; // y

    private bool setRay = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (targetObj == null || !targetObj.activeSelf)
        {
            SlimePoolManager.Instance.AddObject(gameObject);

            gameObject.SetActive(false);

            return;
        }

        if (!setRay)
        {
            setRay = true;
            originLayer = targetObj.layer;
            targetObj.layer = LayerMask.NameToLayer("RAYCHECKING"); // 다른 오브젝트가 Ray체크에 영향을 주면 안됌!

            upestLocalPos = ShootRay(targetObj.transform.position + new Vector3(0f, shootRayOffest, 0f), Vector2.down) - (Vector2)targetObj.transform.position;
            upestLocalPos *= 2f;

            downestLocalPos = ShootRay(targetObj.transform.position + new Vector3(0f, -shootRayOffest, 0f), Vector2.up) - (Vector2)targetObj.transform.position;
            downestLocalPos *= 2f;

            leftestLocalPos = ShootRay(targetObj.transform.position + new Vector3(-shootRayOffest, 0f, 0f), Vector2.right) - (Vector2)targetObj.transform.position;
            leftestLocalPos *= 2f;

            rightestLocalPos = ShootRay(targetObj.transform.position + new Vector3(shootRayOffest, 0f, 0f), Vector2.left) - (Vector2)targetObj.transform.position;
            rightestLocalPos *= 2f;

            targetObj.layer = originLayer;

            gapAboutLefstestToRightest = rightestLocalPos.x - leftestLocalPos.x;
            gapAboutUpestToDownest = upestLocalPos.y - downestLocalPos.y;

            UpdatePointPos();
        }

        UpdateVerticies();

        transform.position = targetObj.transform.position;
    }
    public void OnSpawn(GameObject tObj, Vector2 offS)
    {
        setRay = false;

        targetObj = tObj;
        offset = offS;
    }
    private Vector2 ShootRay(Vector2 startPos, Vector2 direction)
    {
        direction = direction.normalized;

        Ray2D ray = new Ray2D();
        RaycastHit2D hit;

        ray.origin = startPos;
        ray.direction = direction;

        Debug.DrawRay(startPos, direction * Vector2.Distance(startPos, targetObj.transform.position), Color.red, 10f);
        hit = Physics2D.Raycast(ray.origin, ray.direction, Vector2.Distance(startPos, targetObj.transform.position), rayCheckingLayer);

        return hit.point;
    }

    private void UpdatePointPos()
    {
        if(points.Count < 8)
        {
            Debug.LogError(gameObject.name + "의 Point 개수가 모자랍니다.");

            return;
        }

        points[0].transform.localPosition = new Vector2(leftestLocalPos.x, downestLocalPos.y);
        points[1].transform.localPosition = new Vector2(leftestLocalPos.x + gapAboutLefstestToRightest / 12f,
            downestLocalPos.y + gapAboutUpestToDownest / 4f);
        points[2].transform.localPosition = new Vector2(leftestLocalPos.x + gapAboutLefstestToRightest / 8f,
            downestLocalPos.y + gapAboutUpestToDownest / 2f);
        points[3].transform.localPosition = new Vector2(leftestLocalPos.x + gapAboutLefstestToRightest / 3f,
            downestLocalPos.y + gapAboutUpestToDownest / 3f);

        points[4].transform.localPosition = new Vector2(rightestLocalPos.x - gapAboutLefstestToRightest / 3f,
            downestLocalPos.y + gapAboutUpestToDownest / 3f);
        points[5].transform.localPosition = new Vector2(rightestLocalPos.x - gapAboutLefstestToRightest / 8f,
            downestLocalPos.y + gapAboutUpestToDownest / 2f);
        points[6].transform.localPosition = new Vector2(rightestLocalPos.x - gapAboutLefstestToRightest / 12f,
            downestLocalPos.y + gapAboutUpestToDownest / 4f);
        points[7].transform.localPosition = new Vector2(rightestLocalPos.x, downestLocalPos.y);
    }

    public override void UpdateVerticies()
    {
        base.UpdateVerticies();

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 _vertex = points[i].transform.localPosition;

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
        }
    }
    private Vector2 GetTangentVec(int idx, bool isPaste)
    {
        return (isPaste ? spriteShapeController.spline.GetPosition((idx - 1).Limit(0, spriteShapeController.spline.GetPointCount() - 1))
            : spriteShapeController.spline.GetPosition((idx + 1).Limit(0, spriteShapeController.spline.GetPointCount() - 1)))
            - spriteShapeController.spline.GetPosition(idx);
    }
}
