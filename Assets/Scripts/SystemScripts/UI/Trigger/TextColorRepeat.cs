using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TextColorRepeat : UITransition
{
    private Text txt;
    private Sequence seq;
    private Color initColor;

    public Triple<int, float, float> loop_transition_wait = new Triple<int, float, float>(-1,1f,0.5f);
    public Color[] transitionColors;

    protected override void Awake()
    {
        txt = GetComponent<Text>();
        initColor = txt.color;
        seq = DOTween.Sequence();

        seq.SetLoops(loop_transition_wait.first, LoopType.Restart);
        for (int i=0; i<transitionColors.Length; i++)
        {
            seq.Append(txt.DOColor(transitionColors[i], loop_transition_wait.second));
            seq.AppendInterval(loop_transition_wait.third);
            seq.Append(txt.DOColor(initColor, loop_transition_wait.second));
            seq.AppendInterval(loop_transition_wait.third);
        }
    }

    public override void Transition(bool on)
    {
        if (on) seq.Play();
        else seq.Pause();
    }
}
