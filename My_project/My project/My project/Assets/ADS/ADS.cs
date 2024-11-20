using AdjustSdk;
using BaseService;
using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADS : MonoBehaviour
{
    public string bannerAdUnitId = "23e0e2aef8d459da"; // Retrieve the ID from your account
    public string KeyMax = "";
    //bool 

    // Start is called before the first frame update
    void Start()
    {

        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) =>
        {
            
            // AppLovin SDK is initialized, start loading ads
            // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
            // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
            MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;


            UIManager.I.reward.InitReward();
            UIManager.I.interval.InitInterval();
            UIManager.I.aop.AOPInit();
            //MaxSdk.ShowBanner(bannerAdUnitId);
        };

        MaxSdk.SetSdkKey(KeyMax);
        MaxSdk.SetUserId("USER_ID");
       // MaxSdk.InitializeSdk();


        Debug.Log("Init Max Sdk");
    }

    /// <summary>
    /// Tải lại banner quảng cáo với bannerAdUnitId ban đầu.
    /// </summary>
    public void ReloadBanner()
    {
        // Hủy banner hiện tại (nếu có)
        //MaxSdk.DestroyBanner(bannerAdUnitId);

        // Tạo lại banner với ID ban đầu
        LoadBanner();

        //Debug.Log("Banner ad reloaded with the same Ad Unit ID.");
    }

    /// <summary>
    /// Tạo và hiển thị banner.
    /// </summary>
    private void LoadBanner()
    {
        MaxSdk.LoadBanner(bannerAdUnitId);
        MaxSdk.ShowBanner(bannerAdUnitId);

        //MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        //MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);
        //MaxSdk.ShowBanner(bannerAdUnitId);

        //Debug.Log("Banner ad loaded and displayed.");
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(bannerAdUnitId);
        Debug.Log("Banner ad hidden.");
    }

    public void ShowBanner()
    {
        MaxSdk.ShowBanner(bannerAdUnitId);
        Debug.Log("Banner ad shown.");
    }
    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        ADS.TrackPaidAdEventApplovinMax(adInfo);
        Debug.Log("Ad revenue paid.");
    }
    public static void TrackPaidAdEventApplovinMax(MaxSdkBase.AdInfo impressionData)
    {
        try
        {
            List<Parameter> AdRevenueParameters = new List<Parameter>
                {
                    new Parameter("ad_format", impressionData.AdFormat ?? ""),
                    new Parameter(FirebaseAnalytics.ParameterValue, impressionData.Revenue),
                    new Parameter(FirebaseAnalytics.ParameterCurrency, "USD")
                };

            FirebaseAnalytics.LogEvent("ad_impression_mediation", AdRevenueParameters.ToArray());

            //if (currentLevelData != null)
            //{
            //    currentLevelData = UIManager.I.currentSceneLoad;
            //    var paramLevel = new Parameter(FirebaseAnalytics.ParameterLevel, currentLevelData.currentOpenLevel);
            //    AdRevenueParameters.Add(paramLevel);
            //}
            //FirebaseAnalytics.LogEvent("ad_revenue_sdk", AdRevenueParameters.ToArray());
        }
        catch (Exception e)
        {
        }
    }
    public static LevelData currentLevelData;
    public static void LogStartLevel(int startLevel, string mode = "")
    {
        try
        {
            SetCurrentOpenLevel(startLevel, mode);
            List<Parameter> LevelStartParameters = new List<Parameter> { new Parameter(FirebaseAnalytics.ParameterLevel, currentLevelData.currentOpenLevel.ToString()) };
            if (currentLevelData.currentMode.Length > 0)
            {
                LevelStartParameters.Add(new Parameter("level_mode", currentLevelData.currentMode.ToString()));
            }
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, LevelStartParameters.ToArray());
        }
        catch (System.Exception e)
        {
            Debug.Log($"AdsUtils Log Error: {e} ");
        }

    }
   

    public static void LogEndLevel(int startLevel, string mode = "", bool success = true)
    {
        try
        {
            SetCurrentOpenLevel(startLevel, mode, success);
            List<Parameter> LevelStartParameters = new List<Parameter>
                {
                    new Parameter(FirebaseAnalytics.ParameterLevel, currentLevelData.currentOpenLevel.ToString())
                };
            if (currentLevelData.currentMode.Length > 0)
            {
                LevelStartParameters.Add(new Parameter("level_mode", currentLevelData.currentMode.ToString()));
            }
            LevelStartParameters.Add(new Parameter(FirebaseAnalytics.ParameterSuccess, currentLevelData.success.ToString()));
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd, LevelStartParameters.ToArray());
        }
        catch (Exception e)
        {
            Debug.Log($"AdsUtils Log Error: {e} ");
        }

    }
    public static void SetCurrentOpenLevel(int level, string mode = "", bool success = true)
    {
        currentLevelData = new LevelData(level, mode, success);
    }
    //public class LevelData
    //{
    //    public int currentOpenLevel;
    //    public string currentMode = "";
    //    public bool success = true;

    //    public LevelData(int level)
    //    {
    //        this.currentOpenLevel = level;
    //    }

    //    public LevelData(int level, string mode)
    //    {
    //        this.currentOpenLevel = level;
    //        this.currentMode = mode;
    //    }

    //    public LevelData(int level, string mode, bool success)
    //    {
    //        this.currentOpenLevel = level;
    //        this.currentMode = mode;
    //        this.success = success;
    //    }
    //}
}
