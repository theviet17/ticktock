using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using System;
using System.Threading.Tasks;

namespace BaseService
{
    public class RemoteConfigControl : MonoBehaviour
    {
        public static RemoteConfigControl I;
        public Action OnFetchDone;
        public bool isDataFetched = false;

        public PlatformRemoteConfigKey appOpenAds = new("aoa_enable", 1);
        public PlatformRemoteConfigKey showAOAWhenStart = new("showAOAWhenStart", 1);
        public PlatformRemoteConfigKey capping_ads = new("capping_ads", 15);
        public PlatformRemoteConfigKey inter_enable = new("inter_enable", 1);
        public PlatformRemoteConfigKey banner_enable = new("banner_enable", 1);
        public PlatformRemoteConfigKey reward_enable = new("reward_enable", 1);
        public PlatformRemoteConfigKey mrec_enable = new("mrec_enable", 1);
        public PlatformRemoteConfigKey native_enable = new("native_enable", 1);
        
        public PlatformRemoteStringConfigKey id_banner = new("id_banner", "");
        public PlatformRemoteStringConfigKey id_inter = new("id_inter", "");
        public PlatformRemoteStringConfigKey id_aoa = new("id_aoa", "");
        public PlatformRemoteStringConfigKey id_native = new("id_native", "");
        

        public bool isStart = true;

        protected bool isFirebaseInitialized = false;

        void Awake()
        {
            I = this;
        }

        public void InitializeFirebase()
        {
            Debug.Log("UnityRemoteCF RemoteConfig Initializing!");
            LoadData();
            Debug.Log("UnityRemoteCF RemoteConfig LoadFrom PlayerPref!");
            Dictionary<string, object> defaults = new Dictionary<string, object>
              {
                  { appOpenAds.GetKey(), appOpenAds.GetValue() },
                  { showAOAWhenStart.GetKey(), showAOAWhenStart.GetValue() },
                  { capping_ads.GetKey(), capping_ads.GetValue() },
                  { inter_enable.GetKey(), inter_enable.GetValue() },
                  { banner_enable.GetKey(), banner_enable.GetValue() },
                  { reward_enable.GetKey(), reward_enable.GetValue() },
                  { mrec_enable.GetKey(), mrec_enable.GetValue() },
                  { native_enable.GetKey(), native_enable.GetValue() },
                  { id_banner.GetKey(), id_banner.GetValue() },
                  { id_inter.GetKey(), id_inter.GetValue() },
                  { id_aoa.GetKey(), id_aoa.GetValue() },
                  { id_native.GetKey(), id_native.GetValue() },
              };

            Debug.Log("UnityRemoteCF RemoteConfig Doing Set Default To Server!");
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            Debug.Log("UnityRemoteCF RemoteConfig Done Set Default To Server!");

            Debug.Log("UnityRemoteCF RemoteConfig configured and ready!");

            isFirebaseInitialized = true;
            FetchDataAsync();
        }

        public void FetchDataAsync()
        {
            Debug.Log("UnityRemoteCF Fetching data...");
            System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            fetchTask.ContinueWithOnMainThread(FetchComplete);
        }

        void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                Debug.Log("UnityRemoteCF Fetch canceled.");
            }
            else if (fetchTask.IsFaulted)
            {
                Debug.Log("UnityRemoteCF Fetch encountered an error.");
            }
            else if (fetchTask.IsCompleted)
            {
                Debug.Log("UnityRemoteCF Fetch completed successfully!");
            }

            var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
            switch (info.LastFetchStatus)
            {
                case Firebase.RemoteConfig.LastFetchStatus.Success:
                    Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
                    Debug.Log(String.Format("UnityRemoteCF Remote data loaded and ready (last fetch time {0}).",
                                           info.FetchTime));
                    ReflectProperties();
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Failure:
                    switch (info.LastFetchFailureReason)
                    {
                        case Firebase.RemoteConfig.FetchFailureReason.Error:
                            Debug.Log("UnityRemoteCF Fetch failed for unknown reason");
                            break;
                        case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                            Debug.Log("UnityRemoteCF Fetch throttled until " + info.ThrottledEndTime);
                            break;
                    }
                    FetchDataAsync();
                    break;
                case Firebase.RemoteConfig.LastFetchStatus.Pending:
                    Debug.Log("UnityRemoteCF Latest Fetch call still pending.");
                    break;
            }

