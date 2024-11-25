using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class AdManager : MonoBehaviour
{
    //public WindowMessage wM;
    //public GameObject windowMessage;
    private int Tag;
    private bool IsReward; // check xem đã có thưởng hay  chưa
    //public TextDebug textDebug;
 /*   public string appId = "ca-app-pub-1385093244148841~5602672977";*/// "ca-app-pub-3940256099942544~3347511713";
    public string appId = "ca-app-pub-8267376628184641~2117062992";// "ca-app-pub-3940256099942544~3347511713";
#if UNITY_ANDROID
    //string bannerId = "ca-app-pub-3940256099942544/6300978111";
    //string interstitialId = "ca-app-pub-3940256099942544/1033173712";
    //string rewardedId = "ca-app-pub-3940256099942544/5224354917";
    string bannerId = "ca-app-pub-8267376628184641/6387480496";
    string interstitialId = "ca-app-pub-8267376628184641/6508584209";
    string rewardedId = "ca-app-pub-8267376628184641/4778885356";
#elif UNITY_IPHONE
    string bannerId = "ca-app-pub-3940256099942544/2934735716";
    string interstitialId = "ca-app-pub-3940256099942544/4411468910";
    string rewardedId = "ca-app-pub-3940256099942544/1712485313";
#endif
    BannerView bannerView;
    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;
    private void Start()
    {
        LoadBannerAd();
        LoadInterstitialAd();
        LoadRewardedAd();
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        MobileAds.Initialize(initStatus => {
        });
    }
    public void Log(string message)
    {
        Debug.Log(message);
        //textDebug.SetText(message);
    }
    public void OnAdShowReward()
    {
        IsReward = true;
        Log("OnAdShowReward" );
    }
    public void OnAdShowComplete()
    {
        Log("OnAdShowComplete - Tag = " + Tag.ToString());
    }
    public  void OnAdShowFail(string message)
    {
        Log("OnAdShowFail - " +  message);
    }
    public void OnAdLoadFail(string message)
    {
        Log("OnAdLoadFail - " + message);
    }
    public void OnAdLoadSuccess(string adUnitId)
    {
        Log("OnAdLoadSuccess - " + adUnitId);
    }

    public void Show(int tag)
    {
        Tag = tag;
        IsReward = false;
        if (tag == 1)
        {
            if (interstitialAd != null && interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
                LoadInterstitialAd();
            }
            else
            {
                //wM.Show("Ad is not ready");
                OnAdShowFail("interstitial");
                LoadInterstitialAd();
            }
        }
        else if (tag == 2)
        {
            if (rewardedAd != null && rewardedAd.CanShowAd())
            {
                rewardedAd.Show((Reward reward) =>
                {
                    OnAdShowReward();
                });
                LoadRewardedAd();
            }
            else
            {
                //wM.Show("Ad is not ready");
                OnAdShowFail("rewarded");
                LoadRewardedAd();
            }
        }
    }
    #region Banner
    public void LoadBannerAd()
    {
        CreateBannerView();
        ListenToBannerEvents();
        if (bannerView == null)
        {
            CreateBannerView();
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");
        bannerView.LoadAd(adRequest);
    }
    void CreateBannerView()
    {
        if (bannerView != null)
        {
            DestroyBannerAd();
        }
        bannerView = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
    }
    void ListenToBannerEvents()
    {
        bannerView.OnBannerAdLoaded += () =>
        {
            OnAdLoadSuccess("Banner");
        };
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            OnAdLoadFail("Banner - " + error.ToString());
        };  
        bannerView.OnAdFullScreenContentClosed += () =>
        {
            OnAdShowComplete();
        };
    }
    public void DestroyBannerAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }
    #endregion
    #region Interstitial
    public void LoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");
        InterstitialAd.Load(interstitialId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                OnAdLoadFail(error.ToString());
                return;
            }
            OnAdLoadSuccess("Interstitial");
            interstitialAd = ad;
            InterstitialEvent(interstitialAd);
        });
    }
    public void InterstitialEvent(InterstitialAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            OnAdShowComplete();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            OnAdShowFail("Interstitial - " + error);
        };
    }
    #endregion
    #region Rewarded
    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("unity-admob-sample");
        RewardedAd.Load(rewardedId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                OnAdLoadFail(error.ToString());
                return;
            }
            OnAdLoadSuccess("Rewarded");
            rewardedAd = ad;
            RewardedAdEvents(rewardedAd);
        });
    }
    public void RewardedAdEvents(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            OnAdShowComplete();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            OnAdShowFail(error.ToString());
        };
    }
    #endregion
}
