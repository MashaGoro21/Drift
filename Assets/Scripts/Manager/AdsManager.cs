using UnityEngine;
using Unity.Services.LevelPlay;

public class AdsManager : MonoBehaviour
{
    private LevelPlayRewardedAd rewardedAd;

    private void Start()
    {
        LevelPlay.OnInitSuccess += SdkInitializationCompletedEvent;
        LevelPlay.OnInitFailed += SdkInitializationFailedEvent;
        LevelPlay.Init("238808895");
    }

    private void SdkInitializationFailedEvent(LevelPlayInitError error) { }

    private void SdkInitializationCompletedEvent(LevelPlayConfiguration configuration)
    {
        rewardedAd = new LevelPlayRewardedAd("uhmu414hk852ndkd");
        RegisterRewardedEvents();
        rewardedAd.LoadAd();
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

    private void OnAdLoaded(LevelPlayAdInfo info) { }
    private void OnAdLoadFailed(LevelPlayAdError error) { }
    private void OnAdDisplayed(LevelPlayAdInfo info) { }
    private void OnAdDisplayFailed(LevelPlayAdDisplayInfoError err) { }
    private void OnAdRewarded(LevelPlayAdInfo info, LevelPlayReward reward) { LevelManager.Instance.ReceiveDoubleReward(); }
    private void OnAdClosed(LevelPlayAdInfo info) 
    { 
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
