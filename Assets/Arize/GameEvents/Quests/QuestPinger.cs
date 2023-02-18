using UnityEngine;
namespace Ninsar.GameEvents.Quests
{
    public abstract class QuestPinger : ScriptableObject
    {
        public abstract bool IsPinging();
    }

}