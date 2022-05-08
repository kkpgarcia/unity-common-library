using UnityEngine;

public static class Comparison {

    public static bool TolerantEquals(float a, float b)
    {
        return Mathf.Approximately(a, b);
    }

    public static bool TolerantGreaterThanOrEquals(float a, float b)
    {
        return a > b || TolerantEquals(a, b);
    }

    public static bool TolerantLesserThanOrEquals(float a, float b)
    {
        return a < b || TolerantEquals(a, b);
    }
}
