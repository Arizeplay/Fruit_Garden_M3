using System.Collections.Generic;
using Ninsar.GameEvents.Quests.Rewards;
using UnityEngine;

namespace Ninsar.GameEvents.Quests
{
    [CreateAssetMenu(menuName = "Create Quest Level/TargetItems", fileName = "QuestLevelTargetItems", order = 0)]
    public class QuestLevel : ScriptableObject
    {
        public int TargetItems;
        public List<Reward> Rewards = new List<Reward>();
    }
}