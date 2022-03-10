using UnityEngine;

public class MulSpriteColorCtrl : ShaderCtrl
{
    [ColorUsage(true, true)]
    public Color hdrColor;
    public float speed = 0.2f;
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

    public void SetColor(Color hdrC)
    {
        InitSet();
        newMat.SetColor("_Color", hdrC);
    }

    public void SetSpeed(float speed)
    {
        InitSet();
        newMat.SetFloat("_Speed", speed);
    }

    public void SetScale(float scale)
    {
        InitSet();
        newMat.SetFloat("_Scale", scale);
    }
}
