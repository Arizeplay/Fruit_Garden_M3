using System;
using System.Collections.Generic;
using UnityEngine;
using Yurowm.GameCore;
#if UNITY_EDITOR
using UnityEditor;
#endif
using GoogleMobileAds.Api;
using AdMobInterstitialAd = GoogleMobileAds.Api.InterstitialAd;

public class AdMobIntegration : IAdIntegration
{
    public string Regular_Android = "";
    public string Rewarded_Android = "";
    public string Pig_A_Poke_Android = "";
    public string Wheel_Of_Fortune_Android = "";
    public string Regular_iOS = "";
    public string Rewarded_iOS = "";
    public string Pig_A_Poke_iOS = "";
    public string Wheel_Of_Fortune_iOS = "";

    private bool _useDebugAdId;
    private bool _initialized;
    
    Dictionary<AdType, AdMobInterestialWrapper> interstitial = new Dictionary<AdType, AdMobInterestialWrapper>();
    Dictionary<AdType, AdMobRewardedWrapper> rewarded = new Dictionary<AdType, AdMobRewardedWrapper>();

    public override void Initialize()
    {
        if ((typeMask & AdType.Regular) != 0) interstitial.Add(AdType.Regular, null);
        if ((typeMask & AdType.Rewarded) != 0) rewarded.Add(AdType.Rewarded, null);
        if ((typeMask & AdType.PigInAPoke) != 0) rewarded.Add(AdType.PigInAPoke, null);
        if ((typeMask & AdType.WheelOfFortune) != 0) rewarded.Add(AdType.WheelOfFortune, null);

        MobileAds.Initialize(status =>
        {
            foreach (var s in status.getAdapterStatusMap())
            {
                Debug.Log("AdMod Status " + s.Key + " Description: " + s.Value.Description);
            }
                
            _initialized = true;
            
            Debug.Log("Google AdMod Initialized");
        });
    }

    public override void OnUpdate()
    {
        if (!_initialized) return;

        foreach (AdType adType in Enum.GetValues(typeof(AdType)))
        {
            if (adType == AdType.Regular)
            {
                if (!interstitial.ContainsKey(adType) || interstitial[adType] == null || interstitial[adType].InterstitialAd == null)
                {
                    AdMobInterestialWrapper i = new AdMobInterestialWrapper(new AdMobInterstitialAd(GetZoneID(adType)));
                    i.InterstitialAd.LoadAd(new AdRequest.Builder().Build());
                    i.OnAdFailedToLoad += (a, b) =>
                    {
                        interstitial[adType] = null;
                        Debug.LogWarning(adType + " AdMob failed code: " + b.LoadAdError.GetCode());
                    };
                    i.OnAdLoaded += (sender, args) =>
                    {
                        Debug.Log("AdMob loaded: " + adType);
                        ItemCounter.RefreshAll();
                    };
                    i.OnAdClosed += (a, b) =>
                    {
                        onComplete?.Invoke();
                        i.DestroyAd();
                    
                    };

                    Debug.Log("AdMob InterstitialAd Initialization");
                    
                    this.interstitial.Set(adType, i);
                    ItemCounter.RefreshAll();
                }
            }

            if (adType is AdType.Rewarded or AdType.PigInAPoke or AdType.WheelOfFortune)
            {
                if (!rewarded.ContainsKey(adType) || rewarded[adType] == null || rewarded[adType].RewardedAd == null)
                {
                    AdMobRewardedWrapper r = new AdMobRewardedWrapper(new RewardedAd(GetZoneID(adType)));
                    r.RewardedAd.LoadAd(new AdRequest.Builder().Build());
                    r.OnAdFailedToLoad += (a, b) =>
                    {
                        rewarded[adType] = null;
                        Debug.LogWarning(adType + " AdMob failed code: " + b.LoadAdError.GetCode());
                    };
                    r.OnAdLoaded += (sender, args) =>
                    {
                        Debug.Log("AdMob loaded: " + adType);

                        ItemCounter.RefreshAll();
                    };
                    r.OnAdClosed += (a, b) =>
                    {
                        r.DestroyAd();
                    };
                    r.OnAdRewarded += (a, b) =>
                    {
                        onComplete?.Invoke();
                    };
                    
                    Debug.Log("AdMob Reward Initialization");
                    
                    this.rewarded.Set(adType, r);
                    ItemCounter.RefreshAll();
                }
            }
        }

        for (int i = 0; i < interstitial.Keys.Count; i++)
        {
            interstitial[interstitial.Keys.Get(i)]?.Update();
        }

        for (int i = 0; i < rewarded.Keys.Count; i++)
        {
            rewarded[rewarded.Keys.Get(i)]?.Update();
        }
    }

