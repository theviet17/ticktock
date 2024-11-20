using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using GoogleMobileAds.Api;
using TMPro;
using AdjustSdk;
using System.Collections.Generic;
using BaseService;
using System.Threading;
using UnityEngine.UI;

public class NativeAdHandler : MonoBehaviour
{
    private const string TagLog = "UnityNative";

    private NativeAd nativeAd;

    public RawImage AdIconTexture;
    public TMP_Text AdHeadline;
    public TMP_Text AdDescription;

    public string Key;
    //public TMP_Text adCallToAction;
    //public GameObject AdLoaded;
    public GameObject Native;
    public List<TMP_Text> AdTextList;


    private bool nativeAdLoaded;
  
    string lastLoadedNativeKey;
    public CancellationTokenSource tokenSource;

    private bool isNativeLoading = false;


    public void ReLoadNative()
    {
       // yield return new WaitUntil(() => IsReadyRequest());

        //yield return null;
       Debug.Log($"{TagLog} Loaded Keys");
  
        RequestNativeAd();
    }

    private void Start()
    {
        
        //Debug.Log($"{TagLog} OnStart");
        //Debug.Log($"{TagLog} Variable AdsKeyManager: {AdsKeysManager.I}");
        //Debug.Log($"{TagLog} Variable RemoteConfig: {RemoteConfigControl.I}");
        //Debug.Log($"{TagLog} Variable List Id Native Key: {RemoteConfigControl.I.id_native.GetValue()}");

        //yield return new WaitUntil(() => IsReadyRequest());
        //LoadNativedKey();
        Debug.Log($"{TagLog} Loaded Keys");
        RequestNativeAd();
    }

    private void Update()
    {
        // Phương thức Update này có thể được loại bỏ nếu không sử dụng cho mục đích khác
        //Debug.Log($"Is Ready Request Ads Fetched: {RemoteConfigControl.I.isDataFetched}");
        //Debug.Log($"Is Ready Request Ads: {RemoteConfigControl.I != null}");
    }

    private bool IsReadyRequest()
    {
        if (AdsKeysManager.IsTestMode())
        {
            return true;
        }
        return RemoteConfigControl.I != null && RemoteConfigControl.I.isDataFetched;
    }

    public void RequestNativeAd(bool isForceLoad = false)
    {
        if (isNativeLoading && !isForceLoad)
        {
            return;
        }
        isNativeLoading = true;
        Debug.Log($"{TagLog}: Checking OrignButton");
        Debug.Log($"{TagLog}: Start Request Native");
        LoadNativeAds();

        // try
        // {
        //     StartCoroutine(IEDelayLoadNativeAds());
        // }
        // catch (System.Exception e)
        // {
        //     Debug.Log($"{TagLog}AdsError Request: "+ e);
        //     LoadNativeAds();
        // }
    }
    string GetNativeKey()
    {
        string key;
        if (AdsKeysManager.IsTestMode())
        {
            key = AdsKeysManager.GetNativeAdmobAdMobId();
        }
       
        else
        {
            key = Key;
        }
        Debug.Log($"{TagLog} NativeKeys" + key);
        return key;
    }

    private IEnumerator IEDelayLoadNativeAds()
    {
        yield return new WaitUntil(() => AdsKeysManager.GetNativeAdmobAdMobId().Length > 0);
        LoadNativeAds();
    }

    private void LoadNativeAds()
    {
        //var key = AdsKeysManager.GetNativeAdmobAdMobId();
        var key = GetNativeKey();
        lastLoadedNativeKey = key;
        Debug.Log($"{TagLog} Load with key: " + key);
        AdLoader adLoader = new AdLoader.Builder(key)
            .ForNativeAd()
            .Build();
        adLoader.OnNativeAdLoaded += HandleNativeAdLoaded;
        adLoader.OnAdFailedToLoad += HandleNativeAdFailedToLoad;  // Đăng ký trước khi gọi LoadAd
        adLoader.OnNativeAdImpression += HandleNativeAdImpression;
        AdRequest adRequest = new AdRequest();
        adLoader.LoadAd(adRequest);
    }

