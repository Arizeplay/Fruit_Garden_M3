using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;
using TMPro;
using Yurowm.GameCore;

public class PigInAPokeRewarder : MonoBehaviourAssistant<PigInAPokeRewarder>, ILocalized {

    const string localizationItemFormat = "id/{0}";

    public List<Reward> rewards = new List<Reward>();

    public Image icon;
    public TMP_Text description;

    public void GetReward() {
        Reward targetReward = null;

        float rnd = UnityEngine.Random.Range(0, rewards.Sum(x => x.weight));
        foreach (Reward reward in rewards) {
            rnd -= reward.weight;
            if (rnd <= 0) {
                targetReward = reward;
                break;
            }
        }

        if (targetReward != null) {
            CurrentUser.main[targetReward.item] += targetReward.count;
            description.text = $"{LocalizationAssistant.main[string.Format(localizationItemFormat, targetReward.item.ToString())]}";
            icon.sprite = targetReward.icon;
            AudioAssistant.Shot("RewardedAd");
            UIAssistant.main.ShowPage("PigInAPoke");
        }
    }

    public IEnumerator RequriedLocalizationKeys() {
        foreach (string itemID in Enum.GetNames(typeof(ItemID)))
            yield return string.Format(localizationItemFormat, itemID);
    }

    [Serializable]
    public class Reward {
        public ItemID item;
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
