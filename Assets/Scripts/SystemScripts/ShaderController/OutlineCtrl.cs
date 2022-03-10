using UnityEngine;

public class OutlineCtrl : ShaderCtrl  //아웃라인 적용될 스프라이트 옵젝에 이거 달아야 함(아웃라인 메테리얼이 직접 달려있는 것에다가)
{
    [Header("Start Default Values")]
    [ColorUsage(true, true)]
    [SerializeField] private Color outlineHDRColor;
    [SerializeField] private float outlineThickness = 3f;
    [SerializeField] private float outlineIntensity = 1f;

    private void Awake()
    {
        matName = "Outline2DMat";
    }

    public override void AdditionalInitSet()
    {
        newMat.SetColor("_OutlineColor", outlineHDRColor);
        newMat.SetFloat("_OutlineThickness", outlineThickness);
        newMat.SetFloat("_Intensity", outlineIntensity);
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
}
