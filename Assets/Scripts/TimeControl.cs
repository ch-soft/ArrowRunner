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


    public static void SlowTime()
    {
        if (!m_levelFinished)
        {

            Time.timeScale = slowdownFactor;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public static void PunchSlowTime()
    {
        if (!m_levelFinished)
        {

            Time.timeScale = 0.275f;
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
}