    public override string GetAppID()
    {
        return null;
    }

    public override string GetZoneID(AdType type)
    {
        if (_useDebugAdId)
        {
            switch (type)
            {
                case AdType.Regular: return "ca-app-pub-3940256099942544/1033173712";
                case AdType.Rewarded: return "ca-app-pub-3940256099942544/5224354917";
                case AdType.PigInAPoke: return "ca-app-pub-3940256099942544/5224354917";
                case AdType.WheelOfFortune: return "ca-app-pub-3940256099942544/5224354917";
            }
        }

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            {
                switch (type)
                {
                    case AdType.Regular: return Regular_Android;
                    case AdType.Rewarded: return Rewarded_Android;
                    case AdType.PigInAPoke: return Pig_A_Poke_Android;
                    case AdType.WheelOfFortune: return Wheel_Of_Fortune_Android;
                }
            }
                break;
            case RuntimePlatform.IPhonePlayer:
            {
                switch (type)
                {
                    case AdType.Regular: return Regular_iOS;
                    case AdType.Rewarded: return Rewarded_iOS;
                    case AdType.PigInAPoke: return Pig_A_Poke_iOS;
                    case AdType.WheelOfFortune: return Wheel_Of_Fortune_iOS;
                }
            }
                break;
        }

        return "";
    }

    public override bool IsReady(AdType type)
    {
        if (type == AdType.Regular)
        {
            return interstitial.ContainsKey(type) && interstitial[type] != null &&  interstitial[type].InterstitialAd != null &&
                   interstitial[type].InterstitialAd.IsLoaded();
        }
        
        if (type == AdType.Rewarded|| type == AdType.PigInAPoke|| type == AdType.WheelOfFortune)
        {
            return rewarded.ContainsKey(type) && rewarded[type] != null && rewarded[type].RewardedAd != null &&
                   rewarded[type].RewardedAd.IsLoaded();
        }

        return false;
    }


    Action onComplete = null;

    public override void Show(AdType type, Action onComplete)
    {
        if (interstitial.ContainsKey(type))
        {
            this.onComplete = onComplete;
            interstitial[type].InterstitialAd.Show();
        }
        
        if (rewarded.ContainsKey(type))
        {
            this.onComplete = onComplete;
            rewarded[type].RewardedAd.Show();
        }
    }

    public override string GetName()
    {
        return "AdMob";
    }
}

public class AdMobInterestialWrapper
{
    public event EventHandler<EventArgs> OnAdClosed;
    public event EventHandler<EventArgs> OnAdLoaded;
    public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

    public AdMobInterstitialAd InterstitialAd { get; private set; }

    private (object sender, EventArgs e)? _closed;
    private (object sender, EventArgs e)? _loaded;
    private (object sender, AdFailedToLoadEventArgs e)? _failedToLoad;

    public AdMobInterestialWrapper(AdMobInterstitialAd interstitialAd)
    {
        InterstitialAd = interstitialAd;
        InterstitialAd.OnAdClosed += InterstitialAdOnOnAdClosed;
        InterstitialAd.OnAdFailedToLoad += InterstitialAdOnOnAdFailedToLoad;
        InterstitialAd.OnAdLoaded += InterstitialAdOnOnAdLoaded;
    }

    private void InterstitialAdOnOnAdClosed(object sender, EventArgs e) => _closed = (sender, e);

    private void InterstitialAdOnOnAdLoaded(object sender, EventArgs e) =>
        _loaded = (sender, e);
    
    private void InterstitialAdOnOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) =>
        _failedToLoad = (sender, e);

    private void CheckIfClosed()
    {
        if (!(_closed is var (s, e))) return;
        OnAdClosed?.Invoke(s, e);
        _closed = null;
    }
    
    private void CheckIfLoaded()
    {
        if (!(_loaded is var (s, e))) return;
        OnAdLoaded?.Invoke(s, e);
        _loaded = null;
    }

    private void CheckIfFailedToLoad()
    {
        if (!(_failedToLoad is var (s, e))) return;
        OnAdFailedToLoad?.Invoke(s, e);
        _failedToLoad = null;
    }

    public void Update()
    {
        CheckIfClosed();
        CheckIfLoaded();
        CheckIfFailedToLoad();
    }

    public void DestroyAd()
    {
        InterstitialAd.Destroy();
        InterstitialAd = null;
    }
}

