using UnityEngine;

[RequireComponent(typeof(SpriteMask))]
public class FakeSpriteOutline : MonoBehaviour
{
    private SpriteMask sprMsk;

    [SerializeField] private Sprite sprMaskSprite;
    [SerializeField] private float thickness = 1.05f;
    [SerializeField] private Color outlineColor;

    private SpriteRenderer fakeOutlineSpr;

    //[SerializeField] private SpriteMaskInteraction smi = SpriteMaskInteraction.VisibleInsideMask;

    private void Awake()
    {
        sprMsk = GetComponent<SpriteMask>();
        fakeOutlineSpr = transform.GetChild(0).GetComponent<SpriteRenderer>();

        transform.localScale = new Vector3(thickness, thickness, thickness);
        if (!sprMaskSprite) sprMaskSprite = transform.parent.GetComponent<SpriteRenderer>().sprite;
        sprMsk.sprite = sprMaskSprite;

        fakeOutlineSpr.color = outlineColor;

        //fakeOutlineSpr.maskInteraction = smi;
    }
}
