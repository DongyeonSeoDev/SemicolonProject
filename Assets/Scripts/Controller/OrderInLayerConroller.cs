using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class OrderInLayerConroller : MonoBehaviour
{
    private SpriteRenderer spriteRenderer = null;
    private readonly int offest = 20;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        SetOrderInLayer();
    }
    private void SetOrderInLayer()
    {
        spriteRenderer.sortingOrder = offest - (int)Mathf.Round(transform.position.y);
    }
}
