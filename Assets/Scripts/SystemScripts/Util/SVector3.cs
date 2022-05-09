using UnityEngine;

public static class SVector3
{
    public static Vector3 half => Vector3.one * 0.5f;
    public static Vector3 two => Vector3.one * 2f;
    public static Vector3 onePointTwo => new Vector3(1.2f, 1.2f, 1.2f);
    public static Vector3 onePointThree => new Vector3(1.3f, 1.3f, 1.3f);
    public static Vector3 onePointSix => new Vector3(1.6f, 1.6f, 1.6f);
    public static Vector3 zeroPointSeven => new Vector3(0.7f, 0.7f, 0.7f);
    public static Vector3 zeroPointThree => new Vector3(0.3f, 0.3f, 0.3f);
    public static Vector3 Z90 => new Vector3(0, 0, 90);
}
