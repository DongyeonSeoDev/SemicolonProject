using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class OrderInLayerController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer = null;
    private SpriteShapeRenderer spriteShapeRenderer = null;

    private string originSortingLayerName = "";
    private readonly int offest = 20;
    private bool setOrderInLayerAuto = true;

    void Start()
    {
        SetRenderer();

        originSortingLayerName = spriteRenderer != null ?
            spriteRenderer.sortingLayerName : spriteShapeRenderer.sortingLayerName;
    }

    private void SetRenderer()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        }
    }

    void Update()
    {
        if (setOrderInLayerAuto)
        {
            SetOrderInLayerAuto();
        }
    }
    private void SetOrderInLayerAuto()
    {
        if (spriteRenderer == null)
        {
            spriteShapeRenderer.sortingOrder = offest - (int)Mathf.Round(transform.position.y * 2);

            return;
        }
        
        spriteRenderer.sortingOrder = offest - (int)Mathf.Round(transform.position.y * 2);
    }
    public void SetOrderInLayer(string sortingLayerName, int orderInLayer)
    {
        setOrderInLayerAuto = false;

        if (spriteRenderer == null)
        {
            try
            {
                spriteShapeRenderer.sortingLayerName = sortingLayerName;
                spriteShapeRenderer.sortingOrder = orderInLayer;
            }
            catch
            {
                SetRenderer();
            }

            return;
        }

        spriteRenderer.sortingLayerName = sortingLayerName;
        spriteRenderer.sortingOrder = orderInLayer;
    }
    public void StartSetOrderInLayerAuto()
    {
        setOrderInLayerAuto = true;

        if (spriteRenderer == null)
        {
            try
            {
                spriteShapeRenderer.sortingLayerName = originSortingLayerName;
            }
            catch
            {
                SetRenderer();
            }

            return;
        }

        spriteRenderer.sortingLayerName = originSortingLayerName;
    }
}
