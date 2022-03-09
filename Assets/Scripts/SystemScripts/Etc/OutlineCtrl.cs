using UnityEngine;

public class OutlineCtrl : MonoBehaviour  //�ƿ����� ����� ��������Ʈ ������ �̰� �޾ƾ� ��(�ƿ����� ���׸����� ���� �޷��ִ� �Ϳ��ٰ�)
{
    private bool changedMat = false;
    private Material newMat;

    [SerializeField] private bool bStartAutoInitSet = true;

    [Header("Start Default Values")]
    [ColorUsage(true, true)]
    [SerializeField] private Color outlineHDRColor;
    [SerializeField] private float outlineThickness;
    [SerializeField] private float outlineIntensity;


    private void Start()
    {
        if (bStartAutoInitSet) InitSet();
    }

    private void InitSet()
    {
        if (!changedMat)
        {
            Material orgMat = Resources.Load<Material>("System/Materials/Outline2DMat");
            newMat = Instantiate(orgMat);
            GetComponent<SpriteRenderer>().material = newMat;

            newMat.SetTexture("_MainTex", GetComponent<SpriteRenderer>().sprite.texture);
            newMat.SetColor("_OutlineColor", outlineHDRColor);
            newMat.SetFloat("_OutlineThickness", outlineThickness);
            newMat.SetFloat("_Intensity", outlineIntensity);

            changedMat = true;
        }
    }

    public void SetOutline(Color oColor, float thick, float intensity)
    {
        InitSet();

        newMat.SetColor("_OutlineColor", oColor);
        newMat.SetFloat("_OutlineThickness", thick);
        newMat.SetFloat("_Intensity", intensity);
    }

    public void SetMatTexture(Texture tex)
    {
        InitSet();
        newMat.SetTexture("_MainTex", tex);
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
        InitSet();
        newMat.SetFloat("_Intensity", intensity);
    }
}
