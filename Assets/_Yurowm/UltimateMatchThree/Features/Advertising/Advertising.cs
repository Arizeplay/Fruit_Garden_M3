using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Flags]
public enum AdType
{
    Regular = 1 << 0,
    Rewarded = 1 << 1,
    PigInAPoke = 1 << 2,
    WheelOfFortune = 1 << 3,
}

public class Advertising : MonoBehaviourAssistant<Advertising>
{
    public int adDelay = 3;
    public int adFreeLevels = 5;

    public bool disableRegularAds { get; set; }
    
    static List<IAdIntegration> integrations = null;

    [HideInInspector]
    public List<IAdIntegration.Parameters> parameters = new List<IAdIntegration.Parameters>();

    public static Advertising instance;

    DelayedAccess timeAccess;
    Action reward = null;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {
        timeAccess = new DelayedAccess(adDelay * 60);
        integrations = IAdIntegration.allTypes
            .Select(
                type =>
                {
                    IAdIntegration.Parameters parameters =
                        this.parameters.FirstOrDefault(x => x.typeName == type.FullName);

                    if (parameters != null) return IAdIntegration.Deserialize(parameters);

                    return (IAdIntegration) Activator.CreateInstance(type);
                }
            )
            .Where(x => x != null)
            .ToList();
        Debug.Log(integrations.Count + " integrations count");
        integrations.ForEach(x => x.Initialize());

        UIAssistant.onShowPage += OnShowPage;
        DebugPanel.AddDelegate("Show Video Ads", () => ShowAds(AdType.Regular,null, true));
    }

    public void RegisterEvents()
    {
        Project.onLevelComplete.AddListener(ShowAdsOnLevelComplete);
        Project.onLevelFailed.AddListener(ShowAdsOnLevelFailed);
    }

    private void ShowAdsOnLevelFailed()
    {
        ShowAds(AdType.Regular, null, true);
    }

    private void ShowAdsOnLevelComplete()
    {
        if (SessionInfo.current.level % 2 != 0 && SessionInfo.current.level != 1) 
            ShowAds(AdType.Regular, null, true);
    }
    
    private void OnShowPage(UIAssistant.Page page)
    {
        if (page.HasTag("ADS")) main.ShowAds(AdType.Regular);
    }

    private void GiveReward()
    {
        if (reward != null)
        {
            reward.Invoke();
            reward = null;
        }
    }

    void Update()
    {
        if (integrations == null) return;
            
        foreach (IAdIntegration integration in integrations) integration.OnUpdate();

        if (Debug.isDebugBuild && DebugPanel.main)
        {
            foreach (IAdIntegration network in integrations)
            {
                if (!network.active) continue;
                if ((network.typeMask & AdType.Regular) != 0)
                    DebugPanel.Log(network.GetName() + " Regular ID " + network.GetZoneID(AdType.Regular), "Ads", network.IsReady(AdType.Regular));
                if ((network.typeMask & AdType.Rewarded) != 0)
                    DebugPanel.Log(network.GetName() + " Rewarded ID " + network.GetZoneID(AdType.Rewarded), "Ads", network.IsReady(AdType.Rewarded));
                if ((network.typeMask & AdType.PigInAPoke) != 0)
                    DebugPanel.Log(network.GetName() + " Pig In A Poke Rewarded ID" + network.GetZoneID(AdType.PigInAPoke), "Ads", network.IsReady(AdType.PigInAPoke));
                if ((network.typeMask & AdType.WheelOfFortune) != 0)
                    DebugPanel.Log(network.GetName() + " Wheel Of Fortune Rewarded ID " + network.GetZoneID(AdType.WheelOfFortune), "Ads", network.IsReady(AdType.WheelOfFortune));
            }
        }
    }

    public void ShowAds(AdType type, Action reward = null, bool force = false)
    {
        if (disableRegularAds && reward == null)
            return;
        
        this.reward = reward;

        if (Application.isEditor)
        {
            reward?.Invoke();
        }

        var target = integrations.Where(x => x.active && (x.typeMask & type) != 0 && x.IsReady(type)).ToList();
        
        if (target.Count > 0) ShowAds(target.GetRandom(), type, force);

        #region AppMetric

        AppMetrica p = GameObject.Find("AppMetrica").GetComponent<AppMetrica>();
        string json = $"{{\"ADS\":\"{type}\"}}";
        p.SendEvent("Show_ads", json);

        #endregion
    }

    void ShowAds(IAdIntegration integration, AdType type, bool force = false)
    {
        DebugPanel.Log("Ad Request", "Ads", "{0}: {1}".FormatText(integration.GetName(), type));
        StartCoroutine(ShowingAds(integration, type, force));
    }

    IEnumerator ShowingAds(IAdIntegration integration, AdType type, bool force = false)
    {
        if (CPanel.uiAnimation > 0) yield return 0;

        if (!integration.IsReady(type)) yield break;

        if (reward == null && !force)
        {
            if (!timeAccess.GetAccess()) yield break;
        }

        timeAccess.ResetTimer();
        integration.Show(type, GiveReward);
    }

    public int CountOfReadyAds(AdType type)
    {
        return integrations.Count(x => x.active && (x.typeMask & type) != 0 && x.IsReady(type));
    }
}

[Serializable]
public abstract class IAdIntegration
{
    public AdType typeMask;
    public bool active = true;
    public static Type[] allTypes;

    static IAdIntegration()
    {
        allTypes = (typeof(Advertising)).Assembly.GetTypes()
            .Where(x => !x.IsAbstract && (typeof(IAdIntegration)).IsAssignableFrom(x))
            .ToArray();
    }

    public IAdIntegration() { }

    public virtual void Initialize() { }
    public virtual void OnUpdate() { }

    public abstract bool IsReady(AdType type);

    public abstract void Show(AdType type, Action onComplete);

    public abstract string GetZoneID(AdType type);
    public abstract string GetAppID();

    public abstract string GetName();

    public Parameters Serialize()
    {
        return new Parameters(GetType().FullName, JsonUtility.ToJson(this));
    }

    public static IAdIntegration Deserialize(Parameters parameters)
    {
        Type type = allTypes.FirstOrDefault(x => x.FullName == parameters.typeName);

        if (type == null) return null;

        try
        {
            IAdIntegration result = (IAdIntegration) JsonUtility.FromJson(parameters.json, type);

            return result;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }

        return null;
    }

    [Serializable]
    public class Parameters
    {
        public string typeName;
        public string json;

        public Parameters(string typeName, string json)
        {
            this.typeName = typeName;
            this.json = json;
        }
    }
}

#if UNITY_EDITOR
public abstract class AdIntegrationEditor
{
    public abstract void OnGUI(IAdIntegration integration);
    public abstract bool IsSuitable(IAdIntegration integration);
}

public abstract class AdIntegrationEditor<T> : AdIntegrationEditor where T : IAdIntegration
{
    public override void OnGUI(IAdIntegration integration)
    {
        DrawSettings((T) integration);
    }

    public abstract void DrawSettings(T integration);

    public override bool IsSuitable(IAdIntegration integration)
    {
        return integration is T;
    }
}
#endif
