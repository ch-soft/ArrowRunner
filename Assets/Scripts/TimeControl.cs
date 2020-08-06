using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeControl
{
    private static float _normalTime = 1f;
    private static float _slowmoTime = 0.1f;


    public static void SlowTime()
    {
        ChangeTimeScale(_slowmoTime);
    }

    public static void NormalizeTime()
    {
        ChangeTimeScale(_normalTime);
    }

    private static void ChangeTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
