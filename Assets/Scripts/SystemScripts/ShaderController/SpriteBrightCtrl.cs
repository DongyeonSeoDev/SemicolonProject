using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBrightCtrl : ShaderCtrl
{
    [ColorUsage(true, true)] public Color hdrc;
    public float intensity = 0.5f;

    private void Awake()
    {
        matName = "SpriteBrightMat";
    }

    public override void AdditionalInitSet()
    {
        //스타트에서 자동으로 InitSet 함수 호출하는데 여기서 바로 이렇게 하면 StackOverflow일어남. 저 함수 내부에서도 InitSet을 호출해서
        //SetIntensity(intensity);
        //SetColor(hdrc);

        newMat.SetColor("_Color", hdrc);
        newMat.SetFloat("_Intensity", intensity);
    }
}
