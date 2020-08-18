//using Facebook.Unity;
//using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnalyticsManager
{
    //private static Dictionary<string, Dictionary<string, object>> m_savedEvents;

    //public static void Initialize()
    //{
    //    m_savedEvents = new Dictionary<string, Dictionary<string, object>>();

    //    FB.Init(SendSavedEvents);
    //    GameAnalytics.Initialize();
    //}

    //private static void SendSavedEvents()
    //{
    //    foreach( var e in m_savedEvents )
    //    {
    //        if( e.Value != null )
    //            FB.LogAppEvent(e.Key, parameters: e.Value);
    //        else
    //            FB.LogAppEvent(e.Key);
    //    }

    //    m_savedEvents.Clear();
    //}

    //public static void FireRoundStartEvent(int level, int attempt) { 
    //    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, level.ToString(), attempt.ToString());
        
    //    var roundInfo = new Dictionary<string, object>(); 
    //        roundInfo["level"] = level; 
    //        roundInfo["attempt"] = attempt; 

    //    if( FB.IsInitialized )
    //    {
    //        FB.LogAppEvent("levelStart", parameters: roundInfo);
    //    }
    //    else
    //    {
    //        m_savedEvents.Add("levelStart", roundInfo);
    //    }
    //} 
 
    //public static void FireRoundEndEvent(int level, int attempt, int playSeconds) { 
    //    GameAnalytics.NewDesignEvent("round_end");

    //    var roundInfo = new Dictionary<string, object>(); 
    //        roundInfo["level"] = level;
    //        roundInfo["attempt"] = attempt;
    //        roundInfo["playSeconds"] = playSeconds;

    //    if( FB.IsInitialized )
    //    {
    //        FB.LogAppEvent("levelEnd", parameters: roundInfo);
    //    }
    //    else
    //    {
    //        m_savedEvents.Add("levelEnd", roundInfo);
    //    }
    //}

    //public static void FireRoundCompleteEvent(int level, int attempt, int score) { 
    //    GameAnalytics.NewProgressionEvent( 
    //        GAProgressionStatus.Complete, level.ToString(), attempt.ToString(), score.ToString());

    //    var roundInfo = new Dictionary<string, object>(); 
    //        roundInfo["level"] = level;
    //        roundInfo["attempt"] = attempt;
    //        roundInfo["score"] = score;

    //    if( FB.IsInitialized )
    //    { 
    //        FB.LogAppEvent("levelWin", parameters: roundInfo);
    //    }
    //    else
    //    {
    //        m_savedEvents.Add("levelWin", roundInfo);
    //    }
    //} 
 
    //public static void FireRoundFailEvent(int level, int attempt, int score, float progress)
    //{ 
    //    GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, 
    //        level.ToString(), attempt.ToString(), progress.ToString(), score); 
 
    //    var roundInfo = new Dictionary<string, object>(); 
    //        roundInfo["level"] = level;
    //        roundInfo["attempt"] = attempt;
    //        roundInfo["score"] = score;
    //        roundInfo["progress"] = progress;

    //    if( FB.IsInitialized )
    //    {
    //        FB.LogAppEvent("levelFail", parameters: roundInfo);
    //    }
    //    else
    //    {
    //        m_savedEvents.Add("levelFail", roundInfo);
    //    }
    //} 
 
    //public static void FireSingleEvent(string singleEvent) { 
    //    GameAnalytics.NewDesignEvent(singleEvent);

    //    if( FB.IsInitialized )
    //    {
    //        FB.LogAppEvent(singleEvent);
    //    }
    //    else
    //    {
    //        m_savedEvents.Add(singleEvent, null);
    //    }
    //}
}
