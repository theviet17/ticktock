using GoogleMobileAds.Ump.Api;
using System.Collections;
using System.Collections.Generic;
using BaseMe;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using UnityEngine;
using UnityEngine.Events;
public class AdmobPrivacy : Singleton<AdmobPrivacy>
{

    [SerializeField] private string[] listId;

    public UnityEvent<bool> EventConsentGathered;

    private IEnumerator Start()
    {
        ApplyTest();
#if UNITY_IOS
        
        var status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
        while (status == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            status = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            yield return null;
        }
        
#endif
        Debug.Log("UnityDebugLog: Show FOrm");
        ShowForm();
        Debug.Log("UnityDebugLog After Show Form Ads Available: " + ConsentInformation.CanRequestAds());
        yield return null;
    }

    private void ApplyTest()
    {
        
    }

    private void ShowForm()
    {
        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,
        };

        if (listId != null && listId.Length > 0)
        {
            List<string> listIdTest = new();
            listIdTest.AddRange(listId);
            var debugSettings = new ConsentDebugSettings
            {
                DebugGeography = DebugGeography.EEA,
                TestDeviceHashedIds = listIdTest
            };
            request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false,
                ConsentDebugSettings = debugSettings,
            };
        }

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    void OnConsentInfoUpdated(FormError consentError)
    {
        Debug.Log("UnityDebugLog: OnConsentInfoUpdated");
        if (consentError != null)
        {
            Debug.Log("UnityDebugLog: OnConsentInfoUpdated null");
            // Handle the error.
            UnityEngine.Debug.LogError(consentError);
            InitializeAdsWhenReady();
            return;
        }

        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                Debug.Log("UnityDebugLog: LoadAndShowConsentFormIfRequired error: " + formError);
                // Consent gathering failed.
                UnityEngine.Debug.LogError(consentError);
                ShowForm();
                return;
            }

            // Consent has been gathered.
            Debug.Log("UnityDebugLog: LoadAndShowConsentFormIfRequired gather, Ads Available: " + ConsentInformation.CanRequestAds());
            InitializeAdsWhenReady();
        });
    }

    private void InitializeAdsWhenReady()
    {
        if (ConsentInformation.CanRequestAds())
        {
            GoogleMobileAdsManager.I.InitializeEngineAds();
            AdsManager.I.InitializeEngineAds();    
        }
        
    }
}
