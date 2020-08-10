using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeControl
{
    private static float _normalTime = 1f;
    private static float _slowmoTime = 0.3f;


    public static void SlowTime()
    {
        ChangeTimeScale(_slowmoTime, 0.02f * Time.timeScale);
    }

    public static IEnumerator NormalizeTime(float unfreezeTime)
    {
        yield return new WaitForSecondsRealtime(unfreezeTime);

        ChangeTimeScale(_normalTime, 0.02f);
    }

    private static void ChangeTimeScale(float timeScale, float fixedDeltaTimeMultiplier)
    {
        Time.timeScale = timeScale;

        Time.fixedDeltaTime = fixedDeltaTimeMultiplier;
    }
}
