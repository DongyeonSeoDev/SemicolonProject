using UnityEngine;
using UnityEngine.UI;

public class MobSpeciesIcon : MonoBehaviour
{
    private RectTransform rectTr;

    public Image speciesImg;
    public RectTransform targetRectTrm;

    public void Set(RectTransform rect, EnemySpecies type)
    {
        if(!rectTr) rectTr = GetComponent<RectTransform>();

        targetRectTrm = rect;
        speciesImg.sprite = type.ToEnemySpeciesSprite();
    }

    private void LateUpdate()
    {
        if(targetRectTrm)
        {
            rectTr.anchoredPosition = targetRectTrm.anchoredPosition + new Vector2(-(targetRectTrm.rect.width * 0.5f) - 5f, 0);
        }
    }

    private void OnDisable()
    {
        targetRectTrm = null;
    }
}
