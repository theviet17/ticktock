using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppOpenAdTest : MonoBehaviour
{
    public string appOpenAdUnitId = "a39cee3a73033825"; // Đặt ID quảng cáo App Open của bạn
    private bool isAppOpenAdLoaded = false;

    // Biến này sẽ lưu trữ ID của quảng cáo App Open
    void Start()
    {
        
    }

    public void AOPInit()
    {
        MaxSdk.LoadAppOpenAd(appOpenAdUnitId);
        MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenAdRevenuePaidEvent;
    }
 

    public void ShowAdIfReady(Action action)
    {
        if (MaxSdk.IsAppOpenAdReady(appOpenAdUnitId))
        {
            MaxSdk.ShowAppOpenAd(appOpenAdUnitId);
            action.Invoke();
        }
        else
        {
            action.Invoke();
            MaxSdk.LoadAppOpenAd(appOpenAdUnitId);
        }
    }
    private void OnAppOpenAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        ADS.TrackPaidAdEventApplovinMax(adInfo);
        Debug.Log("Ad revenue paid.");
    }
}
