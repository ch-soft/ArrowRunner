using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeControl
{
    public static bool m_characterIsAlive;
    public static bool m_levelFinished;

    private static float _normalTime = 1f;
    private static float _slowmoTime = 0.15f;

    public static float slowdownFactor = 0.125f;
    public static float slowdownLength = 2f;

    public static void AutoNormalizeTime()
    {
        //Time.timeScale += (1f / slowdownLength) * Time.unscaledDeltaTime;
        //Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }

    public static void SlowTime()
    {
        if (!m_levelFinished)
        {
            //ChangeTimeScale(_slowmoTime, Time.timeScale * 0.02f);

            Time.timeScale = slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public static void PunchSlowTime()
    {
        if (!m_levelFinished)
        {
            //ChangeTimeScale(_slowmoTime, Time.timeScale * 0.02f);

            Time.timeScale = 0.25f;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public static IEnumerator NormalizeTime(float unfreezeTime)
    {
        yield return new WaitForSecondsRealtime(unfreezeTime);
        if (Time.timeScale != 1f)
        {
            Time.timeScale = _normalTime;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }

    }

    private static void ChangeTimeScale(float timeScale, float fixedDeltaTimeMultiplier)
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = fixedDeltaTimeMultiplier;
    }
}
