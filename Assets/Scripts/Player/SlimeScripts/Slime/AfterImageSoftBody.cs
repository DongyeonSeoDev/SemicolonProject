using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AfterImageSoftBody : SoftBody
{
    private SpriteShapeRenderer spriteShapeRenderer = null;
    private Material[] materials = null;

    [Header("알파값은 적용 안됌")]
    [SerializeField]
    private Color afterImageColor = Color.white;

    [SerializeField]
    private float fadeOutSpeed = 1f;
    [SerializeField]
    private float startAlbedoPercentage = 50f;

    [Serializable]
    private struct AlbedoDatas
    {
        public float originAlbedo;
        public float startAlbedo;
        public float fadeOutTime;
        public float fadeOutTimer;
    }

    [SerializeField]
    private AlbedoDatas[] albedoDatas;

    public void Awake()
    {
        spriteShapeRenderer = spriteShapeController.GetComponent<SpriteShapeRenderer>();
        materials = spriteShapeRenderer.materials;
        albedoDatas = new AlbedoDatas[materials.Length];

        for (int i = 0; i < albedoDatas.Length; i++)
        {
            albedoDatas[i].originAlbedo = materials[i].color.a;
        }
    }
    void Update()
    {
        CheckTimer();
        FadeOut();
        UpdateVerticies();
    }
    public void OnSpawn(List<Transform> pPList)
    {
        for (int i = 0; i < points.Count; i++)
        {
            points[i].position = pPList[i].position;
        }

        for(int i = 0; i < materials.Length; i++)
        { 
            albedoDatas[i].startAlbedo = albedoDatas[i].originAlbedo * (startAlbedoPercentage / 100f);
            albedoDatas[i].fadeOutTime = albedoDatas[i].startAlbedo / fadeOutSpeed;
            albedoDatas[i].fadeOutTimer = albedoDatas[i].fadeOutTime;
        }
    }
    private void CheckTimer()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            if (albedoDatas[i].fadeOutTimer > 0f)
            {
                albedoDatas[i].fadeOutTimer -= Time.deltaTime;
            }

            if (albedoDatas[i].fadeOutTimer <= 0f)
            {
                albedoDatas[i].fadeOutTimer = 0f;

                SlimePoolManager.Instance.AddObject(gameObject);
                gameObject.SetActive(false);
            }
        }
    }
    private void FadeOut()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = new Vector4( afterImageColor.r,  afterImageColor.g,
                afterImageColor.b, Mathf.Lerp(0, albedoDatas[i].startAlbedo, albedoDatas[i].fadeOutTimer / albedoDatas[i].fadeOutTime));
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
