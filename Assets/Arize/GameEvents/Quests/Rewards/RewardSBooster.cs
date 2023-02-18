using System.Linq;
using UnityEngine;
using Yurowm.GameCore;
namespace Ninsar.GameEvents.Quests.Rewards
{
    [CreateAssetMenu(menuName = "Create Quest Reward/SBooster", fileName = "QuestRewardSBooster", order = 0)]
    public class RewardSBooster : Reward
    {
        public override (Item, int) Apply()
        {
            var boosters = Content.GetPrefabList<ISingleUseBooster>();
            if (!boosters.Any()) return (null, 0);
            
            var item = boosters.GetRandom().itemID;
            CurrentUser.main[item] += 1;
            
            return (new InventoryItem(item), 1);
        }
    }
}