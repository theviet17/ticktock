using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interstitial : MonoBehaviour
{
    public string adUnitId = "d9a62d3d22828eee"; // Interstitial Ad Unit ID
    private int retryAttempt;
    private Action onAdClosedCallback; // Callback sẽ được thực thi khi quảng cáo đóng
    bool isInterstitialLoaded = false;
    void Start()
    {
     
        // Gắn các callback
       

        // Tải quảng cáo interstitial ban đầu
        //LoadInterstitial();
    }
    public void InitInterval()
    {
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;
    }

    /// <summary>
    /// Tải quảng cáo interstitial.
    /// </summary>
    public void LoadInterstitial()
    {
        //Debug.Log("Loading interstitial ad...");
        //MaxSdk.LoadInterstitial(adUnitId);
        // Kiểm tra nếu quảng cáo interstitial chưa được tải và chưa hiển thị
        if (!isInterstitialLoaded)
        {
            MaxSdk.LoadInterstitial(adUnitId);
            Debug.Log("Loading interstitial ad...");
        }
        else
        {
            Debug.Log("Interstitial ad already loaded.");
        }
    }

    /// <summary>
    /// Hiển thị quảng cáo interstitial nếu đã sẵn sàng và gắn callback.
    /// </summary>
    /// <param name="onAdClosed">Hàm callback sẽ được gọi khi người dùng đóng quảng cáo.</param>
    public void ShowInterstitial(Action onAdClosed = null)
    {
        if (MaxSdk.IsInterstitialReady(adUnitId))
        {
            isInterstitialLoaded = false;
            Debug.Log("Showing interstitial ad...");
            onAdClosedCallback = onAdClosed; // Lưu callback để thực thi khi quảng cáo đóng
            MaxSdk.ShowInterstitial(adUnitId);
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet.");
            onAdClosed?.Invoke(); // Nếu không có quảng cáo, gọi callback ngay lập tức
        }
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isInterstitialLoaded = true;    
        Debug.Log("Interstitial ad loaded.");
        retryAttempt = 0; // Reset số lần thử lại
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        isInterstitialLoaded = false;
        Debug.LogWarning($"Failed to load interstitial ad: {errorInfo.Message}");

        retryAttempt++;
        double retryDelay = Mathf.Pow(2, Mathf.Min(6, retryAttempt));

        Debug.Log($"Retrying to load interstitial ad in {retryDelay} seconds...");
        Invoke(nameof(LoadInterstitial), (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Interstitial ad displayed.");
    }

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        isInterstitialLoaded = false;
        Debug.LogError($"Failed to display interstitial ad: {errorInfo.Message}");
        LoadInterstitial();
    }

    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Interstitial ad clicked.");
    }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        Debug.Log("Interstitial ad hidden.");

        // Thực thi callback (nếu có) khi quảng cáo bị đóng
        onAdClosedCallback?.Invoke();
        onAdClosedCallback = null; // Reset callback

        // Tải lại quảng cáo mới
        LoadInterstitial();
    }
    private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        ADS.TrackPaidAdEventApplovinMax(adInfo);
        Debug.Log("Ad revenue paid.");
    }

    
}
