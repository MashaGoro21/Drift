using UnityEngine;
using Unity.Services.LevelPlay;
using UnityEditor.PackageManager;

public class AdsManager : MonoBehaviour
{
    private LevelPlayRewardedAd rewardedAd;

    private void Start()
    {
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        LevelPlay.Init("238808895");
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error) 
    {
        Debug.LogError($"LevelPlay initialization failed: {error}");
    }

    private void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
    {
        Debug.Log("LevelPlay initialized successfully");

        rewardedAd = new LevelPlayRewardedAd("uhmu414hk852ndkd");
        RegisterRewardedEvents();
        rewardedAd.LoadAd();

        Debug.Log("Rewarded ad loading started");
    }

    private void RegisterRewardedEvents()
    {
        rewardedAd.OnAdLoaded += OnAdLoaded;
        rewardedAd.OnAdLoadFailed += OnAdLoadFailed;
        rewardedAd.OnAdDisplayed += OnAdDisplayed;
        rewardedAd.OnAdDisplayFailed += OnAdDisplayFailed;
        rewardedAd.OnAdRewarded += OnAdRewarded;
        rewardedAd.OnAdClosed += OnAdClosed;
    }

    private void OnAdLoaded(LevelPlayAdInfo info) 
    {
        Debug.Log("Rewarded ad loaded");
    }
    private void OnAdLoadFailed(LevelPlayAdError error) 
    {
        Debug.LogError($"Rewarded ad load failed: {error}");
    }
    private void OnAdDisplayed(LevelPlayAdInfo info) 
    {
        Debug.Log("Rewarded ad displayed");
    }

    private void OnAdDisplayFailed(LevelPlayAdDisplayInfoError error) 
    {
        Debug.LogError($"Rewarded ad display failed: {error}");

        rewardedAd.LoadAd();

        if (MusicPlayer.Instance != null)
            MusicPlayer.Instance.gameObject.SetActive(true);
    }

    private void OnAdRewarded(LevelPlayAdInfo info, LevelPlayReward reward) 
    {
        Debug.Log("Reward received");

        LevelManager.Instance.ReceiveDoubleReward(); 
    }
    private void OnAdClosed(LevelPlayAdInfo info) 
    {
        Debug.Log("Rewarded ad closed");

        rewardedAd.LoadAd(); 
        MusicPlayer.Instance.gameObject.SetActive(true); 
    }

    public void ShowRewardedAd()
    {
        if (rewardedAd != null && rewardedAd.IsAdReady())
        {
            MusicPlayer.Instance.gameObject.SetActive(false);
            rewardedAd.ShowAd();
        }
    }
}
