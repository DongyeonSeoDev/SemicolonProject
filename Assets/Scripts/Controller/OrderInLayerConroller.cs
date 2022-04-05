using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class OrderInLayerConroller : MonoBehaviour
{
    private SpriteRenderer spriteRenderer = null;
    private SpriteShapeRenderer spriteShapeRenderer = null;

    private string originSortingLayerName = "";
    private readonly int offest = 20;
    private bool setOrderInLayerAuto = true;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(spriteRenderer == null)
        {
            spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        }

        originSortingLayerName = spriteRenderer != null ? 
            spriteRenderer.sortingLayerName : spriteShapeRenderer.sortingLayerName;
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
        if(spriteRenderer == null)
        {
            spriteShapeRenderer.sortingOrder = offest - (int)Mathf.Round(transform.position.y);

            return;
        }
        
        spriteRenderer.sortingOrder = offest - (int)Mathf.Round(transform.position.y);
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
            spriteShapeRenderer.sortingLayerName = originSortingLayerName;

            return;
        }

        spriteRenderer.sortingLayerName = originSortingLayerName;
    }
}
