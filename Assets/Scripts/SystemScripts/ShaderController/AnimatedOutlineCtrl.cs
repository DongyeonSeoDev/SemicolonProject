using UnityEngine;

public class AnimatedOutlineCtrl : ShaderCtrl  
{
    [Header("Start Default Values")]
    [ColorUsage(true, true)]
    [SerializeField] private Color outlineHDRColor;
    [SerializeField] private float outlineThickness = 3f;
    [SerializeField] private float outlineIntensity = 1f;
    [ColorUsage(true, true)]
    [SerializeField] private Color outlineHDRColor2;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float scale = 8f;


    private void Awake()
    {
        matName = "Outline2DAnimatedMat";
    }

    public override void AdditionalInitSet()
    {
        newMat.SetColor("_OutlineColor", outlineHDRColor);
        newMat.SetFloat("_OutlineThickness", outlineThickness);
        newMat.SetFloat("_Intensity", outlineIntensity);
        newMat.SetColor("_OutlineColor2", outlineHDRColor2);
        newMat.SetFloat("_Speed", speed);
        newMat.SetFloat("_NoiseScale", scale);
    }

    public void SetOutline(Color oColor, float thick, float intensity)
    {
        InitSet();

        newMat.SetColor("_OutlineColor", oColor);
        newMat.SetFloat("_OutlineThickness", thick);
        newMat.SetFloat("_Intensity", intensity);
    }

    public void SetOutlineColor(Color hdrC)
    {
        InitSet();
        newMat.SetColor("_OutlineColor", hdrC);
    }

    public void SetOutlineThick(float thick)
    {
        InitSet();
        newMat.SetFloat("_OutlineThickness", thick);
    }

    public void SetOutlineIntensity(float intensity)
    {
        base.SetIntensity(intensity);
    }

    public void SetOutlineColor2(Color c)
    {
        InitSet();
        newMat.SetColor("_OutlineColor2", c);
    }
}
