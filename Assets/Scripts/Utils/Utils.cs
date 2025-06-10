using System;
using UnityEngine;

public static class Utils
{
    public static float GetRemainTime(DateTime endTime)
    {
        TimeSpan remaining = endTime - DateTime.UtcNow;
        return Mathf.Max(0f, (float)remaining.TotalSeconds);
    }
}
