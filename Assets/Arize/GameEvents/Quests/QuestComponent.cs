using UnityEngine;

namespace Ninsar.GameEvents.Quests
{
    public abstract class QuestComponent
    {
        public abstract GameObject Instantiate(Quest quest, Transform parent);
        public abstract void Update();
    }
}