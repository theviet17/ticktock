using System;
using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using BaseMe;
using GoogleMobileAds.Api.AdManager;
using BaseService;

public class GoogleMobileAdsManager : Singleton<GoogleMobileAdsManager>
{
    // Start is called before the first frame update

    [SerializeField] private bool enableAdmobBanner = true;

    [SerializeField] private bool loadBannerWhenStart = true;

    [SerializeField] private bool AutoInitializeWhenStart = true;
    
    private InterstitialAd _interstitialAd;
    private int interstitialRetryAttempt;
    static Action actionFullAds;
    private bool doneAdsInterval = true; // True đã countdown xong và cho phép hiển thị, false thì chưa
    public bool affectedToAOA = false;
    public bool lastTimeShowAsBackFill = false;

    private bool isBannerNormalShow = false;
    private bool isBannerCollapseShow = false;

    private Dictionary<string, BannerView> mapBanner = new();

    public UnityEvent<bool> OnBannerStateChanged = new();

    protected override void OnRegisterInstance()
    {
        if (AutoInitializeWhenStart)
        {
            InitializeEngineAds();
        }
    }

    public void InitializeEngineAds()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus =>
        {
            if (enableAdmobBanner && loadBannerWhenStart)
            {
                LoadBanner();
            }
            RequestAndLoadInterstitialAd();
            try
            {
                if (AdsManager.I.IsAdsEnable())
                {
                    AppOpenAdManager.Instance.LoadAd();
                }
            }
            catch(System.Exception e)
            {
                Debug.Log("Unity AOA INit error: " + e);
            }
        });

        RequestAndLoadInterstitialAd();
    }

    void SetAdsInterval()
    {
        doneAdsInterval = false;
        StartCoroutine(WaitEndAdsInterval());
    }

    IEnumerator WaitEndAdsInterval()
    {
        yield return new WaitForSecondsRealtime(RemoteConfigControl.I.capping_ads.GetValue());
        doneAdsInterval = true;
    }

    #region Banner

    private BannerView  bannerView;
    private BannerView  bannerViewNormal;

    private const string KEY_BANNER_NORMAL = "Normal";
    private const string KEY_BANNER_COLLAPSE = "Collapse";

    public void LoadBanner()
    {
        LoadBannerCollapsible();
        LoadBannerNormal();
    }

    public void LoadBannerCollapsible(bool showAfterLoadSuccess = false)//, string key = null)
    {
        if (AdsKeysManager.IsNoAds() || RemoteConfigControl.I.banner_enable.GetValue()!=1)
        {
            return;
        }

        bool forceShowMaxBanner = false;
        if (!forceShowMaxBanner)
        {
            Debug.Log($"GoogleMobileAdsManager Start LoadBannerCollapsible: {AdsKeysManager.GetBannerAdMobId()}");
            var bannerID = AdsKeysManager.GetBannerCollapsibleAdMobId();

            AdSize adaptiveSize = //AdSize.SmartBanner;
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            AdRequest request = new AdRequest();
            request.Extras.Add("collapsible", "bottom");
            //request.Extras.Add("collapsible_request_id", System.Guid.NewGuid().ToString());
            if (bannerView != null && !bannerView.IsDestroyed)
            {
                HideAndDestroyBannerCollapse();//HideAndDestroyBanner(fixKey);
            }
            this.bannerView = new BannerView(bannerID, adaptiveSize, AdPosition.Bottom);
            this.bannerView.LoadAd(request);
            bannerView.OnAdPaid += adValue => { LogcatUtils.TrackPaidAdEventAdmob(adValue, "collapsible",bannerView.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName); };
            bannerView.OnBannerAdLoadFailed += error =>
            {
                Debug.Log($"GoogleMobileAdsManager LoadBannerCollapsible Fail: {error.ToString()}");
                if (showAfterLoadSuccess)
                    AdsManager.I.ShowBanner();
            };
            bannerView.OnBannerAdLoaded += () =>
            {
            
            };
            if (!showAfterLoadSuccess)
            {
                HideBannerCollapsible();
            }
            else
            {
                isBannerCollapseShow = true;
                CheckBannerStateChanged();
            }
        }
        else
        {
            
        }
        

        if (showAfterLoadSuccess && forceShowMaxBanner)
        {
            AdsManager.I.ShowBanner();
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));

            LogcatUtils.TrackPaidAdEventAdmob(adValue, "inter",interstitialAd.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName);
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            //Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () => { Debug.Log("Interstitial ad was clicked."); };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            AppOpenAdManager.Instance.pauseByShowingInter = true;
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            if (affectedToAOA)
                AdsManager.I.canShowAppOpenAds = true;
            if (actionFullAds != null)
            {
                actionFullAds.Invoke();
                actionFullAds = null;

                AdsManager.I.SetAdsInterval();
                SetAdsInterval();
            }

            Invoke("RequestAndLoadInterstitialAd", 0.5f);
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            if (affectedToAOA)
                AdsManager.I.canShowAppOpenAds = true;
            if (actionFullAds != null)
            {
                actionFullAds.Invoke();
                actionFullAds = null;

                if (!lastTimeShowAsBackFill)
                    SetAdsInterval();
            }

            Invoke("RequestAndLoadInterstitialAd", 0.5f);
        };
    }

    private string GetKeyBannerWithTypeCollapse(string key)
    { 
        return $"{key}_{KEY_BANNER_COLLAPSE}";
    }

    public void HideAndDestroyBannerCollapse()//string key)
    {
        if (AdsKeysManager.IsNoAds())
        {
            return;
        }
        HideBannerCollapsible();
        if (bannerView!=null && !bannerView.IsDestroyed)
        {
            bannerView.Hide();
            bannerView.Destroy();
        }
        
    }

    public void LoadBannerNormal(bool showAfterLoadSuccess = false)//, string key = null)
    {
        if (AdsKeysManager.IsNoAds())
        {
            return;
        }

        bool forceShowMaxBanner = false;
        if (!forceShowMaxBanner)
        {
            Debug.Log($"GoogleMobileAdsManager Start LoadBannerCollapsible: {AdsKeysManager.GetBannerAdMobId()}");
            var bannerID = AdsKeysManager.GetBannerCollapsibleAdMobId();

            AdSize adaptiveSize = //AdSize.SmartBanner;
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
            AdRequest request = new AdRequest();
            //request.Extras.Add("collapsible_request_id", System.Guid.NewGuid().ToString());
            if (bannerViewNormal != null && !bannerViewNormal.IsDestroyed)
            {
                HideAndDestroyBannerNormal();
            }
            this.bannerViewNormal = new BannerView(bannerID, adaptiveSize, AdPosition.Bottom);
            this.bannerViewNormal.LoadAd(request);
            bannerViewNormal.OnAdPaid += adValue => { LogcatUtils.TrackPaidAdEventAdmob(adValue, "banner",bannerViewNormal.GetResponseInfo().GetLoadedAdapterResponseInfo().AdSourceName); };
            bannerViewNormal.OnBannerAdLoadFailed += error =>
            {
                Debug.Log($"GoogleMobileAdsManager LoadBannerCollapsible Fail: {error.ToString()}");
                if (showAfterLoadSuccess)
                    AdsManager.I.ShowBanner();
            };
            bannerViewNormal.OnBannerAdLoaded += () =>
            {
            
            };
            if (!showAfterLoadSuccess)
            {
                HideBannerNormal();
            }
            else
            {
                isBannerNormalShow = true;
                CheckBannerStateChanged();
            }
        }
        else
        {
            
        }
        

        if (showAfterLoadSuccess && forceShowMaxBanner)
        {
            AdsManager.I.ShowBanner();
        }
    }
    
    public void HideAndDestroyBannerNormal()
    {
        if (AdsKeysManager.IsNoAds())
        {
            return;
        }
        HideBannerCollapsible();
        if (bannerViewNormal!=null && !bannerViewNormal.IsDestroyed)
        {
            bannerViewNormal.Hide();
            bannerViewNormal.Destroy();
        }
        
    }

    public void ShowBannerCollapsible()
    {
        if (AdsKeysManager.IsNoAds())
        {
            return;
        }
        HideBannerNormal();
        try
        {
            this.bannerView.Show();
            isBannerCollapseShow = true;
        }
        catch (System.Exception e)
        {
        }
        CheckBannerStateChanged();
    }

    public void ShowBannerNormal()
    {
        if (AdsKeysManager.IsNoAds())
        {
            return;
        }
        HideBannerCollapsible();
        try
        {
            this.bannerViewNormal.Show();
            isBannerNormalShow = true;
        }
        catch (System.Exception e)
        {
        }
        CheckBannerStateChanged();
    }

    private void CheckBannerStateChanged()
    {
        if (IsBannerVisible())
        {
            OnBannerStateChanged.Invoke(true);
        }
        else
        {
            OnBannerStateChanged.Invoke(false);
        }
    }

    public void HideBannerCollapsible(BannerView b = null)
    {
        isBannerCollapseShow = false;
        if (AdsKeysManager.IsNoAds())
        {
            return;
        }
        try
        {
            if (b==null)
                this.bannerView.Hide();
            else
                b.Hide();
        }
        catch (System.Exception e)
        {
        }
        CheckBannerStateChanged();
    }

    public void HideBannerNormal()
    {
        isBannerNormalShow = false;
        if (AdsKeysManager.IsNoAds())
        {
            return;
        }
        try
        {
            this.bannerViewNormal.Hide();
        }
        catch (System.Exception e)
        {
        }
        CheckBannerStateChanged();
    }

    public void HideAllBanner()
    {
        HideBannerNormal();
        HideBannerCollapsible();
    }

    public void OnlyHideDisplayOfBannerForShowBannerMax()
    {
        this.bannerViewNormal?.Hide();
        this.bannerView?.Hide();
    }

    public bool IsBannerVisible()
    {
        return isBannerCollapseShow || isBannerNormalShow;
    }

    #endregion

    #region inter

    public void RequestAndLoadInterstitialAd()
    {
        // Debug.LogError("===gm Requesting Interstitial ad.");

        string adUnitId = AdsKeysManager.GetInterstitialAdMobId();

        // Clean up interstitial before using it
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }


        AdRequest adRequest = new AdRequest();
        // Load an interstitial ad
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialRetryAttempt = 0;
                _interstitialAd = ad;
                RegisterEventHandlers(_interstitialAd);
            });
    }


    public void ShowInterstitialAd(Action actionDone, bool showAsBackfill = false)
    {
        lastTimeShowAsBackFill = showAsBackfill;
        if (showAsBackfill)
        {
            doneAdsInterval = true;
            Debug.Log("GoogleMobileAdsManager Show Backfill Inter");
        }

        if (doneAdsInterval)
        {
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                //Debug.LogErrorFormat("===gm  ad ready, show ad");
                if (!showAsBackfill)
                    doneAdsInterval = false;
                actionFullAds = actionDone;
                _interstitialAd.Show();
            }
            else
            {
                //Debug.LogErrorFormat("===gm  ad not ready");
                if (affectedToAOA)
                    AdsManager.I.canShowAppOpenAds = true;
                actionDone.Invoke();
            }
        }
        else
        {
            //Debug.LogErrorFormat(" ===gm  miss time");
            Debug.Log($"GoogleMobileAdsManager Show Backfill Inter, Backfill: {showAsBackfill}");
              if (affectedToAOA)
          if (affectedToAOA)
                AdsManager.I.canShowAppOpenAds = true;
            actionDone.Invoke();
        }
    }

    public void ShowInterAdsWithChecking(System.Action<AdsManager.AdsResult> action)
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            //Debug.LogErrorFormat("===gm  ad ready, show ad");
            actionFullAds = () =>
            {
                action.Invoke(AdsManager.AdsResult.Success);
            };
            _interstitialAd.Show();
        }
        else
        {
            if (affectedToAOA)
                AdsManager.I.canShowAppOpenAds = true;
            action.Invoke(AdsManager.AdsResult.Fail);
        }
    }

    public void DestroyInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
        }
    }

    #endregion

    #region Utility

    ///<summary>
    /// Log the message and update the status text on the main thread.
    ///<summary>
    private void PrintStatus(string message)
    {
        Debug.LogError(message);
    }

    #endregion


}