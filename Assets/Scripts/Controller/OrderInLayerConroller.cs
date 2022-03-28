using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class OrderInLayerConroller : MonoBehaviour
{
    private SpriteRenderer spriteRenderer = null;
    private SpriteShapeRenderer spriteShapeRenderer = null;
    private readonly int offest = 20;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(spriteRenderer == null)
        {
            spriteShapeRenderer = GetComponent<SpriteShapeRenderer>();
        }
    }

    void Update()
    {
        SetOrderInLayer();
    }
    private void SetOrderInLayer()
    {
        if(spriteRenderer == null)
        {
            spriteShapeRenderer.sortingOrder = offest - (int)Mathf.Round(transform.position.y);

            return;
        }
        
        spriteRenderer.sortingOrder = offest - (int)Mathf.Round(transform.position.y);
    }
}
