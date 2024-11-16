using System;
using System.Collections;
using System.Collections.Generic;
using AdjustSdk;
using Firebase.Analytics;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Events;

namespace BaseService
{
    public class LogcatUtils
    {
        public static LevelData currentLevelData;

        public static void TrackPaidAdEventAdmob(AdValue adValue, string adFormat, string adSourceName)
        {
            try
            {
                List<Parameter> AdRevenueParameters = new List<Parameter>()
                {
                    new Parameter(FirebaseAnalytics.ParameterValue, Convert.ToDouble(adValue.Value) / 1000000f),
                    new Parameter(FirebaseAnalytics.ParameterCurrency, "USD"),
                    new Parameter("ad_format",
                        adFormat), // adFormat: collapsible, banner, inter, reward, app_open, native, rewarded_inter, mrec, audio
                    // not required (these are for level analytics)
                    // new Parameter(FirebaseAnalytics.ParameterLevel, GameController.instance.currentOpenLevel),
                    // new Parameter("level_mode", currentMode.ToString())
                };

                FirebaseAnalytics.LogEvent("ad_impression_admob", AdRevenueParameters.ToArray());
            
                if (currentLevelData != null)
                {
                    var paramLevel = new Parameter(FirebaseAnalytics.ParameterLevel, currentLevelData.currentOpenLevel);
                    AdRevenueParameters.Add(paramLevel);
                }

                FirebaseAnalytics.LogEvent("ad_revenue_sdk", AdRevenueParameters.ToArray());

                AdjustAdRevenue adRevenue = new AdjustAdRevenue("admob_sdk");
                adRevenue.SetRevenue(adValue.Value / 1000000f, adValue.CurrencyCode);
                adRevenue.AdRevenueNetwork = adSourceName;
                Adjust.TrackAdRevenue(adRevenue);
            }
            catch (Exception e)
            {
                
            }
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
                
                if (currentLevelData != null)
                {
                    var paramLevel = new Parameter(FirebaseAnalytics.ParameterLevel, currentLevelData.currentOpenLevel);
                    AdRevenueParameters.Add(paramLevel);
                }
                FirebaseAnalytics.LogEvent("ad_revenue_sdk", AdRevenueParameters.ToArray());
            }
            catch (Exception e)
            {
            }

            try
            {
                AdjustAdRevenue adRevenue = new AdjustAdRevenue("applovin_max_sdk");
                adRevenue.SetRevenue(impressionData.Revenue, "USD");
                adRevenue.AdRevenueNetwork = impressionData.NetworkName;
                adRevenue.AdRevenueUnit = impressionData.AdUnitIdentifier;
                adRevenue.AdRevenuePlacement = impressionData.Placement;
                Adjust.TrackAdRevenue(adRevenue);
            }
            catch (Exception e)
            {
                
            }
        }
    
        public static void TrackPaidAdEventApplovinMax(string adUnitId)
        {
            var info = MaxSdk.GetAdInfo(adUnitId);
            TrackPaidAdEventApplovinMax((info));
        }
        
        private static void LogAdjust(string token)
        {
            try
            {
                var adjustEvent = new AdjustEvent(token);
                Adjust.TrackEvent(adjustEvent);
            }
            catch(System.Exception e)
            {
                Debug.Log($"AdsUtilsLog: Log Adjust Error: {e}");
            }
            
        }

        public static void LogAdjust(AdjustKeysManager.EventType typeEvent)
        {
            if (AdjustKeysManager.InstanceIsValid())
            {
                var key = AdjustKeysManager.I.GetToken(typeEvent);
                Debug.Log($"AdsUtilsLog: Log Event: {typeEvent} with token: {key.Length}");
                if (key.Length > 0)
                {
                    LogAdjust(key);
                }
            }
            else
            {
                Debug.Log("AdsUtilsLog: AdjustKeyManager not init");
            }
            
        }
        
        public static void SetCurrentOpenLevel(int level, string mode = "", bool success = true)
        {
            currentLevelData = new LevelData(level, mode, success);
        }

        public static void LogStartLevel(int startLevel, string mode = "")
        {
            try
            {
                SetCurrentOpenLevel(startLevel, mode);
                List<Parameter> LevelStartParameters = new List<Parameter>{ new Parameter(FirebaseAnalytics.ParameterLevel, currentLevelData.currentOpenLevel.ToString())};
                if (currentLevelData.currentMode.Length > 0)
                {
                    LevelStartParameters.Add(new Parameter("level_mode", currentLevelData.currentMode.ToString()));
                }
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart, LevelStartParameters.ToArray());
            }
            catch (System.Exception e)
            {
                Debug.Log($"AdsUtils Log Error: {e} " );
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
                Debug.Log($"AdsUtils Log Error: {e} " );
            }
            
        }
        
    }
    
    public class LevelData
    {
        public int currentOpenLevel;
        public string currentMode = "";
        public bool success = true;

        public LevelData(int level)
        {
            this.currentOpenLevel = level;
        }

        public LevelData(int level, string mode)
        {
            this.currentOpenLevel = level;
            this.currentMode = mode;
        }
        
        public LevelData(int level, string mode, bool success)
        {
            this.currentOpenLevel = level;
            this.currentMode = mode;
            this.success = success;
        }
    }

    public static class AdsUtils
    {
        public static void ShowInterstitialAdsWithBackFill(System.Action action)
        {
            AdsManager.ShowInterAds(action);
        }

        public static void LoadAndShowBannerCollapsible(bool showAfterLoadSuccess)
        {
            GoogleMobileAdsManager.I.LoadBannerCollapsible(showAfterLoadSuccess);
        }
        
        public static void LoadAndShowBannerNormal(bool showAfterLoadSuccess)
        {
            GoogleMobileAdsManager.I.LoadBannerNormal(showAfterLoadSuccess);
        }
        
        public static void HideAndDestroyBannerCollapsible()
        {
            GoogleMobileAdsManager.I.HideBannerCollapsible();
        }
        
        public static void HideAndDestroyBannerNormal()
        {
            GoogleMobileAdsManager.I.HideBannerNormal();
        }

        public static void HideAllBanner()
        {
            AdsManager.HideAllBanner();
        }

        public static void RegisterBannerStateChanged(this MonoBehaviour monoBehaviour, UnityAction<bool> bannerChanged, bool checkEventImmediately = true)
        {
            monoBehaviour.StartCoroutine(WaitForCondition(GoogleMobileAdsManager.InstanceIsValid, () =>
            {
                GoogleMobileAdsManager.I.OnBannerStateChanged.AddListener(bannerChanged);
                if (checkEventImmediately)
                {
                    bannerChanged.Invoke(GoogleMobileAdsManager.I.IsBannerVisible());
                }
            }));
        }

        public static void UnRegisterBannerStateChanged(this MonoBehaviour monoBehaviour, UnityAction<bool> bannerChanged)
        {
            try
            {
                GoogleMobileAdsManager.I.OnBannerStateChanged.RemoveListener(bannerChanged);
            }
            catch (System.NullReferenceException e)
            {
                
            }
            catch (System.Exception e)
            {
                
            }
        }

        public static IEnumerator WaitForCondition(Func<bool> condition, System.Action action)
        {
            while (!condition())
            {
                yield return null;
            }

            // Thực thi hành động khi điều kiện được thỏa mãn
            action.Invoke();
        }

        public static void ShowRewardWithBackFill(System.Action<AdsManager.AdsResult> action)
        {
            AdsManager.I.ShowRewardVideo(action);
        }
        
    }
}

