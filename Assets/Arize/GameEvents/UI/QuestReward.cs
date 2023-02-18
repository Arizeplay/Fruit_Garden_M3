using System;
using System.Collections.Generic;
using _Yurowm.UltimateMatchThree.Scripts.Assistants;
using Ninsar.GameEvents.Quests.Rewards;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Ninsar.GameEvents.UI
{
    public class QuestReward : MonoBehaviour
    {
        public Image Icon;
        public TMP_Text Count;

        public UnityEvent OnShow;
        public UnityEvent OnHide;

        public void SetVisible(bool value)
        {
            Icon.gameObject.SetActive(value);
            Count.gameObject.SetActive(value);

            if (value)
            {
                OnShow.Invoke();
            }
            else
            {
                OnHide.Invoke();
            }
        }
        
        public void SetRewards(List<Reward> rewards)
        {
            var reward = rewards[0];

            switch (reward)
            {
                case RewardItemID rewardItemID:
                    Icon.sprite = ItemIcons.main.GetIconOrNull(rewardItemID.ItemID);
                    Count.text = rewardItemID.Count.ToString();
                    break;
                case RewardMBooster rewardMBooster:
                    break;
                case RewardSale rewardSale:
                    break;
                case RewardSBooster rewardSBooster:
                    break;
            }
        }
    }
}