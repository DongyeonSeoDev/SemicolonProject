using UnityEngine;

public class DissolveCtrl : ShaderCtrl
{
    [SerializeField] private float speed = 240f;

    public override void AdditionalInitSet()
    {
        newMat.SetFloat("_Scale", speed);
    }

    public void SetFade(float fade)
    {
        newMat.SetFloat("_Fade", fade);
    }
}
