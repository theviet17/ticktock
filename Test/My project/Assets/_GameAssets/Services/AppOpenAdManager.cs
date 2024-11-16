using System;
using BaseService;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using GoogleMobileAds.Api.AdManager;
using UnityEngine;

public class AppOpenAdManager
{

    private static AppOpenAdManager instance;

    private AppOpenAd appOpenAd;

    private bool isShowingAd = false;

    public bool pauseByShowingInter = false;

    public static bool isLoadingSplash = false;

    static Action actionFullAds;

    public static AppOpenAdManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AppOpenAdManager();
            }

            return instance;
        }
    }

    private bool IsAdAvailable
    {
        get { return appOpenAd != null; }
    }


    public void LoadAd()
    {
        AdManagerAdRequest request = new AdManagerAdRequest();

        string idAdmob = AdsKeysManager.GetAOAAdMobId();

        // Load an app open ad for portrait orientation
        AppOpenAd.Load(idAdmob, request, ((ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Unity Log App open ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log("Unity Log App open ad loaded with response : "
                      + ad.GetResponseInfo());

            appOpenAd = ad;
            RegisterEventHandlers(ad);
        }));
    }

    public void ShowWhenStartIfNecessary(Action actionDone)
    {
        try
        {
            if (RemoteConfigControl.I.showAOAWhenStart.GetValue() == 1)
            {
                ShowAdIfAvailable(actionDone);   
            }
            else
            {
                actionDone.Invoke();
            }
        }
        catch (System.Exception e)
        {
            actionDone.Invoke();
        }
    }

    public void ShowAdIfAvailable(Action actionDone)
    {
        RunInMainThread(() =>
        {
            Debug.LogFormat("+===============ShowAdIfAvailable: {0}", RemoteConfigControl.I.appOpenAds.GetValue());
            // #if UNITY_EDITOR
            //             actionDone();
            //             return;
            // #else

#if UNITY_ANDROID
            if (RemoteConfigControl.I.appOpenAds.GetValue() == 0 || !AdsManager.I.canShowAppOpenAds || !AdsManager.I.IsAdsEnable())
            {
                Debug.Log("Unity AOA Log Not Show AppOpen Ads");
                RemoteConfigControl.I.isStart = false;
                actionDone.Invoke();
                return;
            }
#elif UNITY_IOS
            if (RemoteConfigControl.I.appOpenAds.GetValue() == 0 || !AdsManager.I.canShowAppOpenAds || !AdsManager.I.IsAdsEnable())
        {
            RemoteConfigControl.I.isStart = false;
            actionDone.Invoke();
            return;
        }
#endif
            if (!IsAdAvailable)
            {
                LoadAd();
                Debug.Log("Unity AOA not available");
            }

            if (!IsAdAvailable || isShowingAd || PlayerPrefs.GetInt("NoAds", 0) != 0)
            {
                Debug.Log("Unity AOA not show");
                actionDone.Invoke();
                return;
            }

            if (appOpenAd.CanShowAd())
            {
                actionFullAds = actionDone;
                appOpenAd.Show();
            }
            else
            {
                actionDone.Invoke();
            }
// #endif
        });
    }

    private async void RunInMainThread(Action action)
    {
        await UniTask.SwitchToMainThread();
        action.Invoke();
    }

    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            // if (actionFullAds != null)
            // {
            //     actionFullAds.Invoke();
            //     actionFullAds = null;
            // }

            Debug.Log(String.Format("Unity AOA App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));

            LogcatUtils.TrackPaidAdEventAdmob(adValue, "app_open", ad.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName);
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () => { Debug.Log("Unity AOA App open ad recorded an impression."); };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () => { Debug.Log("Unity AOA App open ad was clicked."); };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            isShowingAd = true;

            // if (actionFullAds != null)
            // {
            //     actionFullAds.Invoke();
            //     actionFullAds = null;
            // }

            Debug.Log("Unity AOA App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            ad = null;
            isShowingAd = false;
            LoadAd();
            if (actionFullAds != null)
            {
                actionFullAds.Invoke();
                actionFullAds = null;
            }

            Debug.Log("Unity AOA App open ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Unity AOA App open ad failed to open full screen content " +
                           "with error : " + error);
            ad = null;
            LoadAd();
            if (actionFullAds != null)
            {
                actionFullAds.Invoke();
                actionFullAds = null;
            }
        };
    }
}