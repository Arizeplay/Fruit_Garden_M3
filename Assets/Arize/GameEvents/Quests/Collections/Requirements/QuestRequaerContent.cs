using System;
using UnityEngine;
using Yurowm.GameCore;
namespace Ninsar.GameEvents.Quests.Collections.Requirements
{
    [Serializable]
    public class QuestRequireContent : QuestRequirement
    {
        [SerializeField, ContentSelector]
        private ISlotContent _content;
        
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
            Project.onSlotContentDestroyed.AddListener(OnContentDestroyed);
        }

        private void OnContentDestroyed(ISlotContent content)
        {
            if (content.EqualContent(_content))
            {
                OnItemAdd(1);
            }
        }
    }
}