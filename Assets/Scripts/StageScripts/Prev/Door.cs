using UnityEngine;
using DG.Tweening;

public class Door : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.DOColor(Color.clear, 0f);
        transform.DOMoveY(1f, 0f).SetRelative();

        gameObject.SetActive(false);
    }

    public void Open()
    {
        sr.DOColor(Color.clear, 1f);
        transform.DOMoveY(1f, 1f).SetRelative().OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    public void Close()
    {
        gameObject.SetActive(true);

        sr.DOColor(Color.white, 1f);
        transform.DOMoveY(-1f, 1f).SetRelative();
    }
}
