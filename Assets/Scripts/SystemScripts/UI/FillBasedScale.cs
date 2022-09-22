using UnityEngine;
using DG.Tweening;

public class FillBasedScale : MonoBehaviour
{
    private float fillAmount = -1f;

    public FillType fillType = FillType.X;
    public Vector3 baseScale = new Vector3(1, 1, 1);

    public float FillAmount
    {
        get => GetFillAmount();
        set => SetFillAmount(value);
    }

    /// <summary>
    /// SetFillAmount
    /// </summary>
    /// <param name="rate">0~1 float value</param>
    /// <param name="duration">fill duration</param>
    /// <param name="unscaled">apply Time unscaled</param>
    /// <param name="onComplete">callback</param>
    public void SetFillAmount(float rate, float duration = -1f, bool unscaled = false, System.Action onComplete = null)
    {
        fillAmount = Mathf.Clamp(rate, 0f, 1f);
        Vector3 s = Vector3.one;

        switch(fillType)
        {
            case FillType.X:
                {
                    float x = baseScale.x * fillAmount;
                    s = new Vector3(x, baseScale.y, baseScale.z);
                }
                break;
            case FillType.Y:
                {
                    float y = baseScale.y * fillAmount;
                    s = new Vector3(baseScale.x, y, baseScale.z);
                }
                break;
            case FillType.Z:
                {
                    float z = baseScale.z * fillAmount;
                    s = new Vector3(baseScale.x, baseScale.y, z);
                }
                break;
            case FillType.XY:
                {
                    float x = baseScale.x * fillAmount;
                    float y = baseScale.y * fillAmount;
                    s = new Vector3(x, y, baseScale.z);
                }
                break;
            case FillType.XZ:
                {
                    float x = baseScale.x * fillAmount;
                    float z = baseScale.z * fillAmount;
                    s = new Vector3(x, baseScale.y, z);
                }
                break;
            case FillType.YZ:
                {
                    float y = baseScale.y * fillAmount;
                    float z = baseScale.z * fillAmount;
                    s = new Vector3(baseScale.x, y, z);
                }
                break;
            case FillType.XYZ:
                {
                    float x = baseScale.x * fillAmount;
                    float y = baseScale.y * fillAmount;
                    float z = baseScale.z * fillAmount;
                    s = new Vector3(x, y, z);
                }
                break;
        }

        if (duration <= 0f)
        {
            transform.localScale = s;
        }
        else
        {
            transform.DOScale(s, duration).OnComplete(()=>onComplete?.Invoke()).SetUpdate(unscaled);
        }
    }

    public float GetFillAmount()
    {
        if (fillAmount >= 0f) return fillAmount;
        else
        {
            switch (fillType)
            {
                case FillType.X:
                    return transform.localScale.x / baseScale.x;
                case FillType.Y:
                    return transform.localScale.y / baseScale.y;
                case FillType.Z:
                    return transform.localScale.z / baseScale.z;
                case FillType.XY:
                    return transform.localScale.x / baseScale.x;
                case FillType.XZ:
                    return transform.localScale.z / baseScale.z;
                case FillType.YZ:
                    return transform.localScale.y / baseScale.y;
                case FillType.XYZ:
                    return transform.localScale.x / baseScale.x;
                default:
                    return -1f;
            }
        }
    }
}
