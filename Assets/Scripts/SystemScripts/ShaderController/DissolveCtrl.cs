using UnityEngine;

public class DissolveCtrl : ShaderCtrl
{
    [SerializeField] private float speed = 240f;

    private void Awake()
    {
        matName = "DissolveMat";
    }

    public override void AdditionalInitSet()
    {
        newMat.SetFloat("_Scale", speed);
    }

    public void SetFade(float fade)
    {
        base.InitSet();
        newMat.SetFloat("_Fade", fade);
    }
}
