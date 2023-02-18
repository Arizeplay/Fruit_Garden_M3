using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Yurowm.GameCore;
public class AdRandomRewarder : MonoBehaviourAssistant<AdRandomRewarder>, ILocalized {

    const string localizationItemFormat = "id/{0}";

    [FormerlySerializedAs("rewards")]
    public List<Reward> Rewards = new List<Reward>();

    public Image icon;
    public TMP_Text description;

    
    public Reward GetReward(List<ItemID> without = null) {
        Reward targetReward = null;

        var rewards = new List<Reward>(Rewards);
        rewards.RemoveAll(x => BoosterAssistant.main.SingleUseBoosters.All(b => b.itemID != x.booster.itemID));
        
        without ??= new List<ItemID>();
        
        foreach (var itemID in without)
        {
            rewards.Remove(rewards.First(x => x.booster.itemID == itemID));
        }
        
        float rnd = UnityEngine.Random.Range(0, rewards.Sum(x => x.weight));
        foreach (Reward reward in rewards) {
            rnd -= reward.weight;
            if (rnd <= 0) {
                targetReward = reward;
                break;
            }
        }

        if (targetReward != null) {
            CurrentUser.main[targetReward.booster.itemID] += targetReward.count;
            description.text = $"{LocalizationAssistant.main[string.Format(localizationItemFormat, targetReward.booster.itemID.ToString())]}";
            icon.sprite = targetReward.icon;
            AudioAssistant.Shot("RewardedAd");
            UIAssistant.main.ShowPage("AdRandom");
        }

        return targetReward;
    }

    public IEnumerator RequriedLocalizationKeys() {
        foreach (string itemID in Enum.GetNames(typeof(ItemID)))
            yield return string.Format(localizationItemFormat, itemID);
    }

    [Serializable]
    public class Reward {
        public int selected;
        public IBooster booster;
        public int count;
        public float weight;
        public Sprite icon;

        public Reward(IBooster booster, int count, float weight) {
            this.booster = booster;
            this.count = count;
            this.weight = weight;
        }
    }
}