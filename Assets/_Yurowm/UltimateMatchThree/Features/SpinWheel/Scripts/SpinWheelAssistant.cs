using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using Yurowm.GameCore;
using System.Xml.Linq;

public class SpinWheelAssistant : MonoBehaviourAssistant<SpinWheelAssistant>, ILocalized {

    const string localizationItemFormat = "id/{0}";
    public const int itemCount = 12;

    
    public List<Reward> rewards = new List<Reward>();

    public Reward EmitReward() {
        Reward result = null;

        float rnd = UnityEngine.Random.Range(0, rewards.Sum(x => x.weight));
        foreach (Reward reward in rewards) {
            rnd -= reward.weight;
            if (rnd <= 0) {
                result = reward;
                break;
            }
        }

        if (result != null) {
            switch (result.rewardType)
            {
                case Reward.RewardType.Item:
                    CurrentUser.main[result.item] += result.count;
                    break;
                case Reward.RewardType.Action:
                    switch (result.actionReward)
                    {
                        case Reward.ActionReward.Refuel:
                            CurrentUser.main.lifeSystem.Refuel();
                            break;
                        case Reward.ActionReward.UltimateLifeOneHour:
                            CurrentUser.main.lifeSystem.SetUnlimitedLives(1);
                            break;
                    }
                    break;
            }
            UserUtils.WriteProfileOnDevice(CurrentUser.main);
        }

        return result;
    }

    public IEnumerator RequriedLocalizationKeys() {
        foreach (string itemID in Enum.GetNames(typeof(ItemID)))
            yield return string.Format(localizationItemFormat, itemID);
        yield return "notifications/daily-title";
        yield return "notifications/daily-message";
    }

    [Serializable]
    public class Reward {
        public enum Type {Regular, Rare, Unique}
        public enum RewardType {Item, Action}
        public enum ActionReward {Refuel, UltimateLifeOneHour}
        public int index = 0;
        public RewardType rewardType;
        public ActionReward actionReward;
        public ItemID item;
        public Type type;
        public int count;
        public float weight;
        public Sprite icon;

        public Reward(ItemID item, int count, float weight) {
            this.count = count;
            this.item = item;
            this.weight = weight;
        }
    }
}

public class DailySpinTask : ScheduledTask, ITimer {
    const int eventHour = 1;
    const int maxSpin = 5;
    const ItemID spinID = ItemID.spin;

    CurrentUser user;

    DateTime? nextFreeSpin = null;
    readonly TimeSpan notificationSpan = TimeSpan.FromHours(4);
    TimeSpan? zone = null;

    private bool notification = false;
    
    public DailySpinTask(CurrentUser user) {
        uniqueID = "DailySpin";
        this.user = user;
    }

    public override NextCall OnStart(DateTime time) {
        if (!zone.HasValue) {
            zone = TrueTime.Zone;
            nextFreeSpin = null;
        } else if (nextFreeSpin.HasValue && zone.Value != TrueTime.Zone) {
            nextFreeSpin -= zone.Value;
            zone = TrueTime.Zone;
            nextFreeSpin += zone.Value;
            user.Save();
            UpdateNotification();
        }
        if (!nextFreeSpin.HasValue) {
            nextFreeSpin = GetNextFreeSpin(TrueTime.NowLocal);
            user.Save();
            UpdateNotification();
        }
        return new NextCall(Update);
    }

    void Update() {
        if (nextFreeSpin.Value <= TrueTime.NowLocal) {
            if (user[spinID] < maxSpin)
                user[spinID]++;
            zone = TrueTime.Zone;
            DateTime next = GetNextFreeSpin(TrueTime.NowLocal);
            nextFreeSpin = next;
            user.Save();
            ItemCounter.RefreshAll();
        }
        SetNextCall(Update, nextFreeSpin.Value - zone.Value);
        UpdateNotification();
    }

    DateTime GetNextFreeSpin(DateTime time) {
        time += new TimeSpan(eventHour, 0, 0);
        return time;
    }

    public string GetTimer() {
        if (!TrueTime.IsKnown || !nextFreeSpin.HasValue)
            return "...";
        if (user[spinID] >= maxSpin)
            return "Max";
        TimeSpan span = nextFreeSpin.Value - TrueTime.NowLocal;
        return string.Format("{0:00}:{1:00}:{2:00}",
               Mathf.FloorToInt((float) span.TotalHours), span.Minutes, span.Seconds);
    }

