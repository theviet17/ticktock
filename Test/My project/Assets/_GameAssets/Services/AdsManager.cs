using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using BaseMe;
using BaseService;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class AdsManager : Singleton<AdsManager>
{
    public bool canShowAppOpenAds = false;
    public bool OpenNativeAds = false;
    public bool NativeAdsEnable = false;

    public bool doneAdsInterval { get; set; } = true; // True đã countdown xong và cho phép hiển thị, false thì chưa
    private WaitForSecondsRealtime waitRewarded;

    static Action<AdsResult> actionRewarded;
    static Action actionFullAds;
    public static Action<bool> actionIngameAnoyingAds;

    static string forceAdsPosition; //ad start
    static string rewardAdsPosition;

    private bool isBannerShowing;
    private bool isAdsShowing;

    private int interstitialRetryAttempt;
    private int bannerRetryAttempt;
    private int rewardedRetryAttempt;

    private int tracking_timeLoadAds;
    private int cooldown_interval = 0;
    

    [SerializeField] private bool bannerMaxEnable = false;

    [SerializeField] private bool isDebugAds = false;
    
    public bool IsAdsEnable()
    {
        return AdsKeysManager.GetMode() != AdsKeysManager.AdsMode.Disable && PlayerPrefs.GetInt("NoAds", 0) == 0;
    }

    protected override void OnRegisterInstance()
    {
        
    }

    public void InitializeEngineAds()
    {
        waitRewarded = new WaitForSecondsRealtime(0.2f);
        
        
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.
            Debug.Log("MAX SDK Initialized");
            if (IsAdsEnable())
            {
                Debug.Log("Ads Manager Start Init Inter");
                InitializeInterstitialAds();
                Debug.Log("Ads Manager Start Init Rewarded");
                if (AdsKeysManager.IsRewardMaxEnable())
                    InitializeRewardedAds();
                Debug.Log("Ads Manager Start Init Banner");
                if (bannerMaxEnable)
                    InitializeBannerAds();
                Debug.Log("Ads Manager End Init Banner");
                if (AdsKeysManager.IsMrecMaxEnable())
                    InitializeMRecAds();
            }

            if (isDebugAds)
                MaxSdk.ShowMediationDebugger();
        };

        MaxSdk.SetSdkKey(AdsKeysManager.MaxSdkKey);
        DOVirtual.DelayedCall(0.01f, () => MaxSdk.InitializeSdk());
    }


    void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log($"Unity Log: Pause {pauseStatus} __ {canShowAppOpenAds}, {OpenNativeAds}, {AppOpenAdManager.Instance.pauseByShowingInter}");
        if (!pauseStatus)
        {
            if (canShowAppOpenAds && OpenNativeAds == false && !AppOpenAdManager.Instance.pauseByShowingInter && !AppOpenAdManager.isLoadingSplash)
            {
                AppOpenAdManager.Instance.ShowAdIfAvailable(() => { });
            }

            if (AppOpenAdManager.Instance.pauseByShowingInter)
            {
                AppOpenAdManager.Instance.pauseByShowingInter = false;
            }

            OpenNativeAds = false;
        }
    }

    public void SetAdsInterval()
    {
        doneAdsInterval = false;

        StartCoroutine(WaitEndAdsInterval());
    }

    IEnumerator WaitEndAdsInterval()
    {
        int interval = 40;
        try
        {
#if UNITY_EDITOR
            interval = RemoteConfigControl.I.capping_ads.GetValue();
#elif UNITY_ANDROID
            interval = RemoteConfigControl.I.capping_ads.GetValue();
#elif UNITY_IOS
    interval = RemoteConfigControl.I.capping_ads.GetValue();
#endif
        }
        catch (Exception e)
        {
        }

        yield return new WaitForSecondsRealtime(interval);

        Debug.Log("Unity capingTime " + interval);
        doneAdsInterval = true;
    }

    #region Retry

    private void ReinitAdSdk()
    {
        if (CheckInternet() && !MaxSdk.IsInitialized())
        {
            MaxSdk.SetSdkKey(AdsKeysManager.MaxSdkKey);
            MaxSdk.InitializeSdk();
        }
    }

    public void TryInitAndRequestAd()
    {
        ReinitAdSdk();
        LoadRewardedAd();
        LoadInterstitial();
        InitializeBannerAds();
        InitializeMRecAds();
    }

    #endregion

    #region Interstitial Ad Methods

    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += InterstitialOnAdLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += InterstitialOnAdLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += InterstitialOnAdDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialOnAdDisplayFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += InterstitialOnAdClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += InterstitialOnAdHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += InterstitialOnAdRevenuePaidEvent;

        //fix anr
        LoadFullAds();
    }

    // Load the first interstitial
    void LoadFullAds()
    {
        LoadInterstitial();
    }

    public string GetInterstitialAdUnitId()
    {
        return AdsKeysManager.GetMaxInterId();
    }

    private bool isMaxLoading = false;
    public void LoadInterstitial()
    {
        Debug.Log("Load Full Ads");
        if (MaxSdk.IsInitialized())
        {
            if (!MaxSdk.IsInterstitialReady(GetInterstitialAdUnitId()) && !isMaxLoading)
            {
                isMaxLoading = true;
                MaxSdk.LoadInterstitial(GetInterstitialAdUnitId());
                tracking_timeLoadAds = (int) Time.time;
            }
            else
            {
                Debug.Log("Load Full Ads - AdsIsReady - Not Load");
            }
        }
        else
        {
            interstitialRetryAttempt++;
            double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

            Debug.Log("ReLoad Full Ads - Max Not Ready");

            Invoke("LoadInterstitial", (float) retryDelay);
        }
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterCallLoad);
    }

    public void AppsFlyerLog(string eventName)
    {
        
    }

    public void AppsFlyerLog(string eventName, string paramName, int value)
    {
        var param = new Dictionary<string, string>();
        param.Add(paramName, value.ToString()); ;
        
    }

    private static int countAdsInter;

    public static void ShowInterAds(Action actionDone, string position = "", bool ignore_ads_interval = false)
    {
        countAdsInter++;
        if (countAdsInter > 2)
            AdsManager.I.ShowAdUnit(actionDone, position, ignore_ads_interval);
        else
            actionDone.Invoke();
    }

    public void ShowAdUnit(Action actionDone, string position, bool ignore_ads_interval = false)
    {
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterCallShow);
        // #if UNITY_EDITOR
        //         Debug.Log("Show Full Ads: " + position);
        //         actionDone();
        //         return;
        // #else
        canShowAppOpenAds = false;
        forceAdsPosition = position;

        if (!IsAdsEnable() || RemoteConfigControl.I.inter_enable.GetValue()!=1)
        {
            //Debug.Log("AppUnityLog: check_noads_" + !IsAdsEnable());
            actionDone.Invoke();
            canShowAppOpenAds = true;
            return;
        }

        bool forceShowGoogleAdmob = false;
        if (doneAdsInterval || ignore_ads_interval)
        {
            LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterPassedCapping);

            if (AdsKeysManager.IsTestMode())
            {
                forceShowGoogleAdmob = true;
            }
            //if (MaxSdk.IsInterstitialReady(GetInterstitialAdUnitId()) && !forceShowGoogleAdmob)
            //{
            //    LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterAvailable);
            //    Debug.Log("AppUnityLog ad ready, show ad");
            //    doneAdsInterval = false;
            //    actionFullAds = actionDone;
            //    isAdsShowing = true;
            //    MaxSdk.ShowInterstitial(GetInterstitialAdUnitId());
            //    LogAdEvent("inter_ad", "InterstitialAvailable", string.Empty);
            //}
            //else
            //{
            //    //canShowAppOpenAds = true;
            //    Debug.Log("AppUnityLog ad not ready");
            //    //actionDone.Invoke();

            //    if (CheckInternet())
            //    {
            //        LogAdEvent("inter_ad", "InterstitialNotReady_InternetReady", string.Empty);
            //    }
            //    else
            //    {
            //        // show no internet
            //        LogAdEvent("inter_ad", "InterstitialNotReady_InternetNotReady", string.Empty);
            //    }

            //    LogAdEvent("inter_ad", "InterstitialNotReady", string.Empty);
            //    // if (!forceShowGoogleAdmob)
            //    // {
            //    //     LoadInterstitial();
            //    // }
            //    GoogleMobileAdsManager.I.ShowInterstitialAd(actionDone, true);
            //    LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterNotAvailable);
            //}
            //canShowAppOpenAds = true;
            if (!forceShowGoogleAdmob)
            {
                Debug.Log("AppUnityLog ad not ready");
                //actionDone.Invoke();

                if (CheckInternet())
                {
                    LogAdEvent("inter_ad", "InterstitialNotReady_InternetReady", string.Empty);
                }
                else
                {
                    // show no internet
                    LogAdEvent("inter_ad", "InterstitialNotReady_InternetNotReady", string.Empty);
                }

                LogAdEvent("inter_ad", "InterstitialNotReady", string.Empty);
                // if (!forceShowGoogleAdmob)
                // {
                //     LoadInterstitial();
                // }
                GoogleMobileAdsManager.I.ShowInterstitialAd(actionDone, true);
                LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterNotAvailable);
            }
            
        }
        else
        {
            canShowAppOpenAds = true;
            Debug.Log("miss time");
            actionDone.Invoke();
            LogAdEvent("inter_ad", "InterstitialNotPassCapping", string.Empty);
            LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterNotPassedCapping);
        }
        // #endif
    }

    private void InterstitialOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isMaxLoading = false;
        // Interstitial ad is ready to be shown. MaxSdk.IsInterstitialReady(interstitialAdUnitId) will now return 'true'
        interstitialRetryAttempt = 0;
        int time = (int) Time.time - tracking_timeLoadAds;
        LogAdEvent("inter_ad", "InterstitialAdLoadedEvent", string.Empty);
        LogAdEvent("inter_ad", $"InterstitialAdLoadedEvent_TimeLoad_{time}", string.Empty);
        AppsFlyerLog("solar_inter_called");
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterLoadSuccess);
    }

    private void InterstitialOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isMaxLoading = false;
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        interstitialRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));
        Invoke("LoadInterstitial", (float) retryDelay);

        LogAdEvent("inter_ad", "InterstitialAdLoadFailedEvent", string.Empty);
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterLoadFail);
    }

    private void InterstitialOnAdDisplayedEvent(string adUnitID, MaxSdkBase.AdInfo adInfo)
    {
        AppOpenAdManager.Instance.pauseByShowingInter = true;
        AppsFlyerLog("solar_inter_displayed");
        AppsFlyerLog("solar_inter_displayed_unity");
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.InterDisplay);
        LogAdEvent("inter_ad", "InterstitialOnAdDisplayedEvent", string.Empty);
    }

    private void InterstitialOnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        RunInMainThread(() =>
        {
            // Interstitial ad failed to display. We recommend loading the next ad
            if (actionFullAds != null)
            {
                actionFullAds.Invoke();
                actionFullAds = null;
                SetAdsInterval();
            }

            canShowAppOpenAds = true;
            //fix anr
            Invoke("LoadInterstitial", 0.5f);

            LogAdEvent("inter_ad", "InterstitialAdShowFailedEvent", string.Empty);
        });
    }

    private void InterstitialOnAdClickedEvent(string adUnitID, MaxSdkBase.AdInfo adInfo)
    {
        LogAdEvent("inter_ad", "InterstitialAdClickEvent", string.Empty);
        LogAdEvent("inter_ad", $"InterstitialAdClickEvent_{forceAdsPosition}", string.Empty);
    }

    private void InterstitialOnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        RunInMainThread(() =>
        {
            if (actionFullAds != null)
            {
                actionFullAds.Invoke();
                actionFullAds = null;
                SetAdsInterval();
            }

            canShowAppOpenAds = true;
            //fix anr
            Invoke("LoadInterstitial", 0.5f);

            LogAdEvent("inter_ad", "InterstitialAdClosedEvent", string.Empty);
            LogAdEvent("inter_ad", $"InterstitialAdClosedEvent_{forceAdsPosition}", string.Empty);
        });
    }

    private async void RunInMainThread(Action action)
    {
        await UniTask.SwitchToMainThread();
        action.Invoke();
    }

    private void InterstitialOnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        LogcatUtils.TrackPaidAdEventApplovinMax(impressionData);
    }
    

    public void CheckResetAdsInterval()
    {
        if (isAdsShowing)
        {
            Debug.Log("Reset AdsShowing");
            isAdsShowing = false;
            SetAdsInterval();
        }
    }

    #endregion

    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += RewardedOnAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += RewardedOnAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += RewardedOnAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += RewardedOnAdDisplayFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += RewardedOnAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += RewardedOnAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += RewardedOnAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += RewardedOnAdReceivedRewardEvent;

        // Load the first RewardedAd
        //fix anr
        Invoke("LoadRewardedAd", 0.3f);
    }

    public string GetRewardedAdUnitId()
    {
        return AdsKeysManager.GetMaxRewardId();
    }

    private void LoadRewardedAd()
    {
        if (MaxSdk.IsInitialized() && AdsKeysManager.IsRewardMaxEnable())
        {
            MaxSdk.LoadRewardedAd(GetRewardedAdUnitId());
            LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardCallLoad);
        }
    }

    public void ShowRewardVideo(Action<AdsResult> actionDone, string position = "")
    {
        // #if UNITY_EDITOR
        //         actionDone();
        //         return;
        // #else

        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardCallShow);
        if (AdsKeysManager.GetMode() == AdsKeysManager.AdsMode.Disable || RemoteConfigControl.I.reward_enable.GetValue()!=1)
        {
            actionDone.Invoke(AdsResult.Success);
            return;
        }

        canShowAppOpenAds = false;
        rewardAdsPosition = position;
        AppsFlyerLog("af_rewarded_ad_eligible");
        bool forceShowGoogleAdmob = false;
        if (AdsKeysManager.IsTestMode())
        {
            forceShowGoogleAdmob = true;
        }
        if(!forceShowGoogleAdmob)
        {
            isAdsShowing = true;
            GoogleMobileAdsManager.I.ShowInterAdsWithChecking((result) =>
            {
                isAdsShowing = false;
                canShowAppOpenAds = true;
                actionDone.Invoke(result);


                //ShowMessage("Ads not ready yet!");
            });
            if (CheckInternet())
            {
                LogAdEvent("reward_ad", "RewardedVideoAdNotReady_InternetReady", string.Empty);
            }
            else
            {
                //ShowMessage("No internet connection!");
                LogAdEvent("reward_ad", "RewardedVideoAdNotReady_InternetNotReady", string.Empty);
            }

            LogAdEvent("reward_ad", "RewardedVideoAdNotReady", string.Empty);
            LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardNotAvailable);
        }
      

        //if (MaxSdk.IsRewardedAdReady(GetRewardedAdUnitId()) && !forceShowGoogleAdmob)
        //{
        //    actionRewarded = actionDone;
        //    isAdsShowing = true;
        //    MaxSdk.ShowRewardedAd(GetRewardedAdUnitId());
        //    LogAdEvent("reward_ad", "RewardedVideoAvailable", string.Empty);
        //    LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardAvailable);
        //}
        //else
        //{
        //    //show inters instead
        //    if (MaxSdk.IsInterstitialReady(GetInterstitialAdUnitId()) && !forceShowGoogleAdmob)
        //    {
        //        doneAdsInterval = false;
        //        actionFullAds = () =>
        //        {
        //            actionDone.Invoke(AdsResult.Success);
        //        };
        //        isAdsShowing = true;
        //        MaxSdk.ShowInterstitial(GetInterstitialAdUnitId());
        //    }
        //    else
        //    {
        //        isAdsShowing = true;
        //        GoogleMobileAdsManager.I.ShowInterAdsWithChecking((result) =>
        //        {
        //            isAdsShowing = false;
        //            canShowAppOpenAds = true;
        //            actionDone.Invoke(result);
                  
                   
        //            //ShowMessage("Ads not ready yet!");
        //        });
        //    }

        //    //log reward
        //    if (CheckInternet())
        //    {
        //        LogAdEvent("reward_ad", "RewardedVideoAdNotReady_InternetReady", string.Empty);
        //    }
        //    else
        //    {
        //        //ShowMessage("No internet connection!");
        //        LogAdEvent("reward_ad", "RewardedVideoAdNotReady_InternetNotReady", string.Empty);
        //    }

        //    LogAdEvent("reward_ad", "RewardedVideoAdNotReady", string.Empty);
        //    LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardNotAvailable);
        //}
    }

    private void RewardedOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        Debug.Log("Rewarded ad loaded");

        // Reset retry attempt
        rewardedRetryAttempt = 0;
        AppsFlyerLog("af_rewarded_api_called");
        LogAdEvent("reward_ad", "RewardedVideoAdLoadSuccessEvent", string.Empty);
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardLoadSuccess);

    }

    private void RewardedOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        rewardedRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, rewardedRetryAttempt));
        Invoke("LoadRewardedAd", (float) retryDelay);

        LogAdEvent("reward_ad", "RewardedVideoAdLoadFailedEvent", string.Empty);
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardLoadFail);
    }

    private void RewardedOnAdDisplayFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo,
        MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. We recommend loading the next ad
        Invoke("LoadRewardedAd", 0.5f);
        canShowAppOpenAds = true;
        LogAdEvent("reward_ad", "RewardedVideoAdShowFailedEvent", string.Empty);
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardShowFail);

    }

    private void RewardedOnAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Invoke("LoadRewardedAd", 0.5f);
        AppsFlyerLog("af_rewarded_ad_displayed");
        LogAdEvent("reward_ad", "RewardedVideoAdDisplayedEvent", string.Empty);
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardDisplay);
    }

    private void RewardedOnAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked");
    }

    private void RewardedOnAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        if (actionRewarded != null)
        {
            StartCoroutine(WaitRewarded());
        }
        Invoke("LoadRewardedAd", 0.5f);
        SetAdsInterval();
        canShowAppOpenAds = true;
        LogAdEvent("reward_ad", "RewardedVideoAdDismissedEvent", string.Empty);
    }

    private void RewardedOnAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad was displayed and user should receive the reward

        //if (actionRewarded != null)
        //{
        //    StartCoroutine(WaitRewarded());
        //}
        Debug.Log("UnityAppLog Reward Received");
        IsReceivedRewarded = true;
        //SetAdsInterval();
        canShowAppOpenAds = true;
        LogAdEvent("reward_ad", "RewardedVideoAdRewardedEvent", string.Empty);
        LogAdEvent("reward_ad", $"RewardedVideoAdRewardedEvent_{rewardAdsPosition}", string.Empty);
        LogcatUtils.LogAdjust(AdjustKeysManager.EventType.RewardCompleted);
    }

    private void RewardedOnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        LogcatUtils.TrackPaidAdEventApplovinMax(impressionData);
    }

    IEnumerator WaitRewarded()
    {
        yield return waitRewarded;
        if (IsReceivedRewarded)
        {
            IsReceivedRewarded = false;
            actionRewarded.Invoke(AdsResult.Success);
        }
        else
        {
            actionRewarded.Invoke(AdsResult.Fail);
        }
    }

    private bool IsReceivedRewarded = false;

    #endregion

    #region Banner Ad Methods

    private void InitializeBannerAds()
    {
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += BannerOnAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += BannerOnAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnAdRevenuePaidEvent;
        Invoke("DelayInitBanner", 0.5f);
    }

    private void OnAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo impressionData)
    {
        LogcatUtils.TrackPaidAdEventApplovinMax(impressionData);
    }

    public string GetBannerAdUnitId()
    {
        return AdsKeysManager.GetMaxBannerId();
    }

    private void DelayInitBanner()
    {
        // Banners are automatically sized to 320x50 on phones and 728x90 on tablets.
        // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments.
        MaxSdk.CreateBanner(GetBannerAdUnitId(), MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerExtraParameter(GetBannerAdUnitId(), "adaptive_banner", "false");
        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(GetBannerAdUnitId(), Color.black);
        MaxSdk.SetBannerWidth(GetBannerAdUnitId(), (float) Screen.width);
        // hide banner after load success
        HideBanner();

//#endif
    }

    public void ShowBanner()
    {
        if (IsAdsEnable())
        {
            MaxSdk.ShowBanner(GetBannerAdUnitId());
            GoogleMobileAdsManager.I.OnlyHideDisplayOfBannerForShowBannerMax();
            HideMRec();
        }
    }

    public void HideBanner()
    {
        MaxSdk.HideBanner(GetBannerAdUnitId());
    }

    private void ToggleBannerVisibility()
    {
        if (!isBannerShowing)
        {
            MaxSdk.ShowBanner(GetBannerAdUnitId());
        }
        else
        {
            MaxSdk.HideBanner(GetBannerAdUnitId());
        }

        isBannerShowing = !isBannerShowing;
    }

    // Fired when a banner is loaded
    private void BannerOnAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        bannerRetryAttempt = 0;
        //HideBanner();
        Debug.Log("Banner Loaded:" + adUnitId);
        LogAdEvent("banner_ad", "banner_loaded", string.Empty);
    }

    // Fired when a banner has failed to load
    private void BannerOnAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        bannerRetryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, bannerRetryAttempt));

        Invoke("DelayInitBanner", (float) retryDelay);
        LogAdEvent("banner_ad", "banner_load_fail", errorInfo.Message);
    }

    #endregion

    #region MRec

    public string GetMrecAdUnitId()
    {
        return AdsKeysManager.GetMaxMrecId();
    }

    public void InitializeMRecAds()
    {
        if (!AdsKeysManager.IsMrecMaxEnable())
            return;
        // MRECs are sized to 300x250 on phones and tablets
        MaxSdk.CreateMRec(GetMrecAdUnitId(), MaxSdkBase.AdViewPosition.BottomCenter);

        MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnMRecAdLoadedEvent;
        MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnMRecAdLoadFailedEvent;
        MaxSdkCallbacks.MRec.OnAdClickedEvent += OnMRecAdClickedEvent;
        MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnMRecAdRevenuePaidEvent;
        MaxSdkCallbacks.MRec.OnAdExpandedEvent += OnMRecAdExpandedEvent;
        MaxSdkCallbacks.MRec.OnAdCollapsedEvent += OnMRecAdCollapsedEvent;
    }

    private void LoadMrec()
    {
        MaxSdk.LoadMRec(GetMrecAdUnitId());
    }

    public void OnMRecAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Unity Log Mrec Loaded");
    }

    public void OnMRecAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo error)
    {
        Debug.Log("Unity Log Mrec Loaded Failed");
        LoadMrec();
    }

    public void OnMRecAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    public void OnMRecAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    public void OnMRecAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    public void OnMRecAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
    }

    public void ShowMRec()
    {
        if (!AdsKeysManager.IsMrecMaxEnable())
            return;
        GoogleMobileAdsManager.I.HideAllBanner();
        MaxSdk.ShowMRec(GetMrecAdUnitId());
    }

    public void HideMRec()
    {
        if (!AdsKeysManager.IsMrecMaxEnable())
            return;
        MaxSdk.HideMRec(GetMrecAdUnitId());
    }

    #endregion

    public void ShowMessage(string msg)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidJavaObject @static =
            new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject androidJavaObject = new AndroidJavaClass("android.widget.Toast");
        androidJavaObject.CallStatic<AndroidJavaObject>("makeText", new object[]
        {
            @static,
            msg,
            androidJavaObject.GetStatic<int>("LENGTH_SHORT")
        }).Call("show", Array.Empty<object>());
