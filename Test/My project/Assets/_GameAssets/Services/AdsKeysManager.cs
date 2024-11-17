using System.Collections;
using System.Collections.Generic;
using BaseMe;
using BaseService;
using UnityEngine;
public class AdsKeysManager : Singleton<AdsKeysManager>
{
    private const string BANNER_ID_ADMOB_COLLAPSIBLE_TEST_GAM = "/6499/example/adaptive-banner"; //Test
    private const string INTER_ID_ADMOB_TEST_GAM = "/6499/example/interstitial";
    private const string AOA_ID_ADMOB_TEST_GAM = "/6499/example/app-open";
    private const string NATIVE_ID_ADMOB_TEST_GAM = "/6499/example/native";
    #if UNITY_IOS
    private const string BANNER_ID_ADMOB_COLLAPSIBLE_TEST = "ca-app-pub-3940256099942544/2435281174"; //Test
    private const string INTER_ID_ADMOB_TEST = "ca-app-pub-3940256099942544/4411468910";
    private const string AOA_ID_ADMOB_TEST = "ca-app-pub-3940256099942544/5575463023";
    private const string NATIVE_ID_ADMOB_TEST = "ca-app-pub-3940256099942544/3986624511";
    #else 
    private const string BANNER_ID_ADMOB_COLLAPSIBLE_TEST = "ca-app-pub-3940256099942544/9214589741"; //Test
    private const string INTER_ID_ADMOB_TEST = "ca-app-pub-3940256099942544/1033173712";
    private const string AOA_ID_ADMOB_TEST = "ca-app-pub-3940256099942544/9257395921";
    private const string NATIVE_ID_ADMOB_TEST = "ca-app-pub-3940256099942544/2247696110";
    #endif

    [Header("Applovin Max")]
    [TextArea(4, 4)]
    [SerializeField]
    private string maxSdkKey =
        "yiGW3A3cZmhIPRTXzn3UF_1PDfTGOTAT2f09uBrPJDJe4EGiuwN8tmDEKAIG2HpuEPGUWQHbxDTyP8k6Utsi3M"; //

    [Header("Test mode mediation for google")] [SerializeField]
    private GoogleAdTestMode keyTestMode = GoogleAdTestMode.Admob;
    
    public static string MaxSdkKey => I.maxSdkKey;

    [Header("Max Android")]
    [SerializeField]
    private string interstitialMaxId = "";
    [SerializeField]
    private string rewardedMaxId = "";
    [SerializeField]
    private string bannerMaxId = "";
    [SerializeField]
    private string mrecMaxId = "";

    [Header("Google Admob Android")]
    [SerializeField]
    public string BANNER_ID_ADMOB = "";
    [SerializeField]
    public string BANNER_ID_ADMOB_COLLAPSIBLE = "";

    [SerializeField]
    public string INTER_ID_ADMOB = "";

    [SerializeField]
    public string AOA_ID_ADMOB = "";
    
    [SerializeField]
    public string NATIVE_ID_ADMOB = "";

    [Header("Google Admob IOS")]
    [SerializeField]
    public string BANNER_ID_ADMOB_IOS = "";
    [SerializeField]
    public string BANNER_ID_ADMOB_COLLAPSIBLE_IOS = "";

    [SerializeField]
    public string INTER_ID_ADMOB_IOS = "";

    [SerializeField]
    public string AOA_ID_ADMOB_IOS = "";
    
    [SerializeField]
    public string NATIVE_ID_ADMOB_IOS = "";

    [Header("Max IOS")]
    [SerializeField]
    private string interstitialMaxIdIOS = "";
    [SerializeField]
    private string rewardedMaxIdIOS = "";
    [SerializeField]
    private string bannerMaxIdIOS = "";
    [SerializeField]
    private string mrecMaxIdIOS = "";

#if UNITY_IOS
    public static string GetMaxInterId() => I.interstitialMaxIdIOS;
    public static string GetMaxRewardId() => I.rewardedMaxIdIOS;
    public static string GetMaxBannerId() => I.bannerMaxIdIOS;
    public static string GetMaxMrecId() => I.mrecMaxIdIOS;

    public static bool IsMrecMaxEnable() => I.mrecMaxIdIOS.Length > 0;
    public static bool IsBannerMaxEnable() => I.bannerMaxIdIOS.Length > 0;
    public static bool IsRewardMaxEnable() => I.rewardedMaxIdIOS.Length > 0;
    public static bool IsAOAEnable() => I.AOA_ID_ADMOB_IOS.Length > 0;
    public static bool IsNativeEnable() => I.NATIVE_ID_ADMOB_IOS.Length > 0;
#else

    public static string GetMaxInterId() => I.interstitialMaxId;
    public static string GetMaxRewardId() => I.rewardedMaxId;
    public static string GetMaxBannerId() => I.bannerMaxId;
    public static string GetMaxMrecId() => I.mrecMaxId;

