using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeControl
{
    public static bool m_characterIsAlive;
    public static bool m_levelFinished;

    private static float _normalTime = 1f;
    private static float _slowmoTime = 0.15f;

    public static float slowdownFactor = 0.05f;
    public static float slowdownLength = 20f;

    public static void AutoNormalizeTime()
    {
        Time.timeScale += (1.0f / slowdownLength) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
        //if (Time.timeScale == 1.0f)
        //{
        //    Time.fixedDeltaTime = Time.deltaTime;
        //}
    }

    public static void SlowTime()
    {
        ChangeTimeScale(_slowmoTime, Time.timeScale * 0.02f);
    }

  

    public static IEnumerator NormalizeTime(float unfreezeTime)
    {
        yield return new WaitForSecondsRealtime(unfreezeTime);
        ChangeTimeScale(_normalTime, 0.02f);
    }

    private static void ChangeTimeScale(float timeScale, float fixedDeltaTimeMultiplier)
    {
        //Time.timeScale = timeScale;
        //Time.fixedDeltaTime = fixedDeltaTimeMultiplier;

        Time.timeScale = timeScale;
        Time.fixedDeltaTime = fixedDeltaTimeMultiplier;

        //Time.timeScale = slowdownFactor;
        //Time.fixedDeltaTime = Time.timeScale * .02f;
    }
}
