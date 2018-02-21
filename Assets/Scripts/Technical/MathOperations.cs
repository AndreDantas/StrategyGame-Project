using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathOperations
{

    public static float ClampMax(float value, float maxValue)
    {

        if (value > maxValue)
            value = maxValue;
        return value;
    }
    public static float ClampMin(float value, float minValue)
    {
        if (value < minValue)
            value = minValue;
        return value;
    }
    public static int ClampMax(int value, int maxValue)
    {
        if (value > maxValue)
            value = maxValue;
        return value;
    }
    public static int ClampMin(int value, int minValue)
    {
        if (value < minValue)
            value = minValue;
        return value;
    }

    public static int Sign(int value)
    {
        if (value >= 0)
            return 1;
        else
            return -1;
    }
}
