using System;
using System.Collections.Generic;
using Ninsar.GameEvents.Quests.Rewards;
using UnityEngine;

namespace Ninsar.GameEvents.Quests
{
    public abstract class Quest : GameEvent
    {
        public static Quest selected;
        public static event Action<Quest> OnQuestsReady;
        public static event Action<Quest> OnQuestsComplete;
        public static event Action<Dictionary<Reward.Item, int>> OnGettedRewardItems;

        [SerializeField]
        private bool _showPageIfReady = true;
        
        [SerializeField]
        private string _page;
        
        [SerializeField]
        protected string label;

        [SerializeField]
        protected Sprite icon;

        public abstract void ResetStats();
        
        public abstract int CurrentTargets { get; }
        public abstract QuestLevel CurrentLevel { get; }
        public abstract float Progress { get; }
        
        public abstract void CompleteIfReady();

        public string GetLabel() => label;

        public Sprite GetIcon() => icon;

        public string GetPage() => _page;

        public bool ShowPageIfReady => _showPageIfReady;
        
        public abstract bool IsReady();
        public abstract bool IsPinging();
        public abstract bool IsComplete();

        public abstract RarityType GetRarity();

        public abstract int GetStars(out int maxCount);

        protected void QuestsReady()
        {
            OnQuestsReady?.Invoke(this);
        }
        
        protected void QuestsComplete()
        {
            OnQuestsComplete?.Invoke(this);
        }
        
        protected void GettedRewardItems(Dictionary<Reward.Item, int> items)
        {
            OnGettedRewardItems?.Invoke(items);
        }
    }
}