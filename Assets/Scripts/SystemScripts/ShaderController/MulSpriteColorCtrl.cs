using UnityEngine;

public class MulSpriteColorCtrl : ShaderCtrl
{
    [ColorUsage(true, true)]
    public Color hdrColor;
    [Range(-50f, 50f)]
    public float speed = 0.2f;
    [Range(0f,100f)]
    public float scale = 5f;
    public float intensity = 1f;

    private void Awake()
    {
        matName = "MulSpriteColorMat";
    }
    

    public override void AdditionalInitSet()
    {
        newMat.SetColor("_Color", hdrColor);
        newMat.SetFloat("_Speed", speed);
        newMat.SetFloat("_Intensity", intensity);
        newMat.SetFloat("_Scale", scale);
    }

    public void SetSpeed(float speed)
    {
        base.InitSet();
        newMat.SetFloat("_Speed", speed);
    }

    public void SetScale(float scale)
    {
        base.InitSet();
        newMat.SetFloat("_Scale", scale);
    }
}
