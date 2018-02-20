using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathOperations
{

    public static float ClampMax(float value, float topValue)
    {
        if (value > topValue)
            value = topValue;
        return value;
    }
    public static float ClampMin(float value, float bottomValue)
    {
        if (value < bottomValue)
            value = bottomValue;
        return value;
    }
    public static int ClampMax(int value, int topValue)
    {
        if (value > topValue)
            value = topValue;
        return value;
    }
    public static int ClampMin(int value, int bottomValue)
    {
        if (value < bottomValue)
            value = bottomValue;
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
