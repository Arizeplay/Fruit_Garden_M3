using Ninsar.GameEvents.Quests;
using UnityEngine;

namespace Ninsar.GameEvents.UI
{
    public abstract class QuestStars : MonoBehaviour
    {
        public abstract void UpdateInfo(Quest quest);
        public abstract void StopUpdate();
    }
}