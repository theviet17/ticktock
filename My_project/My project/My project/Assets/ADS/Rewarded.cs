using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Rewarded : MonoBehaviour
{

    public string adUnitId = "30faaf12223d6256";
    private Action rewardAction;
    private bool isRewardedLoaded = false;

    void Start()
    {
       
        // Attach callbacks
       

        // Load the first rewarded ad
      
    }
    public void InitReward()
    {
        Debug.Log("Init Max Sdk Reward");
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
    }

    public void LoadRewardedAd()
    {
        if (!MaxSdk.IsRewardedAdReady(adUnitId))
        {
            
            MaxSdk.LoadRewardedAd(adUnitId);
            Debug.Log("Loading rewarded ad...");
        }
        else
        {
            Debug.Log("Rewarded ad already loaded.");
        }
    }

    public void ShowRewardedAd(Action onRewardComplete, Action fail)
    {
        if (MaxSdk.IsRewardedAdReady(adUnitId))
        {
            float timeBreak = 1f;
            UIManager.I.teeBreakPanel.gameObject.SetActive(true);
            UIAnimation.Fade(UIManager.I.teeBreakPanel, 0.3f, true, 0.98f);
            UIManager.I.teeBreakPanel.GetComponentInChildren<TMP_Text>().text = "Skipping...";
            DOVirtual.DelayedCall(timeBreak, () =>
            {
                isRewardedLoaded = false;
                rewardAction = onRewardComplete; // Lưu hành động để gọi khi xem xong quảng cáo
                MaxSdk.ShowRewardedAd(adUnitId);

                UIManager.I.teeBreakPanel.gameObject.SetActive(false);

            });
            
        }
        else
        {
            Debug.Log("Rewarded ad not ready yet.");
            fail?.Invoke();
            LoadRewardedAd();
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isRewardedLoaded = true;
        retryAttempt = 0;
        Debug.Log("Rewarded ad loaded.");
    }
    int retryAttempt;
    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isRewardedLoaded = false;
        Debug.Log("Failed to load rewarded ad. Retrying...");
        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));
        Invoke("LoadRewardedAd", (float)retryDelay);
        //LoadRewardedAd();
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad displayed.");
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        isRewardedLoaded = false;
        Debug.Log("Failed to display rewarded ad.");
        LoadRewardedAd();
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad clicked.");
    }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Rewarded ad hidden. Loading next ad.");
        LoadRewardedAd();
    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("User finished watching ad. Granting reward.");
        rewardAction?.Invoke(); // Gọi action trả thưởng nếu tồn tại
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        ADS.TrackPaidAdEventApplovinMax(adInfo);
        Debug.Log("Ad revenue paid.");
    }
}