    private void HandleNativeAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
        Debug.Log($"{TagLog} Native ad failed to load: " + args.LoadAdError);
        var timeRetry = Math.Pow(2, Math.Min(6, interstitialRetryAttempt + 1));
        Debug.Log($"{TagLog} Requesting again after {timeRetry} seconds");
        //isNativeLoading = false;
        tokenSource = new CancellationTokenSource();
        RequestNativeAdAgain().Forget();
    }

    private int interstitialRetryAttempt = 0;
    private async UniTaskVoid RequestNativeAdAgain()
    {
        interstitialRetryAttempt++;

        double retryDelay = Math.Pow(2, Math.Min(6, interstitialRetryAttempt));

        await UniTask.Delay(TimeSpan.FromSeconds(retryDelay), cancellationToken: tokenSource.Token);

        RequestNativeAd(true);
    }

    private void HandleNativeAdImpression(object sender, EventArgs adValue)
    {
        Debug.Log($"{TagLog} OnNativeImpression");
        try
        {
            if (adValue.GetType() == typeof(AdValueEventArgs))
            {
                var nativeAdValue = (AdValueEventArgs)adValue;
                LogcatUtils.TrackPaidAdEventAdmob(nativeAdValue.AdValue, lastLoadedNativeKey, "native");
                Debug.Log($"{TagLog} OnNativeImpression Sent");
            }
            else
            {
                Debug.Log($"{TagLog} Impresion is not AdValueEventArgs, type is: {adValue.GetType()}");
            }
        }
        catch (System.Exception e)
        {
            Debug.Log($"{TagLog} Log Impression Error {e}");
        }
    }

    private void HandleNativeAdLoaded(object sender, NativeAdEventArgs args)
    {
        isNativeLoading = false;
        Debug.Log($"{TagLog} ad loaded.");
        nativeAd = args.nativeAd;
        if (nativeAd != null)
        {
            nativeAd.OnPaidEvent += OnPaidNative;
        }
        nativeAdLoaded = true;
        Debug.Log($"{TagLog} nativeAdLoaded  " + nativeAdLoaded);
        UpdateAdUI();
    }

    private void OnPaidNative(object sender, AdValueEventArgs args)
    {
        Debug.Log($"{TagLog} OnPaid Native");
        LogcatUtils.TrackPaidAdEventAdmob(args.AdValue, lastLoadedNativeKey,  "native");
        Debug.Log($"{TagLog} OnPaid TrackPaidAdEventAdmob");
    }

    private void UpdateAdUI()
    {
        if (!nativeAdLoaded || RemoteConfigControl.I.native_enable.GetValue() == 0)
        {
            Native.SetActive(false);
            //AdLoaded.SetActive(false);
            Debug.Log($"{TagLog}No ads loaded");
            return;
        }

        Native.SetActive(true);
        //AdLoaded.SetActive(true);

        Texture2D iconTexture = nativeAd.GetIconTexture();

        AdIconTexture.texture = iconTexture;

        AdHeadline.text = nativeAd.GetHeadlineText() ?? "No headline";
        AdDescription.text = nativeAd.GetBodyText() ?? "No description";
        for (int i = 0; i < AdTextList.Count; i++)
        {
            AdTextList[i].text = nativeAd.GetCallToActionText() ?? "No call to action";
        }
        //adCallToAction.text = nativeAd.GetCallToActionText() ?? "No call to action";

        nativeAd.RegisterIconImageGameObject(AdIconTexture.gameObject);
        nativeAd.RegisterHeadlineTextGameObject(AdHeadline.gameObject);
        nativeAd.RegisterBodyTextGameObject(AdDescription.gameObject);
        //nativeAd.RegisterCallToActionGameObject(adCallToAction.gameObject);
        //if(nonOriginButton.transform.childCount > 1)
        //{
        //    nativeAd.RegisterCallToActionGameObject(nonOriginButton.transform.GetChild(0).gameObject);
        //    nativeAd.RegisterCallToActionGameObject(nonOriginButton.transform.GetChild(1).gameObject);
        //}
        //else
        //{
        //    nativeAd.RegisterCallToActionGameObject(nonOriginButton.gameObject);
        //}
        //nativeAd.RegisterCallToActionGameObject(originButton.gameObject);

        Debug.Log($"{TagLog}Ad LOADED and UI updated");
    }

    private void CancelRetryLoop()
    {
        if (tokenSource != null)
        {
            try
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
                tokenSource = null;
            }
            catch (System.Exception e)
            {

            }
        }

    }

    
}