    void UpdateNotification() {
        string name = "DailyBonus";

        if (notification)
        {
            if (nextFreeSpin.HasValue)
                Notifications.ScheduleNotification(name, nextFreeSpin.Value + notificationSpan,
                    LocalizationAssistant.main["notifications/daily-title"],
                    LocalizationAssistant.main["notifications/daily-message"]);
            else
                Notifications.CancelNotification(name);
        }
    }

    public override void Serialize(XElement xml) {
        if (nextFreeSpin.HasValue) xml.Add(new XAttribute("time", nextFreeSpin.Value.Ticks));
        if (zone.HasValue) xml.Add(new XAttribute("zone", zone.Value.Ticks));
    }

    public override Dictionary<string, object> Serialize() {
        Dictionary<string, object> json = base.Serialize();

        if (nextFreeSpin.HasValue) json.Set("time", nextFreeSpin.Value.Ticks);
        else json.Set("time", null);

        if (zone.HasValue) json.Set("zone", zone.Value.Ticks);
        else json.Set("zone", null);

        return json;
    }

    public override void Deserialize(XElement xml) {
        XAttribute attribute = xml.Attribute("time");
        if (attribute != null) nextFreeSpin = new DateTime(long.Parse(attribute.Value));
        else nextFreeSpin = null;
        attribute = xml.Attribute("zone");
        if (attribute != null) zone = new TimeSpan(long.Parse(attribute.Value));
        else zone = null;
    }

    public override void Deserialize(Dictionary<string, object> json) {
        object value = json.Get("time");
        if (value != null) nextFreeSpin = new DateTime((long) (double) value); 
        else nextFreeSpin = null;
        value = json.Get("zone");
        if (value != null) zone = new TimeSpan((long) value);
        else zone = null;
    }
}

public class DailyTokenTask : ScheduledTask, ITimer{
    const int eventMinut = 4;
    const int maxTokens = 10;
    const ItemID adTokenID = ItemID.adToken;

    CurrentUser user;

    DateTime? nextFreeToken = null;
    TimeSpan? zone = null;

    public DailyTokenTask(CurrentUser user) {
        uniqueID = "DailyToken";
        this.user = user;
    }
    
    public override NextCall OnStart(DateTime time) {
        if (!zone.HasValue) {
            zone = TrueTime.Zone;
            nextFreeToken = null;
        } else if (nextFreeToken.HasValue && zone.Value != TrueTime.Zone) {
            nextFreeToken -= zone.Value;
            zone = TrueTime.Zone;
            nextFreeToken += zone.Value;
            user.Save();
        }
        
        if (!nextFreeToken.HasValue) {
            nextFreeToken = GetNextFreeToken(TrueTime.NowLocal);
            user.Save();
        }
        return new NextCall(Update);
    }

    void Update() {
        if (nextFreeToken.Value <= TrueTime.NowLocal) {
            if (user[adTokenID] < maxTokens)
                user[adTokenID]++;
            zone = TrueTime.Zone;
            DateTime next = GetNextFreeToken(TrueTime.NowLocal);
            nextFreeToken = next;
            user.Save();
            ItemCounter.RefreshAll();
        }
        SetNextCall(Update, nextFreeToken.Value - zone.Value);
    }

    DateTime GetNextFreeToken(DateTime time) {
        time += new TimeSpan(0, eventMinut, 0);
        return time;
    }

    public string GetTimer() {
        if (!TrueTime.IsKnown || !nextFreeToken.HasValue)
            return "...";
        
        if (IsAdTokenMax())
            return "Max";
        TimeSpan span = nextFreeToken.Value - TrueTime.NowLocal;
        return string.Format("{0:00}:{1:00}:{2:00}",
               Mathf.FloorToInt((float) span.TotalHours), span.Minutes, span.Seconds);
    }
    
    public bool IsAdTokenMax() => user[adTokenID] >= maxTokens;

