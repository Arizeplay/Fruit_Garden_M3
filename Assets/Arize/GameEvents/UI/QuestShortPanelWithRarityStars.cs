using UnityEngine.UI;

namespace Ninsar.GameEvents.UI
{
    public class QuestShortPanelWithRarityStars : QuestShortPanel
    {
        public QuestReward QuestRewards;
        public QuestStars Stars;

        public Image Block;
        
        public override void UpdateInfo()
        {
            base.UpdateInfo();

            if (Quest.Progress > 0 || Quest.CurrentTargets > 0)
            {
                Block.gameObject.SetActive(false);
            }
            
            var currentLevel = Quest.CurrentLevel;

            Stars.UpdateInfo(Quest);

            QuestRewards.SetVisible(!Quest.IsComplete());
            QuestRewards.SetRewards(currentLevel.Rewards);
        }
    }
}