using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialAdsScript : MonoBehaviour, IUnityAdsListener
{
#if UNITY_ANDROID
    readonly string gameId = "4215399";
    readonly string mInterstitialSurfaceId = "Interstitial_Android"; 
    readonly string mBannerSurfaceId = "Banner_Android";
#endif
#if UNITY_IOS
    readonly string gameId = "4215398";
    readonly string mInterstitialSurfaceId = "Interstitial_iOS";
    readonly string mBannerSurfaceId = "Banner_iOS";
#endif
    bool mTestMode = true;

    public delegate void DelegateAdFinish(string surfacingId, ShowResult showResult);
    public DelegateAdFinish onAdFinish;

    void Start()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, mTestMode);
        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);
        StartCoroutine(ShowBannerWhenInitialized());
    }

    public void ShowInterstitialAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show(mInterstitialSurfaceId);
        }
        else
        {
            Debug.Log("Interstitial ad not ready at the moment! Please try again later!");
        }
    }

    IEnumerator ShowBannerWhenInitialized()
    {
        while (!Advertisement.isInitialized)
        {
            yield return new WaitForSeconds(2.0f);
        }
        Advertisement.Banner.Show(mBannerSurfaceId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string surfacingId)
    {
        // If the ready Ad Unit or legacy Placement is rewarded, activate the button: 
        if (surfacingId == mInterstitialSurfaceId)
        {
            //myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string surfacingId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            //Debug.LogWarning(“The ad did not finish due to an error.”);
        }
        onAdFinish?.Invoke(surfacingId, showResult);
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string surfacingId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}
