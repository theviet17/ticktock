using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;
#if UNITY_IOS
// Include the IosSupport namespace if running on iOS:
using Unity.Advertisement.IosSupport;
#endif

public enum VirtualCurrencySource
{
    pick_up_in_game,
    watch_ad_shop,
    win_game_normal,
    win_game_watch_ad,
    open_box_end_game,
    update_strength,
    update_health,
    buy_item_shop_
}

public static class AnalyticsEvent
{
    public static string LEVEL_START = "level_start";
    public static string LEVEL_COMPLETE = "level_complete";
    public static string LEVEL_FAIL = "level_fail";
    public static string EARN_VIRTUAL_CURRENCY = "earn_virtual_currency";
    public static string SPEND_VIRTUAL_CURRENCY = "spend_virtual_currency";
    public static string TIMES_PER_FIRST_SECTION = "times_per_first_section";
    public static string UNLOCK_TABLE = "unlock_table";
    public static string ATT_SHOWED = "ATT_SHOWED";
    public static string ATT_SUCCESS = "ATT_SUCCESS";

}
public class AnalyticsManager : MonoBehaviour
{
    private const string FIRSTTIME = "FIRSTTIME";
    private DateTime firstTimeOnTheGame;
    private DateTime timeStart;
    public static AnalyticsManager instance;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
#if UNITY_EDITOR
        Application.runInBackground = false;
#endif
        GetDateTime();
        timeStart = System.DateTime.Now;

#if UNITY_IOS
        // Check the user's consent status.
        // If the status is undetermined, display the request request: 
        if(ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED) {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }

    private void ATTShow()
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.ATT_SHOWED);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void ATTSuccess()
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.ATT_SUCCESS);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private void GetDateTime()
    {
        try
        {
            if (PlayerPrefs.HasKey(FIRSTTIME))
            {
                string dateTimeString = PlayerPrefs.GetString(FIRSTTIME);
                System.DateTime dateTime = System.DateTime.Parse(dateTimeString);
                firstTimeOnTheGame = dateTime;
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    // void OnApplicationPause(bool pauseStatus)
    // {
    //     if (pauseStatus)
    //     {
    //         if (firstTimeOnTheGame.Year < 2021)
    //         {
    //
    //             firstTimeOnTheGame = System.DateTime.Now;
    //             PlayerPrefs.SetString(FIRSTTIME, firstTimeOnTheGame.ToString());
    //             LogEventTimesInFirstSection((int)(System.DateTime.Now - timeStart).TotalSeconds);
    //         }
    //     }
    // }

    public void LogLevelStartEvent(string level)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.LEVEL_START,
            new Firebase.Analytics.Parameter("level", level));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogLevelStartNoParamEvent()
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.LEVEL_START);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogLevelCompleteEvent(string level)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.LEVEL_COMPLETE,
            new Firebase.Analytics.Parameter("level", level));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
    public void LogLevelCompleteNoParamEvent()
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.LEVEL_COMPLETE);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogLevelFailEvent(string level, string failCount)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.LEVEL_FAIL,
           new Firebase.Analytics.Parameter("level", level),
           new Firebase.Analytics.Parameter("failcount", failCount));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogLevelFailNoParamEvent()
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.LEVEL_FAIL);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogEarnVirtualCurrencyEvent(string virtualCurrencyName, string value, string source)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.EARN_VIRTUAL_CURRENCY,
            new Firebase.Analytics.Parameter("virtualCurrencyName", virtualCurrencyName),
            new Firebase.Analytics.Parameter("value", value),
            new Firebase.Analytics.Parameter("source", source));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogSpendVirtualCurrencyEvent(string virtualCurrencyName, string value, string item_name)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.SPEND_VIRTUAL_CURRENCY,
            new Firebase.Analytics.Parameter("virtualCurrencyName", virtualCurrencyName),
            new Firebase.Analytics.Parameter("value", value),
            new Firebase.Analytics.Parameter("item_name", item_name));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogAdsEvent(string eventName, string placement, string error)
    {
        try
        {
            if (placement.Equals(string.Empty))
            {
                if (error.Equals(string.Empty))
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName,
                    new Firebase.Analytics.Parameter("placement", placement));
                }
                else
                {
                    Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName);
                }
            }
            else
            {
                Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName,
                new Firebase.Analytics.Parameter("placement", placement),
                new Firebase.Analytics.Parameter("error", error));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"AnalyticsManagerLog: Error Event: {e}");
        }
    }

    public void LogEventTimesInFirstSection(long seconds)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent(AnalyticsEvent.TIMES_PER_FIRST_SECTION, "timeplayed", seconds.ToString());
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogEventCheckPoint(string table)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent($"{AnalyticsEvent.UNLOCK_TABLE}{table}");
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SetUserproperties(string level)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.SetUserProperty("level", level);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogSelectGame(string gameID)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("select_game",
            new Firebase.Analytics.Parameter("gameID", gameID));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void LogTimePlayGame(string gameID, long seconds)
    {
        try
        {
            Firebase.Analytics.FirebaseAnalytics.LogEvent("time_play_game",
                new Firebase.Analytics.Parameter("gameID", gameID),
                new Firebase.Analytics.Parameter("timeplayed", seconds));
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
