using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChangeToValue
{
    public static float ChangeToFloat(float value, float changeTo = 0f, float increment = 0.05f, float minDistance = 0.05f)
    {
        if (value == changeTo)
            return changeTo;
        increment = Mathf.Clamp(increment, 0.00001f, 999999f);
        minDistance = Mathf.Clamp(minDistance, 0.00001f, 999999f);
        if ((value <= minDistance && value >= changeTo) || (value >= -minDistance && value <= changeTo))
            return changeTo;
        else
        {
            if (value > changeTo)
            {
                if (value - increment < changeTo)
                    return changeTo;
                else
                    return value - increment;
            }
            else
            {
                if (value + increment > changeTo)
                    return changeTo;
                else
                    return value + increment;
            }
        }
    }
}