    public override void Serialize(XElement xml) {
        if (nextFreeToken.HasValue) xml.Add(new XAttribute("time", nextFreeToken.Value.Ticks));
        if (zone.HasValue) xml.Add(new XAttribute("zone", zone.Value.Ticks));
    }

    public override Dictionary<string, object> Serialize() {
        Dictionary<string, object> json = base.Serialize();

        if (nextFreeToken.HasValue) json.Set("time", nextFreeToken.Value.Ticks);
        else json.Set("time", null);

        if (zone.HasValue) json.Set("zone", zone.Value.Ticks);
        else json.Set("zone", null);

        return json;
    }

    public override void Deserialize(XElement xml) {
        XAttribute attribute = xml.Attribute("time");
        if (attribute != null) nextFreeToken = new DateTime(long.Parse(attribute.Value));
        else nextFreeToken = null;
        attribute = xml.Attribute("zone");
        if (attribute != null) zone = new TimeSpan(long.Parse(attribute.Value));
        else zone = null;
    }

    public override void Deserialize(Dictionary<string, object> json) {
        object value = json.Get("time");
        if (value != null) nextFreeToken = new DateTime((long) (double) value); 
        else nextFreeToken = null;
        value = json.Get("zone");
        if (value != null) zone = new TimeSpan((long) value);
        else zone = null;
    }
}

public class RewardedSpinTask : ScheduledTask, ITimer{
    readonly TimeSpan delay = new TimeSpan(0, 4, 0);
    const ItemID spinID = ItemID.spin;
    const ItemID adTokenID = ItemID.adToken;
    const int maxSpin = 5;
    CurrentUser user;

    DateTime? nextReward = null;

    public RewardedSpinTask(CurrentUser user) {
        uniqueID = "RewardedSpin";
        this.user = user;
    }

    public override NextCall OnStart(DateTime time) {
        if (!nextReward.HasValue) 
            return null;
        return new NextCall(Update, nextReward.Value);
    }

    void Update() {
        if (nextReward.Value <= TrueTime.Now) {
            nextReward = null;
            user.Save();
            ItemCounter.RefreshAll();
        }
        TrueTime.Unschedule(this);
    }

    public bool IsAvailable()
    {
        Debug.Log(Advertising.main.CountOfReadyAds(AdType.WheelOfFortune) + " Advertising.main.CountOfReadyAds(AdType.WheelOfFortune)");
        Debug.Log(!nextReward.HasValue +  " !nextReward.HasValue");
        Debug.Log(TrueTime.IsKnown + " TrueTime.IsKnown");
        return TrueTime.IsKnown && IsSpinLessMax() && IsAdTokenGreatZero();
    }

    public bool IsSpinLessMax() => user[spinID] < maxSpin;
    
    public bool IsAdTokenGreatZero() => user[adTokenID] > 0;

    public void Get() {
        if (IsAvailable()) {
            user[adTokenID]--;
            BerryAnalytics.Event("Rewarded Ads Spin");
            Advertising.main.ShowAds(AdType.WheelOfFortune,() => {
                user[spinID]++;
                user.Save();
                TrueTime.Schedule(this);
                ItemCounter.RefreshAll();
            });
        }
    }

    public string GetTimer() {
        if (!TrueTime.IsKnown || !nextReward.HasValue)
            return "...";
        TimeSpan span = nextReward.Value - TrueTime.Now;
        return string.Format("{0:00}:{1:00}:{2:00}",
               Mathf.FloorToInt((float) span.TotalHours), span.Minutes, span.Seconds);
    }

    public override void Serialize(XElement xml) {
        if (nextReward.HasValue)
            xml.Add(new XAttribute("time", nextReward.Value.Ticks));
    }

    public override Dictionary<string, object> Serialize() {
        Dictionary<string, object> json = base.Serialize();

        if (nextReward.HasValue) json.Set("time", nextReward.Value.Ticks);
        else json.Set("time", null);

        return json;
    }

    public override void Deserialize(XElement xml) {
        XAttribute attribute = xml.Attribute("time");
        if (attribute != null) nextReward = new DateTime(long.Parse(attribute.Value));
        else nextReward = null;
    }

    public override void Deserialize(Dictionary<string, object> json) {
        object value = json.Get("time");
        if (value != null) nextReward = new DateTime((long) (double) value);
        else nextReward = null;
    }
}