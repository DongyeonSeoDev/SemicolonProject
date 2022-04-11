using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AfterImageSoftBody : SoftBody
{
    private SpriteShapeRenderer spriteShapeRenderer = null;
    private Material[] materials = null;

    private float fadeOutSpeed = 1f;

    private float fadeOutTime = 0f;
    private float fadeOutTimer = 0f;

    public void Awake()
    {
        spriteShapeRenderer = spriteShapeController.GetComponent<SpriteShapeRenderer>();
        materials = spriteShapeRenderer.materials;
    }

    void Update()
    {
        CheckTimer();
        FadeOut();
        UpdateVerticies();
    }
    public void OnSpawn(List<Transform> pPList, float fOSpeed)
    {
        fadeOutSpeed = fOSpeed;

        for (int i = 0; i < points.Count; i++)
        {
            points[i].position = pPList[i].position;
        }

        fadeOutTime = materials[0].color.a / fadeOutSpeed;
        fadeOutTimer = fadeOutTime;
    }
    private void CheckTimer()
    {
        if(fadeOutTimer > 0f)
        {
            fadeOutTimer -= Time.deltaTime;

            if(fadeOutTimer <= 0f)
            {
                fadeOutTimer = 0f;
                
                SlimePoolManager.Instance.AddObject(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
    private void FadeOut()
    {
        foreach(var item in materials)
        {
            item.color = new Vector4(item.color.r, item.color.g, item.color.b,
                Mathf.Lerp(0, item.color.a, fadeOutTimer / fadeOutTime));
        }
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

            Vector2 _lt = spriteShapeController.spline.GetLeftTangent(i);

            Vector2 _newRt = Vector2.Perpendicular(_towardsCenter) * _lt.magnitude;
            Vector2 _newLt = -_newRt;

            spriteShapeController.spline.SetRightTangent(i, _newRt);
            spriteShapeController.spline.SetLeftTangent(i, _newLt);
        }
    }
}