    public static bool IsMrecMaxEnable() => I.mrecMaxId.Length > 0;
    public static bool IsBannerMaxEnable() => I.bannerMaxId.Length > 0;
    public static bool IsRewardMaxEnable() => I.rewardedMaxId.Length > 0 && RemoteConfigControl.I.reward_enable.GetValue()==1;
    public static bool IsAOAEnable() => I.AOA_ID_ADMOB.Length > 0;
    public static bool IsNativeEnable() => I.NATIVE_ID_ADMOB.Length > 0;
#endif


    [SerializeField]
    private AdsMode adsMode = AdsMode.Enable;

    public static AdsMode GetMode() => I.adsMode;

    public static bool IsTestMode() => I.adsMode == AdsMode.TestMode;
    public static bool IsNoAds() => I.adsMode == AdsMode.Disable;


#if UNITY_IOS

    public static string GetBannerAdMobId()
    {
        return !IsTestMode() ? I.BANNER_ID_ADMOB_IOS : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?BANNER_ID_ADMOB_COLLAPSIBLE_TEST_GAM:BANNER_ID_ADMOB_COLLAPSIBLE_TEST);
    }

    public static string GetBannerCollapsibleAdMobId()
    {
        return !IsTestMode() ? I.BANNER_ID_ADMOB_IOS : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?BANNER_ID_ADMOB_COLLAPSIBLE_TEST_GAM:BANNER_ID_ADMOB_COLLAPSIBLE_TEST);
    }

    public static string GetInterstitialAdMobId()
    {
        return !IsTestMode() ? I.INTER_ID_ADMOB_IOS : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?INTER_ID_ADMOB_TEST_GAM:INTER_ID_ADMOB_TEST);
    }

    public static string GetAOAAdMobId()
    {
        return !IsTestMode() ? I.AOA_ID_ADMOB_IOS : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?AOA_ID_ADMOB_TEST_GAM:AOA_ID_ADMOB_TEST);
    }
    
    public static string GetNativeAdmobAdMobId()
    {
        return !IsTestMode() ? I.NATIVE_ID_ADMOB_IOS : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?NATIVE_ID_ADMOB_TEST_GAM:NATIVE_ID_ADMOB_TEST);
    }
    
#else
    public static string GetBannerAdMobId()
    {
        return !IsTestMode() ? I.BANNER_ID_ADMOB : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?BANNER_ID_ADMOB_COLLAPSIBLE_TEST_GAM:BANNER_ID_ADMOB_COLLAPSIBLE_TEST);
    }

    public static string GetBannerCollapsibleAdMobId()
    {
        return !IsTestMode() ? I.BANNER_ID_ADMOB : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?BANNER_ID_ADMOB_COLLAPSIBLE_TEST_GAM:BANNER_ID_ADMOB_COLLAPSIBLE_TEST);
    }

    public static string GetInterstitialAdMobId()
    {
        return !IsTestMode() ? I.INTER_ID_ADMOB : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?INTER_ID_ADMOB_TEST_GAM:INTER_ID_ADMOB_TEST);
    }

    public static string GetAOAAdMobId()
    {
        return !IsTestMode() ? I.AOA_ID_ADMOB : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?AOA_ID_ADMOB_TEST_GAM:AOA_ID_ADMOB_TEST);
    }
    
    public static string GetNativeAdmobAdMobId()
    {
        return !IsTestMode() ? I.NATIVE_ID_ADMOB : (I.keyTestMode==GoogleAdTestMode.GoogleAdManager?NATIVE_ID_ADMOB_TEST_GAM:NATIVE_ID_ADMOB_TEST);
    }

#endif

    public void SetIdAdmob(AdmobId platformId)
    {
        AOA_ID_ADMOB = platformId.aoa;
        INTER_ID_ADMOB = platformId.inter;
        BANNER_ID_ADMOB = platformId.banner;
        BANNER_ID_ADMOB_COLLAPSIBLE = platformId.banner;
        NATIVE_ID_ADMOB = platformId.native;

        AOA_ID_ADMOB_IOS = platformId.aoa;
        INTER_ID_ADMOB_IOS = platformId.inter;
        BANNER_ID_ADMOB_IOS = platformId.banner;
        BANNER_ID_ADMOB_COLLAPSIBLE_IOS = platformId.banner;
        NATIVE_ID_ADMOB_IOS = platformId.native;
    }
    
    public class AdmobId
    {
        public string inter;
        public string banner;
        public string aoa;
        public string native;
    }

    public enum AdsMode
    {
        Enable, Disable, TestMode
    }

    public enum GoogleAdTestMode
    {
        Admob, GoogleAdManager
    }
}