public class AdMobRewardedWrapper
{
    public event EventHandler<EventArgs> OnAdClosed;
    public event EventHandler<EventArgs> OnAdLoaded;
    public event EventHandler<Reward> OnAdRewarded;
    public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

    public RewardedAd RewardedAd { get; private set; }

    private (object sender, EventArgs e)? _closed;
    private (object sender, EventArgs e)? _loaded;
    private (object sender, Reward e)? _rewarded;
    private (object sender, AdFailedToLoadEventArgs e)? _failedToLoad;

    public AdMobRewardedWrapper(RewardedAd rewardedAd)
    {
        RewardedAd = rewardedAd;
        RewardedAd.OnAdClosed += RewardedAdOnOnAdClosed;
        RewardedAd.OnAdLoaded += RewardedAdOnOnAdLoaded;
        RewardedAd.OnAdFailedToLoad += RewardedAdOnOnAdFailedToLoad;
        RewardedAd.OnUserEarnedReward += RewardedAdOnOnUserEarnedReward;
    }

    private void RewardedAdOnOnAdClosed(object sender, EventArgs e) => _closed = (sender, e);
    private void RewardedAdOnOnAdLoaded(object sender, EventArgs e) => _loaded = (sender, e);

    private void RewardedAdOnOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e) =>
        _failedToLoad = (sender, e);
    
    private void RewardedAdOnOnUserEarnedReward(object sender, Reward e) =>
        _rewarded = (sender, e);

    private void CheckIfClosed()
    {
        if (!(_closed is var (s, e))) return;
        OnAdClosed?.Invoke(s, e);
        _closed = null;
    }    
    
    private void CheckIfLoaded()
    {
        if (!(_loaded is var (s, e))) return;
        OnAdLoaded?.Invoke(s, e);
        _loaded = null;
    }

    private void CheckIfFailedToLoad()
    {
        if (!(_failedToLoad is var (s, e))) return;
        OnAdFailedToLoad?.Invoke(s, e);
        _failedToLoad = null;
    }
    
    private void CheckIfRewarded()
    {
        if (!(_rewarded is var (s, r))) return;
        OnAdRewarded?.Invoke(s, r);
        _rewarded = null;
    }

    public void Update()
    {
        CheckIfClosed();
        CheckIfLoaded();
        CheckIfFailedToLoad();
        CheckIfRewarded();
    }
    
    public void DestroyAd()
    {
        RewardedAd.Destroy();
        RewardedAd = null;
    }
}

#if UNITY_EDITOR
public class AdMobIntegrationEditor : AdIntegrationEditor<AdMobIntegration>
{
    public override void DrawSettings(AdMobIntegration integration)
    {
        integration.Regular_Android = EditorGUILayout.TextField("Android Regular ID", integration.Regular_Android);
        integration.Rewarded_Android = EditorGUILayout.TextField("Android Rewarded ID", integration.Rewarded_Android);
        integration.Pig_A_Poke_Android = EditorGUILayout.TextField("Android Pig-A-Poke Rewarded ID", integration.Pig_A_Poke_Android);
        integration.Wheel_Of_Fortune_Android = EditorGUILayout.TextField("Android Wheel Of Fortune Rewarded ID", integration.Wheel_Of_Fortune_Android);
        integration.Regular_iOS = EditorGUILayout.TextField("iOS Regular ID", integration.Regular_iOS);
        integration.Rewarded_iOS = EditorGUILayout.TextField("iOS Rewarded ID", integration.Rewarded_iOS);
        integration.Pig_A_Poke_iOS = EditorGUILayout.TextField("iOS Pig-A-Poke Rewarded ID", integration.Pig_A_Poke_iOS);
        integration.Wheel_Of_Fortune_iOS = EditorGUILayout.TextField("iOS Wheel Of Fortune Rewarded ID", integration.Wheel_Of_Fortune_iOS);
    }
}
#endif

public class ADMOB_sdsymbol : IScriptingDefineSymbol
{
    public override string GetBerryLink()
    {
        return null;
    }

    public override string GetDescription()
    {
        return "The implementation of Google AdMob advertising network. Requires AdMob SDK";
    }

    public override string GetSybmol()
    {
        return "ADMOB";
    }
}