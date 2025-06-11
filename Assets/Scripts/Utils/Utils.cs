using System;
using UnityEngine;

public static class Utils
{
    public static int GetRemainTime(DateTime endTime)
    {
        TimeSpan remaining = endTime - DateTime.UtcNow;
        return Convert.ToInt32(Mathf.Max(0f, (float)remaining.TotalSeconds));
    }
}
