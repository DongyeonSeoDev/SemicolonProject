using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAvoidCloseCheckCollider : AvoidCloseCheckCollider
{
    private BoxCollider2D boxCol2D = null;
    private BoxCollider2D parentBoxCol2D = null;

    private void Awake()
    {
        boxCol2D = GetComponent<BoxCollider2D>();
        parentBoxCol2D = transform.parent.GetComponent<BoxCollider2D>();

        SetVertex();
    }
    public void SetVertex()
    {
        boxCol2D.offset = parentBoxCol2D.offset;
        boxCol2D.size = parentBoxCol2D.size;

        boxCol2D.size += new Vector2(distanceToMiddle, distanceToMiddle);
    }
}
