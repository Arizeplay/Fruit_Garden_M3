using System.Linq;
using UnityEngine;
using Yurowm.GameCore;
namespace Ninsar.GameEvents.Quests.Rewards
{
    [CreateAssetMenu(menuName = "Create Quest Reward/MBooster", fileName = "QuestRewardMBooster", order = 0)]
    public class RewardMBooster : Reward
    {
        public override (Item, int) Apply()
        {
            var boosters = Content.GetPrefabList<IMultipleUseBooster>();
            if (!boosters.Any()) return (null, 0);
            
            var item = boosters.GetRandom().itemID;
            CurrentUser.main[item] += 1;
            
            return (new InventoryItem(item), 1);
        }
    }
}