using UnityEngine;

public static partial class Util
{
    public static Color Change255To1Color(float r, float g, float b, float a = 255f)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
}
