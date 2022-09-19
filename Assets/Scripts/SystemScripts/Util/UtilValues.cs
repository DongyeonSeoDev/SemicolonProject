using System.Collections.Generic;
using UnityEngine;

public static class UtilValues
{
    private static bool setGray100 = false;
    private static Color gray100;

    public static Color Gray100
    {
        get
        {
            if (!setGray100)
            {
                gray100 = Util.Change255To1Color(100, 100, 100, 255);
                setGray100 = true;
            }
            return gray100;
        }
    }

}