            AdsKeysManager.AdmobId platformId = new AdsKeysManager.AdmobId();
            platformId.aoa = id_aoa.GetValue();
            platformId.inter = id_inter.GetValue();
            platformId.banner = id_banner.GetValue();
            platformId.native = id_native.GetValue();

            Debug.Log("Fetch Data Complete And SEt id Admob");
            
            AdsKeysManager.I.SetIdAdmob(platformId);
            GoogleMobileAdsManager.I.InitializeEngineAds();
            
            if (OnFetchDone != null) OnFetchDone.Invoke();
        }

        private void ReflectProperties()
        {
            appOpenAds.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(appOpenAds.GetKey()).DoubleValue);
            showAOAWhenStart.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(showAOAWhenStart.GetKey()).DoubleValue);
            capping_ads.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(capping_ads.GetKey()).DoubleValue);
            inter_enable.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(inter_enable.GetKey()).DoubleValue);
            banner_enable.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(banner_enable.GetKey()).DoubleValue);
            reward_enable.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(reward_enable.GetKey()).DoubleValue);
            mrec_enable.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(mrec_enable.GetKey()).DoubleValue);
            native_enable.SetValue((int)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(native_enable.GetKey()).DoubleValue);
            id_inter.SetValue((string)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(id_inter.GetKey()).StringValue);
            id_banner.SetValue((string)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(id_banner.GetKey()).StringValue);
            id_aoa.SetValue((string)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(id_aoa.GetKey()).StringValue);
            id_native.SetValue((string)Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.GetValue(id_native.GetKey()).StringValue);
            SaveData();
            isDataFetched = true;
        }

        // save cache data
        private void SaveData()
        {
            PlayerPrefs.SetInt(appOpenAds.GetKey(), appOpenAds.GetValue());
            PlayerPrefs.SetInt(showAOAWhenStart.GetKey(), showAOAWhenStart.GetValue());
            PlayerPrefs.SetInt(capping_ads.GetKey(), capping_ads.GetValue());
            PlayerPrefs.SetInt(inter_enable.GetKey(), inter_enable.GetValue());
            PlayerPrefs.SetInt(banner_enable.GetKey(), banner_enable.GetValue());
            PlayerPrefs.SetInt(reward_enable.GetKey(), reward_enable.GetValue());
            PlayerPrefs.SetInt(native_enable.GetKey(), native_enable.GetValue());
            PlayerPrefs.SetInt(mrec_enable.GetKey(), mrec_enable.GetValue());
            
            PlayerPrefs.SetString(id_inter.GetKey(), id_inter.GetValue());
            PlayerPrefs.SetString(id_banner.GetKey(), id_banner.GetValue());
            PlayerPrefs.SetString(id_aoa.GetKey(), id_aoa.GetValue());
            PlayerPrefs.SetString(id_native.GetKey(), id_native.GetValue());
        }

        //load cache data
        private void LoadData()
        {
            appOpenAds.SetValue(PlayerPrefs.GetInt(appOpenAds.GetKey(), appOpenAds.GetValue()));
            showAOAWhenStart.SetValue(PlayerPrefs.GetInt(showAOAWhenStart.GetKey(), showAOAWhenStart.GetValue()));
            capping_ads.SetValue(PlayerPrefs.GetInt(capping_ads.GetKey(), capping_ads.GetValue()));
            inter_enable.SetValue(PlayerPrefs.GetInt(inter_enable.GetKey(), inter_enable.GetValue()));
            banner_enable.SetValue(PlayerPrefs.GetInt(banner_enable.GetKey(), banner_enable.GetValue()));
            reward_enable.SetValue(PlayerPrefs.GetInt(reward_enable.GetKey(), reward_enable.GetValue()));
            native_enable.SetValue(PlayerPrefs.GetInt(native_enable.GetKey(), native_enable.GetValue()));
            mrec_enable.SetValue(PlayerPrefs.GetInt(mrec_enable.GetKey(), mrec_enable.GetValue()));
            id_inter.SetValue(PlayerPrefs.GetString(id_inter.GetKey(), id_inter.GetValue()));
            id_banner.SetValue(PlayerPrefs.GetString(id_banner.GetKey(), id_banner.GetValue()));
            id_aoa.SetValue(PlayerPrefs.GetString(id_aoa.GetKey(), id_aoa.GetValue()));
            id_native.SetValue(PlayerPrefs.GetString(id_native.GetKey(), id_native.GetValue()));
        }
        
    }
}

