using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteRendererColorTweening : MonoBehaviour
{
    private SpriteRenderer spr;
    private Sequence seq;

    public Color[] transitionColors;
    public Triple<float, float,int> transition_wait_loop = new Triple<float, float,int>(1f,0.3f,-1);
    public Ease _ease;
    public LoopType loopType = LoopType.Restart;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        seq = DOTween.Sequence();

        for(int i=0; i<transitionColors.Length; i++)
        {
            seq.Append(spr.DOColor(transitionColors[i], transition_wait_loop.first).SetEase(_ease));
            seq.AppendInterval(transition_wait_loop.second);
        }
        seq.Append(spr.DOColor(transitionColors[0], transition_wait_loop.first * 0.4f));
        seq.SetLoops(transition_wait_loop.third, loopType);
    }

    private void OnEnable()
    {
        seq.Play();
    }

    private void OnDisable()
    {
        seq.Pause();
    }
}
