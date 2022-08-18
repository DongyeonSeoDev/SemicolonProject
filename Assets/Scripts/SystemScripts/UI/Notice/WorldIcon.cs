using UnityEngine;
using UnityEngine.UI;
using Enemy;

public class WorldIcon : MonoBehaviour
{
    private RectTransform rectTr;

    public Image speciesImg;
    public RectTransform targetRectTrm;

    public Vector2 offset;

    public void Set(RectTransform rect, EnemyType type)
    {
        if(!rectTr) rectTr = GetComponent<RectTransform>();

        targetRectTrm = rect;
        speciesImg.sprite = MonsterCollection.Instance.GetMonsterInfo(type.ToString()).bodyImg;
    }

    private void LateUpdate()
    {
        if(targetRectTrm)
        {
            rectTr.anchoredPosition = targetRectTrm.anchoredPosition + new Vector2(-(targetRectTrm.rect.width * 0.5f), 0) + offset;  //상호작용키 바꾸면 상호작용 텍스트 길이가 달라질 수 있어서 new
        }
    }

    private void OnDisable()
    {
        targetRectTrm = null;
    }
}
