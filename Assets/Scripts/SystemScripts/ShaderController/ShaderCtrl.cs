using UnityEngine;
using UnityEngine.UI;

public abstract class ShaderCtrl : MonoBehaviour   //���̴� ����� ��������Ʈ ������ �̰� �޾ƾ� ��(���̴� ����� ���׸����� ���� �޷��ִ� ������ �� ��ũ��Ʈ��)
{
    protected bool changedMat = false;
    protected Material newMat;

    [SerializeField] protected ApplyMatCompoType cmpnt = ApplyMatCompoType.SPRITERENDERER;
    [SerializeField] protected string matName;
    [SerializeField] protected bool bStartAutoInitSet = true;
    [SerializeField] protected bool bStartAutoSetMatTex = true;


    protected virtual void Start()
    {
        if (bStartAutoInitSet) InitSet();
    }

    protected virtual void InitSet()
    {
        if (!changedMat)
        {
            Material orgMat = Resources.Load<Material>("System/Materials/" + matName);
            newMat = Instantiate(orgMat);

            switch (cmpnt)
            {
                case ApplyMatCompoType.SPRITERENDERER:
                    GetComponent<SpriteRenderer>().material = newMat;
                    break;
                case ApplyMatCompoType.IMAGE:
                    GetComponent<Image>().material = newMat;
                    break;
                case ApplyMatCompoType.RAWIMAGE:
                    GetComponent<RawImage>().material = newMat;
                    break;
            }

            if(bStartAutoSetMatTex)
            {
                switch (cmpnt)
                {
                    case ApplyMatCompoType.SPRITERENDERER:
                        newMat.SetTexture("_MainTex", GetComponent<SpriteRenderer>().sprite.texture);
                        break;
                    case ApplyMatCompoType.IMAGE:
                        newMat.SetTexture("_MainTex", GetComponent<Image>().sprite.texture);
                        break;
                    case ApplyMatCompoType.RAWIMAGE:
                        newMat.SetTexture("_MainTex", GetComponent<RawImage>().texture);
                        break;
                }
            }

            AdditionalInitSet();

            changedMat = true;
        }
    }

    public abstract void AdditionalInitSet();

    //���� ������ �͵� ����
    public virtual void SetMainTex(Texture tex)
    {
        InitSet();
        newMat.SetTexture("_MainTex", tex);
    }

    public virtual void SetIntensity(float intensity)
    {
        InitSet();
        newMat.SetFloat("_Intensity", intensity);
    }

    public virtual void SetColor(Color color)
    {
        InitSet();
        newMat.SetColor("_Color", color);
    }

    public virtual void SetThickness(float thick)
    {
        InitSet();
        newMat.SetFloat("_Thickness", thick);
    }
}
