using System;
namespace Ninsar.GameEvents.Quests.Collections.Requirements
{
    [Serializable]
    public class QuestRequireStars : QuestRequirement
    {
        public override void RegisterEvents()
        {
            SessionAssistant.OnRunLevelDesign += OnRunLevelDesign;
        }
        
        public override void UnregisterEvents()
        {
            SessionAssistant.OnRunLevelDesign -= OnRunLevelDesign;
        }

        private void OnRunLevelDesign(LevelDesign design)
        {
            Project.onLevelComplete.AddListener(() =>
            {
                var score = CurrentUser.main.GetScore(design.number);
                var starsCount = 0;
                starsCount += LevelAssistant.main.GetDesign(design.number).firstStarScore <= score ? 1 : 0;
                starsCount += LevelAssistant.main.GetDesign(design.number).secondStarScore <= score ? 1 : 0;
                starsCount += LevelAssistant.main.GetDesign(design.number).thirdStarScore <= score ? 1 : 0;

                OnItemAdd(starsCount);
            });
        }
    }
}