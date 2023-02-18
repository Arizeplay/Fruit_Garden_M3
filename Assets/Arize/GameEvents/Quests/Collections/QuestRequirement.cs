using System;
namespace Ninsar.GameEvents.Quests.Collections.Requirements
{
    [Serializable]
    public abstract class QuestRequirement
    {
        public event Action<int> onItemAdd;
        public event Action<int> onItemCountChanged;

        public abstract void RegisterEvents();
        public abstract void UnregisterEvents();
        
        protected void OnItemAdd(int count)
        {
            onItemAdd?.Invoke(count);
        }
        
        protected void OnItemCountChanged(int count)
        {
            onItemCountChanged?.Invoke(count);
        }
    }
}