#endif
    }

    public bool CheckInternet()
    {
        return Application.internetReachability == NetworkReachability.NotReachable ? false : true;
    }

    public void RemoveAds()
    {
        PlayerPrefs.SetInt("NoAds", 1);
        HideAllBanner();
        StopCooldownAdsIngame();
    }

    private void LogAdEvent(string evName, string placement, string error)
    {
        if (AnalyticsManager.instance != null)
            AnalyticsManager.instance.LogAdsEvent(evName, placement, error);
    }

    WaitForSeconds delay = new WaitForSeconds(1);
    Coroutine coroutine_IngameAds;

    public void CooldownAdsIngame()
    {
        if (coroutine_IngameAds != null)
            StopCoroutine(coroutine_IngameAds);
        coroutine_IngameAds = StartCoroutine(C_CooldownAdsIngame());
    }

    public void StopCooldownAdsIngame()
    {
        if (coroutine_IngameAds != null)
            StopCoroutine(coroutine_IngameAds);
    }

    IEnumerator C_CooldownAdsIngame()
    {
        cooldown_interval = RemoteConfigControl.I.capping_ads.GetValue();

#if UNITY_IOS
        cooldown_interval = RemoteConfigControl.I.capping_ads.GetValue();
#endif

        while (true)
        {
            yield return delay;
            cooldown_interval -= 1;

            if (cooldown_interval == 5)
            {
                if (MaxSdk.IsInterstitialReady(GetInterstitialAdUnitId()))
                    actionIngameAnoyingAds?.Invoke(true);
                else
                    LoadInterstitial();
            }

            if (cooldown_interval == 0)
            {
                ShowAdUnit(() =>
                {
                    actionIngameAnoyingAds?.Invoke(false);
                    CooldownAdsIngame();
                }, "loop_ads");
            }
        }
    }

    public static void ShowBannerCollapsible(bool show = true)
    {
        if (show && AdsManager.I.IsAdsEnable())
        {
            GoogleMobileAdsManager.I.ShowBannerCollapsible();
        }
        else
        {
            GoogleMobileAdsManager.I.HideBannerCollapsible();
        }
    }

    public static void HideAndDestroyBannerCollapsible()
    {
        GoogleMobileAdsManager.I.HideAndDestroyBannerCollapse();
    }

    public static void LoadBannerCollapsible(bool showAfterLoadSuccess = true)
    {
        if (AdsManager.I.IsAdsEnable())
        {
            GoogleMobileAdsManager.I.LoadBannerCollapsible(showAfterLoadSuccess);
        }
    }

    public static void ShowBannerNormal(bool show = true)
    {
        if (show && AdsManager.I.IsAdsEnable())
        {
            GoogleMobileAdsManager.I.ShowBannerNormal();
        }
        else
        {
            GoogleMobileAdsManager.I.HideBannerNormal();
        }
    }

    public static void HideAllBanner()
    {
        GoogleMobileAdsManager.I.HideAllBanner();
        I.HideBanner();
    }

    public static bool IsAdmobBannerVisible()
    {
        if (!I.IsAdsEnable()){
            return false;
        }
        try
        {
            bool isBannerVisible = GoogleMobileAdsManager.I.IsBannerVisible();
            return isBannerVisible;
        } catch(System.Exception e) { }
        return false;
    }

    public enum AdsResult
    {
        Fail = 0, Success = 1
    }
}