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
        //��ŸƮ���� �ڵ����� InitSet �Լ� ȣ���ϴµ� ���⼭ �ٷ� �̷��� �ϸ� StackOverflow�Ͼ. �� �Լ� ���ο����� InitSet�� ȣ���ؼ�
        //SetIntensity(intensity);
        //SetColor(hdrc);

        newMat.SetColor("_Color", hdrc);
        newMat.SetFloat("_Intensity", intensity);
    }
}
