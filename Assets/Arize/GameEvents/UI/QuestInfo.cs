using Ninsar.GameEvents.Quests;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Ninsar.GameEvents.UI
{
    public abstract class QuestInfo : MonoBehaviour
    {
        public TMP_Text Label;
        public TMP_Text Target;
        public Image Image;
        
        public QuestReward QuestRewards;
        public QuestStars Stars;
        
        protected void UpdateInfo(Quest quest)
        {
            if (Label) Label.text = quest.GetLabel();
            if (Target) Target.text = quest.CurrentTargets + "/" + quest.CurrentLevel.TargetItems;
            if (Target) Target.gameObject.SetActive(!quest.IsComplete());
            if (Image) Image.sprite = quest.GetIcon();

            var currentLevel = quest.CurrentLevel;

            if (Stars) Stars.UpdateInfo(quest);
            if (QuestRewards)
            {
                QuestRewards.SetVisible(!quest.IsComplete());
                QuestRewards.SetRewards(currentLevel.Rewards);
            }
        }
    }
}